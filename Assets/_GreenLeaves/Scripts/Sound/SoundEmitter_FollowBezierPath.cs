using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter_FollowBezierPath : MonoBehaviour
{
    public PathCreation.PathCreator m_path;
    public Transform m_trackingTransform;

    public bool m_follow;
    private void Start()
    {
        if(m_trackingTransform == null)
        {
            Debug.Log("THe follow transform is null, setting to player.", gameObject);
            //m_trackingTransform = Player_Inventory.Instance.transform;
            m_trackingTransform = PlayerInputToggle.Instance.m_playerCamera;
        }
    }
    private void Update()
    {
        if (!m_follow) return;
        transform.position = m_path.path.GetClosestPointOnPath(PlayerInputToggle.Instance.GetSplinePos().position);
    }

    public void ToggleFollow(bool p_newState)
    {
        m_follow = p_newState;
        
    }
}
