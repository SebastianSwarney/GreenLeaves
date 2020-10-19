using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class Arrange_Assets : MonoBehaviour
{
    public Vector2 spacing;

    //public Transform ObjHolder;

    //public List<GameObject> objs_in_holder = new List<GameObject>();

    public GameObject[] objs_to_sort;


    // Start is called before the first frame update
    void Start()
    {
        spacer();

        print("length: " + objs_to_sort.Length);
    }

    void spacer() {

        for (int i = 0; i < objs_to_sort.Length; i++)
        {
            objs_to_sort[i].transform.position += new Vector3(0,0,0);
        }

        //for (int i = 0; i < objs_to_sort.Length; i++) {
        //    if (i % 10 != 0)
        //    {
        //        print("pos[i]: " + objs_to_sort[i].transform.position);
        //        objs_to_sort[i].transform.position += new Vector3(objs_to_sort[i].transform.position.x + spacing.x * (i%10), 0, objs_to_sort[i].transform.position.z - spacing.y * (i % 10));
        //    }
        //    else { 
        //        print("pos[i]: " + objs_to_sort[i].transform.position);
        //        objs_to_sort[i].transform.position = new Vector3(0 , 0, objs_to_sort[i].transform.position.z - spacing.y * (i%10));
        //    }
        //}

        int row = 0;

        for (int i = 0; i < objs_to_sort.Length; i++)
        {
            objs_to_sort[i].transform.position += new Vector3(objs_to_sort[i].transform.position.x + spacing.x * (i % 10), 0, objs_to_sort[i].transform.position.z - spacing.y * row);

            if (i % 10 == 0)
                row++;
        }
    
    }

}
