using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;

public class TerrainTexture : MonoBehaviour
{
    public SplatMapSetting[] m_splatMapSettings;

    public int m_terrainColumnsCount;
    public int m_terrainRows;

    public Column[] m_terrainColumns;

    [System.Serializable]
    public struct Column
    {
        public Terrain[] m_columnContent;
    }


    [Button("Set Textures")]
    private void SetTextureToAllTerrains()
    {
        Terrain[] childTerrains = GetComponentsInChildren<Terrain>();

        foreach (Terrain terrain in childTerrains)
        {
            SetTexture(terrain);
        }
    }

    [Button("Set Neighbors")]
    private void SetNeighbors()
    {
        Terrain[] childTerrains = GetComponentsInChildren<Terrain>();

        foreach (Terrain terrain in childTerrains)
        {
            TerrainNeighbors terrainNeighbors = terrain.GetComponent<TerrainNeighbors>();

            terrain.SetNeighbors(terrainNeighbors.m_left, terrainNeighbors.m_top, terrainNeighbors.m_right, terrainNeighbors.m_bottom);

            terrain.Flush();
        }
    }

    [Button("Make New Terrain")]
    private void MakeSingleTerrain()
    {
        Terrain[] childTerrains = GetComponentsInChildren<Terrain>();

        //This just uses the first of the children terrain as a shorthand for getting variables
        Terrain sampleTerrain = childTerrains[0];

        TerrainData newTerrainData = new TerrainData();

        AssetDatabase.CreateAsset(newTerrainData, "Assets/MyTerrainData.asset");
        AssetDatabase.SaveAssets();

        newTerrainData.heightmapResolution = ((sampleTerrain.terrainData.heightmapResolution - 1) * m_terrainColumnsCount) + 1;
        //newTerrainData.heightmapResolution = 4097;
        GameObject terrainObject = Terrain.CreateTerrainGameObject(newTerrainData);
        newTerrainData.size = new Vector3((m_terrainColumnsCount + 1) * sampleTerrain.terrainData.size.x, sampleTerrain.terrainData.size.y, (m_terrainRows + 1) * sampleTerrain.terrainData.size.x);


        terrainObject.transform.position += Vector3.left * (newTerrainData.size.x / (m_terrainColumnsCount + 1));
        terrainObject.transform.position += Vector3.back * (newTerrainData.size.x / (m_terrainColumnsCount + 1));

        for (int x = 0; x < m_terrainColumns.Length; x++)
        {
            for (int y = 0; y < m_terrainColumns[x].m_columnContent.Length; y++)
            {
                TerrainData oldTerrainData = m_terrainColumns[x].m_columnContent[y].terrainData;
                SetSingleTerrainData(oldTerrainData, newTerrainData, new Vector2Int(x, y));
            }
        }


        //TerrainData terrainData = childTerrains[0].terrainData;
        //float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        //newTerrainData.SetHeights(0, 0, heights);

        /*
        TerrainData newTerrainData = new TerrainData();
        newTerrainData.heightmapResolution = 1025;
        GameObject terrainObject = Terrain.CreateTerrainGameObject(newTerrainData);
        newTerrainData.size = new Vector3(2000, 1200, 2000);
        TerrainData terrainData = childTerrains[0].terrainData;

        float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        for (int y = 0; y < terrainData.heightmapResolution; y++)
        {
            for (int x = 0; x < terrainData.heightmapResolution; x++)
            {
                heights[x, y] = heights[x, y] * 0.5f;
            }
        }

        //newTerrainData.SetHeights(newTerrainData.heightmapResolution / 2, newTerrainData.heightmapResolution / 2, heights);
        newTerrainData.SetHeights(0, 0, heights);
        */
    }

    private void SetSingleTerrainData(TerrainData p_oldData, TerrainData p_newData, Vector2Int p_positionIndex)
    {
        float[,] heights = p_oldData.GetHeights(0, 0, p_oldData.heightmapResolution, p_oldData.heightmapResolution);

        Vector2Int pos = new Vector2Int(p_oldData.heightmapResolution * p_positionIndex.x, p_oldData.heightmapResolution * p_positionIndex.y);

        p_newData.SetHeights(pos.x, pos.y, heights);
    }

