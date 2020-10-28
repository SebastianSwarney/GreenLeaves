using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
	public Vector3 m_testVelocity;
	public float m_testSpeed;

	[Space]

	public LayerMask m_collisionMask;
	public float m_maxSlopeAngle;

	private CharacterController m_characterController;

	private Vector3 m_playerBottom;
	private Vector3 m_playerTop;

	private Vector3 m_localHorizontalMovement;

	private void Start()
	{
		m_characterController = GetComponent<CharacterController>();
	}

	private void Update()
	{
		Move(((transform.forward * m_testSpeed) + m_testVelocity) * Time.deltaTime);
	}

	public void Move(Vector3 p_moveAmount)
	{
		UpdateRaycastOrigins();
		UpdateMovementVariables(p_moveAmount);

		if (p_moveAmount.y < 0)
		{
			DecendSlope(ref p_moveAmount);
		}

		m_characterController.Move(p_moveAmount);
	}

	private void UpdateRaycastOrigins()
	{
		m_playerBottom = transform.position - (transform.up * (m_characterController.height / 2));
		m_playerTop = transform.position + (transform.up * (m_characterController.height / 2));
	}

	private void UpdateMovementVariables(Vector3 p_moveAmount)
	{
		m_localHorizontalMovement = transform.InverseTransformDirection(GetHorizontalMovement(p_moveAmount));
	}

	private void DecendSlope(ref Vector3 p_moveAmount)
	{
		Vector3 rayOrigin = m_playerBottom;
		RaycastHit hit;

		if (Physics.Raycast(rayOrigin, Vector3.up, out hit, Mathf.Infinity, m_collisionMask))
		{
			Quaternion slopeQuatOffset = Quaternion.FromToRotation(Vector3.up, hit.normal);
			p_moveAmount = slopeQuatOffset * p_moveAmount;

			float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

			if (slopeAngle != 0 && slopeAngle <= m_maxSlopeAngle)
			{


				/*
				Vector3 localNormal = transform.InverseTransformVector(hit.normal);
				Vector3 slopeCross = Vector3.Cross(localNormal, m_localHorizontalMovement);

				float moveDistance = m_localHorizontalMovement.magnitude;

				if (hit.distance - m_characterController.skinWidth >= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * moveDistance)
				{
					float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
					Vector3 horizontalMoveAmount = -m_localHorizontalMovement.normalized * (Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance);

					SetHorizontalVelocity(ref p_moveAmount, horizontalMoveAmount);
					p_moveAmount.y -= descendmoveAmountY;
				}

				if (Mathf.Sign(slopeCross.x) < 0)
				{

				}
				*/
			}
		}
	}

	private void SetHorizontalVelocity(ref Vector3 p_moveAmount, Vector3 p_newMovement)
	{
		p_moveAmount = new Vector3(p_newMovement.x, p_moveAmount.y, p_newMovement.z);
	}

	private Vector3 GetHorizontalMovement(Vector3 p_moveAmount)
	{
		return new Vector3(p_moveAmount.x, 0, p_moveAmount.z);
	}

	private Vector3 AbsoluteVector(Vector3 p_vector)
	{
		return new Vector3(Mathf.Abs(p_vector.x), Mathf.Abs(p_vector.y), Mathf.Abs(p_vector.z));
	}

	public bool HitCeiling()
	{
		if ((m_characterController.collisionFlags & CollisionFlags.Above) != 0)
		{
			return true;
		}

		return false;
	}

	public bool HitGround()
	{
		if ((m_characterController.collisionFlags & CollisionFlags.Below) != 0)
		{
			return true;
		}

		return false;
	}
}
