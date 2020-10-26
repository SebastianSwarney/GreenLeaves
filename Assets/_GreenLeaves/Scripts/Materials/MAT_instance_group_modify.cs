using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MAT_instance_group_modify : MonoBehaviour
{
    #region material instances
    public GameObject[] go_to_modify;
    public Renderer[] go_renderer;
    #endregion material instances


    #region properties
    public Color color;
    [ColorUsage(false,true)]
    public Color ambient_color;
    [ColorUsage(false, true)]
    public Color specular_color;
    [Range(0, 50)]
    public float glosiness;
    public Color rim_color;
    [Range(0, 1)]
    public float rim_amount;
    [Range(0, 1)]
    public float rim_threshold;
    #endregion properties


    void get_materials_array() {
        
        go_renderer = new Renderer[go_to_modify.Length];
        int i = 0;

        foreach (GameObject go in go_to_modify)
        {
            go_renderer[i] = go.GetComponent<Renderer>();
            i++;
        }
    }

    void set_material_properties() {
        
        int ambient_colorID = Shader.PropertyToID("_AmbientColor");
        int specular_colorID = Shader.PropertyToID("_SpecularColor");
        int GlosinessID = Shader.PropertyToID("_Glosiness");
        int Rim_ColorID = Shader.PropertyToID("_RimColor");
        int Rim_AmountID = Shader.PropertyToID("_RimAmount");
        int Rim_ThresholdID = Shader.PropertyToID("_RimThreshold");

        foreach (Renderer go in go_renderer)
        {
            go.sharedMaterial.color = color;
            go.sharedMaterial.SetColor(ambient_colorID, ambient_color);
            go.sharedMaterial.SetColor(specular_colorID, specular_color);
            go.sharedMaterial.SetFloat(GlosinessID, glosiness);
            go.sharedMaterial.SetColor(Rim_ColorID, rim_color);
            go.sharedMaterial.SetFloat(Rim_AmountID, rim_amount);
            go.sharedMaterial.SetFloat(Rim_ThresholdID, rim_threshold);
        }
    }

    private void Start()
    {
        get_materials_array();
    }


    // Update is called once per frame
    void OnValidate()
    {
        set_material_properties();
    }
}
