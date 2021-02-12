using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAvoidanceDetection : MonoBehaviour
{
    public int m_avoidanceRayCount;
    public float m_maxAngle;
    private float m_anglePerRay;

    public float m_maxHeightDifference;

    public LayerMask m_collisionMask;
    public LayerMask m_groundMask;

    public Vector3 m_targetDir;

    public Vector3 m_bounds;
    public int m_amountOfMiddleRaycasts;

    private enum AISteeringState { Right, Left };
    private AISteeringState m_currentState;

    [Header("Debug Vars")]
    public bool m_debugAvoidance;
    public Gradient m_rayColors;
    public float m_debugDis;

    private void Awake()
    {
        m_anglePerRay = m_maxAngle / (float)m_avoidanceRayCount;
    }

    public void Respawn()
    {
        m_currentState = Random.Range(0f, 1f) > .5f ? AISteeringState.Left : AISteeringState.Right;
    }

    public Vector3 GetAvoidanceDir(Vector3 p_inputDir, float p_inputDis, out bool p_canCalculate)
    {
        Vector3 groundPos = transform.position + Vector3.up * m_bounds.y;
        Vector3 collisionPos = transform.position;
        p_canCalculate = true;
        for (int i = 0; i < m_avoidanceRayCount; i++)
        {
            if (i == 0)
            {

                if (CheckMiddleRaycasts(collisionPos, groundPos, p_inputDir, p_inputDis))
                {
                    return p_inputDir;
                }

            }
            else
            {

                switch (m_currentState)
                {
                    case AISteeringState.Left:

                        Debug.DrawLine(collisionPos + transform.right * m_bounds.x, collisionPos + transform.right * m_bounds.x + Quaternion.Euler(0, m_anglePerRay * i, 0) * p_inputDir * p_inputDis, m_rayColors.Evaluate(i / (float)m_avoidanceRayCount));
                        Debug.DrawLine(groundPos + transform.right * m_bounds.x, groundPos + transform.right * m_bounds.x + Quaternion.Euler(0, m_anglePerRay * i, 0) * p_inputDir * p_inputDis, m_rayColors.Evaluate(i / (float)m_avoidanceRayCount));
                        if (!Physics.Raycast(collisionPos + transform.right * m_bounds.x, Quaternion.Euler(0, m_anglePerRay * i, 0) * p_inputDir, p_inputDis, m_collisionMask)
                        && !Physics.Raycast(groundPos + transform.right * m_bounds.x, Quaternion.Euler(0, m_anglePerRay * i, 0) * p_inputDir, p_inputDis, m_groundMask) &&
                        CanTraverseHeight(groundPos + transform.right * m_bounds.x + Quaternion.Euler(0, m_anglePerRay * i, 0) * p_inputDir * p_inputDis))
                        {
                            return Quaternion.Euler(0, m_anglePerRay * i, 0) * p_inputDir;
                        }

                        Debug.DrawLine(collisionPos - transform.right * m_bounds.x, collisionPos - transform.right * m_bounds.x + Quaternion.Euler(0, -m_anglePerRay * i, 0) * p_inputDir * p_inputDis, m_rayColors.Evaluate(i / (float)m_avoidanceRayCount));
                        Debug.DrawLine(groundPos - transform.right * m_bounds.x, groundPos - transform.right * m_bounds.x + Quaternion.Euler(0, -m_anglePerRay * i, 0) * p_inputDir * p_inputDis, m_rayColors.Evaluate(i / (float)m_avoidanceRayCount));
                        if (!Physics.Raycast(collisionPos - transform.right * m_bounds.x, Quaternion.Euler(0, -m_anglePerRay * i, 0) * p_inputDir, p_inputDis, m_collisionMask)
                            && !Physics.Raycast(groundPos - transform.right * m_bounds.x, Quaternion.Euler(0, -m_anglePerRay * i, 0) * p_inputDir, p_inputDis, m_groundMask)
                            && CanTraverseHeight(groundPos - transform.right * m_bounds.x + Quaternion.Euler(0, -m_anglePerRay * i, 0) * p_inputDir * p_inputDis))
                        {
                            return Quaternion.Euler(0, -m_anglePerRay * i, 0) * p_inputDir;
                        }

                        break;

                    case AISteeringState.Right:

                        Debug.DrawLine(collisionPos - transform.right * m_bounds.x, collisionPos - transform.right * m_bounds.x + Quaternion.Euler(0, -m_anglePerRay * i, 0) * p_inputDir * p_inputDis, m_rayColors.Evaluate(i / (float)m_avoidanceRayCount));
                        Debug.DrawLine(groundPos - transform.right * m_bounds.x, groundPos - transform.right * m_bounds.x + Quaternion.Euler(0, -m_anglePerRay * i, 0) * p_inputDir * p_inputDis, m_rayColors.Evaluate(i / (float)m_avoidanceRayCount));
                        if (!Physics.Raycast(collisionPos - transform.right * m_bounds.x, Quaternion.Euler(0, -m_anglePerRay * i, 0) * p_inputDir, p_inputDis, m_collisionMask)
                            && !Physics.Raycast(groundPos - transform.right * m_bounds.x, Quaternion.Euler(0, -m_anglePerRay * i, 0) * p_inputDir, p_inputDis, m_groundMask)
                            && CanTraverseHeight(groundPos - transform.right * m_bounds.x + Quaternion.Euler(0, -m_anglePerRay * i, 0) * p_inputDir * p_inputDis))
                        {
                            return Quaternion.Euler(0, -m_anglePerRay * i, 0) * p_inputDir;
                        }
                        Debug.DrawLine(collisionPos + transform.right * m_bounds.x, collisionPos + transform.right * m_bounds.x + Quaternion.Euler(0, m_anglePerRay * i, 0) * p_inputDir * p_inputDis, m_rayColors.Evaluate(i / (float)m_avoidanceRayCount));
                        Debug.DrawLine(groundPos + transform.right * m_bounds.x, groundPos + transform.right * m_bounds.x + Quaternion.Euler(0, m_anglePerRay * i, 0) * p_inputDir * p_inputDis, m_rayColors.Evaluate(i / (float)m_avoidanceRayCount));
                        if (!Physics.Raycast(collisionPos + transform.right * m_bounds.x, Quaternion.Euler(0, m_anglePerRay * i, 0) * p_inputDir, p_inputDis, m_collisionMask)
                        && !Physics.Raycast(groundPos + transform.right * m_bounds.x, Quaternion.Euler(0, m_anglePerRay * i, 0) * p_inputDir, p_inputDis, m_groundMask) &&
                        CanTraverseHeight(groundPos + transform.right * m_bounds.x + Quaternion.Euler(0, m_anglePerRay * i, 0) * p_inputDir * p_inputDis))
                        {
                            return Quaternion.Euler(0, m_anglePerRay * i, 0) * p_inputDir;
                        }


                        break;
                }



            }
        }
        p_canCalculate = false;
        return p_inputDir;
    }

    public bool CheckMiddleRaycasts(Vector3 p_collisionPos, Vector3 p_groundCheckPos, Vector3 p_inputDir, float p_inputDis)
    {

        if (Physics.Raycast(p_collisionPos, p_inputDir, p_inputDis, m_collisionMask) || Physics.Raycast(p_groundCheckPos, p_inputDir, p_inputDis, m_groundMask))
        {

            Debug.DrawLine(p_collisionPos, p_collisionPos + p_inputDir * p_inputDis, m_rayColors.Evaluate(0));
            return false;
        }
        if (!CanTraverseHeight(p_collisionPos + p_inputDir * p_inputDis))
        {
            return false;
        }
        Debug.DrawLine(p_collisionPos + transform.right * m_bounds.x, p_collisionPos + transform.right * m_bounds.x + p_inputDir * p_inputDis, m_rayColors.Evaluate(0));
        Debug.DrawLine(p_collisionPos - transform.right * m_bounds.x, p_collisionPos - transform.right * m_bounds.x + p_inputDir * p_inputDis, m_rayColors.Evaluate(0));
        for (int i = 0; i < m_amountOfMiddleRaycasts; i++)
        {
            if (Physics.Raycast(p_collisionPos + transform.right * m_bounds.x * ((float)i + 1) / (float)m_amountOfMiddleRaycasts, p_inputDir, p_inputDis, m_collisionMask) ||
               Physics.Raycast(p_collisionPos - transform.right * m_bounds.x * ((float)i + 1) / (float)m_amountOfMiddleRaycasts, p_inputDir, p_inputDis, m_collisionMask) ||
               Physics.Raycast(p_groundCheckPos + transform.right * m_bounds.x * ((float)i + 1) / (float)m_amountOfMiddleRaycasts, p_inputDir, p_inputDis, m_groundMask) ||
               Physics.Raycast(p_groundCheckPos - transform.right * m_bounds.x * ((float)i + 1) / (float)m_amountOfMiddleRaycasts, p_inputDir, p_inputDis, m_groundMask))
            {

                Debug.DrawLine(p_collisionPos + transform.right * m_bounds.x * ((float)i + 1) / (float)m_amountOfMiddleRaycasts, p_collisionPos + transform.right * m_bounds.x * ((float)i + 1) / (float)m_amountOfMiddleRaycasts + p_inputDir * p_inputDis, m_rayColors.Evaluate(0));
                Debug.DrawLine(p_collisionPos - transform.right * m_bounds.x * ((float)i + 1) / (float)m_amountOfMiddleRaycasts, p_collisionPos - transform.right * m_bounds.x * ((float)i + 1) / (float)m_amountOfMiddleRaycasts + p_inputDir * p_inputDis, m_rayColors.Evaluate(0));

                Debug.DrawLine(p_groundCheckPos + transform.right * m_bounds.x * ((float)i + 1) / (float)m_amountOfMiddleRaycasts, p_groundCheckPos + transform.right * m_bounds.x * ((float)i + 1) / (float)m_amountOfMiddleRaycasts + p_inputDir * p_inputDis, m_rayColors.Evaluate(0));
                Debug.DrawLine(p_groundCheckPos - transform.right * m_bounds.x * ((float)i + 1) / (float)m_amountOfMiddleRaycasts, p_groundCheckPos - transform.right * m_bounds.x * ((float)i + 1) / (float)m_amountOfMiddleRaycasts + p_inputDir * p_inputDis, m_rayColors.Evaluate(0));
                return false;
            }

            if (!CanTraverseHeight(p_groundCheckPos + transform.right * m_bounds.x * ((float)i + 1) / (float)m_amountOfMiddleRaycasts + p_inputDir * p_inputDis) ||
                !CanTraverseHeight(p_groundCheckPos - transform.right * m_bounds.x * ((float)i + 1) / (float)m_amountOfMiddleRaycasts + p_inputDir * p_inputDis))
            {
                return false;
            }
        }
        return true;
    }


    public bool CanTraverseHeight(Vector3 point)
    {
        Debug.DrawLine(point, point - Vector3.up * m_maxHeightDifference, m_rayColors.Evaluate(0));
        return Physics.Raycast(point, -Vector3.up, m_maxHeightDifference, m_groundMask);
    }
    private void OnDrawGizmos()
    {
        if (!m_debugAvoidance) return;

        float anglePerRay = m_maxAngle / (float)m_avoidanceRayCount;
        Vector3 groundCheckPos = transform.position + Vector3.up * m_bounds.y;
        Vector3 colPos = transform.position;
        for (int i = 0; i < m_avoidanceRayCount; i++)
        {
            if (i == 0)
            {
                for (int o = 0; o < m_amountOfMiddleRaycasts; o++)
                {
                    Debug.DrawLine(colPos + transform.right * m_bounds.x * o / m_amountOfMiddleRaycasts, colPos + transform.right * m_bounds.x * o / m_amountOfMiddleRaycasts + transform.forward * m_debugDis, m_rayColors.Evaluate(0));
                    Debug.DrawLine(colPos - transform.right * m_bounds.x * o / m_amountOfMiddleRaycasts, colPos - transform.right * m_bounds.x * o / m_amountOfMiddleRaycasts + transform.forward * m_debugDis, m_rayColors.Evaluate(0));

                    Debug.DrawLine(groundCheckPos + transform.right * m_bounds.x * o / m_amountOfMiddleRaycasts, groundCheckPos + transform.right * m_bounds.x * o / m_amountOfMiddleRaycasts + transform.forward * m_debugDis, m_rayColors.Evaluate(0));
                    Debug.DrawLine(groundCheckPos - transform.right * m_bounds.x * o / m_amountOfMiddleRaycasts, groundCheckPos - transform.right * m_bounds.x * o / m_amountOfMiddleRaycasts + transform.forward * m_debugDis, m_rayColors.Evaluate(0));

                    Debug.DrawLine(groundCheckPos + transform.right * m_bounds.x * o / m_amountOfMiddleRaycasts + transform.forward * m_debugDis, groundCheckPos + transform.right * m_bounds.x * o / m_amountOfMiddleRaycasts + transform.forward * m_debugDis + Vector3.down * m_maxHeightDifference);
                    Debug.DrawLine(groundCheckPos - transform.right * m_bounds.x * o / m_amountOfMiddleRaycasts + transform.forward * m_debugDis, groundCheckPos * m_bounds.y + transform.right * m_bounds.x * o / m_amountOfMiddleRaycasts + transform.forward * m_debugDis + Vector3.down * m_maxHeightDifference);
                }
            }

            Debug.DrawLine(colPos + transform.right * m_bounds.x, colPos + transform.right * m_bounds.x + Quaternion.Euler(0, anglePerRay * i, 0) * transform.forward * m_debugDis, m_rayColors.Evaluate(i / (float)m_avoidanceRayCount));
            Debug.DrawLine(colPos - transform.right * m_bounds.x, colPos - transform.right * m_bounds.x + Quaternion.Euler(0, -anglePerRay * i, 0) * transform.forward * m_debugDis, m_rayColors.Evaluate(i / (float)m_avoidanceRayCount));

            Debug.DrawLine(groundCheckPos + transform.right * m_bounds.x, groundCheckPos + transform.right * m_bounds.x + Quaternion.Euler(0, anglePerRay * i, 0) * transform.forward * m_debugDis, m_rayColors.Evaluate(i / (float)m_avoidanceRayCount));
            Debug.DrawLine(groundCheckPos - transform.right * m_bounds.x, groundCheckPos - transform.right * m_bounds.x + Quaternion.Euler(0, -anglePerRay * i, 0) * transform.forward * m_debugDis, m_rayColors.Evaluate(i / (float)m_avoidanceRayCount));

            Debug.DrawLine(groundCheckPos + transform.right * m_bounds.x + Quaternion.Euler(0, anglePerRay * i, 0) * transform.forward * m_debugDis, groundCheckPos + transform.right * m_bounds.x + Quaternion.Euler(0, anglePerRay * i, 0) * transform.forward * m_debugDis + Vector3.down * m_maxHeightDifference, m_rayColors.Evaluate(i / (float)m_avoidanceRayCount));
            Debug.DrawLine(groundCheckPos - transform.right * m_bounds.x + Quaternion.Euler(0, -anglePerRay * i, 0) * transform.forward * m_debugDis, groundCheckPos - transform.right * m_bounds.x + Quaternion.Euler(0, -anglePerRay * i, 0) * transform.forward * m_debugDis + Vector3.down * m_maxHeightDifference, m_rayColors.Evaluate(i / (float)m_avoidanceRayCount));
        }
    }
}