    private void SetTexture(Terrain p_terrain)
    {
        TerrainData terrainData = p_terrain.terrainData;

        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float y_01 = (float)y / (float)terrainData.alphamapHeight;
                float x_01 = (float)x / (float)terrainData.alphamapWidth;

                float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapResolution), Mathf.RoundToInt(x_01 * terrainData.heightmapResolution));
                Vector3 normal = terrainData.GetInterpolatedNormal(y_01, x_01);
                float steepness = terrainData.GetSteepness(y_01, x_01);
                float[] splatWeights = new float[terrainData.alphamapLayers];

                float steepnessNormalized = steepness / 90f;
                float heightNormalized = height / terrainData.heightmapResolution;

                List<TerrainLayer> terrainLayers = new List<TerrainLayer>();

                for (int i = 0; i < m_splatMapSettings.Length; i++)
                {
                    m_splatMapSettings[i].SetLayer(ref terrainLayers);
                }

                terrainData.SetTerrainLayersRegisterUndo(terrainLayers.ToArray(), "Terrain Layers");


                for (int i = 0; i < m_splatMapSettings.Length; i++)
                {
                    float splatValue = 0;

                    m_splatMapSettings[i].SetMap(heightNormalized, steepnessNormalized, ref splatValue);

                    splatWeights[i] = splatValue;
                }



                // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                float z = splatWeights.Sum();

                // Loop through each terrain texture
                for (int i = 0; i < terrainData.alphamapLayers; i++)
                {
                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;

                    // Assign this point to the splatmap array
                    splatmapData[x, y, i] = splatWeights[i];
                }
            }
        }

        // Finally assign the new splatmap to the terrainData:
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }
}

[System.Serializable]
public class SplatMapSetting
{
    [Range(0, 1)]
    public float m_totalStrength;

    public SlopeMask m_slopeMask;
    public HeightMask m_heightMask;

    public TerrainLayer m_layerTexture;

    public void SetLayer(ref List<TerrainLayer> p_terrainLayers)
    {
        p_terrainLayers.Add(m_layerTexture);
    }

    public void SetMap(float p_normalizedHeight, float p_normalizedSteepness, ref float p_splatValue)
    {


        if (m_totalStrength != 0)
        {
            float slopeValue = m_slopeMask.ApplyMask(p_normalizedHeight, p_normalizedSteepness, ref p_splatValue);

            float heightValue = m_heightMask.ApplyMask(p_normalizedHeight, p_normalizedSteepness, ref p_splatValue);

            if (m_slopeMask.m_useMask)
            {
                p_splatValue = slopeValue * m_totalStrength;
            }

            if (m_heightMask.m_useMask)
            {
                p_splatValue = heightValue * m_totalStrength;
            }

            if (m_heightMask.m_useMask && m_slopeMask.m_useMask)
            {
                p_splatValue = (slopeValue * heightValue) * m_totalStrength;
            }

            if (!m_heightMask.m_useMask && !m_slopeMask.m_useMask)
            {
                p_splatValue = m_totalStrength;
            }
        }
    }
}

[System.Serializable]
public abstract class MaskTypeBase
{
    public bool m_useMask;

    [Range(0, 1)]
    public float m_strength;

    public abstract float ApplyMask(float p_normalizedHeight, float p_normalizedSteepness, ref float p_splatValue);

}

[System.Serializable]
public class SlopeMask : MaskTypeBase
{
    [MinMaxSlider(0, 1, true)]
    public Vector2 m_minMaxSlope;

    public AnimationCurve m_slopeCurve;

    public override float ApplyMask(float p_normalizedHeight, float p_normalizedSteepness, ref float p_splatValue)
    {
        float currentSteepnessValue = Mathf.InverseLerp(m_minMaxSlope.x, m_minMaxSlope.y, p_normalizedSteepness);

        return m_slopeCurve.Evaluate(currentSteepnessValue) * m_strength;
    }
}

[System.Serializable]
public class HeightMask : MaskTypeBase
{
    [MinMaxSlider(0, 1, true)]
    public Vector2 m_minMaxHeight;

    public AnimationCurve m_heightCurve;

    public override float ApplyMask(float p_normalizedHeight, float p_normalizedSteepness, ref float p_splatValue)
    {
        float currentHeightValue = Mathf.InverseLerp(m_minMaxHeight.x, m_minMaxHeight.y, p_normalizedHeight);

        return m_heightCurve.Evaluate(currentHeightValue) * m_strength;
    }
}

