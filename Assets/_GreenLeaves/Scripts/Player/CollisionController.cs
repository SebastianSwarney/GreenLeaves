using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
	public LayerMask m_collisionMask;
	public float m_maxSlopeAngle;

	public bool m_logVelocityMag;
	public float m_castSkinWidth;

	private CharacterController m_characterController;
	private Vector3 m_horizontalMovement;
	private CapsuleCollider m_capsuleCollider;

	[HideInInspector]
	public Vector3 m_averageNormal;
	private Vector3 m_oldAverageNormal;
	private int m_amountToAverage;

	private bool m_hitAnythingLastMovement;

	private Vector3 m_playerTop;
	private Vector3 m_playerBottom;

	private bool m_isAdjusting;

	public Transform m_wallTransform;

	private void Start()
	{
		m_characterController = GetComponent<CharacterController>();
		m_capsuleCollider = GetComponent<CapsuleCollider>();

		m_hitAnythingLastMovement = true;
	}

	public void Move(Vector3 p_moveAmount, bool p_isClimbing)
	{
		UpdateMovementVariables(p_moveAmount);
		UpdateCastPoints();

		//ClimbSlopeAbove(ref p_moveAmount);

		ClimbSlopeBelow(ref p_moveAmount);
		DecendSlopeBelow(ref p_moveAmount, p_isClimbing);

		if (m_amountToAverage != 0)
		{
			m_amountToAverage = 0;
			m_averageNormal = Vector3.zero;
		}

		m_hitAnythingLastMovement = false;

		m_characterController.Move(p_moveAmount);

		if (m_logVelocityMag)
		{
			Debug.Log(m_characterController.velocity.magnitude + " velocity mag");
		}
	}

	private void UpdateMovementVariables(Vector3 p_moveAmount)
	{
		//m_localHorizontalMovement = transform.InverseTransformDirection(GetHorizontalMovement(p_moveAmount));
		m_horizontalMovement = GetHorizontalMovement(p_moveAmount);

		if (m_amountToAverage != 0)
		{
			m_averageNormal /= m_amountToAverage;
			m_oldAverageNormal = m_averageNormal;
		}

		if (m_amountToAverage == 0)
		{
			m_averageNormal = m_oldAverageNormal;
		}
	}

	private void UpdateCastPoints()
	{
		Vector3 bottom = transform.position + m_characterController.center + Vector3.up * -m_characterController.height * 0.5F;
		Vector3 top = bottom + Vector3.up * m_characterController.height;

		m_playerBottom = bottom;
		m_playerTop = top;
	}

	private float GetAdjustedHitDistance(Vector3 p_hitPoint)
	{
		Vector3 closestPoint = m_capsuleCollider.ClosestPoint(p_hitPoint);
		float distanceInsideOfCollider = Vector3.Distance(p_hitPoint, closestPoint);

		Debug.DrawLine(p_hitPoint, closestPoint, Color.red, 0f, false);

		if (p_hitPoint == Vector3.zero)
		{
			distanceInsideOfCollider = 0;
		}

		return distanceInsideOfCollider;
	}

	private void ClimbSlopeAbove(ref Vector3 p_moveAmount)
	{

		Vector3 localXAxis = Vector3.Cross(m_horizontalMovement, Vector3.up);
		Vector3 forwardRotation = Vector3.ProjectOnPlane(m_averageNormal, localXAxis);
		Quaternion upwardSlopeOffset = Quaternion.FromToRotation(Vector3.up, forwardRotation);
		Vector3 targetMoveAmount = (upwardSlopeOffset * m_horizontalMovement);

		m_wallTransform.rotation = Quaternion.LookRotation(m_averageNormal);

		Vector3 normalCross = Vector3.Cross(Vector3.up, m_averageNormal);
		Vector3 movementSlopeCross = Vector3.Cross(normalCross, m_horizontalMovement).normalized;

		if (m_hitAnythingLastMovement)
		{
			//Vector3 horizontalMovement = targetMoveAmount - (targetMoveAmount * m_characterController.skinWidth);
			//p_moveAmount.y = (targetMoveAmount.y + m_characterController.skinWidth);

			DebugExtension.DebugArrow(transform.position, p_moveAmount, Color.green, 0f, false);
		}

		if (Mathf.Sign(movementSlopeCross.y) > 0)
		{
			if (m_hitAnythingLastMovement)
			{
				if (p_moveAmount.y <= targetMoveAmount.y)
				{
					targetMoveAmount.y += m_characterController.skinWidth;
					p_moveAmount = targetMoveAmount;
				}
			}
		}
	}

	private void ClimbSlopeBelow(ref Vector3 p_moveAmount)
	{
		Vector3 localXAxis = Vector3.Cross(m_horizontalMovement, Vector3.up);
		Vector3 forwardRotation = Vector3.ProjectOnPlane(m_averageNormal, localXAxis);
		Quaternion upwardSlopeOffset = Quaternion.FromToRotation(Vector3.up, forwardRotation);
		Vector3 targetMoveAmount = (upwardSlopeOffset * m_horizontalMovement);
		DebugExtension.DebugArrow(transform.position, targetMoveAmount, Color.green, 0f, false);

		Vector3 normalCross = Vector3.Cross(Vector3.up, m_averageNormal);
		Vector3 movementSlopeCross = Vector3.Cross(normalCross, m_horizontalMovement).normalized;

		if (Mathf.Sign(movementSlopeCross.y) > 0)
		{
			if (m_hitAnythingLastMovement)
			{
				if (p_moveAmount.y <= targetMoveAmount.y)
				{
					targetMoveAmount.y -= m_characterController.skinWidth;
					p_moveAmount = targetMoveAmount;
				}
			}
		}
	}

	private void DecendSlopeBelow(ref Vector3 p_moveAmount, bool p_isClimbing)
	{
		if (!m_hitAnythingLastMovement)
		{
			Vector3 rayDir = Vector3.down;
			float castRadius = m_characterController.radius - m_castSkinWidth;
			float rayLength = Mathf.Abs(m_horizontalMovement.magnitude) + (m_castSkinWidth * 2);
			Vector3 bottomOrigin = m_playerBottom - (rayDir * m_castSkinWidth);
			Vector3 topOrigin = m_playerTop - (rayDir * m_castSkinWidth);

			DebugExtension.DebugCapsule(bottomOrigin, topOrigin, Color.red, castRadius);

			RaycastHit[] hits = Physics.CapsuleCastAll(bottomOrigin, topOrigin, castRadius, rayDir, rayLength, m_collisionMask);

			for (int i = 0; i < hits.Length; i++)
			{
				if (hits[i].point == Vector3.zero)
				{
					RaycastHit fallbackHit;

					if (Physics.Raycast(bottomOrigin, rayDir, out fallbackHit, rayLength, m_collisionMask))
					{
						m_hitAnythingLastMovement = true;
						m_averageNormal = hits[i].normal;
					}
				}
				else
				{
					m_averageNormal = hits[i].normal;
					m_hitAnythingLastMovement = true;
				}
			}
		}

		Vector3 localXAxis = Vector3.Cross(m_horizontalMovement, Vector3.up);
		Vector3 forwardRotation = Vector3.ProjectOnPlane(m_averageNormal, localXAxis);
		Quaternion upwardSlopeOffset = Quaternion.FromToRotation(Vector3.up, forwardRotation);
		Vector3 targetMoveAmount = (upwardSlopeOffset * m_horizontalMovement);
		DebugExtension.DebugArrow(transform.position, targetMoveAmount, Color.green, 0f, false);

		m_wallTransform.rotation = Quaternion.LookRotation(m_averageNormal);

		Vector3 normalCross = Vector3.Cross(Vector3.up, m_averageNormal);
		Vector3 movementSlopeCross = Vector3.Cross(normalCross, m_horizontalMovement).normalized;

		if (p_moveAmount.y < 0)
		{
			if (Mathf.Sign(movementSlopeCross.y) < 0)
			{
				if (m_hitAnythingLastMovement)
				{
					targetMoveAmount.y -= m_characterController.skinWidth;
					p_moveAmount = targetMoveAmount;
					p_moveAmount.y += targetMoveAmount.y;
				}
			}
		}
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		m_hitAnythingLastMovement = true;
		m_averageNormal += hit.normal;
		m_amountToAverage++;
	}

	private void OnCollisionStay(Collision collision)
	{

	}

	private void SetHorizontalVelocity(ref Vector3 p_moveAmount, Vector3 p_newMovement)
	{
		p_moveAmount = new Vector3(p_newMovement.x, p_moveAmount.y, p_newMovement.z);
	}

	private Vector3 GetHorizontalMovement(Vector3 p_moveAmount)
	{
		return new Vector3(p_moveAmount.x, 0, p_moveAmount.z);
	}

	public bool HitSide()
	{
		if ((m_characterController.collisionFlags & CollisionFlags.Sides) != 0)
		{
			return true;
		}

		return false;
	}

	public bool HitAbove()
	{
		if ((m_characterController.collisionFlags & CollisionFlags.Above) != 0)
		{
			return true;
		}

		return false;
	}

	public bool HitBelow()
	{
		if ((m_characterController.collisionFlags & CollisionFlags.Below) != 0)
		{
			return true;
		}

		return false;
	}
}
