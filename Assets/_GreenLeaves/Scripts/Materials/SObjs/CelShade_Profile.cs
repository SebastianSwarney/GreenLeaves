using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CelShader", menuName = "ScriptableObjects/Shaders/CelShader", order = 0)]
public class CelShade_Profile : ScriptableObject
{
    #region CelShader
    [Space]
    [Header("Base/Diffuse Color")]
    [ColorUsage(false)]
    public Color Base_Color;
    public Texture Albedo_Tex;
    public Texture Normal_Tex;
    [Space]

    [Header("Light Cutoff")]
    [Range(0, 1)]
    public float LightCutoff;
    [Range(1, 4)]
    public float ShadowBands;

    [Header("Specular")]
    public Texture Specular_Tex;
    [Range(0, 1)]
    public float Smoothness;
    [ColorUsage(false, true)]
    public Color SpecularColor;

    [Header("Rim")]
    [Range(0, 1)]
    public float RimSize;
    [ColorUsage(false, true)]
    public Color RimColor;
    [Range(0, 1)]
    public int ShadowRim;
    [Space]

    [Header("Emission")]
    [ColorUsage(false, true)]
    public Color Emission;
    #endregion CelShader
}