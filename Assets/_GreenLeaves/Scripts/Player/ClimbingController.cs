using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingController : RaycastController
{
	public float maxSlopeAngle = 80;

	public CollisionInfo collisions;
	[HideInInspector]
	public Vector2 playerInput;


	public Vector3 testVel;
	public Transform movementDirection;

	public bool disableController;

	public override void Start()
	{
		base.Start();
		collisions.faceDir = 1;
	}

	private void Update()
	{
		Vector3 velcoity = testVel;
		Move(velcoity * Time.deltaTime, new Vector2(1, 0));

		if (disableController)
		{
			characterController.detectCollisions = false;
			characterController.enableOverlapRecovery = false;
		}
		else
		{
			characterController.detectCollisions = true;
			characterController.enableOverlapRecovery = true;
		}
	}

	public void Move(Vector3 moveAmount, bool standingOnPlatform)
	{
		Move(moveAmount, Vector3.zero, standingOnPlatform);
	}

	public void Move(Vector3 moveAmount, Vector2 input, bool standingOnPlatform = false)
	{
		Vector3 horizontalMovement = new Vector3(moveAmount.x, 0, moveAmount.z);
		movementDirection.rotation = Quaternion.LookRotation(horizontalMovement.normalized, Vector3.up);

		UpdateRaycastOrigins();

		collisions.Reset();
		collisions.moveAmountOld = moveAmount;
		playerInput = input;

		if (!disableController)
		{
			if (moveAmount.y < 0)
			{
				
			}

			DescendSlope(ref moveAmount);

			if (moveAmount.x != 0)
			{
				collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
			}

			ForwardCollisions(ref moveAmount);

			if (moveAmount.y != 0)
			{
				VerticalCollisions(ref moveAmount);
			}
		}

		//transform.Translate(moveAmount, Space.World);
		characterController.Move(moveAmount);

		if (standingOnPlatform)
		{
			collisions.below = true;
		}
	}

	void ForwardCollisions(ref Vector3 moveAmount)
	{
		//float directionX = collisions.faceDir;
		float directionX = Mathf.Sign(moveAmount.x);
		float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

		if (Mathf.Abs(moveAmount.x) < skinWidth)
		{
			rayLength = 2 * skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector3 rayOrigin = transform.position - (transform.up * (characterController.height / 2));
			rayOrigin += (transform.forward * (characterController.radius - skinWidth));
			rayOrigin += Vector3.up * (horizontalRaySpacing * i);

			RaycastHit hit;

			Debug.DrawRay(rayOrigin, transform.forward * rayLength, Color.red);

			if (Physics.Raycast(rayOrigin, transform.forward, out hit, rayLength, collisionMask))
			{
				if (hit.distance == 0)
				{
					continue;
				}

				float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

				if (i == 0 && slopeAngle <= maxSlopeAngle)
				{
					if (collisions.descendingSlope)
					{
						collisions.descendingSlope = false;
						moveAmount = collisions.moveAmountOld;
					}
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld)
					{
						distanceToSlopeStart = hit.distance - skinWidth;
						moveAmount.x -= distanceToSlopeStart * directionX;
					}

					ClimbSlope(ref moveAmount, slopeAngle, hit.normal);

					moveAmount.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
				{
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope)
					{
						moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}
			}
		}
	}

	void VerticalCollisions(ref Vector3 moveAmount)
	{
		float directionY = Mathf.Sign(moveAmount.y);
		float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector3 rayOrigin = transform.position + (transform.up * directionY * (characterController.height / 2));
			rayOrigin -= (movementDirection.forward * characterController.radius);
			rayOrigin += movementDirection.forward * (verticalRaySpacing * i + moveAmount.x);

			RaycastHit hit;

			Debug.DrawRay(rayOrigin, Vector3.up * directionY * rayLength, Color.red);

			if (Physics.Raycast(rayOrigin, Vector3.up * directionY, out hit, rayLength, collisionMask))
			{
				moveAmount.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				if (collisions.climbingSlope)
				{
					moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}

		if (collisions.climbingSlope)
		{
			float directionX = Mathf.Sign(moveAmount.x);
			rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
			//Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
			Vector3 rayOrigin = transform.position - (transform.up * (characterController.height / 2)) + (transform.forward * characterController.radius) + (Vector3.up * moveAmount.y);

			RaycastHit hit;

			//Debug.DrawRay(rayOrigin, Vector3.right * directionX * 1f, Color.blue);

			if (Physics.Raycast(rayOrigin, Vector3.right * directionX, out hit, 1f, collisionMask))
			{
				float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

				if (slopeAngle != collisions.slopeAngle)
				{
					//Debug.Log("ran");

					Debug.DrawLine(rayOrigin, hit.point, Color.cyan);

					moveAmount.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
					collisions.slopeNormal = hit.normal;
				}
			}

		}
	}

	
	void ClimbSlope(ref Vector3 moveAmount, float slopeAngle, Vector3 slopeNormal)
	{
		Debug.Log("im here");

		float moveDistance = Mathf.Abs(moveAmount.x);
		float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (moveAmount.y <= climbmoveAmountY)
		{
			moveAmount.y = climbmoveAmountY;
			moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
			collisions.slopeNormal = slopeNormal;
		}
	}

	void DescendSlope(ref Vector3 moveAmount)
	{
		/*
		RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
		RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, collisionMask);
		
		if (maxSlopeHitLeft ^ maxSlopeHitRight)
		{
			SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
			SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
		}
		*/

		if (!collisions.slidingDownMaxSlope)
		{
			float directionX = Mathf.Sign(moveAmount.x);

			//Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
			//RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

			Vector3 rayOrigin = transform.position - (transform.up * (characterController.height / 2)) - (transform.forward * characterController.radius);

			RaycastHit hit;

			if (Physics.Raycast(rayOrigin, Vector3.up, out hit, Mathf.Infinity, collisionMask))
			{
				float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

				if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
				{
					if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
					{
						float moveDistance = Mathf.Abs(moveAmount.x);
						float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
						moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
						moveAmount.y += descendmoveAmountY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
						collisions.slopeNormal = hit.normal;
					}

					if (Mathf.Sign(hit.normal.x) == directionX)
					{

					}
				}
			}
		}
	}

	/*
	void SlideDownMaxSlope(RaycastHit2D hit, ref Vector3 moveAmount)
	{

		if (hit)
		{
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle > maxSlopeAngle)
			{
				moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

				collisions.slopeAngle = slopeAngle;
				collisions.slidingDownMaxSlope = true;
				collisions.slopeNormal = hit.normal;
			}
		}

	}
	*/

	void ResetFallingThroughPlatform()
	{
		collisions.fallingThroughPlatform = false;
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
