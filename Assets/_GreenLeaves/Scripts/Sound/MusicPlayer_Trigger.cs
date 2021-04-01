using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer_Trigger : MonoBehaviour
{
    public enum MusicTriggerType { Summit, Climbing, Exploration, Stop}
    public MusicTriggerType m_triggerType;
    

    public string m_playerTag;
    [Header("Climbing Song")]
    public int m_climbingLevel;

    [Header("Debug")]
    public bool m_debug;
    public Color m_debugColor = Color.yellow;
    public BoxCollider m_boxCollider;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == m_playerTag)
        {
            MusicPlayer.Instance.ChangeSong(m_triggerType, m_climbingLevel);
            if (m_triggerType == MusicTriggerType.Summit)
            {
                Credits.Instance.StartCredits();
                gameObject.SetActive(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!m_debug || m_boxCollider == null) return;
        Gizmos.color = m_debugColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, m_boxCollider.size);
    }
}
