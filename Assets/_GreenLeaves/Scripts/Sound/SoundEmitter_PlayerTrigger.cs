﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter_PlayerTrigger : MonoBehaviour
{
    public FMODUnity.StudioEventEmitter m_soundEmitter;

    [Range(0,1)]
    public float m_soundPlayChance;

    public string m_playerLayer;

    public GenericWorldEvent m_playerInZone, m_playerLeftZone;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer(m_playerLayer))
        {
            m_playerInZone.Invoke();
            ToggleSound(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(m_playerLayer))
        {
            m_playerLeftZone.Invoke();
            ToggleSound(false);
        }
    }

    public void ToggleSound(bool p_newState)
    {
        if (m_soundPlayChance >= 1)
        {
            if (p_newState)
            {
                m_soundEmitter.Play();
            }
            else
            {
                m_soundEmitter.Stop();
            }
            return;
        }
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