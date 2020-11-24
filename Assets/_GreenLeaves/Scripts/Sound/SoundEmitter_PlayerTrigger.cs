using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter_PlayerTrigger : MonoBehaviour
{
    public FMODUnity.StudioEventEmitter m_soundEmitter;

    public float m_soundPlayChance;

    public string m_playerLayer;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer(m_playerLayer))
        {
            ToggleSound(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(m_playerLayer))
        {
            ToggleSound(false);
        }
    }

    public void ToggleSound(bool p_newState)
    {
        if (p_newState)
        {
            if(Random.Range(0,1f) < m_soundPlayChance)
            {
                m_soundEmitter.Play();
            }
        }
        else
        {
            m_soundEmitter.Stop();
        }
    }
}
