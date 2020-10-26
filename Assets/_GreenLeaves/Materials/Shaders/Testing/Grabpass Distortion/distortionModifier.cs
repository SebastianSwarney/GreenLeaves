using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class distortionModifier : MonoBehaviour
{
    Renderer rend;
    public float scrollSpeed;
    public Vector2 mapRange;

    public float seed;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //float offsetX = Mathf.Lerp(mapRange.x, mapRange.y, 
        //                (Mathf.Sin(Time.time * scrollSpeed + transform.position.x)+1)*.5f);
        //float offsetY = Mathf.Lerp(mapRange.x, mapRange.y, 
        //                (Mathf.Sin(Time.time * scrollSpeed + transform.position.z) +1)*.5f);


        float offsetX = Mathf.Lerp(mapRange.x, mapRange.y,
                            Mathf.PerlinNoise((Time.time + transform.position.x + seed + 62) * scrollSpeed, 
                                                (Time.time + transform.position.z + seed + 29) * scrollSpeed));
        float offsetY = Mathf.Lerp(mapRange.x, mapRange.y,
                            Mathf.PerlinNoise((Time.time + transform.position.x + seed + 529) * scrollSpeed, 
                                                (Time.time + transform.position.z + seed + 49) * scrollSpeed));

        Vector2 offset = new Vector2(offsetX, offsetY);

        rend.sharedMaterial.SetTextureOffset("_DistortionGuide", new Vector2(offset.x, offset.y));
    }
}