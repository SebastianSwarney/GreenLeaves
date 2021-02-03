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

    public int cellSize = 64;
    public int cellDivisions = 4;

    public int objectPlacementResolution = 256;

    [PreviewField(Height = 250f, Alignment = ObjectFieldAlignment.Left)]
    public Texture2D m_texture;

    public bool highPrecisionCollision;

    public LayerMask collisionLayerMask;
    public LayerMask m_terrainMask;

    public Dictionary<Terrain, Cell[,]> terrainCells = new Dictionary<Terrain, Cell[,]>();

    public bool m_removeExtraObjects;

    public bool m_limitPlacement;

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

                if (Physics.BoxCast(wPos + (Vector3.up * 600f), halfExents, -Vector3.up, out terrainHit, Quaternion.identity, 700f, collisionLayerMask))
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

    [Button("Place Objects")]
    private void PlaceObjects()
    {
        BuildCollisionTexture(false);

        Terrain terrain = m_terrain;

        float width = terrain.terrainData.detailWidth;
        float length = terrain.terrainData.detailHeight;

        int adjustmentAmount = terrain.terrainData.detailWidth / objectPlacementResolution;

        float adjustedWidth = width / adjustmentAmount;
        float adjustedLength = length / adjustmentAmount;

        float reducedRange = adjustedWidth / 2;

        if (m_limitPlacement)
        {
            width = adjustedWidth - reducedRange;
            length = adjustedLength - reducedRange;
        }

        Texture2D texture = m_texture;
        List<ObjectSpawnData> spawnData = new List<ObjectSpawnData>();

        //Texture2D texture = new Texture2D((int)width, (int)length, TextureFormat.Alpha8, false);

        for (int i = 0; i < m_palette.m_objectList.Length; i++)
        {
            for (int x = 0; x < width; x += adjustmentAmount)
            {
                for (int y = 0; y < length; y += adjustmentAmount)
                {
                    ObjectBrushObjectList objectListToUse = m_palette.m_objectList[i].m_objectList;

                    Vector3 wPos = terrain.DetailToWorld(y, x);
                    //wPos += objectListToUse.GetDistanceVariation();
                    Vector2 normalizedPos = terrain.GetNormalizedPosition(wPos);

                    float curvature = terrain.SampleConvexity(normalizedPos);
                    curvature = TerrainSampler.ConvexityToCurvature(curvature);

                    terrain.SampleHeight(normalizedPos, out _, out wPos.y, out _);
                    float slope = terrain.GetSlope(normalizedPos);

                    Vector3 slopeNormal = terrain.terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);

                    Color pixelvalue = texture.GetPixel(x / adjustmentAmount, y / adjustmentAmount);

                    Vector3 spawnCheckPos = wPos;

                    if (pixelvalue.r > 0)
					{
                        RaycastHit hit;

                        if (Physics.Raycast(wPos + Vector3.up * 600f, Vector3.down, out hit, 700f, m_terrainMask))
						{
                            float castSlope = Vector3.Angle(hit.normal, Vector3.up);

                            if (objectListToUse.CheckSpawn(hit.point.y / terrain.terrainData.size.y, 0, castSlope))
                            {
                                spawnData.Add(new ObjectSpawnData(objectListToUse, hit.point, slopeNormal));

                                Color color = new Color(0, 0, 0, 1f);
                                texture.SetPixel(x / adjustmentAmount, y / adjustmentAmount, color);
                            }
                        }
					}
					else if (Random.value > pixelvalue.a)
					{
                        if (objectListToUse.CheckSpawn(wPos.y / terrain.terrainData.size.y, curvature, slope))
                        {
                            spawnData.Add(new ObjectSpawnData(objectListToUse, wPos, slopeNormal));

                            Color color = new Color(0, 0, 0, 1f);
                            texture.SetPixel(x / adjustmentAmount, y / adjustmentAmount, color);
                        }
                    }
                }
            }
        }

        texture.Apply();
        m_texture = texture;

        SpawnAllObjects(spawnData.ToArray());
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

    }

    private List<GameObject> GetAllChildRootObjects()
    {
        List<GameObject> childObjects = new List<GameObject>();

        foreach (Transform child in m_terrain.transform)
        {
            if (PrefabUtility.IsAnyPrefabInstanceRoot(child.gameObject))
            {
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

    #region Collision Code

    public bool InsideOccupiedCell(Terrain terrain, Vector3 worldPos, Vector2 normalizedPos)
    {
        if (terrainCells == null) return false;

        //No collision cells baked for terrain, user will probably notice
        if (terrainCells.ContainsKey(terrain) == false) return false;

        Cell[,] cells = terrainCells[terrain];

        Vector2Int cellIndex = Cell.PositionToCellIndex(terrain, normalizedPos, cellSize);
        Cell mainCell = cells[cellIndex.x, cellIndex.y];

        if (mainCell != null)
        {
            Cell subCell = mainCell.GetSubcell(worldPos, cellSize, cellDivisions);

            if (subCell != null)
            {
                return true;
            }
            else
            {
                //Cell doesn't exist
                return false;
            }
        }
        else
        {
            Debug.LogErrorFormat("Position {0} falls outside of the cell grid", worldPos);
        }

        return false;
    }


    [Button("Build Colliders")]
    private void BuildColliders()
    {
        Terrain terrain = m_terrain;

        RaycastHit hit;

        terrainCells.Clear();

        int xCount = Mathf.CeilToInt(terrain.terrainData.size.x / cellSize);
        int zCount = Mathf.CeilToInt(terrain.terrainData.size.z / cellSize);

        Cell[,] cellGrid = new Cell[xCount, zCount];

        for (int x = 0; x < xCount; x++)
        {
            for (int z = 0; z < zCount; z++)
            {
                Vector3 wPos = new Vector3(terrain.GetPosition().x + (x * cellSize) + (cellSize * 0.5f), 0f, terrain.GetPosition().z + (z * cellSize) + (cellSize * 0.5f));

                Vector2 normalizeTerrainPos = terrain.GetNormalizedPosition(wPos);

                terrain.SampleHeight(normalizeTerrainPos, out _, out wPos.y, out _);

                Cell cell = Cell.New(wPos, cellSize);
                cell.Subdivide(cellDivisions);

                cellGrid[x, z] = cell;

                for (int sX = 0; sX < cellDivisions; sX++)
                {
                    for (int sZ = 0; sZ < cellDivisions; sZ++)
                    {
                        //Sample corners of cell
                        if (highPrecisionCollision)
                        {
                            Bounds b = cell.subCells[sX, sZ].bounds;

                            Vector3[] corners = new Vector3[]
                            {
                                        //BL corner
                                        new Vector3(b.min.x, b.center.y, b.min.z),
                                        //TL corner
                                        new Vector3(b.min.x, b.center.y, b.min.z + b.size.z),
                                        //BR corner
                                        new Vector3(b.max.x, b.center.y, b.min.z),
                                        //TR corner
                                        new Vector3(b.max.x, b.center.y, b.max.z),
                            };

                            int hitCount = corners.Length;
                            for (int i = 0; i < corners.Length; i++)
                            {
                                if (Physics.Raycast(corners[i] + (Vector3.up * 100f), -Vector3.up, out hit, 150f, collisionLayerMask))
                                {
                                    //Require to check for type, since its possible to hit a neighboring terrains
                                    if (hit.collider.GetType() == typeof(TerrainCollider))
                                    {
                                        hitCount--;
                                    }
                                }
                                else
                                {
                                    hitCount--;
                                }
                            }

                            //Remove cell when all rays missed
                            if (hitCount == 0) cell.subCells[sX, sZ] = null;
                        }
                        //Sample center of cell
                        else
                        {
                            /*
                            //Remove cell if hitting terrain
                            if (Physics.Raycast(cell.subCells[sX, sZ].bounds.center + (Vector3.up * 50f), -Vector3.up, out hit, 100f, collisionLayerMask))
                            {
                                if (hit.collider.gameObject == terrain.gameObject)
                                {
                                    cell.subCells[sX, sZ] = null;
                                }
                            }
                            */

                            RaycastHit terrainHit;

                            if (Physics.BoxCast(cell.subCells[sX, sZ].bounds.center + (Vector3.up * 50f), cell.subCells[sX, sZ].bounds.extents, -Vector3.up, out terrainHit, Quaternion.identity, 100f, collisionLayerMask))
                            {
                                if (terrainHit.collider.gameObject == terrain.gameObject)
                                {
                                    cell.subCells[sX, sZ] = null;
                                }
                            }
                        }
                    }
                }
            }

        }

        terrainCells.Add(terrain, cellGrid);
    }

    #endregion
}
