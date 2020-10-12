using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderPlacement : MonoBehaviour
{
	public Transform m_upwardPlacementCastPoint;

	public Transform m_edgeSweepPoint;

	public Transform m_edgePlacementCastPoint;

	public LayerMask m_groundMask;

	public float m_ladderLength;

	public float m_edgePlacementRange;

	public float m_minMaxAngle;

	public float m_placementStep = 0.1f;

	private float m_currentUpwardPlacementAngle;

	private float m_currentEdgePlacementAngle;

	private Vector3 m_lastPosition;

	private Vector3 m_ladderStartPoint;
	private Vector3 m_ladderEndPoint;

	private float m_currentEdgeSweepDistance;

	public Transform m_ladder;

	public float m_ladderTiltPercent;

	public float m_edgeAngle;

	private void Update()
	{
		Debug.DrawLine(m_ladderStartPoint, m_ladderEndPoint, Color.red);
	}

	public void RunLadderPlacement()
	{
		if (transform.position != m_lastPosition)
		{
			m_lastPosition = transform.position;
		}

		FindLadderPlacementMode();

		m_ladder.position = m_ladderStartPoint;
		m_ladder.LookAt(m_ladderEndPoint);
	}
	private void FindLadderPlacementMode()
	{
		PlaceLadderUpwards();
		FindEdge();
	}

	private void PlaceLadderUpwards()
	{
		m_currentUpwardPlacementAngle = 89;
		m_upwardPlacementCastPoint.transform.localRotation = Quaternion.AngleAxis(m_currentUpwardPlacementAngle, Vector3.right);

		RaycastHit groundingHit;

		if (Physics.Raycast(transform.position, Vector3.down, out groundingHit, Mathf.Infinity, m_groundMask))
		{
			m_upwardPlacementCastPoint.position = groundingHit.point + (Vector3.up * 0.01f);
		}

		while (Physics.Raycast(m_upwardPlacementCastPoint.position, m_upwardPlacementCastPoint.forward, m_ladderLength, m_groundMask) && CheckMinMaxAngle(m_currentUpwardPlacementAngle))
		{
			m_currentUpwardPlacementAngle -= m_placementStep;
			m_upwardPlacementCastPoint.transform.localRotation = Quaternion.AngleAxis(m_currentUpwardPlacementAngle, Vector3.right);
		}

		m_ladderStartPoint = m_upwardPlacementCastPoint.position;
		m_ladderEndPoint = m_upwardPlacementCastPoint.position + (m_upwardPlacementCastPoint.forward * m_ladderLength);
	}

	private void FindEdge()
	{
		m_currentEdgeSweepDistance = 0f;
		m_edgeSweepPoint.position = transform.position;

		bool overAnEdge = false;

		float yPosition = 0;

		while (!overAnEdge && m_currentEdgeSweepDistance <= m_ladderLength)
		{
			RaycastHit groundingHit;

			if (Physics.Raycast(transform.position, transform.forward, out groundingHit, m_ladderLength, m_groundMask))
			{
				break;
			}

			RaycastHit edgeHit;
			Physics.Raycast(m_edgeSweepPoint.position, Vector3.down, out edgeHit, Mathf.Infinity, m_groundMask);

			m_currentEdgeSweepDistance += m_placementStep;
			m_edgeSweepPoint.position = transform.position + (transform.forward * m_currentEdgeSweepDistance);

			float slopeAngle = Vector3.Angle(Vector3.up, edgeHit.normal);

			if (slopeAngle > m_edgeAngle)
			{
				Vector3 notFlatPos = new Vector3(m_edgeSweepPoint.position.x, yPosition, m_edgeSweepPoint.position.z);

				m_edgeSweepPoint.position = notFlatPos;
				overAnEdge = true;
			}
			else
			{
				yPosition = edgeHit.point.y;
			}
		}

		if (overAnEdge)
		{
			Vector3 flatLadderEndPos = m_upwardPlacementCastPoint.position + (m_upwardPlacementCastPoint.forward * m_ladderLength);

			float currentEdgePercentage = Vector3.Distance(m_edgeSweepPoint.position, flatLadderEndPos) / m_ladderLength;

			if (currentEdgePercentage > m_ladderTiltPercent)
			{
				PlaceLadderFromLedge(Mathf.Lerp(0, m_ladderLength, currentEdgePercentage));
			}
		}
	}

	private void PlaceLadderFromLedge(float p_remainingLength)
	{
		m_edgePlacementCastPoint.position = m_edgeSweepPoint.position;
		//m_edgePlacementCastPoint.position = m_edgeSweepPoint.position + (m_edgeSweepPoint.forward * 0.5f);

		m_currentEdgePlacementAngle = 89;
		m_edgePlacementCastPoint.transform.localRotation = Quaternion.AngleAxis(m_currentEdgePlacementAngle, Vector3.right);

		while (Physics.Raycast(m_edgePlacementCastPoint.position, m_edgePlacementCastPoint.forward, p_remainingLength, m_groundMask) && CheckMinMaxAngleNeg(m_currentEdgePlacementAngle))
		{
			m_currentEdgePlacementAngle -= m_placementStep;
			m_edgePlacementCastPoint.transform.localRotation = Quaternion.AngleAxis(m_currentEdgePlacementAngle, Vector3.right);
		}

		m_ladderStartPoint = m_edgePlacementCastPoint.position + (m_edgePlacementCastPoint.forward * p_remainingLength);
		m_ladderEndPoint = m_ladderStartPoint - (m_edgePlacementCastPoint.forward * m_ladderLength);
	}

	private bool CheckMinMaxAngle(float p_angleToCheck)
	{
		bool underMax = false;
		bool aboveMin = false;

		if (p_angleToCheck >= -m_minMaxAngle)
		{
			underMax = true;
		}

		if (p_angleToCheck <= m_minMaxAngle)
		{
			aboveMin = true;
		}

		if (aboveMin && underMax)
		{
			return true;
		}

		return false;
	}

	private bool CheckMinMaxAngleNeg(float p_angleToCheck)
	{
		bool underMax = false;
		bool aboveMin = false;

		if (p_angleToCheck > -m_minMaxAngle)
		{
			underMax = true;
		}

		if (p_angleToCheck < m_minMaxAngle)
		{
			aboveMin = true;
		}

		if (aboveMin && underMax)
		{
			return true;
		}

		return false;
	}
}
