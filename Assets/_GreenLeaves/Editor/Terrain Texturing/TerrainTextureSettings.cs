using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TerrainTextureSettings : ScriptableObject
{
    public SplatMapSetting[] m_splatMapSettings;
}

[System.Serializable]
public class SplatMapSetting
{
    [Range(0, 1)]
    public float m_totalStrength;

    public TerrainLayer m_layerTexture;

    [Header("Height Mask Poperties")]
    public HeightMask m_heightMask;

    [Header("Slope Mask Poperties")]
    public SlopeMask m_slopeMask;

    [Header("Curvature Mask Poperties")]
    public CurvatureMask m_curvatureMask;

    public void SetLayer(ref List<TerrainLayer> p_terrainLayers)
    {
        p_terrainLayers.Add(m_layerTexture);
    }

	public float GetSplatValue(float p_normalizedHeight, float p_curvature, float p_slope)
	{
        //float heightSpawnChance = m_heightMask.GetMaskValue(p_normalizedHeight);
		float curvatureSpawnRange = m_curvatureMask.GetMaskValue(p_curvature);
		float slopeSpawnChance = m_slopeMask.GetMaskValue(p_slope);

        if (!m_slopeMask.m_useMask)
        {
            return m_totalStrength;
        }

		return slopeSpawnChance;
	}
}
