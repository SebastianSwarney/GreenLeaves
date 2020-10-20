using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TestRaycaster : MonoBehaviour
{
    public Color m_raycastColor;

    public Vector3 hitPoint;

    RaycastHit hit;
    public LayerMask m_detectMask;
    public string hitObjectName;
    private void Update()
    {
        
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100,m_detectMask ))
        {
            hitPoint = hit.point;
            hitObjectName = hit.transform.name;
        }
        else
        {
            hitPoint = transform.position + Vector3.down * 100;
        }        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = m_raycastColor;

        Gizmos.DrawLine(transform.position, hitPoint);
        Gizmos.DrawSphere(hitPoint, .25f);
    }

}
