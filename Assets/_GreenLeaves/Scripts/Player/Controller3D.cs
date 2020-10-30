using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller3D : MonoBehaviour
{
	public float maxSlopeAngle = 80;

	public LayerMask layerMask;
	public CollisionInfo collisions;
	[HideInInspector]
	public Vector2 playerInput;
	public float skinWidth;
	public float skinWidthHeight;

	private CapsuleCollider capsuleCollider;

	private Vector3 lastHitPoint;
	private Vector3 lastHitNormal;

	//private float verticalRayLength;

	public Vector3 testVelocity;
	public float testSpeed;

	private void Start()
	{
		capsuleCollider = GetComponent<CapsuleCollider>();
	}

	private void FixedUpdate()
	{
		Move((transform.forward * testSpeed) * Time.fixedDeltaTime, false);
	}

	public void Move(Vector3 moveAmount, bool standingOnPlatform)
	{
		Move(moveAmount, Vector3.zero, standingOnPlatform);
	}

	public void Move(Vector3 moveAmount, Vector2 input, bool standingOnPlatform = false)
	{
		collisions.Reset();
		collisions.moveAmountOld = moveAmount;
		playerInput = input;

		HorizontalCollisions(ref moveAmount);

		if (moveAmount.y != 0)
		{
			//VerticalCollisions(ref moveAmount);
		}

		transform.Translate(moveAmount, Space.World);
	}

	private float GetAdjustedHitDistance(Vector3 p_hitPoint)
	{
		Vector3 closestPoint = capsuleCollider.ClosestPoint(p_hitPoint);
		float distanceInsideOfCollider = Vector3.Distance(p_hitPoint, closestPoint);

		Debug.DrawLine(p_hitPoint, closestPoint, Color.red, 0f, false);

		if (p_hitPoint == Vector3.zero)
		{
			distanceInsideOfCollider = 0;
		}

		return distanceInsideOfCollider;
	}

	void HorizontalCollisions(ref Vector3 moveAmount)
	{
		float directionX = -1;
		float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

		if (Mathf.Abs(moveAmount.x) < skinWidth)
		{
			rayLength = 2 * skinWidth;
		}

		float dstBetweenRays = .15f;

		int verticalRayCount = Mathf.RoundToInt(capsuleCollider.height / dstBetweenRays);
		float verticalRaySpacing = capsuleCollider.height / (verticalRayCount - 1);

		for (int verticalIndex = 0; verticalIndex < verticalRayCount; verticalIndex++)
		{
			Vector3 bottom = transform.position + capsuleCollider.center + Vector3.up * -capsuleCollider.height * 0.5F;
			Vector3 rayOrigin = bottom;
			rayOrigin += Vector3.up * (verticalRaySpacing * verticalIndex);

			float totalAngle = 180f;
			float raySpacing = 10f;
			int rayCount = Mathf.RoundToInt(totalAngle / raySpacing);

			for (int radiusIndex = 0; radiusIndex <= rayCount; radiusIndex++)
			{
				float currentAngle = ((radiusIndex * raySpacing) - (raySpacing * (rayCount / 2)));
				Quaternion raySpaceQ = Quaternion.Euler(0, currentAngle, 0);
				Vector3 colliderPointDir = raySpaceQ * transform.forward;

				//Debug.DrawRay(rayOrigin, colliderPointDir, Color.red);

				Ray ray = new Ray(rayOrigin + (colliderPointDir), -colliderPointDir);
				RaycastHit colliderHit;

				if (capsuleCollider.Raycast(ray, out colliderHit, Mathf.Infinity))
				{
					Debug.DrawRay(colliderHit.point, transform.forward * rayLength, Color.blue);

					RaycastHit hit;

					if (Physics.Raycast(colliderHit.point, transform.forward, out hit, rayLength, layerMask))
					{
						moveAmount = moveAmount.normalized * (hit.distance - skinWidth);
						rayLength = hit.distance;
					}
				}


			}
		}

	}

	void VerticalCollisions(ref Vector3 moveAmount)
	{
		float directionY = Mathf.Sign(moveAmount.y);
		float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

		float dstBetweenRays = .1f;

		int verticalRayCount = Mathf.RoundToInt(capsuleCollider.radius * 2f / dstBetweenRays);
		float verticalRaySpacing = capsuleCollider.radius * 2f / (verticalRayCount - 1);

		for (int rightRayIndex = 0; rightRayIndex < verticalRayCount; rightRayIndex++)
		{
			Vector3 bottom = transform.position + capsuleCollider.center + Vector3.up * -capsuleCollider.height * 0.5F;
			Vector3 top = bottom + Vector3.up * capsuleCollider.height;

			top += transform.right * capsuleCollider.radius;
			top += -transform.forward * capsuleCollider.radius;

			Vector3 rightRayOrigin = top;
			rightRayOrigin += Vector3.right * (verticalRaySpacing * rightRayIndex);

			float shortestPoint = 1000000000000000;

			for (int forwardRayIndex = 0; forwardRayIndex < verticalRayCount; forwardRayIndex++)
			{
				Vector3 forwardRayOrigin = rightRayOrigin + Vector3.forward * (verticalRaySpacing * forwardRayIndex);

				Vector3 rayOrigin = forwardRayOrigin;

				Ray ray = new Ray(forwardRayOrigin, Vector3.down);
				RaycastHit colliderHit;

				if (capsuleCollider.Raycast(ray, out colliderHit, Mathf.Infinity))
				{
					rayOrigin = colliderHit.point;

					if (rayOrigin.y < shortestPoint)
					{
						shortestPoint = rayOrigin.y;
					}
				}

				if (rayOrigin != forwardRayOrigin)
				{
					//rayOrigin = capsuleCollider.ClosestPoint(new Vector3(rayOrigin.x, 0, rayOrigin.z) + new Vector3(0, shortestPoint, 0));

					RaycastHit hit;

					if (Physics.Raycast(rayOrigin, Vector3.up * directionY, out hit, rayLength, layerMask))
					{
						moveAmount.y = (hit.distance - skinWidth) * directionY;
						rayLength = hit.distance;
					}

					Debug.DrawRay(rayOrigin, Vector3.up * directionY * rayLength, Color.red, 0f, true);
				}
			}
		}
	}

	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public bool slidingDownMaxSlope;

		public float slopeAngle, slopeAngleOld;
		public Vector3 slopeNormal;
		public Vector3 moveAmountOld;
		public int faceDir;
		public bool fallingThroughPlatform;

		public void Reset()
		{
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;
			slidingDownMaxSlope = false;
			slopeNormal = Vector3.zero;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}
}
