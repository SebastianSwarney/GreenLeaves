using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastController : MonoBehaviour
{
	public LayerMask collisionMask;

	public const float skinWidth = .015f;
	const float dstBetweenRays = .25f;
	[HideInInspector]
	public int horizontalRayCount;
	[HideInInspector]
	public int verticalRayCount;

	[HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;

	[HideInInspector]
	public CharacterController characterController;
	public RaycastOrigins raycastOrigins;

	public virtual void Awake()
	{
		characterController = GetComponent<CharacterController>();
	}

	public virtual void Start()
	{
		CalculateRaySpacing();
	}

	public void UpdateRaycastOrigins()
	{
		Bounds bounds = characterController.bounds;
		bounds.Expand(skinWidth * -2);

		//raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		//raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);

		raycastOrigins.forwardMid = transform.forward * characterController.radius;
		raycastOrigins.topCenter = transform.up * (characterController.height);
		raycastOrigins.topCenter = -transform.up * (characterController.height);
	}

	public void CalculateRaySpacing()
	{
		Bounds bounds = characterController.bounds;
		bounds.Expand(skinWidth * -2);

		float boundsWidth = bounds.size.x;
		float boundsHeight = bounds.size.y;

		horizontalRayCount = Mathf.RoundToInt(characterController.height / dstBetweenRays);
		verticalRayCount = Mathf.RoundToInt((characterController.radius * 2) / dstBetweenRays);

		horizontalRaySpacing = characterController.height / (horizontalRayCount - 1);
		verticalRaySpacing = (characterController.radius * 2) / (verticalRayCount - 1);
	}

	public struct RaycastOrigins
	{
		public Vector3 topCenter, bottomCenter;
		public Vector3 forwardMid;
	}
}
