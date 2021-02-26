using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daytime_TimeBasedParticles : MonoBehaviour
{

    public List<TimeBasedParticle> m_currentParticles;

    private TimeBasedParticle m_currentParticle;

    public bool m_canBePlayed;

    [System.Serializable]
    public class TimeBasedParticle
    {
        public float m_startTime, m_endTime;
        public ParticleSystem m_currentParticle;
        public bool IsInTime(float p_givenTime)
        {
            if (m_startTime < m_endTime)
            {
                return p_givenTime >= m_startTime && p_givenTime <= m_endTime;
            }
            else
            {
                return p_givenTime >= m_startTime || p_givenTime <= m_endTime;
            }
        }
    }

    private void Update()
    {
        foreach (TimeBasedParticle part in m_currentParticles)
        {
            bool isInTime = part.IsInTime(DaytimeCycle_Update.Instance.m_timeOfDay);
            if (isInTime && m_currentParticle == part) return;

            if (isInTime)
            {
                if (m_currentParticle != null)
                {
                    m_currentParticle.m_currentParticle.Stop();
                }
                m_currentParticle = part;

                if (m_canBePlayed)
                {
                    m_currentParticle.m_currentParticle.Play();
                }
            }
            else if (!isInTime && part == m_currentParticle)
            {
                if (m_currentParticle != null)
                {
                    m_currentParticle.m_currentParticle.Stop();
                }
            }
        }
    }

    public void ToggleParticles(bool p_newState)
    {
        if (m_canBePlayed != p_newState)
        {
            m_canBePlayed = p_newState;
            if (m_canBePlayed)
            {
                if (m_currentParticle == null) return;
                m_currentParticle.m_currentParticle.Play();
            }
            else
            {
                if (m_currentParticle == null) return;
                m_currentParticle.m_currentParticle.Stop();
            }

        }
    }



}
