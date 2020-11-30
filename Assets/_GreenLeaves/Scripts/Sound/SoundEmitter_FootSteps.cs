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

    [Header("Random Twigs")]
    public FMODUnity.StudioEventEmitter m_twigEmitter;
    public float m_chance;
    public List<int> m_twigChanceIndexes;
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

        if (Random.Range(0f, 1f) < m_chance)
        {
            if (m_twigChanceIndexes.Contains(m_currentSoundTrack))
            {
                m_twigEmitter.Play();
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        PlaySound(m_currentSound);
    }
}
