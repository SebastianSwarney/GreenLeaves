using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Deer_States : MonoBehaviour
{

    public enum DeerState { Spawned, Running, Searching, Idle}
    public DeerState m_currentState;

    public Health m_health;
    public AI_Vision m_aiVision;

    #region Switch Cases Base comment
    /*
     switch (m_currentState)
        {
            case DeerState.Spawned:
                break;

            case DeerState.Running:
                break;

            case DeerState.Searching:
                break;

            case DeerState.Idle:
                break;
        }
     */
    #endregion

    public void ChangeState(DeerState p_newState)
    {
        switch (m_currentState)
        {
            case DeerState.Spawned:
                break;

            case DeerState.Running:
                break;

            case DeerState.Searching:
                break;

            case DeerState.Idle:
                break;
        }
    }

    public void CheckState()
    {
        switch (m_currentState)
        {
            case DeerState.Spawned:
                break;

            case DeerState.Running:
                break;

            case DeerState.Searching:
                break;

            case DeerState.Idle:
                break;
        }
    }
}
