using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Mat_CelShader_Instance : MonoBehaviour
{
    GameObject go;
    Material go_mat;
    Renderer go_rend;

    public CelShade_Profile cel_profile;

    #region properties
    [Space]
    [Header("Base/Diffuse Color")]
    [ColorUsage(false)]
    Color Base_Color;
    Texture Albedo_Tex;
    Texture Normal_Tex;
    [Space]

    [Header("Light Cutoff")]
    [Range(0, 1)]
    float LightCutoff;
    [Range(1, 4)]
    float ShadowBands;

    [Header("Specular")]
    Texture Specular_Tex;
    [Range(0, 1)]
    float Smoothness;
    [ColorUsage(false, true)]
    Color SpecularColor;

    [Header("Rim")]
    [Range(0, 1)]
    float RimSize;
    [ColorUsage(false, true)]
    Color RimColor;
    [Range(0, 1)]
    int ShadowRim;
    [Space]

    [Header("Emission")]
    [ColorUsage(false, true)]
    Color Emission;
    #endregion properties

    #region mat_IDs
    int color_ID;
    int mainTex_ID;
    int normalTex_ID;

    int lightCutoff_ID;
    int shadowBands_ID;
    
    int specTex_ID;
    int smoothness_ID;
    int specColor_ID;

    int rimSize_ID;
    int rimColor_ID;
    int shadowRim_ID;

    int emission_ID;
    #endregion mat_IDs

    // Start is called before the first frame update
    void Start()
    {
        go = this.gameObject;
        go_rend = go.GetComponent<Renderer>();

        color_ID = Shader.PropertyToID("_Color");
        mainTex_ID = Shader.PropertyToID("_MainTex");
        normalTex_ID = Shader.PropertyToID("_Normal");
        
        lightCutoff_ID = Shader.PropertyToID("_LightCutoff");
        shadowBands_ID = Shader.PropertyToID("_ShadowBands");
        
        specTex_ID = Shader.PropertyToID("_SpecularMap");
        smoothness_ID = Shader.PropertyToID("_Glossiness");
        specColor_ID = Shader.PropertyToID("_SpecularColor");

        rimSize_ID = Shader.PropertyToID("_RimSize");
        rimColor_ID = Shader.PropertyToID("_RimColor");
        shadowRim_ID = Shader.PropertyToID("_ShadowedRim");

        emission_ID = Shader.PropertyToID("_Emission");
    }

    private void Update()
    {
        if (cel_profile != null)
        {
            Base_Color = cel_profile.Base_Color;
            Albedo_Tex = cel_profile.Albedo_Tex;
            Normal_Tex = cel_profile.Normal_Tex;

            LightCutoff = cel_profile.LightCutoff;
            ShadowBands = cel_profile.ShadowBands;

            Specular_Tex = cel_profile.Specular_Tex;
            Smoothness = cel_profile.Smoothness;
            SpecularColor = cel_profile.SpecularColor;

            RimSize = cel_profile.RimSize;
            RimColor = cel_profile.RimColor;
            ShadowRim = cel_profile.ShadowRim;

            Emission = cel_profile.Emission;
        }
        else
        {
            Debug.Log("No shader profile detected");
        }

        #region assign_material_prop
        go_rend.sharedMaterial.SetColor(color_ID, Base_Color);
        go_rend.sharedMaterial.SetTexture(mainTex_ID, Albedo_Tex);
        go_rend.sharedMaterial.SetTexture(normalTex_ID, Normal_Tex);


        go_rend.sharedMaterial.SetFloat(lightCutoff_ID, LightCutoff);
        go_rend.sharedMaterial.SetFloat(shadowBands_ID, ShadowBands);

        go_rend.sharedMaterial.SetTexture(specTex_ID, Specular_Tex);
        go_rend.sharedMaterial.SetFloat(smoothness_ID, Smoothness);
        go_rend.sharedMaterial.SetColor(specColor_ID, SpecularColor);

        go_rend.sharedMaterial.SetFloat(rimSize_ID, RimSize);
        go_rend.sharedMaterial.SetColor(rimColor_ID, RimColor);
        go_rend.sharedMaterial.SetFloat(shadowRim_ID, ShadowRim);

        go_rend.sharedMaterial.SetColor(emission_ID, Emission);
        #endregion assign_material_prop
    }
}
