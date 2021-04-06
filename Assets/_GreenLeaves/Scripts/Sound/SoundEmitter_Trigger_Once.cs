using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter_Trigger_Once : MonoBehaviour
{
    public bool m_played = false;
    public FMODUnity.StudioEventEmitter m_emitter;
    // Update is called once per frame
    void Update()
    {
        if (m_played)
        {
            if (!m_emitter.IsPlaying())
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            m_emitter.Play();
            m_played = true;
        }
    }
}

