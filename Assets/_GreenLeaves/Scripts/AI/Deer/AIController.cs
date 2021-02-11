using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public enum AIState { Idle, Running, Dead }

    public AIState m_currentState;


    public Transform m_playerObject;
    public AIAvoidanceDetection m_avoidanceScript;

    public float m_avoidanceDistance = 5;

    public AIMovement m_movement;

    Vector3 m_targetDir;
    private Vector3 m_currentTargetDir;
    public float m_runningAngleAdjustment;
    private float m_currentChaseAngleAdjustment;
    public float m_startedChaseAngleAdjustmentDis;
    private Coroutine m_adjustChaseAngleDisCor;
    public float m_startledTime;

    public float m_maxChaseDistance;
    public float m_angleAdjustmentValue;

    [Header("Alive Time")]
    public float m_aliveTime;
    private float m_aliveTimer;
    private Coroutine m_disappearCoroutine;

    private bool m_playerInVision;
    [Header("Debugging")]
    public bool m_debug;
    public Color m_chaseAngleColor, m_maxChaseDisColor;

    private void Start()
    {
        m_currentTargetDir = transform.forward;
        m_playerObject = PlayerInputToggle.Instance.transform;
    }

    public void Respawn()
    {

        m_avoidanceScript.Respawn();
        m_currentState = AIState.Idle;
        m_disappearCoroutine = null;
    }

    private IEnumerator DeerLife()
    {
        m_aliveTimer = 0;
        while (m_aliveTimer < m_aliveTime)
        {
            m_aliveTimer += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Big Ooof");
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        switch (m_currentState)
        {
            case AIState.Running:
                PerformChase();
                break;
            case AIState.Idle:
                break;
            case AIState.Dead:
                break;
        }
    }
    public void ChangeStateInt(int p_newState)
    {
        if (p_newState == 0)
        {
            ChangeState(AIState.Idle);
        }
        else if (p_newState == 1)
        {
            ChangeState(AIState.Running);
        }
        else if (p_newState == 2)
        {
            ChangeState(AIState.Dead);
        }
    }


    public void ChangeState(AIState p_newState)
    {
        m_currentState = p_newState;
        switch (m_currentState)
        {
            case AIState.Running:
                m_currentTargetDir = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(m_playerObject.position.x, 0, m_playerObject.position.z);
                if (m_adjustChaseAngleDisCor == null)
                {
                    m_adjustChaseAngleDisCor = StartCoroutine(AdjustChaseAngleDis());
                }
                StartCoroutine(DeerLife());
                break;
            case AIState.Idle:
                break;
            case AIState.Dead:
                break;
        }
    }
    private void PerformChase()
    {
        if (GetDirection(out m_targetDir))
        {
            m_movement.MoveAI(m_targetDir, 100);
            Debug.DrawLine(transform.position, transform.position + m_currentTargetDir, Color.blue);

            m_currentTargetDir = transform.forward;
        }
        else
        {
            //m_currentTargetDir = -transform.forward;
            Debug.DrawLine(transform.position, transform.position + m_currentTargetDir, Color.blue);
            m_movement.MoveAI(m_currentTargetDir, 100);
        }
    }



    private bool GetDirection(out Vector3 p_dir)
    {
        bool canMove = false;
        if (Vector3.Distance(transform.position, m_playerObject.position) < m_currentChaseAngleAdjustment)
        {

            Vector3 dir = (transform.position - m_playerObject.position).normalized;

            p_dir = m_avoidanceScript.GetAvoidanceDir(new Vector3(dir.x, 0, dir.z), m_avoidanceDistance + m_movement.m_currentSpeed, out canMove);

        }
        else
        {
            p_dir = m_avoidanceScript.GetAvoidanceDir(m_currentTargetDir.normalized, m_avoidanceDistance + m_movement.m_currentSpeed, out canMove);
        }
        return canMove;
    }


    private IEnumerator AdjustChaseAngleDis()
    {
        m_currentChaseAngleAdjustment = m_startedChaseAngleAdjustmentDis;
        float timer = 0;
        while (timer < m_startledTime)
        {
            m_currentChaseAngleAdjustment = Mathf.Lerp(m_startedChaseAngleAdjustmentDis, m_runningAngleAdjustment, timer / m_startedChaseAngleAdjustmentDis);
            yield return null;
            timer += Time.deltaTime;
        }
        m_currentChaseAngleAdjustment = Mathf.Lerp(m_startedChaseAngleAdjustmentDis, m_runningAngleAdjustment, 1);
        m_adjustChaseAngleDisCor = null;
    }


    private void OnDrawGizmos()
    {
        if (!m_debug) return;
        Gizmos.color = m_chaseAngleColor;
        Gizmos.DrawSphere(transform.position, m_runningAngleAdjustment);

        Gizmos.color = m_maxChaseDisColor;
        Gizmos.DrawSphere(transform.position, m_maxChaseDistance);
    }
}
