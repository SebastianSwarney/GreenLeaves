using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manipulation_SelfSlice_ScriptedDirection : Manipulation_SelfSlice
{

    public Vector3 m_fallDirection;
    


    [Header("Debug Event")]
    public bool m_debugScriptedEvent;
    public Color m_debugScriptedColor;


    public override void SetUpHulls(GameObject p_upperHull, GameObject p_lowerHull, Vector3 p_fallDir, Vector3 p_worldPos)
    {
        p_upperHull.transform.localScale = transform.localScale;
        p_lowerHull.transform.localScale = transform.localScale;
        p_lowerHull.transform.position = p_upperHull.transform.position = transform.position;
        p_upperHull.transform.parent = p_lowerHull.transform.parent = transform.parent;


        #region prepare the different slices


        ///Assigns the children to the hulls, depending on their relative position to the cut and it's angle
        #region Children assignment
        Transform[] childs = new Transform[m_mesh.transform.childCount + 1];
        childs[childs.Length - 1] = m_durabilityObject;
        m_durabilityObject.gameObject.SetActive(true);
        for (int i = 0; i < m_mesh.transform.childCount; i++)
        {
            childs[i] = m_mesh.transform.GetChild(i);
        }

        foreach (Transform chi in childs)
        {
            if (chi == null) continue;
            if (Vector3.Angle(chi.transform.position - p_worldPos, Vector3.up) < 90)
            {
                chi.transform.parent = p_upperHull.transform;
            }
            else
            {
                chi.transform.parent = p_lowerHull.transform;
            }
        }

        #endregion




        p_lowerHull.AddComponent<MeshCollider>().convex = true;

        if (m_addRB)
        {
            if (m_fallForward)
            {
                Rigidbody newRb = p_upperHull.AddComponent<Rigidbody>();
                newRb.mass = m_startingTreeMass * transform.localScale.y;
                newRb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX;

                newRb.AddForceAtPosition(m_fallDirection.normalized * m_fallInitialForce, m_mesh.transform.position + (Vector3.up * m_applyForcePosition * transform.localScale.y), ForceMode.Impulse);
                Debug.DrawLine(transform.position, m_mesh.transform.position + (Vector3.up * m_applyForcePosition * transform.localScale.y), Color.magenta, 2f);
                newRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
            else
            {
                Rigidbody newRb = p_upperHull.AddComponent<Rigidbody>();
                newRb.AddExplosionForce(1, p_upperHull.transform.position, 3, .5f, ForceMode.Impulse);
                newRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
        }

        #endregion


        m_slicedEvent.Invoke(p_upperHull.transform);
        /*MeshCollider col = upperHull.transform.GetChild(1).gameObject.AddComponent<MeshCollider>();
        col.sharedMesh = upperHull.GetComponent<MeshFilter>().mesh;
        col.convex = true;*/
        BoxCollider col = p_upperHull.transform.GetChild(1).gameObject.AddComponent<BoxCollider>();
        Mesh mesh = p_upperHull.GetComponent<MeshFilter>().mesh;
        col.size = mesh.bounds.size;
        col.center = mesh.bounds.center;

        gameObject.SetActive(false);
        p_upperHull.transform.GetChild(1).transform.localPosition = Vector3.zero;
        p_upperHull.transform.GetChild(1).transform.localRotation = Quaternion.identity;
        p_upperHull.transform.GetComponentInChildren<Tree_VelocityCheck>().AssignTree();

    }


    public override void OnDrawGizmos()
    {
        if (m_debugScriptedEvent)
        {
            Gizmos.color = m_debugScriptedColor;
            Gizmos.DrawLine(transform.position, transform.position + m_fallDirection);
            Gizmos.DrawWireSphere(transform.position + m_fallDirection, .5f);
        }

        base.OnDrawGizmos();

    }
}
