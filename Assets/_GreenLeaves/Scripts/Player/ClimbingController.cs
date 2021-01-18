using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingController : MonoBehaviour
{

	[SerializeField]
	Transform playerInputSpace = default;

	[SerializeField]
	Transform ball = default;

	[SerializeField, Range(0f, 100f)]
	float maxSpeed = 10f, maxClimbSpeed = 4f, maxSwimSpeed = 5f;

	[SerializeField, Range(0f, 100f)]
	float
		maxAcceleration = 10f,
		maxAirAcceleration = 1f,
		maxClimbAcceleration = 40f,
		maxSwimAcceleration = 5f;

	[SerializeField, Range(0f, 10f)]
	float jumpHeight = 2f;

	[SerializeField, Range(0, 5)]
	int maxAirJumps = 0;

	[SerializeField, Range(0, 90)]
	float maxGroundAngle = 25f, maxStairsAngle = 50f;

	[SerializeField, Range(90, 170)]
	float maxClimbAngle = 140f;

	[SerializeField, Range(0f, 100f)]
	float maxSnapSpeed = 100f;

	[SerializeField, Min(0f)]
	float probeDistance = 1f;

	[SerializeField]
	float submergenceOffset = 0.5f;

	[SerializeField, Min(0.1f)]
	float submergenceRange = 1f;

	[SerializeField, Min(0f)]
	float buoyancy = 1f;

	[SerializeField, Range(0f, 10f)]
	float waterDrag = 1f;

	[SerializeField, Range(0.01f, 1f)]
	float swimThreshold = 0.5f;

	[SerializeField]
	LayerMask probeMask = -1, stairsMask = -1, climbMask = -1, waterMask = 0;

	[SerializeField]
	Material
		normalMaterial = default,
		climbingMaterial = default,
		swimmingMaterial = default;

	[SerializeField, Min(0.1f)]
	float ballRadius = 0.5f;

	[SerializeField, Min(0f)]
	float ballAlignSpeed = 180f;

	[SerializeField, Min(0f)]
	float
		ballAirRotation = 0.5f,
		ballSwimRotation = 2f;

	Rigidbody body, connectedBody, previousConnectedBody;

	Vector3 playerInput;

	Vector3 velocity, connectionVelocity;

	Vector3 connectionWorldPosition, connectionLocalPosition;

	Vector3 upAxis, rightAxis, forwardAxis;

	bool desiredJump, desiresClimbing;

	Vector3 contactNormal, steepNormal, climbNormal, lastClimbNormal;

	Vector3 lastContactNormal, lastSteepNormal, lastConnectionVelocity;

	int groundContactCount, steepContactCount, climbContactCount;

	bool OnGround => groundContactCount > 0;

	bool OnSteep => steepContactCount > 0;

	bool Climbing => climbContactCount > 0 && stepsSinceLastJump > 2;

	bool InWater => submergence > 0f;

	bool Swimming => submergence >= swimThreshold;

	float submergence;

	int jumpPhase;

	float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;

	int stepsSinceLastGrounded, stepsSinceLastJump;

	public bool m_useBody;

	public void PreventSnapToGround()
	{
		stepsSinceLastJump = -1;
	}

	void OnValidate()
	{
		minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
		minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
		minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);
	}

	void Awake()
	{
		body = GetComponent<Rigidbody>();
		body.useGravity = false;
		OnValidate();
	}

	void Update()
	{
		playerInput.x = Input.GetAxis("Horizontal");
		playerInput.z = Input.GetAxis("Vertical");
		playerInput = Vector3.ClampMagnitude(playerInput, 1f);

		if (playerInputSpace)
		{
			rightAxis = ProjectDirectionOnPlane(playerInputSpace.right, upAxis);
			forwardAxis = ProjectDirectionOnPlane(playerInputSpace.forward, upAxis);
		}
		else
		{
			rightAxis = ProjectDirectionOnPlane(Vector3.right, upAxis);
			forwardAxis = ProjectDirectionOnPlane(Vector3.forward, upAxis);
		}

		desiresClimbing = true;
	}

	void FixedUpdate()
	{
		//Vector3 gravity = CustomGravity.GetGravity(body.position, out upAxis);

		Vector3 gravity = Vector3.down * 9.8f;
		upAxis = -gravity.normalized;

		UpdateState();

		AdjustVelocity();

		if (Climbing)
		{
			velocity -= contactNormal * (maxClimbAcceleration * 0.9f * Time.deltaTime);
		}
		else if (InWater)
		{
			//velocity += gravity * ((1f - buoyancy * submergence) * Time.deltaTime);
		}
		else if (OnGround && velocity.sqrMagnitude < 0.01f)
		{
			//velocity += contactNormal * (Vector3.Dot(gravity, contactNormal) * Time.deltaTime);
		}
		else if (desiresClimbing && OnGround)
		{
			//velocity += (gravity - contactNormal * (maxClimbAcceleration * 0.9f)) * Time.deltaTime;
		}
		else
		{
			//velocity += gravity * Time.deltaTime;
		}

		body.velocity = velocity;
		ClearState();
	}

	void ClearState()
	{
		lastContactNormal = contactNormal;
		lastSteepNormal = steepNormal;
		lastConnectionVelocity = connectionVelocity;
		groundContactCount = steepContactCount = climbContactCount = 0;
		contactNormal = steepNormal = climbNormal = Vector3.zero;
		connectionVelocity = Vector3.zero;
		previousConnectedBody = connectedBody;
		connectedBody = null;
		submergence = 0f;
	}

	void UpdateState()
	{
		stepsSinceLastGrounded += 1;
		stepsSinceLastJump += 1;
		velocity = body.velocity;

		if (CheckClimbing() || CheckSwimming() || OnGround || SnapToGround() || CheckSteepContacts())
		{
			stepsSinceLastGrounded = 0;
			if (stepsSinceLastJump > 1)
			{
				jumpPhase = 0;
			}
			if (groundContactCount > 1)
			{
				contactNormal.Normalize();
			}
		}
		else
		{
			contactNormal = upAxis;
		}

		if (connectedBody)
		{
			if (connectedBody.isKinematic || connectedBody.mass >= body.mass)
			{
				UpdateConnectionState();
			}
		}
	}

	void UpdateConnectionState()
	{
		if (connectedBody == previousConnectedBody)
		{
			Vector3 connectionMovement = connectedBody.transform.TransformPoint(connectionLocalPosition) - connectionWorldPosition;
			connectionVelocity = connectionMovement / Time.deltaTime;
		}

		connectionWorldPosition = body.position;
		connectionLocalPosition = connectedBody.transform.InverseTransformPoint(connectionWorldPosition);
	}

	bool CheckClimbing()
	{
		if (Climbing)
		{
			if (climbContactCount > 1)
			{
				climbNormal.Normalize();
				float upDot = Vector3.Dot(upAxis, climbNormal);
				if (upDot >= minGroundDotProduct)
				{
					climbNormal = lastClimbNormal;
				}
			}
			groundContactCount = 1;
			contactNormal = climbNormal;
			return true;
		}
		return false;
	}

	bool CheckSwimming()
	{
		if (Swimming)
		{
			groundContactCount = 0;
			contactNormal = upAxis;
			return true;
		}
		return false;
	}

	bool SnapToGround()
	{
		if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2 || InWater)
		{
			return false;
		}
		float speed = velocity.magnitude;
		if (speed > maxSnapSpeed)
		{
			return false;
		}
		if (!Physics.Raycast(body.position, -upAxis, out RaycastHit hit, probeDistance, probeMask, QueryTriggerInteraction.Ignore))
		{
			return false;
		}

		float upDot = Vector3.Dot(upAxis, hit.normal);
		if (upDot < GetMinDot(hit.collider.gameObject.layer))
		{
			return false;
		}

		groundContactCount = 1;
		contactNormal = hit.normal;
		float dot = Vector3.Dot(velocity, hit.normal);
		if (dot > 0f)
		{
			velocity = (velocity - hit.normal * dot).normalized * speed;
		}
		connectedBody = hit.rigidbody;
		return true;
	}

	bool CheckSteepContacts()
	{
		if (steepContactCount > 1)
		{
			steepNormal.Normalize();
			float upDot = Vector3.Dot(upAxis, steepNormal);
			if (upDot >= minGroundDotProduct)
			{
				steepContactCount = 0;
				groundContactCount = 1;
				contactNormal = steepNormal;
				return true;
			}
		}
		return false;
	}

	void AdjustVelocity()
	{
		float acceleration, speed;
		Vector3 xAxis, zAxis;

		acceleration = maxClimbAcceleration;
		speed = maxClimbSpeed;
		xAxis = Vector3.Cross(contactNormal, upAxis);
		zAxis = upAxis;

		if (Climbing)
		{

		}
		else
		{
			acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
			speed = OnGround && desiresClimbing ? maxClimbSpeed : maxSpeed;
			xAxis = rightAxis;
			zAxis = forwardAxis;
		}

		xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
		zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);

		Vector3 relativeVelocity = velocity - connectionVelocity;

		Vector3 adjustment;
		adjustment.x = playerInput.x * speed - Vector3.Dot(relativeVelocity, xAxis);
		adjustment.z = playerInput.z * speed - Vector3.Dot(relativeVelocity, zAxis);
		adjustment.y = Swimming ? playerInput.y * speed - Vector3.Dot(relativeVelocity, upAxis) : 0f;

		adjustment = Vector3.ClampMagnitude(adjustment, acceleration * Time.deltaTime);

		velocity += xAxis * adjustment.x + zAxis * adjustment.z;

		if (Swimming)
		{
			velocity += upAxis * adjustment.y;
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		EvaluateCollision(collision);
	}

	void OnCollisionStay(Collision collision)
	{
		EvaluateCollision(collision);
	}

	void EvaluateCollision(Collision collision)
	{
		int layer = collision.gameObject.layer;
		float minDot = GetMinDot(layer);

		for (int i = 0; i < collision.contactCount; i++)
		{
			Vector3 normal = collision.GetContact(i).normal;
			float upDot = Vector3.Dot(upAxis, normal);

			//desiresClimbing && upDot >= minClimbDotProduct &&

			if ((climbMask & (1 << layer)) != 0)
			{
				climbContactCount += 1;
				climbNormal += normal;
				lastClimbNormal = normal;
				connectedBody = collision.rigidbody;
			}

			/*
			if (upDot >= minDot)
			{
				groundContactCount += 1;
				contactNormal += normal;
				connectedBody = collision.rigidbody;
			}
			else
			{
				if (upDot > -0.01f)
				{
					steepContactCount += 1;
					steepNormal += normal;
					if (groundContactCount == 0)
					{
						connectedBody = collision.rigidbody;
					}
				}
			}
			*/
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if ((waterMask & (1 << other.gameObject.layer)) != 0)
		{
			EvaluateSubmergence(other);
		}
	}

	void OnTriggerStay(Collider other)
	{
		if ((waterMask & (1 << other.gameObject.layer)) != 0)
		{
			EvaluateSubmergence(other);
		}
	}

	void EvaluateSubmergence(Collider collider)
	{
		if (Physics.Raycast(body.position + upAxis * submergenceOffset, -upAxis, out RaycastHit hit, submergenceRange + 1f, waterMask, QueryTriggerInteraction.Collide))
		{
			submergence = 1f - hit.distance / submergenceRange;
		}
		else
		{
			submergence = 1f;
		}
		if (Swimming)
		{
			connectedBody = collider.attachedRigidbody;
		}
	}

	Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
	{
		return (direction - normal * Vector3.Dot(direction, normal)).normalized;
	}

	float GetMinDot(int layer)
	{
		return (stairsMask & (1 << layer)) == 0 ?
			minGroundDotProduct : minStairsDotProduct;
	}
}