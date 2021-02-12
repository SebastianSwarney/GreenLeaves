using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Staggart.VegetationSpawner;

public class TerrainObjectSpawner : OdinEditorWindow
{
    public Terrain m_terrain;

    public int objectPlacementResolution = 256;

    [PreviewField(Height = 256, Alignment = ObjectFieldAlignment.Left)]
    public Texture2D m_texture;

    public LayerMask m_collisionLayerMask;
    public LayerMask m_terrainMask;

    public bool m_removeExtraObjects;

    [InlineEditor]
    public TerrainObjectSpawnerPalette m_palette;

    public int m_grassDensity;

    #region Old variables
    /*
    [PreviewField(Height = 256, Alignment = ObjectFieldAlignment.Left)]
    public Texture2D m_noiseTexture;

    public float xOrg;
    public float yOrg;

    public float m_noiseAmplitude;
    public float m_noiseFrequency;

    public float scale = 1.0F;
    */
    #endregion

    #region Editor Utils
    [MenuItem("Tools/Terrain Object Spawner")]
    private static void OpenWindow()
    {
        GetWindow<TerrainObjectSpawner>().Show();
    }

    private void ShowProgressBar(float p_progress, float p_maxProgress, string p_message = "MeshToTerrian")
    {
        float displayProgress = p_progress / p_maxProgress;
        EditorUtility.DisplayProgressBar("Object to Terrain", p_message, displayProgress);
    }
	#endregion

	#region Noise Code
    /*
	[Button("Make Noise")]
    private void CreateNoiseTexture()
    {
        Terrain terrain = m_terrain;

        float width = terrain.terrainData.detailWidth;
        float length = terrain.terrainData.detailHeight;

        int adjustmentAmount = terrain.terrainData.detailWidth / objectPlacementResolution;

        float adjustedWidth = width / adjustmentAmount;
        float adjustedLength = length / adjustmentAmount;

        Texture2D noiseTex = new Texture2D((int)adjustedWidth, (int)adjustedLength, TextureFormat.RGBA32, false);

        Color[] pix = new Color[noiseTex.width * noiseTex.height];

        float y = 0.0F;

        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
                float xCoord = xOrg + x / noiseTex.width * scale;
                float yCoord = yOrg + y / noiseTex.height * scale;
                float sample = Mathf.PerlinNoise(xCoord / m_noiseFrequency, yCoord / m_noiseFrequency) * m_noiseAmplitude;
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }

        noiseTex.SetPixels(pix);
        noiseTex.Apply();

        m_noiseTexture = noiseTex;
    }
    */
	#endregion

	#region Collision Code
	[Button("Build Collision Texture")]
    private void BuildCollisionTextureInspector()
	{
        BuildCollisionTexture(true, false);
	}

    private void BuildCollisionTexture(bool updateInspector, bool p_ignoreChildObjects)
	{
        Terrain terrain = m_terrain;

        #region Terrain Dimensions
        float width = terrain.terrainData.detailWidth;
        float length = terrain.terrainData.detailHeight;

        int adjustmentAmount = terrain.terrainData.detailWidth / objectPlacementResolution;

        adjustmentAmount = 1;

        float adjustedWidth = width / adjustmentAmount;
        float adjustedLength = length / adjustmentAmount;

        float cellSize = (terrain.terrainData.size.x / adjustedWidth) / 2;
        Vector3 halfExents = new Vector3(cellSize, cellSize, cellSize);
        #endregion

        Texture2D texture = new Texture2D((int)adjustedWidth, (int)adjustedLength, TextureFormat.RGBA32, false);

        for (int x = 0; x < width; x += adjustmentAmount)
        {
            for (int y = 0; y < length; y += adjustmentAmount)
            {
                Vector3 worldPos = terrain.DetailToWorld(y, x);
                worldPos += halfExents;

                Vector2Int texturePosition = new Vector2Int(x / adjustmentAmount, y / adjustmentAmount);

                GetCollisionPixel(worldPos, texturePosition, halfExents, texture, terrain, p_ignoreChildObjects);
            }
        }

        if (updateInspector)
		{
            texture.Apply();
        }

        m_texture = texture;
    }

    private void GetCollisionPixel(Vector3 p_worldPosition, Vector2Int p_texturePosition, Vector3 p_halfExtents, Texture2D p_texture, Terrain p_terrain, bool p_ignoreChildObjects)
    {
        RaycastHit terrainHit;

        if (Physics.BoxCast(p_worldPosition + (Vector3.up * 600f), p_halfExtents, -Vector3.up, out terrainHit, Quaternion.identity, 700f, m_collisionLayerMask))
        {
            if (terrainHit.collider.gameObject != p_terrain.gameObject)
            {
                Color color = new Color(0, 0, 0, 1f);

                if (CheckCollisionLayer(m_terrainMask, terrainHit.collider.gameObject))
                {
					if (p_ignoreChildObjects)
					{
                        color = new Color(1f, 0, 0, 1f);
                    }
					else if (terrainHit.collider.transform.root != p_terrain.transform)
					{
                        color = new Color(1f, 0, 0, 1f);
                    }
                }

                p_texture.SetPixel(p_texturePosition.x, p_texturePosition.y, color);
            }
        }
    }
    #endregion

