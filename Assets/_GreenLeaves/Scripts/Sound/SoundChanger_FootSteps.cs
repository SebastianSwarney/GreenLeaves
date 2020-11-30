using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundChanger_FootSteps : MonoBehaviour
{
    public int m_soundTypeIndex;
    public int m_specialSoundIndex;
    public float m_specialSoundChance;

    private int m_soundLayer;
    

    private void Start()
    {
        m_soundLayer = LayerMask.NameToLayer("Sounds");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == m_soundLayer)
        {
            if(other.gameObject.GetComponent<SoundEmitter_FootSteps>() != null)
            {
                SoundEmitter_FootSteps newStep = other.gameObject.GetComponent<SoundEmitter_FootSteps>();
                newStep.m_currentSound = m_soundTypeIndex;
                newStep.SwapTwigEffect(m_specialSoundIndex, m_specialSoundChance);
            }
        }
    }
}
