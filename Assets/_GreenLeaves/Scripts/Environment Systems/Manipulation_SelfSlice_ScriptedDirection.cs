using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manipulation_SelfSlice_ScriptedDirection : Manipulation_SelfSlice
{

    public Transform m_fallParent;
    public List<GameObject> m_myMeshes;

    public GenericWorldEvent m_treeHitEvent;

    public Vector3 m_hitPos;

    [Header("Debug Event")]
    public bool m_debugScriptedEvent;
    public Color m_debugScriptedColor;


    public override void SliceMe(Vector3 p_worldPos, Vector3 p_forwardDir)
    {
        base.SliceMe(transform.position + m_hitPos, p_forwardDir);
    }
    public override void SetUpHulls(GameObject p_upperHull, GameObject p_lowerHull, Vector3 p_fallDir, Vector3 p_worldPos)
    {

        p_upperHull.transform.localScale = new Vector3(m_mesh.transform.localScale.x * transform.localScale.x, m_mesh.transform.localScale.y * transform.localScale.y, m_mesh.transform.localScale.z * transform.localScale.z) ;
        p_lowerHull.transform.localScale = new Vector3(m_mesh.transform.localScale.x * transform.localScale.x, m_mesh.transform.localScale.y * transform.localScale.y, m_mesh.transform.localScale.z * transform.localScale.z); ;
        p_lowerHull.transform.position = p_upperHull.transform.position = m_mesh.transform.position;
        p_upperHull.transform.rotation = p_lowerHull.transform.rotation = m_mesh.transform.rotation;
        p_upperHull.transform.parent = p_lowerHull.transform.parent = transform;


        #region prepare the different slices


        ///Assigns the children to the hulls, depending on their relative position to the cut and it's angle
        #region Children assignment
        Transform[] childs = new Transform[m_mesh.transform.childCount + 1];
        childs[childs.Length - 1] = m_durabilityObject;

        for (int i = 0; i < m_mesh.transform.childCount; i++)
        {
            childs[i] = m_mesh.transform.GetChild(i);
        }

        
        foreach (Transform chi in childs)
        {
            if (chi == null) continue;
            if (Vector3.Angle(chi.transform.position + p_worldPos, Vector3.up) < 90)
            {
                chi.transform.parent = p_upperHull.transform;
            }
            else
            {
                chi.transform.parent = p_lowerHull.transform;
            }
        }

        #endregion




        //p_lowerHull.AddComponent<MeshCollider>().convex = true;
        #endregion


        p_upperHull.transform.parent = m_fallParent;

        foreach (GameObject newMesh in m_myMeshes)
        {
            newMesh.SetActive(false);
        }

        m_slicedEvent.Invoke(p_upperHull.transform);

    }

    public void TreeHitGround()
    {
        m_treeHitEvent.Invoke();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + m_hitPos, .5f);
    }
}
