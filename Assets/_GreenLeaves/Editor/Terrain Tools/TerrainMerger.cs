using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using System.IO;

public class TerrainMerger : OdinEditorWindow
{
    public bool m_overwriteTerrainData;
    public bool m_testMode;
    public Terrain m_sampleTerrain;
    public Column[] m_terrainColumns;

    [System.Serializable]
    public struct Column
    {
        public Terrain[] m_columnContent;
    }

    [MenuItem("Tools/Terrain Merger")]
    private static void OpenWindow()
    {
        GetWindow<TerrainMerger>().Show();
    }

    [Button("Make New Terrain")]
    private void MakeNewTerrain()
    {
        #region File creation
        string savePath = "Assets/_GreenLeaves/Terrain Data/";
        string fileName = "TD_" + SceneManager.GetActiveScene().name + ".asset";
        fileName = fileName.Replace(" ", "");

        if (!Directory.Exists(savePath))
        {
            Debug.LogError("Terrain data folder path is missing");
            return;
        }

        string absoluteSavePath = Application.dataPath + "/_GreenLeaves/Terrain Data/";

        if (File.Exists(absoluteSavePath + fileName))
        {
            if (!m_overwriteTerrainData)
            {
                Debug.LogError("A terrain data already exists for this scene");
                return;
            }
        }

        TerrainData newTerrainData = new TerrainData();

        if (!m_testMode)
        {
            AssetDatabase.CreateAsset(newTerrainData, savePath + fileName);
            AssetDatabase.SaveAssets();
        }
        #endregion

        //This is currently kinda hardcoded to work for the terrain that we have rn but I think thats ok
        int terrainTileDimensions = m_terrainColumns.Length;
        newTerrainData.heightmapResolution = ((m_sampleTerrain.terrainData.heightmapResolution - 1) * terrainTileDimensions) + 1;

        //newTerrainData.alphamapResolution = newTerrainData.heightmapResolution - 1;

        GameObject terrainObject = Terrain.CreateTerrainGameObject(newTerrainData);
        newTerrainData.size = new Vector3((terrainTileDimensions + 1) * m_sampleTerrain.terrainData.size.x, m_sampleTerrain.terrainData.size.y, (terrainTileDimensions + 1) * m_sampleTerrain.terrainData.size.x);

        terrainObject.transform.position += Vector3.left * (newTerrainData.size.x / (terrainTileDimensions + 1));
        terrainObject.transform.position += Vector3.back * (newTerrainData.size.x / (terrainTileDimensions + 1));

        for (int x = 0; x < m_terrainColumns.Length; x++)
        {
            for (int y = 0; y < m_terrainColumns[x].m_columnContent.Length; y++)
            {
                TerrainData oldTerrainData = m_terrainColumns[x].m_columnContent[y].terrainData;
                SetSingleTerrainData(oldTerrainData, newTerrainData, new Vector2Int(x, y));
            }
        }

        terrainObject.GetComponent<Terrain>().Flush();
    }

    private void SetSingleTerrainData(TerrainData p_oldData, TerrainData p_newData, Vector2Int p_positionIndex)
    {
        float[,] heights = p_oldData.GetHeights(0, 0, p_oldData.heightmapResolution, p_oldData.heightmapResolution);

        Vector2Int pos = new Vector2Int(p_oldData.heightmapResolution * p_positionIndex.x, p_oldData.heightmapResolution * p_positionIndex.y);

        p_newData.SetHeights(pos.x, pos.y, heights);
    }
}
