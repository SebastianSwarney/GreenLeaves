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

    public bool highPrecisionCollision;

    public LayerMask collisionLayerMask;

    public Dictionary<Terrain, Cell[,]> terrainCells = new Dictionary<Terrain, Cell[,]>();

    public GameObject m_testRock;

    public GameObject[] m_testObjects;

    public AnimationCurve m_testCurve;

    public float m_spawnChance;

    public Vector2 m_slopeSpawnRange;
    public Vector2 m_heightSpawnRange;

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

    [Button("Place Objects")]
    private void PlaceObjects()
    {
        //RemoveObjects();
        Terrain terrain = m_terrain;

        float reducedRange = terrain.terrainData.detailWidth / 2;

        List<Vector3> spawnPos = new List<Vector3>();

        for (int x = 0; x < terrain.terrainData.detailWidth - reducedRange; x++)
        {
            for (int y = 0; y < terrain.terrainData.detailHeight - reducedRange; y++)
            {
                if ((Random.value * 100f) >= m_spawnChance)
                {
                    continue;
                }

                Vector3 wPos = terrain.DetailToWorld(y, x);
                Vector2 normalizedPos = terrain.GetNormalizedPosition(wPos);
                float curvature = terrain.SampleConvexity(normalizedPos);
                //0=concave, 0.5=flat, 1=convex
                curvature = TerrainSampler.ConvexityToCurvature(curvature);

                terrain.SampleHeight(normalizedPos, out _, out wPos.y, out _);

                float slope = terrain.GetSlope(normalizedPos);

                float y_01 = (float)y / (float)terrain.terrainData.alphamapHeight;
                float x_01 = (float)x / (float)terrain.terrainData.alphamapWidth;

                Vector3 normal = terrain.terrainData.GetInterpolatedNormal(y_01, x_01);

                float heightSpawnChance = Mathf.InverseLerp(m_heightSpawnRange.y, m_heightSpawnRange.x, wPos.y);

                if ((Random.value) >= heightSpawnChance)
                {
                    continue;
                }

                float slopeSpawnChance = Mathf.InverseLerp(m_slopeSpawnRange.y, m_slopeSpawnRange.x, slope);

                if ((Random.value) >= slopeSpawnChance)
                {
                    continue;
                }

                spawnPos.Add(wPos);

                #region Old
                /*
                if (curvature > 0.25f && curvature < 0.75 && curvature != 0.5f)
                {
                    if (slope < 30 && slope > 1)
                    {
                        if ((Random.value * 100f) <= 30f)
                        {
                            if (!InsideOccupiedCell(terrain, wPos, normalizedPos))
                            {
                                GameObject objectToSpawn = m_testObjects[Random.Range(0, m_testObjects.Length)];

                                GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(objectToSpawn);

                                if (newObject == null)
                                {
                                    Debug.LogError("Error instantiating prefab");
                                    return;
                                }

                                newObject.transform.rotation = Quaternion.Euler(newObject.transform.rotation.eulerAngles.x, Random.Range(0, 360), newObject.transform.rotation.eulerAngles.z);

                                //newObject.transform.rotation = Quaternion.LookRotation(Vector3.Cross(normal, Vector3.up), normal);

                                newObject.transform.position += Vector3.down * 1;

                                newObject.transform.localPosition = wPos;
                                newObject.transform.parent = transform;
                            }
                        }
                    }
                }
                */
                #endregion
            }
        }

        SpawnAllObjects(spawnPos.ToArray());
    }

    private GameObject GetSpawnedObject(ref List<Transform> p_spawnedObjects, GameObject p_objectToPlace)
    {
        foreach (Transform prevObject in p_spawnedObjects)
        {
            if (prevObject != null)
            {
                if (PrefabUtility.IsPartOfAnyPrefab(prevObject.gameObject))
                {
                    GameObject foundPrefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(prevObject.gameObject).transform.root.gameObject;

                    if (p_objectToPlace == foundPrefabAsset)
                    {
                        p_spawnedObjects.Remove(prevObject);
                        return foundPrefabAsset;
                    }
                }
            }
        }

        return null;
    }

    private void SpawnAllObjects(Vector3[] p_spawnPositions)
    {
        List<Transform> previouslySpawnedObjects = new List<Transform>();
        previouslySpawnedObjects.AddRange(m_terrain.GetComponentsInChildren<Transform>());

        for (int i = 0; i < p_spawnPositions.Length; i++)
        {
            ShowProgressBar(i, p_spawnPositions.Length, "Spawning object number " + i + " out of " + p_spawnPositions.Length + " objects");

            GameObject objectToSpawn = m_testObjects[Random.Range(0, m_testObjects.Length)];

            GameObject prevObject = GetSpawnedObject(ref previouslySpawnedObjects, objectToSpawn);

            if (prevObject == null)
            {
                SpawnObject(p_spawnPositions[i], objectToSpawn, true);
            }
            else
            {
                SpawnObject(p_spawnPositions[i], objectToSpawn, false);
            }


        }

        foreach (Transform oldObject in previouslySpawnedObjects)
        {
            if (oldObject != m_terrain.transform)
            {
                DestroyImmediate(oldObject.root.gameObject);
            }
        }
    }

    private void SpawnObject(Vector3 p_worldPos, GameObject p_objectToSpawn, bool spawnNewObject)
    {
        GameObject newObject;

        if (spawnNewObject)
        {
            newObject = (GameObject)PrefabUtility.InstantiatePrefab(p_objectToSpawn);
        }
        else
        {
            newObject = p_objectToSpawn;
        }

        if (newObject == null)
        {
            Debug.LogError("Error instantiating prefab");
            return;
        }

        newObject.transform.rotation = Quaternion.Euler(newObject.transform.rotation.eulerAngles.x, Random.Range(0, 360), newObject.transform.rotation.eulerAngles.z);
        newObject.transform.position += Vector3.down * 1;

        newObject.transform.localPosition = p_worldPos;

        newObject.transform.parent = m_terrain.transform;
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

    private void OnDrawGizmos()
    {
        foreach (KeyValuePair<Terrain, Cell[,]> item in terrainCells)
        {
            foreach (Cell cell in item.Value)
            {
                if ((UnityEditor.SceneView.lastActiveSceneView.camera.transform.position - cell.bounds.center).magnitude > 150f) continue;

                foreach (Cell subCell in cell.subCells)
                {
                    if (subCell == null) continue;
                    Gizmos.color = new Color(1f, 0.05f, 0.05f, 1f);
                    Gizmos.DrawWireCube(new Vector3(subCell.bounds.center.x, subCell.bounds.center.y, subCell.bounds.center.z),
                        new Vector3(subCell.bounds.size.x, subCell.bounds.size.y, subCell.bounds.size.z));
                }

                Gizmos.color = new Color(0.66f, 0.66f, 1f, 0.25f);
                Gizmos.DrawWireCube(
                    new Vector3(cell.bounds.center.x, cell.bounds.center.y, cell.bounds.center.z),
                    new Vector3(cell.bounds.size.x, cell.bounds.size.y, cell.bounds.size.z)
                    );
            }
        }
    }

}
