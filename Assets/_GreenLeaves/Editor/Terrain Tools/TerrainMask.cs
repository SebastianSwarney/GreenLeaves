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
