using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daytime_TimeBasedEvent : MonoBehaviour
{

    public float m_startTime, m_endTime;
    public bool m_eventActive;
    private bool m_currentCheck;
    public GenericWorldEvent m_timeStarted, m_timeEnded;

    private void Update()
    {
        m_currentCheck = WithinTime();
        if (m_eventActive != m_currentCheck)
        {
            m_eventActive = m_currentCheck;
            if (m_eventActive)
            {
                m_timeStarted.Invoke();
            }
            else
            {
                m_timeEnded.Invoke();
            }
        }
    }
    private bool WithinTime()
    {
        if (m_endTime > m_startTime)
        {
            if (DaytimeCycle_Update.Instance.m_timeOfDay > m_startTime && DaytimeCycle_Update.Instance.m_timeOfDay < m_endTime)
            {
                return true;
            }
        }
        else
        {
            if(DaytimeCycle_Update.Instance.m_timeOfDay > m_startTime || DaytimeCycle_Update.Instance.m_timeOfDay < m_endTime)
            {
                return true;
            }
        }
        return false;
    }
}
