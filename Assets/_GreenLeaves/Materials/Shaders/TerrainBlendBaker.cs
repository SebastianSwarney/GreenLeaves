using System;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainBlendBaker : MonoBehaviour
{
    //Shader that renders object based on distance to camera
    public Shader depthShader;

    //The render texture which will store the depth of our terrain
    public RenderTexture depthTexture;

    //The camera this script is attached to
    private Camera cam;

    //The context menu tag allows us to run methods from the inspector
    [ContextMenu("Bake Depth Texture")]
    public void BakeTerrainDepth()
    {
        //Call our update camera method
        UpdateBakingCamera();

        if(depthShader != null && depthTexture != null)
        {
            //Set the camera replacement shader to the depth shader that we will assign in the inspector
            cam.SetReplacementShader(depthShader, "RenderType");
            //set the target render texture of the camera to the depth texture
            cam.targetTexture = depthTexture;
            //Set the render texture we just created as a global shader texture variable
            Shader.SetGlobalTexture("TB_DEPTH", depthTexture);
        }
        else
        {
            Debug.Log("you need to assign the depth shader and depth texture in the inspector");
        }

    }

    private void UpdateBakingCamera()
    {
        if(cam == null)
        {
            cam = GetComponent<Camera>();
        }

        //the total width of the bounding box of our cameras view
        Shader.SetGlobalFloat("TB_SCALE", GetComponent<Camera>().orthographicSize * 2);
        //find the bottom corner of the texture in world scale by subtracting the size of the camera from its x and z position
        Shader.SetGlobalFloat("TB_OFFSET_X", cam.transform.position.x - cam.orthographicSize);
        Shader.SetGlobalFloat("TB_OFFSET_Z", cam.transform.position.z - cam.orthographicSize);
        //we'll also need the relative y position of the camera, Lets get this by subtracting the far clip plane form the camera y position

        Shader.SetGlobalFloat("TB_OFFSET_Y", cam.transform.position.y - cam.farClipPlane);
        //we'll also need the far clip plane intself to know the range of y values in the depth texture
        Shader.SetGlobalFloat("TB_FARCLIP", cam.farClipPlane);

        //NOTE: some of the arithmatic here could be moved to the shader but keeping it here makes the shader cleaner

    }

    
}
