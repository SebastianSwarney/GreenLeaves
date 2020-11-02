using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instance_Trees : MonoBehaviour
{
    public GameObject Container;

    public GameObject[] Trunk_Obj;
    public GameObject[] Bush_Obj;

    public Material[] Trunk_Mat;
    public Material[] Bush_Mat;

    public string[] Tree_types;

    public int season_spacing = 10;
    public int type_spacing = 5;

    private void Start()
    {
        createTrees();
    }


    void createTrees()
    {
        //iterate over all the trees inside trunk obj array
        for (int trees_index = 0; trees_index < Trunk_Obj.Length; trees_index++)
        {
            //create a tree with the assigned trunk and bush material
            for (int type_index = 0; type_index < Tree_types.Length; type_index++)
            {
                GameObject c_tree_empty = new GameObject("Tree_0" + trees_index + "_" + Tree_types[type_index]);
                c_tree_empty.transform.parent = this.transform;

                
                c_tree_empty.transform.position = c_tree_empty.transform.position + new Vector3(season_spacing * trees_index, 0, type_spacing * type_index);
                

                GameObject c_tree_trunk = Instantiate(Trunk_Obj[trees_index], c_tree_empty.transform);
                GameObject c_tree_bush = Instantiate(Bush_Obj[trees_index], c_tree_empty.transform);

                Material c_tree_trunk_mat = Trunk_Mat[type_index];
                Material c_tree_bush_mat = Bush_Mat[type_index];

                c_tree_trunk.GetComponent<Renderer>().material = c_tree_trunk_mat;
                c_tree_bush.GetComponent<Renderer>().material = c_tree_bush_mat;

                
            }
        }

    }

}
