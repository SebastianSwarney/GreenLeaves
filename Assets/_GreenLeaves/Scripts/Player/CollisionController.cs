using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
	public LayerMask m_collisionMask;
	public float m_maxSlopeAngle;

	private CharacterController m_characterController;

	private Vector3 m_playerBottom;
	private Vector3 m_playerTop;

	private Vector3 m_localHorizontalMovement;
	private Vector3 m_horizontalMovement;
	private Vector3 m_verticalMovement;

	private Vector3 m_targetMovementAmount;
	private bool allGoodBoss;

	[HideInInspector]
	public Vector3 m_averageNormal;
	[HideInInspector]
	public Vector3 m_averagePoint;
	[HideInInspector]
	public Vector3 m_oldAverageNormal;
	[HideInInspector]
	public Vector3 m_oldAveragePoint;
	[HideInInspector]
	public int m_amountToAverage;

	private Vector3 m_moveAmount;

	public bool m_logVelocityMag;

	private List<ControllerColliderHit> m_hits;

	public Transform m_averageNormalTransform;

	public float m_castSkinWidth;
	public float m_castHeightOffset;

	public float f;

	public CapsuleCollider cap;

	private bool m_isAdjusting;

	private int index;

	private Vector3 m_closestPos;
	private Vector3 m_closestNrml;

	private bool m_hitAnythingLastMovement;

	private void Start()
	{
		m_characterController = GetComponent<CharacterController>();
		m_hits = new List<ControllerColliderHit>();

		m_averageNormal = Vector3.up;
	}

	public void Move(Vector3 p_moveAmount)
	{
		UpdateRaycastOrigins();
		UpdateMovementVariables(p_moveAmount);

		ClimbSlope(ref p_moveAmount);

		if (m_horizontalMovement.magnitude != 0)
		{
			m_hitAnythingLastMovement = false;
		}

		m_characterController.Move(p_moveAmount);

		if (m_logVelocityMag)
		{
			Debug.Log(m_characterController.velocity.magnitude + " velocity mag");
		}
	}

	public void Move(Vector3 p_moveAmount, bool p_isClimbing)
	{
		Debug.DrawRay(transform.position, m_averageNormal, Color.yellow, 0f, false);
		Debug.DrawRay(m_averagePoint, m_averageNormal, Color.red, 0f, false);

		UpdateRaycastOrigins();
		UpdateMovementVariables(p_moveAmount);

		ClimbSlope(ref p_moveAmount);

		if (m_amountToAverage != 0)
		{
			m_amountToAverage = 0;
			m_averageNormal = Vector3.zero;
			m_averagePoint = Vector3.zero;
		}

		m_moveAmount = p_moveAmount;
		m_characterController.Move(p_moveAmount);

		if (m_amountToAverage != 0)
		{
			m_averageNormal /= m_amountToAverage;
			m_averagePoint /= m_amountToAverage;

			m_oldAverageNormal = m_averageNormal;
			m_oldAveragePoint = m_averagePoint;
		}

		if (m_amountToAverage == 0)
		{
			m_averageNormal = m_oldAverageNormal;
			m_averagePoint = m_oldAveragePoint;
		}

		if (m_averageNormal != Vector3.zero)
		{
			//m_averageNormalTransform.rotation = Quaternion.LookRotation(m_averageNormal, Vector3.up);
		}

		//Debug.DrawRay(transform.position, m_averageNormalTransform.forward, Color.blue, 0f, false);
		//Debug.DrawRay(transform.position, m_averageNormalTransform.right, Color.red, 0f, false);

		if (m_logVelocityMag)
		{
			Debug.Log(m_characterController.velocity.magnitude + " velocity mag");
		}
	}

	private void UpdateRaycastOrigins()
	{
		m_playerBottom = transform.position - (transform.up * (m_characterController.height / 2));
		m_playerBottom -= transform.up * m_characterController.skinWidth;

		m_playerTop = transform.position + (transform.up * (m_characterController.height / 2));
		m_playerTop -= Vector3.down * m_characterController.skinWidth;
	}

	private void UpdateMovementVariables(Vector3 p_moveAmount)
	{
		m_localHorizontalMovement = transform.InverseTransformDirection(GetHorizontalMovement(p_moveAmount));
		m_horizontalMovement = GetHorizontalMovement(p_moveAmount);
		m_verticalMovement = GetVerticalMovement(p_moveAmount);
	}

	private void AdjustCastPoint(ref Vector3 p_moveAmount)
	{
		Vector3 rayOrigin = transform.position;
		RaycastHit hit;

		if (Physics.Raycast(rayOrigin, -m_averageNormal, out hit, Mathf.Infinity, m_collisionMask))
		{
			//Debug.DrawLine(rayOrigin, hit.point, Color.green, 0f, false);
			//Debug.DrawRay(rayOrigin, hit.normal, Color.blue, 0f, false);
			
			//m_amountToAverage++;
			//m_averageNormal = hit.normal;
		}
	}

	private float GetAdjustedHitDistance(Vector3 p_hitPoint, float p_distance)
	{
		Vector3 closestPoint = Physics.ClosestPoint(p_hitPoint, cap, cap.transform.position, cap.transform.rotation);
		float distanceInsideOfCollider = Vector3.Distance(p_hitPoint, closestPoint);

		if (distanceInsideOfCollider < 0)
		{
			distanceInsideOfCollider = 0;
		}

		return distanceInsideOfCollider;
	}

	private void ClimbSlope(ref Vector3 p_moveAmount)
	{
		if (!m_hitAnythingLastMovement)
		{
			/*
			Vector3 bottom = transform.position + m_characterController.center + Vector3.up * -m_characterController.height * 0.5F;
			Vector3 top = bottom + Vector3.up * m_characterController.height;
			bottom += Vector3.up * m_castSkinWidth;
			float castRadius = m_characterController.radius - m_castSkinWidth;

			float rayLength = m_horizontalMovement.magnitude;
			Vector3 rayDir = m_horizontalMovement.normalized;

			if (rayLength < m_castSkinWidth)
			{
				rayLength = 2 * m_castSkinWidth;
			}

			DebugExtension.DebugCapsule(bottom, top, Color.blue, castRadius, 0f, false);

			RaycastHit hit;

			if (Physics.CapsuleCast(bottom, top, castRadius, rayDir, out hit, rayLength, m_collisionMask))
			{
				//m_closestNrml = hit.normal;
				//m_closestPos = hit.point;
				//m_hitAnythingLastMovement = true;

				bottom += rayDir * hit.distance;
				top += rayDir * hit.distance;

				DebugExtension.DebugCapsule(bottom, top, Color.red, castRadius, 0f, false);
			}
			*/
		}

		Vector3 bottom = transform.position + m_characterController.center + Vector3.up * -m_characterController.height * 0.5F;
		Vector3 top = bottom + Vector3.up * m_characterController.height;

		bottom += Vector3.up * m_castSkinWidth;
		
		Vector3 rayDir = m_horizontalMovement.normalized;
		float castRadius = m_characterController.radius - m_castSkinWidth;
		float rayLength = m_horizontalMovement.magnitude + m_castSkinWidth;


		if (rayLength < m_castSkinWidth)
		{
			rayLength = 2 * m_castSkinWidth;
		}

		RaycastHit hit;

		if (Physics.CapsuleCast(bottom, top, castRadius, rayDir, out hit, rayLength, m_collisionMask))
		{
			Vector3 normalCross = Vector3.Cross(Vector3.up, hit.normal);
			Vector3 movementSlopeCross = Vector3.Cross(normalCross, m_horizontalMovement).normalized;

			Vector3 targetMoveAmount = Quaternion.FromToRotation(Vector3.up, hit.normal) * m_horizontalMovement;
			targetMoveAmount = targetMoveAmount * movementSlopeCross.y;

			DebugExtension.DebugArrow(transform.position, targetMoveAmount, Color.green, 0f, false);

			if (Mathf.Sign(movementSlopeCross.y) > 0)
			{
				if (p_moveAmount.y <= targetMoveAmount.y)
				{
					p_moveAmount = targetMoveAmount;
				}
			}
		}
	}

	private void CalculateSlopeBelowNewNewNew(ref Vector3 p_moveAmount)
	{
		Vector3 rayDir = m_horizontalMovement.normalized;
		float rayLength = m_horizontalMovement.magnitude + m_castSkinWidth;

		Vector3 bottom = transform.position + m_characterController.center + Vector3.up * -m_characterController.height * 0.5F;
		Vector3 top = bottom + Vector3.up * m_characterController.height;

		bottom += Vector3.up * m_castSkinWidth;
		//top += Vector3.down * m_castSkinWidth;

		float castRadius = m_characterController.radius - m_castSkinWidth;

		DebugExtension.DebugCapsule(bottom, top, Color.red, castRadius, 0f, false);

		RaycastHit[] sweepHits = Physics.CapsuleCastAll(bottom, top, castRadius, rayDir, rayLength, m_collisionMask);

		for (int i = 0; i < sweepHits.Length; i++)
		{
			Debug.DrawLine(transform.position, sweepHits[i].point, Color.red, 0f, false);


			float slopeAngle = Vector3.Angle(sweepHits[i].normal, Vector3.up);
			float distance = sweepHits[i].distance - m_characterController.skinWidth;

			//float climbAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * (m_horizontalMovement.magnitude * Time.fixedDeltaTime);
			float climbAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * (m_horizontalMovement.magnitude);

			Vector3 normalCross = Vector3.Cross(Vector3.up, sweepHits[i].normal);
			Vector3 movementSlopeCross = Vector3.Cross(normalCross, m_horizontalMovement);

			if (Mathf.Sign(movementSlopeCross.y) > 0)
			{
				if (distance <= climbAmountY)
				{
					if (p_moveAmount.y <= climbAmountY)
					{
						if (climbAmountY >= 0)
						{
							p_moveAmount.y = climbAmountY;
						}

						Vector3 horizontalMovement = m_horizontalMovement.normalized * (Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * m_horizontalMovement.magnitude);
						SetHorizontalVelocity(ref p_moveAmount, horizontalMovement);
					}
				}
			}

			Debug.DrawRay(sweepHits[i].point, Vector3.up * distance, Color.magenta, 0f, false);
			Debug.DrawRay(sweepHits[i].point, Vector3.up * climbAmountY, Color.black, 0f, false);
		}
	}

	private void CalculateSlopeBelowNewNew(ref Vector3 p_moveAmount)
	{
		Vector3 rayDir = Vector3.down;

		Vector3 bottom = transform.position + m_characterController.center + Vector3.up * -m_characterController.height * 0.5F;
		Vector3 top = bottom + Vector3.up * m_characterController.height;

		//bottom += Vector3.up * m_castHeightOffset;
		//top += Vector3.up * m_castHeightOffset;

		//bottom += Vector3.up * m_castHeightOffset;
		//top += Vector3.up * m_castHeightOffset;

		//bottom += Vector3.up * m_castSkinWidth;
		//top += Vector3.down * m_castSkinWidth;

		float castRadius = m_characterController.radius + m_castSkinWidth;

		DebugExtension.DebugCapsule(bottom, top, Color.red, castRadius, 0f, false);

		RaycastHit[] sweepHits = Physics.CapsuleCastAll(bottom, top, castRadius, Vector3.down, p_moveAmount.magnitude * Time.fixedDeltaTime, m_collisionMask);

		Vector3 localAverageNormal = Vector3.zero;
		Vector3 localPoint = Vector3.zero;
		float localDistance = 0f;

		for (int i = 0; i < sweepHits.Length; i++)
		{
			localAverageNormal += sweepHits[i].normal;
			localDistance += sweepHits[i].distance;

			Debug.DrawLine(bottom, sweepHits[i].point, Color.red, 0f, false);

			if (i == 0)
			{
				localPoint += sweepHits[i].point;
				Debug.DrawLine(bottom, sweepHits[i].point, Color.cyan, 0f, false);
			}
		}

		localAverageNormal /= sweepHits.Length;
		localDistance /= sweepHits.Length;

		float slopeAngle = Vector3.Angle(localAverageNormal, Vector3.up);
		float distance = GetAdjustedHitDistance(localPoint, localDistance) - m_characterController.skinWidth;

		if (distance < 0)
		{
			distance = 0;
		}

		Vector3 normalCross = Vector3.Cross(Vector3.up, localAverageNormal);
		Vector3 movementSlopeCross = Vector3.Cross(normalCross, m_horizontalMovement);

		if (Mathf.Sign(movementSlopeCross.y) > 0)
		{
			float checkClimbAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * (m_horizontalMovement.magnitude * Time.fixedDeltaTime);

			float climbAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * (m_horizontalMovement.magnitude);

			if (distance <= checkClimbAmountY)
			{
				if (p_moveAmount.y <= climbAmountY)
				{
					p_moveAmount.y = climbAmountY;
				}
			}

			Debug.DrawRay(localPoint, Vector3.up * distance, Color.magenta, 0f, false);
			Debug.DrawRay(localPoint, Vector3.up * checkClimbAmountY, Color.black, 0f, false);
		}
		else if (Mathf.Sign(movementSlopeCross.y) < 0)
		{
			float tangent = Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * (m_horizontalMovement.magnitude * Time.fixedDeltaTime);

			if (distance <= tangent)
			{
				float decendAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * (m_horizontalMovement.magnitude);
				//p_moveAmount.y += -decendAmountY;
			}

			Debug.DrawRay(localPoint, Vector3.up * distance, Color.magenta, 0f, false);
			Debug.DrawRay(localPoint, Vector3.up * tangent, Color.black, 0f, false);
		}


	}

	private void CalculateSlopeBelowNew(ref Vector3 p_moveAmount)
	{
		Vector3 rayOrigin = transform.position;
		RaycastHit hit = new RaycastHit();

		Vector3 rayDir = -m_averageNormal;

		if (!HitBelow())
		{
			rayDir = -transform.up;
		}
		else
		{
			rayDir = -m_averageNormal;
		}

		if (Physics.SphereCast(rayOrigin, m_characterController.radius, rayDir, out hit, Mathf.Infinity, m_collisionMask))
		{
			Debug.DrawLine(rayOrigin, hit.point, Color.blue, 0f, false);

			Quaternion moveRot = Quaternion.LookRotation(p_moveAmount, Vector3.up);
			float slopeAngleHere = Vector3.Angle(m_averageNormal, Vector3.up);
			Quaternion slopeRot = Quaternion.AngleAxis(-slopeAngleHere, Vector3.right);

			m_averageNormalTransform.rotation = moveRot * slopeRot;
			//m_averageNormal = hit.normal;
		}

		Vector3 targetMoveAmount = m_averageNormalTransform.forward * m_horizontalMovement.magnitude;

		float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
		float checkClimbAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * (targetMoveAmount.magnitude * Time.fixedDeltaTime);
		float distance = GetAdjustedHitDistance(hit.point, hit.distance);

		if (distance < 0)
		{
			distance = 0;
		}

		Debug.DrawRay(hit.point, -rayDir * distance, Color.cyan, 0f, true);
		Debug.DrawRay(hit.point, -rayDir * checkClimbAmountY, Color.black, 0f, true);

		float climbAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * (targetMoveAmount.magnitude);

		if (distance <= checkClimbAmountY)
		{
			if (p_moveAmount.y <= climbAmountY)
			{
				//SetHorizontalVelocity(ref p_moveAmount, targetMoveAmount);
				p_moveAmount.y = climbAmountY;
				//Debug.DrawRay(transform.position, p_moveAmount, Color.red, 0f, false);
				//Debug.Log("ran");
			}
		}
	}

	private void CalculateSlopeBelow(ref Vector3 p_moveAmount)
	{
		Vector3 rayOrigin = transform.position;
		RaycastHit hit;

		if (Physics.Raycast(rayOrigin, -m_averageNormal, out hit, Mathf.Infinity, m_collisionMask))
		{
			//Debug.DrawLine(rayOrigin, hit.point, Color.green, 0f, false);
			//Debug.DrawRay(rayOrigin, hit.normal, Color.blue, 0f, false);
			//m_amountToAverage++;
			//m_averageNormal = hit.normal;
		}

		float slopeAngle = Vector3.Angle(m_averageNormal, Vector3.up);

		Quaternion slopeOffset = Quaternion.FromToRotation(Vector3.up, m_averageNormal);
		Vector3 targetMoveAmount = slopeOffset * m_horizontalMovement;

		Vector3 normalCross = Vector3.Cross(Vector3.up, m_averageNormal);
		Vector3 movementSlopeCross = Vector3.Cross(normalCross, m_horizontalMovement);

		if (p_moveAmount.y <= 0)
		{

		}

		if (slopeAngle != 0)
		{
			if (Mathf.Sign(movementSlopeCross.y) > 0)
			{
				//Debug.Log("NOPPERS UP");

				//p_moveAmount = targetMoveAmount;
				//SetHorizontalVelocity(ref p_moveAmount, targetMoveAmount);
				float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * (targetMoveAmount.magnitude);

				if (p_moveAmount.y > climbmoveAmountY)
				{
					//p_moveAmount.y = climbmoveAmountY;
					Debug.DrawRay(transform.position, p_moveAmount, Color.red, 0f, false);
				}
			}
			else if (Mathf.Sign(movementSlopeCross.y) < 0)
			{
				//Debug.Log("NOPPERS DOWN");

				p_moveAmount = targetMoveAmount;
				//SetHorizontalVelocity(ref p_moveAmount, targetMoveAmount);
				float tangent = Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * (targetMoveAmount.magnitude);
				float distance = (hit.distance - m_characterController.height / 2); //Should make this average distance of the hits because it is currently relying on correction hit

				if (distance < 0)
				{
					distance = 0;
				}

				if (distance <= tangent)
				{
					float moveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * targetMoveAmount.magnitude;
					p_moveAmount.y -= moveAmountY;
					Debug.DrawRay(transform.position, p_moveAmount, Color.red, 0f, false);
				}

				if (slopeAngle != 0 && slopeAngle <= m_maxSlopeAngle)
				{

				}
				else
				{
					//Slide down max slope code goes here I think
				}
			}
		}

		m_localHorizontalMovement = transform.InverseTransformDirection(GetHorizontalMovement(p_moveAmount));
	}

	private void CalculateSlopeAbove(ref Vector3 p_moveAmount)
	{
		if (Vector3.Angle(m_averageNormal, Vector3.up) < 91)
		{
			return;
		}

		Vector3 rayOrigin = transform.position;
		RaycastHit hit;

		if (Physics.Raycast(rayOrigin, -m_averageNormal, out hit, Mathf.Infinity, m_collisionMask))
		{
			m_averageNormal = hit.normal;
		}

		float slopeAngle = Vector3.Angle(m_averageNormalTransform.up, Vector3.up);

		Quaternion slopeOffset = Quaternion.FromToRotation(Vector3.up, m_averageNormalTransform.up);
		Vector3 targetMoveAmont = slopeOffset * p_moveAmount;

		float tangent = Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * (p_moveAmount.magnitude * Time.fixedDeltaTime);
		float distance = (hit.distance - m_characterController.height / 2);

		if (distance < 0)
		{
			distance = 0;
		}

		p_moveAmount = targetMoveAmont;

		float moveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * targetMoveAmont.magnitude;
		p_moveAmount.y += moveAmountY;

		if (slopeAngle != 0 && slopeAngle <= m_maxSlopeAngle)
		{
			if (distance <= tangent)
			{

			}
		}

		Debug.DrawRay(transform.position, p_moveAmount, Color.red, 0f, false);
		//p_moveAmount = Vector3.zero;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{

	}

	#region Old Below Slope Code
	private void DecendSlopeBelow(ref Vector3 p_moveAmount)
	{
		Vector3 rayOrigin = transform.position;
		RaycastHit hit;

		if (Physics.Raycast(rayOrigin, -m_averageNormal, out hit, Mathf.Infinity, m_collisionMask))
		{
			m_averageNormal = hit.normal;
		}

		float slopeAngle = Vector3.Angle(m_averageNormal, Vector3.up);
		//float tangent = Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * (p_moveAmount.magnitude * Time.fixedDeltaTime);
		float tangent = Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * (m_horizontalMovement.magnitude * Time.fixedDeltaTime); // the current line above might break stuff if it is bring this line back 
		float distance = (hit.distance - m_characterController.height / 2);

		if (distance < 0)
		{
			distance = 0;
		}

		Quaternion slopeOffset = Quaternion.FromToRotation(Vector3.up, m_averageNormal);
		Vector3 targetMoveAmont = slopeOffset * m_horizontalMovement;

		//Debug.Log(slopeAngle);

		if (!(p_moveAmount.y > 0))
		{
			if (slopeAngle != 0 && slopeAngle <= m_maxSlopeAngle)
			{
				if (distance < tangent)
				{
					Debug.DrawRay(transform.position, transform.up * distance, Color.black, 0f, false);
					Debug.DrawRay(transform.position, transform.up * tangent, Color.cyan, 0f, false);

					p_moveAmount = targetMoveAmont;
					float moveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * p_moveAmount.magnitude;
					p_moveAmount.y += -moveAmountY;
				}
			}
		}
	}

	private void ClimbSlopeBelow(ref Vector3 p_moveAmount)
	{
		Vector3 rayOrigin = transform.position;
		RaycastHit hit;

		if (Physics.Raycast(rayOrigin, -m_averageNormal, out hit, Mathf.Infinity, m_collisionMask))
		{
			m_averageNormal = hit.normal;
		}

		float slopeAngle = Vector3.Angle(m_averageNormal, Vector3.up);
		//float tangent = Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * (p_moveAmount.magnitude * Time.fixedDeltaTime);
		float tangent = Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * (m_horizontalMovement.magnitude * Time.fixedDeltaTime); // the current line above might break stuff if it is bring this line back 
		float distance = (hit.distance - m_characterController.height / 2);

		if (distance < 0)
		{
			distance = 0;
		}

		Quaternion slopeOffset = Quaternion.FromToRotation(Vector3.up, m_averageNormal);
		Vector3 targetMoveAmont = slopeOffset * m_horizontalMovement;

		//Debug.Log(slopeAngle);

		float moveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * targetMoveAmont.magnitude;

		//Debug.DrawRay(transform.position, transform.up * p_moveAmount.y, Color.black, 0f, false);
		//Debug.DrawRay(transform.position, transform.up * moveAmountY, Color.cyan, 0f, false);

		if (HitSide() || HitBelow())
		{
			if (p_moveAmount.y <= moveAmountY)
			{
				//p_moveAmount = targetMoveAmont;
				//p_moveAmount.y = moveAmountY;
			}
		}

		if (slopeAngle != 0 && slopeAngle <= m_maxSlopeAngle)
		{

			if (distance < tangent)
			{

			}
		}
	}
	#endregion

	#region Above Player Slope
	private void ClimbSlopeAbove(ref Vector3 p_moveAmount)
	{
		if (p_moveAmount.y < 0)
		{
			Vector3 rayOrigin = m_playerTop;
			RaycastHit hit;

			if (Physics.Raycast(rayOrigin, Vector3.up, out hit, Mathf.Infinity, m_collisionMask))
			{
				Quaternion slopeQuatOffset = Quaternion.FromToRotation(Vector3.down, hit.normal);
				Vector3 targetMoveAmont = slopeQuatOffset * m_horizontalMovement;

				//Debug.DrawRay(transform.position, targetMoveAmont, Color.red, 0f, false);

				p_moveAmount = targetMoveAmont;
			}
		}
	}

	private void DecendSlopeAbove(ref Vector3 p_moveAmount)
	{
		Vector3 rayOrigin = transform.position;
		RaycastHit hit;

		if (Physics.Raycast(rayOrigin, -m_averageNormal, out hit, Mathf.Infinity, m_collisionMask))
		{
			m_averageNormal = hit.normal;
		}

		float slopeAngle = Vector3.Angle(m_averageNormal, Vector3.down);
		float tangent = Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * (m_horizontalMovement.magnitude * Time.fixedDeltaTime);
		float distance = (hit.distance - m_characterController.height / 2);

		if (distance < 0)
		{
			distance = 0;
		}

		Quaternion slopeOffset = Quaternion.FromToRotation(Vector3.down, m_averageNormal);
		Vector3 targetMoveAmont = slopeOffset * m_horizontalMovement;

		//Debug.Log(slopeAngle);

		if (slopeAngle != 0)
		{
			if (distance < tangent)
			{
				Debug.DrawRay(transform.position, transform.up * distance, Color.black, 0f, false);
				Debug.DrawRay(transform.position, transform.up * tangent, Color.cyan, 0f, false);

				p_moveAmount = targetMoveAmont;
				float moveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * p_moveAmount.magnitude;
				p_moveAmount.y += moveAmountY;
			}
		}

		if (!(p_moveAmount.y < 0))
		{

		}
	}
	#endregion

	#region Old
	private void OldCode(ControllerColliderHit hit)
	{
		float lostMovementAmount = m_targetMovementAmount.magnitude - hit.controller.velocity.magnitude;
		Vector3 targetMovement = hit.controller.velocity.normalized * lostMovementAmount;

		if (allGoodBoss)
		{
			//m_characterController.Move(m_characterController.velocity * Time.deltaTime);
			//Debug.DrawRay(transform.position, m_characterController.velocity, Color.red);

			//totalMag += m_characterController.velocity.magnitude;
			//Debug.Log(totalMag);

			//Debug.DrawRay(transform.position, Vector3.up * targetMoveAmont.y, Color.red, 0f, false);

			Vector3 pointOnCol = m_characterController.ClosestPoint(hit.point) + Vector3.up * ((m_characterController.radius / 2) - m_characterController.skinWidth);
			float dst = Vector3.Distance(pointOnCol, hit.point);

			float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

			float measureAmount = Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * (m_targetMovementAmount.magnitude);

			Debug.DrawRay(pointOnCol, Vector3.up * dst, Color.red);
			Debug.DrawRay(pointOnCol, Vector3.up * measureAmount, Color.blue);

			allGoodBoss = false;

		}
	}
	#endregion

	private void SetHorizontalVelocity(ref Vector3 p_moveAmount, Vector3 p_newMovement)
	{
		p_moveAmount = new Vector3(p_newMovement.x, p_moveAmount.y, p_newMovement.z);
	}

	private Vector3 GetHorizontalMovement(Vector3 p_moveAmount)
	{
		return new Vector3(p_moveAmount.x, 0, p_moveAmount.z);
	}

	private Vector3 GetVerticalMovement(Vector3 p_moveAmount)
	{
		return new Vector3(0, p_moveAmount.y, 0);
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