    #region Object Placement Code
    [Button("Place Objects")]
    private void ObjectPlacementLoop()
    {
        BuildCollisionTexture(false, true);

        List<GameObject> childObjects = GetAllChildRootObjects();
        DisableAllChildObjects(childObjects);

        Terrain terrain = m_terrain;


		//Texture2D texture = new Texture2D((int)adjustedWidth, (int)adjustedLength, TextureFormat.RGBA32, false);
        List<ObjectSpawnData> spawnData = new List<ObjectSpawnData>();

        Texture2D texture = m_texture;

        for (int i = 0; i < m_palette.m_objectListsInUse.Length; i++)
        {
            #region Terrain Dimensions
            float width = terrain.terrainData.detailWidth;
            float length = terrain.terrainData.detailHeight;

            int adjustmentAmount = terrain.terrainData.detailWidth / m_palette.m_objectListsInUse[i].m_objectPlacementResolution;

            float adjustedWidth = width / adjustmentAmount;
            float adjustedLength = length / adjustmentAmount;

            float cellSize = (terrain.terrainData.size.x / adjustedWidth) / 2;
            Vector3 halfExents = new Vector3(cellSize, cellSize, cellSize);
            #endregion

            for (int x = 0; x < width; x += adjustmentAmount)
            {
                for (int y = 0; y < length; y += adjustmentAmount)
                {
                    Vector3 worldPos = terrain.DetailToWorld(y, x);
                    worldPos += halfExents;

                    //Vector2Int texturePosition = new Vector2Int(x / adjustmentAmount, y / adjustmentAmount);
                    Vector2Int texturePosition = new Vector2Int(x, y);
                    //GetCollisionPixel(worldPos, texturePosition, halfExents, texture, terrain);                    

                    worldPos += new Vector3(Random.Range(-halfExents.x, halfExents.x), 0, Random.Range(-halfExents.y, halfExents.y));
                    Vector2 normalizedPos = terrain.GetNormalizedPosition(worldPos);

                    float curvature = terrain.SampleConvexity(normalizedPos);
                    curvature = TerrainSampler.ConvexityToCurvature(curvature);

                    terrain.SampleHeight(normalizedPos, out _, out worldPos.y, out _);

                    float slope = terrain.GetSlope(normalizedPos);

                    Vector3 slopeNormal = terrain.terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);

					#region Texture Collision
					Color pixelvalue = texture.GetPixel(texturePosition.x, texturePosition.y);

					/*
                    if (1 == pixelvalue.a && pixelvalue.r < 1)
					{
                        continue;
					}
                    */

					if (pixelvalue == Color.black)
					{
                        continue;
                    }

                    if (pixelvalue.r == 1)
                    {
                        RaycastHit hit;

                        if (Physics.Raycast(worldPos + Vector3.up * 600f, Vector3.down, out hit, 700f, m_terrainMask))
                        {
                            worldPos = hit.point;
                            slope = Vector3.Angle(hit.normal, Vector3.up);
                            slopeNormal = hit.normal;
                            curvature = 0;
                        }
                    }
					#endregion

					ObjectBrushObjectList objectListToUse = m_palette.m_objectListsInUse[i].m_objectList;

                    //terrain.SampleHeight(normalizedPos, out _, out worldPos.y, out _);

                    float adjustedWorldHeight = terrain.SampleHeight(worldPos);

                    if (objectListToUse.CheckSpawn(worldPos.y / terrain.terrainData.size.y, curvature, slope))
                    {
                        spawnData.Add(new ObjectSpawnData(objectListToUse, new Vector3(worldPos.x, adjustedWorldHeight, worldPos.z), slopeNormal));
                        //Color color = new Color(0, 0, 0, 1f);
                        Color color = Color.black;
                        texture.SetPixel(x, y, color);
                    }
                }
            }
        }

        //texture.Apply();
        m_texture = texture;
        m_texture.Apply();

