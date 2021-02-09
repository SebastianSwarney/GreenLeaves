using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu]
public class TerrainTextureSettings : ScriptableObject
{
    public SplatMapSetting[] m_splatMapSettings;
}

[System.Serializable]
public class SplatMapSetting
{
    [FoldoutGroup("Splat Map Setting")]
    [Range(0, 1)]
    public float m_totalStrength;

    [FoldoutGroup("Splat Map Setting")]
    public TerrainLayer m_layerTexture;

    [FoldoutGroup("Splat Map Setting")]
    [Header("Height Mask Poperties")]
    public HeightMask m_heightMask;

    [FoldoutGroup("Splat Map Setting")]
    [Header("Slope Mask Poperties")]
    public SlopeMask m_slopeMask;

    [FoldoutGroup("Splat Map Setting")]
    [Header("Curvature Mask Poperties")]
    public CurvatureMask m_curvatureMask;

    public void SetLayer(ref List<TerrainLayer> p_terrainLayers)
    {
        p_terrainLayers.Add(m_layerTexture);
    }

	public float GetSplatValue(float p_normalizedHeight, float p_curvature, float p_slope)
	{
		if (!m_heightMask.m_useMask && !m_slopeMask.m_useMask && ! m_curvatureMask.m_useMask)
		{
            return m_totalStrength;
		}

        float splatValue = 1;

		if (m_heightMask.m_useMask)
		{
            float heightSpawnChance = m_heightMask.GetMaskValue(p_normalizedHeight);
            splatValue *= heightSpawnChance;
        }

		if (m_slopeMask.m_useMask)
		{
            float slopeSpawnChance = m_slopeMask.GetMaskValue(p_slope);
            splatValue *= slopeSpawnChance;
        }

        if (m_curvatureMask.m_useMask)
        {
            float curvatureSpawnRange = m_curvatureMask.GetMaskValue(p_curvature);
            splatValue *= curvatureSpawnRange;
        }

		return splatValue * m_totalStrength;
	}
}
