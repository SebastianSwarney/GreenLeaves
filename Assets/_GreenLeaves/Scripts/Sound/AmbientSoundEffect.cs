using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundEffect : MonoBehaviour
{
    public bool m_canPlaySound;
    public FMODUnity.StudioEventEmitter m_eventEmitter;
    public float m_minTimeBetweenTracks, m_maxTimeBetweenTracks;



    public List<TypeOfSound> m_ambientSounds;
    [System.Serializable]
    public class TypeOfSound
    {
        [FMODUnity.EventRef]
        public string m_eventName;
        public float m_minDaytime, m_maxDaytime;
        public List<Vector3> m_offsetFromPlayer;
        public Color m_debugColor;
    }


    private float m_currentTimer, m_currentRandomTime;


    [Header("Debug")]
    public bool m_debug;
    public int m_soundToDebug;
    private void Start()
    {
        transform.parent = PlayerInputToggle.Instance.transform;
        transform.localEulerAngles = Vector3.zero;
        m_eventEmitter.transform.parent = null;
        //m_currentRandomTime = Random.Range(m_minTimeBetweenTracks, m_maxTimeBetweenTracks);
    }
    private void Update()
    {
        if (!m_eventEmitter.IsPlaying())
        {
            m_currentTimer += Time.deltaTime;
            if (m_currentTimer > m_currentRandomTime)
            {
                Vector3 outPos = Vector3.zero;
                string current = GetCurrentSongType(out outPos);
                if (current != "oof" && m_canPlaySound)
                {
                    if (m_eventEmitter.Event != current)
                    {
                        m_eventEmitter.Event = current;
                        m_eventEmitter.Lookup();
                    }

                    
                    m_eventEmitter.transform.position = transform.position + (Quaternion.Euler(0, transform.eulerAngles.y, 0) * outPos);
                    m_eventEmitter.Play();
                }
                else
                {
                    Debug.Log("No sound data available for time: " + DaytimeCycle_Update.Instance.m_timeOfDay.ToString(), this.gameObject);
                }
                m_currentTimer = 0;
                m_currentRandomTime = Random.Range(m_minTimeBetweenTracks, m_maxTimeBetweenTracks);
            }
        }
    }
    public string GetCurrentSongType(out Vector3 p_soundPos)
    {
        float currentTime = DaytimeCycle_Update.Instance.m_timeOfDay;

        p_soundPos = Vector3.zero;
        foreach (TypeOfSound data in m_ambientSounds)
        {
            if (data.m_minDaytime < data.m_maxDaytime)
            {
                if (currentTime >= data.m_minDaytime && currentTime <= data.m_maxDaytime)
                {
                    p_soundPos = data.m_offsetFromPlayer[Random.Range(0, data.m_offsetFromPlayer.Count)];
                    return data.m_eventName;
                }
            }
            else
            {
                if (currentTime > data.m_minDaytime || currentTime < data.m_maxDaytime)
                {
                    p_soundPos = data.m_offsetFromPlayer[Random.Range(0, data.m_offsetFromPlayer.Count)];
                    return data.m_eventName;
                }
            }
        }

        return "oof";
    }

    private void OnDrawGizmos()
    {
        if (!m_debug) return;
        if (m_ambientSounds.Count > 0)
        {
            if (m_soundToDebug < m_ambientSounds.Count)
            {
                if (m_ambientSounds[m_soundToDebug].m_offsetFromPlayer.Count > 0)
                {
                    Gizmos.color = m_ambientSounds[m_soundToDebug].m_debugColor;
                    foreach (Vector3 vec in m_ambientSounds[m_soundToDebug].m_offsetFromPlayer)
                    {
                        Gizmos.DrawLine(transform.position, transform.position + (Quaternion.Euler(0, transform.eulerAngles.y, 0) * vec));
                    }
                }
            }
        }
    }
}
