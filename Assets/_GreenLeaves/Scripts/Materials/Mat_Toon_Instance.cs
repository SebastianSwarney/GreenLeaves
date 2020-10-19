using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Mat_Toon_Instance : MonoBehaviour
{
    public GameObject go;
    public Color color;
    public Color ambient_color;

    public Material material;

    Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        //rend = GetComponent<Renderer>();
        //rend.material.shader = Shader.Find("Ambient Color");

        go = this.gameObject;
        material = go.GetComponent<MeshRenderer>().material;
        
    }

    private void OnValidate()
    {
        material.color = color;

        int ambient_colorID = Shader.PropertyToID("_AmbientColor");

        material.SetColor(ambient_colorID, ambient_color);
    }
}
