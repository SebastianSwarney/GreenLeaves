using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using Staggart.VegetationSpawner;
using Sirenix.OdinInspector.Editor;

public class TerrainTexture : OdinEditorWindow
{
    public Terrain m_terrain;

    [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
    public TerrainTextureSettings m_terrainTextureSettings;

	[MenuItem("Tools/Terrain Texture")]
    private static void OpenWindow()
    {
        GetWindow<TerrainTexture>().Show();
    }

    [Button("Set Textures")]
    private void SetTextureToAllTerrains()
    {
        SetTexture(m_terrain);
    }

    private void SetTexture(Terrain p_terrain)
    {
        Terrain terrain = m_terrain;
        TerrainData terrainData = p_terrain.terrainData;

        float width = terrain.terrainData.alphamapWidth;
        float length = terrain.terrainData.alphamapHeight;

        List<TerrainLayer> terrainLayers = new List<TerrainLayer>();

        for (int i = 0; i < m_terrainTextureSettings.m_splatMapSettings.Length; i++)
        {
            m_terrainTextureSettings.m_splatMapSettings[i].SetLayer(ref terrainLayers);
        }

        if (terrainData.terrainLayers != terrainLayers.ToArray())
        {
            terrainData.SetTerrainLayersRegisterUndo(terrainLayers.ToArray(), "Terrain Layers");
        }

        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int x = 0; x < width; x ++)
        {
            for (int y = 0; y < length; y ++)
            {
                float xValue = (float)y / (float)length;
                float yValue = (float)x / (float)width;

                float height = terrainData.GetHeight(Mathf.RoundToInt(xValue * terrainData.heightmapResolution), Mathf.RoundToInt(yValue * terrainData.heightmapResolution));
                float heightNormalized = height / terrainData.heightmapResolution;
                float steepness = terrainData.GetSteepness(xValue, yValue);
                Vector3 normal = terrainData.GetInterpolatedNormal(xValue, yValue);


                float curvature = terrain.SampleConvexity(new Vector2(xValue, yValue), 10f);
                curvature = TerrainSampler.ConvexityToCurvature(curvature);

                float[] splatWeights = new float[terrainData.alphamapLayers];

                for (int i = 0; i < m_terrainTextureSettings.m_splatMapSettings.Length; i++)
                {
                    float splatValue = m_terrainTextureSettings.m_splatMapSettings[i].GetSplatValue(heightNormalized, curvature, steepness);
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