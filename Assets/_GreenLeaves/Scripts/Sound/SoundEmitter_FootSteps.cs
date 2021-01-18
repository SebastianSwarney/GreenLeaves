using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter_FootSteps : MonoBehaviour
{

    [FMODUnity.EventRef]
    public string[] m_soundEffect;

    public FMODUnity.StudioEventEmitter m_eventEmitter;


    public int m_currentSound;
    private int m_currentSoundTrack;

    [Header("Random Twigs")]
    [FMODUnity.EventRef]
    public string[] m_twigSoundEffect;
    public FMODUnity.StudioEventEmitter m_twigEmitter;
    public float m_chance;
    private int m_currentTwigIndex;

    /*
     public bool m_debugSound;
     private void OnValidate()
    {
        if (m_debugSound)
        {
            m_debugSound = false;
            PlaySound(m_currentSound);
        }
    }*/
    public void PlaySound()
    {
        //Debug.Log()
        m_eventEmitter.Event = m_soundEffect[m_currentSound];
        if (m_currentSoundTrack != m_currentSound)
        {
            m_eventEmitter.Lookup();
        }
        m_currentSoundTrack = m_currentSound;
        m_eventEmitter.Play();

        if (Random.Range(0f, 1f) < m_chance)
        {

            m_twigEmitter.Play();

        }

    }
    public void SwapTwigEffect(int p_newIndex, float p_newChance)
    {
        m_chance = p_newChance;
        if (m_currentTwigIndex == p_newIndex)
        {
            return;
        }
        m_currentTwigIndex = p_newIndex;
        m_twigEmitter.Event = m_twigSoundEffect[p_newIndex];
        m_twigEmitter.Lookup();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger: " + other.gameObject.name, other.gameObject);
        PlaySound();
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        PlaySound();
    }
    */
}
