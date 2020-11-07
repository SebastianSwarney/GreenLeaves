using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class CustomImageEffect : MonoBehaviour
{

    public Material mat;
    public Camera my_camera;

    public DepthTextureMode depthmode;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);   
    }


    private void OnValidate()
    {

        //CameraDepthModeSet(DepthTextureMode.Depth);

        //my_camera = gameObject.GetComponent<Camera>();

        CameraDepthModeSet(depthmode);


    }



    void CameraDepthModeSet(DepthTextureMode mode)
    {
        my_camera = gameObject.GetComponent<Camera>();

        switch (depthmode)
        {
            case DepthTextureMode.Depth:
                my_camera.depthTextureMode = DepthTextureMode.Depth;
                break;
            case DepthTextureMode.DepthNormals:
                my_camera.depthTextureMode = DepthTextureMode.DepthNormals;
                break;
            case DepthTextureMode.MotionVectors:
                my_camera.depthTextureMode = DepthTextureMode.MotionVectors;
                break;
            case DepthTextureMode.None:
                my_camera.depthTextureMode = DepthTextureMode.None;
                break;
        }
    }
}
