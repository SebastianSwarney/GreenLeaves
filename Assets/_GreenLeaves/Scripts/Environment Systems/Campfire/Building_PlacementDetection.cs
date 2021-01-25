using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_PlacementDetection : MonoBehaviour
{
    public LayerMask m_detectionMask;

    public float m_startingHeight;

    [Header("Blocking Detection")]
    public LayerMask m_blockingMask;
    public Vector3 m_blockingDetectionSize;
    public Color m_blockingColorCheck;

    public bool m_canPlace;
    [System.Serializable]
    public class DetectionCast
    {
        public Vector3 m_offset;
        public float m_distance = 5f;
        public Color m_debugColor = Color.white;
    }

    public List<DetectionCast> m_dectectionCasts;

    [Header("Debugging")]
    public bool m_drawRaycasts;
    public bool m_drawBoxCast;

    public bool CanPlace(out Vector3 p_hitNormal)
    {
        m_canPlace = false;
        p_hitNormal = Vector3.up;
        if (Physics.OverlapBox(transform.position, m_blockingDetectionSize/2, transform.rotation, m_blockingMask).Length > 0)
        {
            return false;
        }
        for (int i = 0; i < m_dectectionCasts.Count; i++)
        {
            if(!Physics.Raycast((transform.position + Vector3.up * m_startingHeight) + m_dectectionCasts[i].m_offset, Vector3.down, m_dectectionCasts[i].m_distance, m_detectionMask))
            {
                return false;
            }
            else
            {
                if(i == 0)
                {
                    RaycastHit hit;
                    if(Physics.Raycast((transform.position + Vector3.up * m_startingHeight) + m_dectectionCasts[i].m_offset, Vector3.down, out hit, m_dectectionCasts[i].m_distance, m_detectionMask))
                    {
                        p_hitNormal = hit.normal;
                    }
                    
                }
            }
        }
        m_canPlace = true;
        return true;
    }



    private void OnDrawGizmos()
    {
        if (m_drawRaycasts)
        {
            foreach (DetectionCast cast in m_dectectionCasts)
            {
                Gizmos.color = cast.m_debugColor;
                Gizmos.DrawLine((transform.position + transform.up * m_startingHeight) + cast.m_offset, transform.position + cast.m_offset + Vector3.down * (cast.m_distance - m_startingHeight));
            }
        }
        if (m_drawBoxCast)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = m_blockingColorCheck;
            Gizmos.DrawWireCube(Vector3.zero, m_blockingDetectionSize);

        }
    }
}
