using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using Staggart.VegetationSpawner;

public class TerrainTexture : MonoBehaviour
{
    [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
    public TerrainTextureSettings m_terrainTextureSettings;

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

                for (int i = 0; i < m_terrainTextureSettings.m_splatMapSettings.Length; i++)
                {
                    m_terrainTextureSettings.m_splatMapSettings[i].SetLayer(ref terrainLayers);
                }

                terrainData.SetTerrainLayersRegisterUndo(terrainLayers.ToArray(), "Terrain Layers");


                for (int i = 0; i < m_terrainTextureSettings.m_splatMapSettings.Length; i++)
                {
                    float splatValue = 0;

                    m_terrainTextureSettings.m_splatMapSettings[i].SetMap(heightNormalized, steepnessNormalized, ref splatValue);

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
    public enum MaskType { slopeMask, heightMask }
    public MaskType m_currentMaskType;

    [Range(0, 1)]
    public float m_totalStrength;

    //[HideIf("m_currentMaskType", MaskType.heightMask)]
    //public SlopeMask m_slopeMask;
    //[HideIf("m_currentMaskType", MaskType.slopeMask)]
    //public HeightMask m_heightMask;

    public TerrainLayer m_layerTexture;

    public void SetLayer(ref List<TerrainLayer> p_terrainLayers)
    {
        p_terrainLayers.Add(m_layerTexture);
    }

    public void SetMap(float p_normalizedHeight, float p_normalizedSteepness, ref float p_splatValue)
    {
        /*
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
        */
    }
}


