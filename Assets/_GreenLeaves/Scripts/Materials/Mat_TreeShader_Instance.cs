using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Mat_TreeShader_Instance : MonoBehaviour
{
    Renderer go_rend;

    public TreeShader_Profile tree_profile;

    #region tree_properties
    [Space]
    [Header("Base/Diffuse Color")]
    [ColorUsage(false)]
    Color Base_Color;
    Texture Albedo_Tex;
    [Space]

    [Header("Light Cutoff")]
    [Range(0, 1)]
    float LightCutoff;
    [Range(0, 1)]
    float TextureCutoff;
    [Space]

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
    [Space]

    [Header("Displacement")]
    Texture Displacement;
    Vector2 Displace_Tiling;
    Vector2 Displace_Offset;
    [Space]
    float Displace_Amount;
    float Displace_Speed;
    [Space]

    [Header("SubSurface Scattering")]
    [ColorUsage(false, true)]
    Color SSS_Color;
    float AreaConcentration;
    #endregion tree_properties

    #region prop_ID
    int colorID;
    int mainTex;
    int lightCutoff;
    int textureCutOff;

    int rimSize;
    int rimColor;
    int shadowRim;

    int emission;

    int displace_guide;
    int displace_amount;
    int displace_speed;

    int sss_color;
    int sss_concentration;
    #endregion prop_ID


    // Start is called before the first frame update
    void Start()
    {
        go_rend = gameObject.GetComponent<Renderer>();

        #region get_prop_ID
        colorID = Shader.PropertyToID("_Color");
        mainTex = Shader.PropertyToID("_MainTex");
        lightCutoff = Shader.PropertyToID("_LightCutoff");
        textureCutOff = Shader.PropertyToID("_TextureCutoff");

        rimSize = Shader.PropertyToID("_RimSize");
        rimColor = Shader.PropertyToID("_RimColor");
        shadowRim = Shader.PropertyToID("_ShadowedRim");

        emission = Shader.PropertyToID("_Emission");

        displace_guide = Shader.PropertyToID("_DisplacementGuide");
        displace_amount = Shader.PropertyToID("_DisplacementAmount");
        displace_speed = Shader.PropertyToID("_DisplacementSpeed");

        sss_color = Shader.PropertyToID("_SSSColor");
        sss_concentration = Shader.PropertyToID("_SSSConcentration");
        #endregion get_prop_ID
    }

    private void Update()
    {
        if (tree_profile != null)
        {
            Base_Color = tree_profile.Base_Color;
            Albedo_Tex = tree_profile.Albedo_Tex;

            LightCutoff = tree_profile.LightCutoff;
            TextureCutoff = tree_profile.TextureCutoff;

            RimSize = tree_profile.RimSize;
            RimColor = tree_profile.RimColor;
            ShadowRim = tree_profile.ShadowRim;

            Emission = tree_profile.Emission;

            Displacement = tree_profile.Displacement;
            Displace_Tiling = tree_profile.Displace_Tiling;
            Displace_Offset = tree_profile.Displace_Offset;
            Displace_Amount = tree_profile.Displace_Amount;
            Displace_Speed = tree_profile.Displace_Speed;

            SSS_Color = tree_profile.SSS_Color;
            AreaConcentration = tree_profile.AreaConcentration;
        }
        else
        {
            Debug.Log("No shader profile detected");
        }


        #region assign_sharedMaterial_prop
        go_rend.sharedMaterial.SetColor(colorID, Base_Color);
        go_rend.sharedMaterial.SetTexture(mainTex, Albedo_Tex);

        go_rend.sharedMaterial.SetFloat(lightCutoff, LightCutoff);
        go_rend.sharedMaterial.SetFloat(textureCutOff, TextureCutoff);

        go_rend.sharedMaterial.SetFloat(rimSize, RimSize);
        go_rend.sharedMaterial.SetColor(rimColor, RimColor);
        go_rend.sharedMaterial.SetFloat(shadowRim, ShadowRim);

        go_rend.sharedMaterial.SetColor(emission, Emission);

        go_rend.sharedMaterial.SetTexture(displace_guide, Displacement);
        go_rend.sharedMaterial.SetTextureScale(displace_guide, Displace_Tiling);
        go_rend.sharedMaterial.SetTextureOffset(displace_guide, Displace_Offset);
        go_rend.sharedMaterial.SetFloat(displace_amount, Displace_Amount);
        go_rend.sharedMaterial.SetFloat(displace_speed, Displace_Speed);

        go_rend.sharedMaterial.SetColor(sss_color, SSS_Color);
        go_rend.sharedMaterial.SetFloat(sss_concentration, AreaConcentration);
        #endregion assign_sharedMaterial_prop
    }
}