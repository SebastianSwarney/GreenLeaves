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
        /*
        Terrain[] childTerrains = m_terrain.GetComponentsInChildren<Terrain>();

        foreach (Terrain terrain in childTerrains)
        {
            SetTexture(terrain);
        }
        */

        SetTexture(m_terrain);
    }

    private void SetTexture(Terrain p_terrain)
    {
        Terrain terrain = m_terrain;
        TerrainData terrainData = p_terrain.terrainData;

        List<TerrainLayer> terrainLayers = new List<TerrainLayer>();

        for (int i = 0; i < m_terrainTextureSettings.m_splatMapSettings.Length; i++)
        {
            m_terrainTextureSettings.m_splatMapSettings[i].SetLayer(ref terrainLayers);
        }

        if (terrainData.terrainLayers != terrainLayers.ToArray())
        {
            //terrainData.SetTerrainLayersRegisterUndo(terrainLayers.ToArray(), "Terrain Layers");
        }

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

                float steepnessNormalized = steepness / 90f;
                float heightNormalized = height / terrainData.heightmapResolution;

                float curvature = terrain.SampleConvexity(new Vector2(y_01, x_01));
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