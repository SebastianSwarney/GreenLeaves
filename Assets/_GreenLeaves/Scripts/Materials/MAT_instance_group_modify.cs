using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MAT_instance_group_modify : MonoBehaviour
{
    #region material instances
    public GameObject[] go_to_modify;
    public Material[] go_materials;
    #endregion material instances


    #region properties
    public Color color;
    public Color ambient_color;
    public Color specular_color;
    [Range(0, 1)]
    public float glosiness;
    public Color rim_color;
    [Range(0, 1)]
    public float rim_amount;
    [Range(0, 1)]
    public float rim_threshold;
    #endregion properties


    void get_materials_array() {
        
        go_materials = new Material[go_to_modify.Length];

        print("go_material length: " + go_materials.Length);

        int i = 0;

        foreach (GameObject go in go_to_modify)
        {
            go_materials[i] = go.GetComponent<MeshRenderer>().material;
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


        foreach (Material go in go_materials)
        {

            go.color = color;
            go.SetColor(ambient_colorID, ambient_color);
            go.SetColor(specular_colorID, specular_color);
            go.SetFloat(GlosinessID, glosiness);
            go.SetColor(Rim_ColorID, rim_color);
            go.SetFloat(Rim_AmountID, rim_amount);
            go.SetFloat(Rim_ThresholdID, rim_threshold);
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
