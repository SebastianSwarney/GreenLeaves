using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingAnimationController : MonoBehaviour
{
	public Transform[] m_hands;
	// the y component of this member is ignored
	public Vector3[] m_restPositions;
	public float m_maxRestingDistance;
	public float m_targetStepDuration;
	public float m_stepDurationError;
	public float m_stepHeight;
	public float m_stepError;

	private bool[] m_isHandMoving;
	private Vector3[] m_targetPositions;
	private Vector3[] m_stepStartPositions;
	private Quaternion[] m_targetRotation;
	private Quaternion[] m_stepStartRotation;
	private float[] m_stepStartTime;
	private float[] m_stepDuration;

	private int m_numHandsUp = 0;

	public LayerMask m_hitLayerHand;

	private void Start()
	{
		m_isHandMoving = new bool[m_hands.Length];
		m_targetPositions = new Vector3[m_hands.Length];
		m_stepStartPositions = new Vector3[m_hands.Length];
		m_targetRotation = new Quaternion[m_hands.Length];
		m_stepStartRotation = new Quaternion[m_hands.Length];
		m_stepStartTime = new float[m_hands.Length];
		m_stepDuration = new float[m_hands.Length];

		for (int i = 0; i < m_hands.Length; i++)
		{
			StartMoving(i, GetTargetPosition(i));
		}
	}

	private void FixedUpdate()
	{
		for (int i = 0; i < m_hands.Length; i++)
		{
			if (!m_isHandMoving[i])
			{
				Vector3 targetPosition = GetTargetPosition(i);
				if (Vector3.Distance(m_hands[i].position, targetPosition) >= m_maxRestingDistance * Mathf.Pow(1.2f, m_numHandsUp))
				{
					StartMoving(i, targetPosition);
				}
			}
			if (m_isHandMoving[i])
			{
				// get relative time
				float t = (Time.time - m_stepStartTime[i]) / m_stepDuration[i];
				// stop if time is up
				if (t >= 1f)
				{
					m_isHandMoving[i] = false;
					m_hands[i].position = m_targetPositions[i];
					m_hands[i].rotation = m_targetRotation[i];
					m_numHandsUp--;
				}
				// interpolate transform
				else
				{
					Vector3 lerp = Vector3.Lerp(
						m_stepStartPositions[i],
						m_targetPositions[i],
						.25f + .5f * t + .25f * Mathf.Pow(2f * t - 1f, 3f));
					float arcHeight = m_stepHeight * (Mathf.Pow(t, 2) - Mathf.Pow(t, 3)) / .148148148f;
					m_hands[i].position = lerp + new Vector3(0f, arcHeight, 0f);
					m_hands[i].rotation = Quaternion.Lerp(m_stepStartRotation[i], m_targetRotation[i], t);
				}
			}
		}
	}

	private void StartMoving(int i, Vector3 targetPosition)
	{
		m_targetPositions[i] = targetPosition;
		m_stepStartPositions[i] = m_hands[i].position;
		m_targetRotation[i] = transform.rotation;
		m_stepStartRotation[i] = m_hands[i].rotation;
		m_isHandMoving[i] = true;
		m_stepStartTime[i] = Time.time;
		m_stepDuration[i] = m_targetStepDuration + Random.Range(-m_stepDurationError, m_stepDurationError);
		m_numHandsUp++;
	}

	private Vector3 GetTargetPosition(int i)
	{
		Vector2 stepError = m_stepError * Random.insideUnitCircle;
		RaycastHit hitInfo;
		Physics.Raycast(transform.position + transform.rotation * new Vector3(m_restPositions[i].x + stepError.x, 0f, m_restPositions[i].z + stepError.y),
			Vector3.down,
			out hitInfo, Mathf.Infinity, m_hitLayerHand);
		return hitInfo.point + (Vector3.up * m_restPositions[i].y);
	}
}
