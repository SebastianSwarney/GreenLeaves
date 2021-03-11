using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform m_followTransform;
    public Vector3 m_offset;

    public bool m_debug;
    public Color m_debugColor;

    private void Awake()
    {
        if (m_followTransform == null) gameObject.SetActive(false);
    }
    private void Update()
    {
        transform.position = m_followTransform.position + m_offset;
    }
    private void OnDrawGizmos()
    {
        if (m_followTransform == null) return;
        if (!m_debug) return;
        Gizmos.color = m_debugColor;
        Gizmos.DrawWireSphere(m_followTransform.position + m_offset, 0.025f);
    }
}
