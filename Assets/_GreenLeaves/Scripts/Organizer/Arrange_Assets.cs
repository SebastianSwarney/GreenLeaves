using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Arrange_Assets : MonoBehaviour
{
    public Vector2 spacing;

    public GameObject[] objs_to_sort;


    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnValidate()
    {
        spacer2();
    }

    void spacer() {

        for (int i = 0; i < objs_to_sort.Length; i++)
        {
            objs_to_sort[i].transform.position = new Vector3(0,0,0);
        }

        int row = 0;

        for (int i = 1; i < objs_to_sort.Length+1; i++)
        {
            objs_to_sort[i].transform.position += new Vector3(objs_to_sort[i].transform.position.x + spacing.x * (i % 10), 0, objs_to_sort[i].transform.position.z - spacing.y * row);

            if ((i+1 % 10) == 0)
                row++;
        }


        

    }

    void spacer2()
    {
        for (int i = 0; i < objs_to_sort.Length; i++)
        {
            objs_to_sort[i].transform.position = new Vector3(0, 0, 0);
        }

        int row2 = 0;

        for (int i = 0; i < objs_to_sort.Length; i++)
        {
            objs_to_sort[i].transform.position += new Vector3(objs_to_sort[i].transform.position.x + spacing.x * (i % 3), 0, objs_to_sort[i].transform.position.z - spacing.y * row2);

            if ((i+1) % 3 == 0)
                row2++;
        }
    }

}
