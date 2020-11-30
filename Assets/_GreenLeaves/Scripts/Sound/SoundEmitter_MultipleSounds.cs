using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter_MultipleSounds : MonoBehaviour
{
    public FMODUnity.StudioEventEmitter m_emitter;
    private int m_currentIndex=10000;
    [FMODUnity.EventRef]
    public string[] m_events;

    public void PlaySound(int p_playIndex)
    {
        if (m_currentIndex != p_playIndex)
        {
            m_currentIndex = p_playIndex;
            m_emitter.Event = m_events[m_currentIndex];
            m_emitter.Lookup();
        }
        m_emitter.Play();
    }
}
