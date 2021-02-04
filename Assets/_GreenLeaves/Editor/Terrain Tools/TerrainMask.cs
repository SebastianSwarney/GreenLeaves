using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class TerrainMask
{
    public bool m_useMask = true;

    [Range(0, 1)]
    public float m_strength = 1f;

    public abstract float GetMaskValue(float p_normalizedValue);
}

[System.Serializable]
public class SlopeMask : TerrainMask
{
    [MinMaxSlider(0, 90, true)]
    public Vector2 m_minMaxSlope;

    public AnimationCurve m_slopeCurve;

    public override float GetMaskValue(float p_normalizedValue)
    {
        if (p_normalizedValue < 0)
        {
            Debug.Log(p_normalizedValue);
        }

        if (p_normalizedValue < m_minMaxSlope.x)
        {
            return 0f;
        }
        else if (p_normalizedValue > m_minMaxSlope.y)
        {
            return 0f;
        }

        float currentSteepnessValue = Mathf.InverseLerp(m_minMaxSlope.x, m_minMaxSlope.y, p_normalizedValue);

        return m_slopeCurve.Evaluate(currentSteepnessValue) * m_strength;
    }
}

[System.Serializable]
public class HeightMask : TerrainMask
{
    [MinMaxSlider(0, 1, true)]
    public Vector2 m_minMaxHeight;

    public AnimationCurve m_heightCurve;

    public override float GetMaskValue(float p_normalizedValue)
    {
        if (p_normalizedValue < m_minMaxHeight.x)
        {
            return 0f;
        }
        else if (p_normalizedValue > m_minMaxHeight.y)
        {
            return 0f;
        }

        float currentHeightValue = Mathf.InverseLerp(m_minMaxHeight.x, m_minMaxHeight.y, p_normalizedValue);

        return m_heightCurve.Evaluate(currentHeightValue) * m_strength;
    }
}

[System.Serializable]
public class CurvatureMask : TerrainMask
{
    [MinMaxSlider(0, 1, true)]
    public Vector2 m_minMaxCurvature;

    public AnimationCurve m_curvatureCurve;

    public override float GetMaskValue(float p_normalizedValue)
    {
        if (p_normalizedValue < m_minMaxCurvature.x)
        {
            return 0f;
        }
        else if (p_normalizedValue > m_minMaxCurvature.y)
        {
            return 0f;
        }

        float currentHeightValue = Mathf.InverseLerp(m_minMaxCurvature.x, m_minMaxCurvature.y, p_normalizedValue);

        return m_curvatureCurve.Evaluate(currentHeightValue) * m_strength;
    }
}

[System.Serializable]
public class NoiseMask : TerrainMask
{
    [MinMaxSlider(0, 1, true)]
    public Vector2 m_minMaxNoiseValue;

    public AnimationCurve m_curvatureCurve;

    [PreviewField(Height = 256, Alignment = ObjectFieldAlignment.Left)]
    public Texture2D m_noiseTexture;

    public float xOrg;
    public float yOrg;

    public float m_noiseAmplitude;
    public float m_noiseFrequency;

    public float scale = 1.0F;

    public override float GetMaskValue(float p_normalizedValue)
    {
        if (p_normalizedValue < m_minMaxNoiseValue.x)
        {
            return 0f;
        }
        else if (p_normalizedValue > m_minMaxNoiseValue.y)
        {
            return 0f;
        }

        float currentHeightValue = Mathf.InverseLerp(m_minMaxNoiseValue.x, m_minMaxNoiseValue.y, p_normalizedValue);

        return m_curvatureCurve.Evaluate(currentHeightValue) * m_strength;
    }

    private void CreateNoiseTexture()
    {
        Terrain terrain = null;

        float width = terrain.terrainData.detailWidth;
        float length = terrain.terrainData.detailHeight;

        int adjustmentAmount = terrain.terrainData.detailWidth / 10;

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
}