        SpawnAllObjects(spawnData.ToArray(), childObjects);
    }
	#endregion

	#region Object Spawning
	private struct ObjectSpawnData
    {
        public ObjectBrushObjectList m_listInUse;
        public Vector3 m_spawnPosition, m_slopeNormal;

        public ObjectSpawnData(ObjectBrushObjectList p_listInUse, Vector3 p_spawnPosition, Vector3 p_slopeNormal)
        {
            m_listInUse = p_listInUse;
            m_spawnPosition = p_spawnPosition;
            m_slopeNormal = p_slopeNormal;
        }
    }


    private void SpawnAllObjects(ObjectSpawnData[] m_allSpawnData, List<GameObject> p_childObjects)
    {
        if (m_allSpawnData.Length > 3000)
        {
            if (!EditorUtility.DisplayDialog("Object Count Warning", "You are trying to spawn " + m_allSpawnData.Length + " objects are you ok with that?", "Yes", "No"))
            {
                Debug.Log("Canceled the object placement");
                return;
            }
        }

        //List<GameObject> childObjects = GetAllChildRootObjects();

        for (int i = 0; i < m_allSpawnData.Length; i++)
        {
            ShowProgressBar(i, m_allSpawnData.Length, "Spawning object number " + i + " out of " + m_allSpawnData.Length + " objects");

            GameObject objectToSpawn = m_allSpawnData[i].m_listInUse.GetObjectFromList();
            GameObject prevObject = GetSpawnedObject(ref p_childObjects, objectToSpawn);

            if (prevObject == null)
            {
                GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(objectToSpawn);
                m_allSpawnData[i].m_listInUse.SpawnObject(m_allSpawnData[i].m_spawnPosition, m_allSpawnData[i].m_slopeNormal, m_terrain.transform, newObject);
            }
            else
            {
                m_allSpawnData[i].m_listInUse.SpawnObject(m_allSpawnData[i].m_spawnPosition, m_allSpawnData[i].m_slopeNormal, m_terrain.transform, prevObject);
            }

        }

        if (m_removeExtraObjects)
        {
            foreach (GameObject child in p_childObjects)
            {
                if (child != m_terrain.gameObject)
                {
                    DestroyImmediate(child);
                }
            }
		}
		else
		{
            DisableAllChildObjects(p_childObjects);
        }

    }

    private void DisableAllChildObjects(List<GameObject> p_childObjects)
    {
        foreach (GameObject child in p_childObjects)
        {
            if (child != m_terrain.gameObject && child.activeSelf == true)
            {
                child.SetActive(false);
            }
        }
    }

    private List<GameObject> GetAllChildRootObjects()
    {
        List<GameObject> childObjects = new List<GameObject>();

        foreach (Transform child in m_terrain.transform)
        {
            if (PrefabUtility.IsAnyPrefabInstanceRoot(child.gameObject))
            {
                //child.gameObject.SetActive(true);
                childObjects.Add(child.gameObject);
            }
        }

        return childObjects;
    }

    private GameObject GetSpawnedObject(ref List<GameObject> p_placedObjects, GameObject p_objectToPlace)
    {
        foreach (GameObject placedObject in p_placedObjects)
        {
            GameObject foundPrefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(placedObject).transform.root.gameObject;

            if (p_objectToPlace == foundPrefabAsset)
            {
                placedObject.SetActive(true);
                p_placedObjects.Remove(placedObject);
                return placedObject;
            }
        }

        return null;
    }
	#endregion

    [Button("Place Grass")]
    private void PlaceGrass()
	{
        Terrain terrain = m_terrain;

        #region Terrain Dimensions
        float width = terrain.terrainData.detailWidth;
        float length = terrain.terrainData.detailHeight;
        #endregion

        int[,] map = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, 0);

        Texture2D texture = m_texture;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                int instanceCount = m_grassDensity;

                Vector2Int texturePosition = new Vector2Int(x, y);
                Color pixelvalue = texture.GetPixel(texturePosition.x, texturePosition.y);

				if (pixelvalue == Color.black)
				{
                    instanceCount = 0;
				}

                /*
                Vector3 wPos = terrain.DetailToWorld(y, x);
                Vector2 normalizedPos = terrain.GetNormalizedPosition(wPos);
                float spawnChance = 0;
                Texture2D splat = terrain.terrainData.GetAlphamapTexture(SpawnerBase.GetSplatmapID(1));
                Vector2Int texelIndex = terrain.SplatmapTexelIndex(normalizedPos);
                Color color = splat.GetPixel(texelIndex.x, texelIndex.y);
                int channel = 1 % 4;
                float value = SpawnerBase.SampleChannel(color, channel);
                if (value > 0)
                {
                    value = Mathf.Clamp01(value - 0.5f);
                }
                value *= 100f;
                spawnChance += value;
                if ((Random.value <= spawnChance) == false)
                {
                    instanceCount = 0;
                }
                */

                map[x, y] = instanceCount;
            }
        }

        terrain.terrainData.SetDetailLayer(0, 0, 0, map);
    }

	[Button("Remove Objects")]
    private void RemoveObjects()
    {
        for (int i = this.m_terrain.transform.childCount; i > 0; --i)
            DestroyImmediate(this.m_terrain.transform.GetChild(0).gameObject);
    }

    public bool CheckCollisionLayer(LayerMask p_layerMask, GameObject p_object)
    {
        if (p_layerMask == (p_layerMask | (1 << p_object.layer)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
