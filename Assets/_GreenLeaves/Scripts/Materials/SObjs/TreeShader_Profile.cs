using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TreeShaders", menuName = "ScriptableObjects/Shaders/TreeShader", order = 1)]
public class TreeShader_Profile : ScriptableObject
{
    #region treeLeafShader
    [Space]
    [Header ("Base/Diffuse Color")]
    [ColorUsage(false)]
    public Color Base_Color;
    public Texture Albedo_Tex;
    [Space]

    [Header ("Light Cutoff")]
    [Range(0, 1)]
    public float LightCutoff;
    [Range(0, 1)]
    public float TextureCutoff;
    [Space]

    [Header("Rim")]
    [Range(0,1)]
    public float RimSize;
    [ColorUsage(false, true)]
    public Color RimColor;
    [Range(0, 1)]
    public int ShadowRim;
    [Space]
    
    [Header("Emission")]
    [ColorUsage(false, true)]
    public Color Emission;
    [Space]
    
    [Header("Displacement")]
    public Texture Displacement;
    public Vector2 Displace_Tiling;
    public Vector2 Displace_Offset;
    [Space]
    public float Displace_Amount;
    public float Displace_Speed;
    [Space]
    
    [Header("SubSurface Scattering")]
    [ColorUsage(false, true)]
    public Color SSS_Color;
    public float AreaConcentration;
    #endregion treeLeafShader

}
