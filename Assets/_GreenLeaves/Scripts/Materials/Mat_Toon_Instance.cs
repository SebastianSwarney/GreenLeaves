using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Mat_Toon_Instance : MonoBehaviour
{
    public GameObject go;
    public Color color;
    public Color ambient_color;

    public Renderer go_rend;


    // Start is called before the first frame update
    void Start()
    {
        go_rend = gameObject.GetComponent<Renderer>();   
    }

    private void OnValidate()
    {
        go_rend.sharedMaterial.color = color;


        int ambient_colorID = Shader.PropertyToID("_AmbientColor");



        go_rend.sharedMaterial.SetColor(ambient_colorID, ambient_color);
    }
}
