using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter_FootSteps : MonoBehaviour
{

    [FMODUnity.EventRef]
    public string[] m_soundEffect;

    public FMODUnity.StudioEventEmitter m_eventEmitter;

    public bool m_debugSound;
    public int m_currentSound;
    private int m_currentSoundTrack;

    private void OnValidate()
    {
        if (m_debugSound)
        {
            m_debugSound = false;
            PlaySound(m_currentSound);
        }
    }
    public void PlaySound(int p_soundType)
    {
        //Debug.Log()
        m_eventEmitter.Event = m_soundEffect[p_soundType];
        if(m_currentSoundTrack != p_soundType)
        {
            m_eventEmitter.Lookup();
        }
        m_currentSoundTrack = p_soundType;
        m_eventEmitter.Play();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        PlaySound(m_currentSound);
    }
}
