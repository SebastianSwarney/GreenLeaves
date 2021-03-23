using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer_Trigger : MonoBehaviour
{
    public enum MusicTriggerType { Summit, Climbing, Exploration }
    public MusicTriggerType m_triggerType;

    public string m_playerTag;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == m_playerTag)
        {
            MusicPlayer.Instance.ChangeSong(m_triggerType);
        }
    }
}
