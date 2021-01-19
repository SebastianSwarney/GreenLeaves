using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;


[ExecuteInEditMode]
public class TerrainTexture : MonoBehaviour
{
    public SplatMapSetting[] m_splatMapSettings;

    private void Update()
    {
        //SetTexture();
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

