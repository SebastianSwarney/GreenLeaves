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

    [PreviewField(Height = 256, Alignment = ObjectFieldAlignment.Left)]
    public Texture2D m_noiseTexture;

    public float xOrg;
    public float yOrg;

    public float m_noiseAmplitude;
    public float m_noiseFrequency;

    public float scale = 1.0F;

    public LayerMask m_collisionLayerMask;
    public LayerMask m_terrainMask;

    public bool m_removeExtraObjects;

    [InlineEditor]
    public TerrainObjectSpawnerPalette m_palette;

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

    #region Object Placement Code

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

    [Button("Build Collision Texture")]
    private void BuildCollisionTextureInspector()
	{
        BuildCollisionTexture(true);
	}

    private void BuildCollisionTexture(bool updateInspector)
	{
        Terrain terrain = m_terrain;

        float width = terrain.terrainData.detailWidth;
        float length = terrain.terrainData.detailHeight;

        int adjustmentAmount = terrain.terrainData.detailWidth / objectPlacementResolution;

        float adjustedWidth = width / adjustmentAmount;
        float adjustedLength = length / adjustmentAmount;

        float cellSize = (terrain.terrainData.size.x / adjustedWidth) / 2;
        Vector3 halfExents = new Vector3(cellSize, cellSize, cellSize);

        Texture2D texture = new Texture2D((int)adjustedWidth, (int)adjustedLength, TextureFormat.RGBA32, false);

        for (int x = 0; x < width; x += adjustmentAmount)
        {
            for (int y = 0; y < length; y += adjustmentAmount)
            {
                Vector3 wPos = terrain.DetailToWorld(y, x);
                wPos += halfExents;

                RaycastHit terrainHit;

                if (Physics.BoxCast(wPos + (Vector3.up * 600f), halfExents, -Vector3.up, out terrainHit, Quaternion.identity, 700f, m_collisionLayerMask))
                {
                    if (terrainHit.collider.gameObject != terrain.gameObject && terrainHit.collider.transform.root != terrain.transform)
                    {
                        Color color = new Color(0, 0, 0, 1f);

                        if (CheckCollisionLayer(m_terrainMask, terrainHit.collider.gameObject))
                        {
                            color = new Color(1f, 0, 0, 1f);
                        }

                        texture.SetPixel(x / adjustmentAmount, y / adjustmentAmount, color);
                    }
                }
            }
        }

		if (updateInspector)
		{
            texture.Apply();
        }

        m_texture = texture;
    }

    [Button("Place Objects")]
    private void PlaceObjects()
    {
        //BuildCollisionTexture(false);

        Terrain terrain = m_terrain;

        float width = terrain.terrainData.detailWidth;
        float length = terrain.terrainData.detailHeight;

        int adjustmentAmount = terrain.terrainData.detailWidth / objectPlacementResolution;

        float adjustedWidth = width / adjustmentAmount;
        float adjustedLength = length / adjustmentAmount;

        float cellSize = (terrain.terrainData.size.x / adjustedWidth) / 2;
        Vector3 halfExents = new Vector3(cellSize, cellSize, cellSize);

        //Texture2D texture = m_texture;
        Texture2D texture = new Texture2D((int)adjustedWidth, (int)adjustedLength, TextureFormat.RGBA32, false);
        List<ObjectSpawnData> spawnData = new List<ObjectSpawnData>();

        for (int i = 0; i < m_palette.m_objectList.Length; i++)
        {
            for (int x = 0; x < width; x += adjustmentAmount)
            {
                for (int y = 0; y < length; y += adjustmentAmount)
                {
                    ObjectBrushObjectList objectListToUse = m_palette.m_objectList[i].m_objectList;

                    Vector3 wPos = terrain.DetailToWorld(y, x);
                    wPos += halfExents;

                    RaycastHit terrainHit;

                    if (Physics.BoxCast(wPos + (Vector3.up * 600f), halfExents, -Vector3.up, out terrainHit, Quaternion.identity, 700f, m_collisionLayerMask))
                    {
                        if (terrainHit.collider.gameObject != terrain.gameObject && terrainHit.collider.transform.root != terrain.transform)
                        {
                            Color color = new Color(0, 0, 0, 1f);

                            if (CheckCollisionLayer(m_terrainMask, terrainHit.collider.gameObject))
                            {
                                color = new Color(1f, 0, 0, 1f);
                            }

                            texture.SetPixel(x / adjustmentAmount, y / adjustmentAmount, color);
                        }
                    }

                    wPos += new Vector3(Random.Range(-halfExents.x, halfExents.x), 0, Random.Range(-halfExents.y, halfExents.y));
                    Vector2 normalizedPos = terrain.GetNormalizedPosition(wPos);

                    float curvature = terrain.SampleConvexity(normalizedPos);
                    curvature = TerrainSampler.ConvexityToCurvature(curvature);

                    terrain.SampleHeight(normalizedPos, out _, out wPos.y, out _);
                    float slope = terrain.GetSlope(normalizedPos);
                    Vector3 slopeNormal = terrain.terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);

                    Vector2Int pixCord = new Vector2Int(x / adjustmentAmount, y / adjustmentAmount);
                    Color pixelvalue = texture.GetPixel(pixCord.x, pixCord.y);

                    Vector3 worldSpawnPos = wPos;
                    float spawnSlope = slope;
                    Vector3 spawnSlopeNormal = slopeNormal;
                    float spawnCurvature = curvature;

					if (1 == pixelvalue.a && pixelvalue.r < 1)
					{
                        continue;
					}

                    Color noisePixelValue = m_noiseTexture.GetPixel(pixCord.x, pixCord.y);

                    if (Random.value > noisePixelValue.r)
					{
                        continue;
                    }

                    if (pixelvalue.r == 1)
                    {
                        RaycastHit hit;

                        if (Physics.Raycast(wPos + Vector3.up * 600f, Vector3.down, out hit, 700f, m_terrainMask))
                        {
                            worldSpawnPos = hit.point;
                            spawnSlope = Vector3.Angle(hit.normal, Vector3.up);
                            spawnSlopeNormal = hit.normal;
                            spawnCurvature = 0;
                        }
                    }

                    if (objectListToUse.CheckSpawn(worldSpawnPos.y / terrain.terrainData.size.y, spawnCurvature, spawnSlope))
                    {
                        spawnData.Add(new ObjectSpawnData(objectListToUse, worldSpawnPos, spawnSlopeNormal));
                        Color color = new Color(0, 0, 0, 1f);
                        texture.SetPixel(pixCord.x, pixCord.y, color);
                    }
                }
            }
        }

        texture.Apply();
        m_texture = texture;

        SpawnAllObjects(spawnData.ToArray());
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


    private void SpawnAllObjects(ObjectSpawnData[] m_allSpawnData)
    {
        if (m_allSpawnData.Length > 3000)
        {
            if (!EditorUtility.DisplayDialog("Object Count Warning", "You are trying to spawn " + m_allSpawnData.Length + " objects are you ok with that?", "Yes", "No"))
            {
                Debug.Log("Canceled the object placement");
                return;
            }
        }

        List<GameObject> childObjects = GetAllChildRootObjects();

        for (int i = 0; i < m_allSpawnData.Length; i++)
        {
            ShowProgressBar(i, m_allSpawnData.Length, "Spawning object number " + i + " out of " + m_allSpawnData.Length + " objects");

            GameObject objectToSpawn = m_allSpawnData[i].m_listInUse.GetObjectFromList();
            GameObject prevObject = GetSpawnedObject(ref childObjects, objectToSpawn);

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
            foreach (GameObject child in childObjects)
            {
                if (child != m_terrain.gameObject)
                {
                    DestroyImmediate(child);
                }
            }
		}
		else
		{
            foreach (GameObject child in childObjects)
            {
                if (child != m_terrain.gameObject && child.activeSelf == true)
                {
                    child.SetActive(false);
                }
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

    [Button("Remove")]
    private void RemoveObjects()
    {
        for (int i = this.m_terrain.transform.childCount; i > 0; --i)
            DestroyImmediate(this.m_terrain.transform.GetChild(0).gameObject);
    }
    #endregion
}
