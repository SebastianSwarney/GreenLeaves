using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class CustomImageEffect : MonoBehaviour
{
    public Material material;
    public Camera my_camera;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }

    private void Start()
    {
        my_camera = gameObject.GetComponent<Camera>();

        my_camera.depthTextureMode = DepthTextureMode.Depth;

        print(my_camera.depthTextureMode);
    }

}
