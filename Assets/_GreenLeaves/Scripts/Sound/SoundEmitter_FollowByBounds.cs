using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter_FollowByBounds : MonoBehaviour
{
    public List<Bounds> m_bounds;
    public Transform m_soundEmitter;
    private Transform m_trackingTransform;

    public bool m_follow;

    [Header("Debug")]
    public bool m_debug;
    public Color m_gizmosColor;
    public Gradient m_gizmosGradient;
    private bool m_runtime;


    private void Awake()
    {
        m_runtime = true;
        for (int i = 0; i < m_bounds.Count; i++)
        {
            m_bounds[i] = new Bounds(m_bounds[i].center + transform.position, m_bounds[i].size);
        }
    }
    private void Start()
    {
        m_trackingTransform = PlayerInputToggle.Instance.m_playerCamera.transform;
    }
    private void Update()
    {
        if (m_follow)
        {
            m_soundEmitter.position = GetClosestBounds();
        }
    }

    public void ToggleFollow(bool p_newState)
    {
        m_follow = p_newState;
    }
    
    public Vector3 GetClosestBounds()
    {
        Bounds current = new Bounds();
        float dis = 10000;
        float currentDis = 0;
        foreach(Bounds bound in m_bounds)
        {
            
            if (bound.Contains(m_trackingTransform.position)) return bound.ClosestPoint(m_trackingTransform.position);
            currentDis = Vector3.Distance(m_trackingTransform.position, bound.ClosestPoint(m_trackingTransform.position));
            if(currentDis < dis)
            {
                current = bound;
                dis = currentDis;
            }
        }
        return current.ClosestPoint(m_trackingTransform.position);
    }
    private void OnDrawGizmos()
    {
        if (!m_debug) return;

        for (int i = 0; i < m_bounds.Count; i++)
        {
            Gizmos.color = m_gizmosGradient.Evaluate((float)i / (float)m_bounds.Count);

            if (!m_runtime)
            {
                Gizmos.DrawCube(m_bounds[i].center + transform.position, m_bounds[i].size);
            }
            else
            {
                Gizmos.DrawCube(m_bounds[i].center, m_bounds[i].size);
            }
        }
    }
}
