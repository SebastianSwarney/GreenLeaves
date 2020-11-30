using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter_FollowBezierPath : MonoBehaviour
{
    public PathCreation.PathCreator m_path;
    public Transform m_trackingTransform;

    public bool m_follow;

    private void Update()
    {
        if (!m_follow) return;
        transform.position = m_path.path.GetClosestPointOnPath(m_trackingTransform.position);
    }

    public void ToggleFollow(bool p_newState)
    {
        m_follow = p_newState;
        
    }
}
