using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    public Rigidbody m_rb;
    public float m_currentSpeed;
    public float m_maxSpeed;
    public float m_breakingDistance;
    public float m_rotationSpeed;
    private Vector3 m_currentVelocity;

    public float m_clampTurnSpeed;
    private void Start()
    {
        m_currentSpeed = m_maxSpeed;
    }
    public Vector3 GetRelativeVector(Vector3 p_vector)
    {
        return new Vector3(p_vector.x, 0, p_vector.z);
    }

    public Vector3 GetPos()
    {
        return new Vector3(transform.position.x, 0, transform.position.z);
    }
    public void MoveAI(Vector3 p_dir, float p_disToTarget)
    {
        //float dirToRotate = Mathf.Sign(Vector3.Dot(GetRelativeVector(transform.right), p_dir));
        transform.Rotate(new Vector3(0, Mathf.Clamp(Vector3.Cross(GetRelativeVector(transform.forward), GetRelativeVector(p_dir)).y, -m_clampTurnSpeed, m_clampTurnSpeed) * m_rotationSpeed, 0), Space.World);


        m_currentSpeed = Mathf.Clamp(p_disToTarget / m_breakingDistance, 0, 1) * m_maxSpeed;
        m_currentVelocity = transform.forward * Mathf.Clamp(p_disToTarget / m_breakingDistance, 0, 1) * m_maxSpeed;
        m_rb.velocity = new Vector3(m_currentVelocity.x, m_rb.velocity.y, m_currentVelocity.z);
    }
}
