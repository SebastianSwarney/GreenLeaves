using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RootMotion.FinalIK;

public class CollisionController : MonoBehaviour
{
	public LayerMask m_groundMask;
	public LayerMask m_slideMask;

	public float m_gravity = -9.8f;

	public Transform m_viewCameraTransform;

	[Header("Horizontal Movement")]
	public float m_playerTurnSpeed = 0.1f;
	public float m_runSpeed;
	public float m_accelerationTime;
	public float m_sprintSpeed;

	private Vector3 m_groundMovementVelocity;
	private Vector3 m_groundMovementVelocitySmoothing;
	private Vector3 m_gravityVelocity;
	private Vector2 m_movementInput;

	private float m_playerTurnSmoothingVelocity;
	private bool m_isGrounded;
	private CharacterController m_characterController;
	private Vector3 m_averageNormal;

	#region Slope Properties
	[Header("Slope")]
	public Transform m_slopeTransform;
	public Vector2 m_minMaxSlideSpeed;
	public float m_slideStartAngle;
	private float m_currentSlopeAngle;
	private bool m_onSlope;

	[HideInInspector]
	public bool m_isSliding;

	private Vector3 m_slopeVelocity;
	public float m_slideEndPushTime;
	private bool m_onSlideSurface;
	public float m_preSlideTime;
	private bool m_runningPreSlide;
	public AnimationCurve m_preSlideCurve;

	private float m_slopeFacingDirection;

	public float m_slopeSpeedSlowStartAngle;
	#endregion

	private Vector3 m_horizontalDirection;

	public float m_slideEndAngle;

	public Text m_slopeText;

	public float m_slideRecoveryTime;


	public AnimationCurve m_slopeSlowCurve;
	public float m_minimumSlopeSpeedPercent;

	private bool m_sprinting;

	private PlayerVisualsController m_playerVisuals;

	private Vector3 m_horizontalVelocity;

	public bool m_playSlideAnimation;

	private float m_currentSlownessFactor;


	[Header("Climb properties")]
	public LayerMask m_climbMask;

	public Transform m_climbTransform;

	private bool m_climbing;

	public float m_climbSpeed;

	private Vector3 m_climbVelocity;

	public float m_climbStartDistance;

	private bool m_onClimbSurface;

	public float m_climbStartTime;

	private bool m_useGravity;

	private Vector3 m_wallStickVelocity;

	private Vector3 m_horizontalWallDirection;

	public float m_wallAlignmentStart = 0.9f;

	public float m_climbAcceleration;

	private Vector3 m_climbMovementSmoothingVelocity;

	private bool m_clamber;

	public float m_clamberSpeed;

	public float m_clamberEndTime;

	public float m_clamberEndSpeed;

	public AnimationCurve m_clamberCurve;

	public Transform m_leftHand;
	public Transform m_rightHand;

	private FullBodyBipedIK m_fullBodyBipedIK;

	private void Start()
	{
		m_characterController = GetComponent<CharacterController>();
		//m_characterController.slopeLimit = m_maxSlopeAngle;
		m_playerVisuals = GetComponent<PlayerVisualsController>();

		m_fullBodyBipedIK = GetComponentInChildren<FullBodyBipedIK>();

		m_fullBodyBipedIK.solver.leftHandEffector.positionWeight = 0f;
		m_fullBodyBipedIK.solver.leftHandEffector.rotationWeight = 0f;

		m_fullBodyBipedIK.solver.rightHandEffector.positionWeight = 0f;
		m_fullBodyBipedIK.solver.rightHandEffector.rotationWeight = 0f;
	}


	public void SetMovementInput(Vector2 p_input)
	{
		m_movementInput = p_input;
	}

	private void FixedUpdate()
	{
		m_isGrounded = m_characterController.isGrounded;

		CalculateSlopeVariables();

		GroundMovement();

		CalculateGravity();

		ClimbLoop();

		//CheckSlide();

		CaculateTotalVelocity();
		DecendSlopeBelow(m_characterController.velocity * Time.fixedDeltaTime);

		m_isGrounded = false;
	}

	private void Update()
	{
		m_playerVisuals.SetAnimations(m_groundMovementVelocity, m_runSpeed, m_sprintSpeed, m_runSpeed * 0.5f);
		//m_playerVisuals.CalculateSlopeEffort(m_currentSlopeAngle, m_slopeSpeedSlowStartAngle, m_slideStartAngle);
		//m_playerVisuals.SetModelRotation(m_horizontalDirection, m_averageNormal, m_slopeFacingDirection, m_currentSlopeAngle, m_slopeSpeedSlowStartAngle, m_slideStartAngle);
	}

	private void CaculateTotalVelocity()
	{
		Vector3 velocity = Vector3.zero;

		velocity += m_groundMovementVelocity;
		velocity += m_slopeVelocity;

		//velocity *= m_currentSlownessFactor;

		velocity += m_climbVelocity;
		velocity += m_wallStickVelocity;

		velocity += m_gravityVelocity;


		ClimbSlopeBelow(ref velocity, new Vector3(velocity.x, 0, velocity.z));

		if (velocity.normalized.magnitude >= 1)
		{
			m_horizontalDirection = new Vector3(velocity.x, 0, velocity.z).normalized;
		}


		m_characterController.Move(velocity * Time.fixedDeltaTime);


		m_horizontalVelocity = new Vector3(m_characterController.velocity.x, 0, m_characterController.velocity.z);

		DebugExtension.DebugArrow(transform.position, m_horizontalDirection);
		
	}

	#region Climb
	private void ClimbLoop()
	{
		ClimbCollision();

		if (CanStartClimb())
		{
			StartCoroutine(RunClimb());
		}
	}

	private bool CanStartClimb()
	{
		float wallFacingDot = Vector3.Dot(m_horizontalDirection, m_horizontalWallDirection);

		if (m_onClimbSurface && wallFacingDot >= m_wallAlignmentStart && m_movementInput.y > 0 && !m_climbing)
		{
			return true;
		}

		return false;
	}

	private bool GroundClimbCancel()
	{
		if (m_movementInput.y < 0 && m_characterController.isGrounded)
		{
			return true;
		}

		return false;
	}

	private IEnumerator RunClimb()
	{
		GetComponentInChildren<Animator>().SetBool("Climb", true);

		m_climbing = true;

		float targetClamberHeight = 0;

		Vector3 ledgeTopPos = Vector3.zero;

		while (m_onClimbSurface && m_climbing && !m_clamber)
		{
			#region Climb movement
			Vector3 verticalClimbMovment = m_climbTransform.up * m_movementInput.y;
			Vector3 horizontalClimbMovment = m_climbTransform.right * -m_movementInput.x;
			Vector3 targetClimbMovement = Vector3.ClampMagnitude(verticalClimbMovment + horizontalClimbMovment, 1) * m_climbSpeed;
			Vector3 climbMovement = Vector3.SmoothDamp(m_climbVelocity, targetClimbMovement, ref m_climbMovementSmoothingVelocity, m_climbAcceleration);
			m_climbVelocity = climbMovement;

			m_wallStickVelocity = -m_climbTransform.forward * m_climbSpeed;

			transform.rotation = Quaternion.LookRotation(m_horizontalWallDirection);
			#endregion

			#region Climb Animation
			Vector3 localClimbVelocity = transform.InverseTransformDirection(m_climbVelocity);
			Vector2 normalClimbVel = new Vector2(Mathf.InverseLerp(-m_climbSpeed, m_climbSpeed, localClimbVelocity.x), Mathf.InverseLerp(-m_climbSpeed, m_climbSpeed, localClimbVelocity.y));
			Vector2 animValue = new Vector2(Mathf.Lerp(-1, 1, normalClimbVel.x), Mathf.Lerp(-1, 1, normalClimbVel.y));
			m_playerVisuals.ClimbAnimations(animValue.x, animValue.y);
			#endregion

			if (GroundClimbCancel())
			{
				m_climbing = false;
			}

			Vector3 top = transform.position + (Vector3.up * (m_characterController.height / 2));

			RaycastHit clamberHit;

			if (Physics.Raycast(top, transform.forward, out clamberHit, m_climbStartDistance, m_climbMask))
			{
				if (Vector3.Angle(clamberHit.normal, Vector3.up) < 90)
				{
					m_clamber = true;

					targetClamberHeight = top.y;

					ledgeTopPos = clamberHit.point;
				}

				ledgeTopPos = clamberHit.point;
			}
			else
			{
				m_clamber = true;

				targetClamberHeight = top.y;
			}

			yield return new WaitForFixedUpdate();
		}

		m_climbMovementSmoothingVelocity = Vector3.zero;
		m_climbVelocity = Vector3.zero;

		float startHeight =  (transform.position + (Vector3.down * (m_characterController.height / 2))).y;

		m_fullBodyBipedIK.solver.leftHandEffector.positionWeight = 1f;
		m_fullBodyBipedIK.solver.leftHandEffector.rotationWeight = 1f;

		m_fullBodyBipedIK.solver.rightHandEffector.positionWeight = 1f;
		m_fullBodyBipedIK.solver.rightHandEffector.rotationWeight = 1f;

		while (m_clamber)
		{
			Debug.DrawLine(transform.position, ledgeTopPos, Color.red);

			m_leftHand.position = ledgeTopPos + (m_climbTransform.right * 0.5f) + (Vector3.up * 0.1f);
			m_rightHand.position = ledgeTopPos - (m_climbTransform.right * 0.5f) + (Vector3.up * 0.1f);

			Vector3 bottom = transform.position + (Vector3.down * (m_characterController.height / 2));

			float heightProgress = Mathf.InverseLerp(startHeight, targetClamberHeight, bottom.y);

			float currentClamberSpeed = Mathf.Lerp(m_clamberSpeed, m_climbSpeed, m_clamberCurve.Evaluate(heightProgress));

			m_climbVelocity.y = currentClamberSpeed;


			RaycastHit clamberHit;

			if (Physics.Raycast(bottom, transform.forward, out clamberHit, m_climbStartDistance, m_climbMask))
			{
				if (Vector3.Angle(clamberHit.normal, Vector3.up) < 90)
				{
					m_clamber = false;
				}
			}
			else
			{
				m_clamber = false;
			}

			Debug.Log(heightProgress);

			if (heightProgress >= m_forwardSlidePercent)
			{
				ledgeTopPos += transform.forward * m_forwardSlideSpeed * Time.fixedDeltaTime;

				GetComponentInChildren<Animator>().SetBool("Climb", false);
			}

			yield return new WaitForFixedUpdate();
		}

		m_wallStickVelocity = Vector3.zero;

		float clamberForwardTimer = 0;

		while (clamberForwardTimer < m_clamberEndTime)
		{
			clamberForwardTimer += Time.fixedDeltaTime;

			m_climbVelocity = transform.forward * m_clamberEndSpeed;

			float progress = clamberForwardTimer / m_clamberEndTime;

			float weightProgress = Mathf.Lerp(1, 0, progress);

			m_fullBodyBipedIK.solver.leftHandEffector.positionWeight = weightProgress;
			m_fullBodyBipedIK.solver.leftHandEffector.rotationWeight = weightProgress;

			m_fullBodyBipedIK.solver.rightHandEffector.positionWeight = weightProgress;
			m_fullBodyBipedIK.solver.rightHandEffector.rotationWeight = weightProgress;

			yield return new WaitForFixedUpdate();
		}

		m_climbMovementSmoothingVelocity = Vector3.zero;
		m_climbVelocity = Vector3.zero;
		m_wallStickVelocity = Vector3.zero;

		m_climbing = false;
	}

	public float m_forwardSlidePercent;
	public float m_forwardSlideSpeed;

	private void ClimbCollision()
	{
		RaycastHit climbHit;

		if (Physics.Raycast(transform.position, transform.forward, out climbHit, m_climbStartDistance, m_climbMask))
		{
			if (Vector3.Angle(climbHit.normal, Vector3.up) >= 90)
			{
				m_climbTransform.rotation = Quaternion.LookRotation(climbHit.normal);

				m_horizontalWallDirection = -new Vector3(m_climbTransform.forward.x, 0, m_climbTransform.forward.z);

				m_onClimbSurface = true;
			}
			else
			{
				m_onClimbSurface = false;
			}
		}
		else
		{
			m_onClimbSurface = false;
		}
	}
	#endregion

	#region Slope
	private void CalculateSlopeVariables()
	{
		m_currentSlopeAngle = Vector3.Angle(m_averageNormal, Vector3.up);

		RaycastHit hit;

		if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, m_groundMask))
		{
			m_currentSlopeAngle = Vector3.Angle(hit.normal, Vector3.up);
		}

		m_slopeText.text = m_currentSlopeAngle.ToString();

		m_slopeTransform.rotation = Quaternion.LookRotation(m_averageNormal);

		Vector3 normalCross = Vector3.Cross(Vector3.up, m_averageNormal);
		Vector3 movementSlopeCross = Vector3.Cross(normalCross, m_horizontalDirection).normalized;

		m_slopeFacingDirection = Mathf.Sign(movementSlopeCross.y);
	}

	private void CheckSlide()
	{
		if (m_slopeFacingDirection < 0)
		{
			//Start the pre slide and leave
			StartSlideLoop(m_slopeFacingDirection);
			return;
		}

		m_currentSlownessFactor = 1;

		if (m_slopeFacingDirection > 0 && m_currentSlopeAngle >= m_slopeSpeedSlowStartAngle)
		{
			float slopeInverse = m_slopeSlowCurve.Evaluate(Mathf.InverseLerp(m_slopeSpeedSlowStartAngle, m_slideStartAngle, m_currentSlopeAngle));
			float currentSlownessFactor = Mathf.Lerp(1, m_minimumSlopeSpeedPercent, slopeInverse);

			m_currentSlownessFactor = currentSlownessFactor;

			//m_groundMovementVelocity *= currentSlownessFactor;
		}
		else
		{
			m_currentSlownessFactor = 1;
		}

		if (m_currentSlopeAngle >= m_slideStartAngle)
		{
			//Debug.Log("Start the pre slide");
			StartSlideLoop(m_slopeFacingDirection);
		}
	}

	private bool CheckSlideConditions()
	{
		if (m_currentSlopeAngle >= m_slideEndAngle && m_onSlideSurface)
		{
			return true;
		}

		return false;
	}

	private void StartSlideLoop(float p_facingDir)
	{
		if (!m_runningPreSlide && !m_isSliding && CheckSlideConditions())
		{
			StartCoroutine(RunPreSlide(p_facingDir));
		}
	}

	private IEnumerator RunPreSlide(float p_facingDir)
	{
		m_runningPreSlide = true;

		float t = 0;

		while (t < m_preSlideTime && CheckSlideConditions())
		{
			t += Time.fixedDeltaTime;
			float progress = m_preSlideCurve.Evaluate(t / m_preSlideTime);

			float currentSpeed = Mathf.Lerp(0, m_minMaxSlideSpeed.x, progress);
			Vector3 targetSlopeVelocity = -m_slopeTransform.up * currentSpeed;
			m_slopeVelocity = targetSlopeVelocity;

			yield return new WaitForFixedUpdate();

		}

		m_runningPreSlide = false;

		StartCoroutine(RunSlide(p_facingDir));
	}

	private IEnumerator RunSlide(float p_facingDir)
	{
		m_isSliding = true;

		m_playSlideAnimation = true;

		while (CheckSlideConditions())
		{
			float currentSlopePercent = Mathf.InverseLerp(m_slideEndAngle, 90, m_currentSlopeAngle);
			float currentSlopeSpeed = Mathf.Lerp(m_minMaxSlideSpeed.x, m_minMaxSlideSpeed.y, currentSlopePercent);
			Vector3 targetSlopeVelocity = -m_slopeTransform.up * currentSlopeSpeed;
			m_slopeVelocity = targetSlopeVelocity;

			SlideRotation(p_facingDir);

			yield return new WaitForFixedUpdate();
		}

		m_slopeVelocity = Vector3.zero;

		StartCoroutine(SlideEndPush(p_facingDir));
	}

	private IEnumerator SlideEndPush(float p_facingDir)
	{
		float t = 0;

		while (t < m_slideEndPushTime)
		{
			t += Time.fixedDeltaTime;

			float progress = t / m_slideEndPushTime;

			float currentSpeed = Mathf.Lerp(m_minMaxSlideSpeed.x, 0, progress);

			Vector3 targetSlopeVelocity = -m_slopeTransform.up * currentSpeed;
			m_slopeVelocity = targetSlopeVelocity;

			SlideRotation(p_facingDir);

			yield return new WaitForFixedUpdate();
		}

		m_slopeVelocity = Vector3.zero;

		m_playSlideAnimation = false;

		t = 0;

		while (t < m_slideRecoveryTime)
		{
			t += Time.fixedDeltaTime;

			yield return new WaitForFixedUpdate();
		}

		m_isSliding = false;
	}

	private void SlideRotation(float p_facingDir)
	{
		float targetAngle = Mathf.Atan2(m_slopeVelocity.normalized.x, m_slopeVelocity.normalized.z) * Mathf.Rad2Deg;
		float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref m_playerTurnSmoothingVelocity, m_playerTurnSpeed);

		transform.rotation = Quaternion.Euler(0, targetAngle, 0);
	}
	#endregion

	#region Gravity

	private bool CheckGravityConditions()
	{
		if (m_climbing)
		{
			return true;
		}

		if (m_isGrounded)
		{
			return true;
		}

		if (m_useGravity)
		{
			return true;
		}

		return false;
	}

	private void CalculateGravity()
	{
		if (CheckGravityConditions())
		{
			m_gravityVelocity.y = 0;

			return;
		}

		m_gravityVelocity.y += m_gravity * Time.fixedDeltaTime;
	}
	#endregion

	#region Ground Movement

	public void OnSprintButtonDown()
	{
		m_sprinting = true;
	}

	public void OnSprintButtonUp()
	{
		m_sprinting = false;
	}

	public bool CheckGroundMovement()
	{
		if (m_isSliding)
		{
			return true;
		}

		if (m_runningPreSlide)
		{
			return true;
		}

		if (m_climbing)
		{
			return true;
		}

		return false;
	}

	private void GroundMovement()
	{
		if (CheckGroundMovement())
		{
			m_groundMovementVelocity = Vector3.zero;
			m_groundMovementVelocitySmoothing = Vector3.zero;

			return;
		}

		Vector3 horizontalInput = Vector3.ClampMagnitude(new Vector3(m_movementInput.x, 0, m_movementInput.y), 1f);

		float targetAngle = transform.eulerAngles.y;

		if (horizontalInput.magnitude > 0)
		{
			targetAngle = Mathf.Atan2(horizontalInput.x, horizontalInput.z) * Mathf.Rad2Deg + m_viewCameraTransform.eulerAngles.y;
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref m_playerTurnSmoothingVelocity, m_playerTurnSpeed);
			transform.rotation = Quaternion.Euler(0, angle, 0);
		}

		float horizontalSpeed;
		float currentAcceleration = m_accelerationTime;

		if (m_sprinting)
		{
			horizontalSpeed = m_sprintSpeed;
		}
		else
		{
			horizontalSpeed = m_runSpeed;
		}

		Vector3 targetMovementRotation = Quaternion.Euler(0, targetAngle, 0f) * Vector3.forward;
		Vector3 actualMovementDir = Quaternion.Euler(0, targetAngle, 0f) * Vector3.forward;

		Vector3 targetHorizontalMovement = (actualMovementDir * horizontalSpeed) * horizontalInput.magnitude;
		Vector3 horizontalMovement = Vector3.SmoothDamp(m_groundMovementVelocity, targetHorizontalMovement, ref m_groundMovementVelocitySmoothing, currentAcceleration);

		m_groundMovementVelocity = new Vector3(horizontalMovement.x, 0, horizontalMovement.z);
	}
	#endregion

	#region Collision

	private bool m_blockGroundSnapping;

	private bool ResetGroundSnap()
	{
		if (m_isGrounded && !m_climbing)
		{
			return true;
		}

		return false;
	}

	private bool CanGroundSnap()
	{
		if (m_climbing)
		{
			return false;
		}

		if (m_blockGroundSnapping)
		{
			return false;
		}

		return true;
	}

	private void DecendSlopeBelow(Vector3 p_moveAmount)
	{
		if (ResetGroundSnap())
		{
			m_blockGroundSnapping = false;
		}

		if (!CanGroundSnap())
		{
			return;
		}

		#region no use
		/*
		Vector3 rayDir = Vector3.down;
		float castRadius = m_characterController.radius - m_castSkinWidth;
		float rayLength = Mathf.Abs(p_moveAmount.magnitude) + (m_castSkinWidth * 2);

		Vector3 bottom = m_characterController.transform.position - new Vector3(0, m_characterController.height / 2, 0);
		Vector3 top = bottom + (Vector3.up * m_characterController.height);

		Vector3 bottomOrigin = bottom - (rayDir * m_castSkinWidth);
		Vector3 topOrigin = top - (rayDir * m_castSkinWidth);

		RaycastHit[] hits = Physics.CapsuleCastAll(bottomOrigin, topOrigin, castRadius, rayDir, rayLength, m_groundMask);

		float distance = 0;

		for (int i = 0; i < hits.Length; i++)
		{
			if (hits[i].point == Vector3.zero)
			{
				RaycastHit fallbackHit;

				if (Physics.Raycast(bottomOrigin, rayDir, out fallbackHit, rayLength, m_groundMask))
				{
					//m_hitAnythingLastMovement = true;
					//m_averageNormal = hits[i].normal;

					distance = fallbackHit.distance;
				}
			}
			else
			{
				distance = hits[i].distance;

				//m_averageNormal = hits[i].normal;
				//m_hitAnythingLastMovement = true;
			}
		}

		m_characterController.Move(new Vector3(0, -(distance), 0));
		*/
		#endregion

		RaycastHit hit;

		float rayLength = p_moveAmount.magnitude;
		rayLength = 2f;

		Vector3 bottom = m_characterController.transform.position - new Vector3(0, m_characterController.height / 2, 0);

		if (Physics.Raycast(bottom, Vector3.down, out hit, rayLength))
		{
			m_characterController.Move(new Vector3(0, -(hit.distance), 0));
		}
		else
		{
			m_blockGroundSnapping = true;
		}
	}

	private void DecendSlopeBelow(ref Vector3 p_moveAmount, Vector3 p_horizontalVelocity)
	{
		Vector3 localXAxis = Vector3.Cross(p_horizontalVelocity, Vector3.up);
		Vector3 forwardRotation = Vector3.ProjectOnPlane(m_averageNormal, localXAxis);
		Quaternion upwardSlopeOffset = Quaternion.FromToRotation(Vector3.up, forwardRotation);
		Vector3 targetMoveAmount = (upwardSlopeOffset * p_horizontalVelocity);

		Vector3 normalCross = Vector3.Cross(Vector3.up, m_averageNormal);
		Vector3 movementSlopeCross = Vector3.Cross(normalCross, p_horizontalVelocity).normalized;

		if (Mathf.Sign(movementSlopeCross.y) < 0)
		{

			if (p_moveAmount.y < 0)
			{

			}

			DebugExtension.DebugArrow(transform.position, targetMoveAmount, Color.green, 0f, false);
			//p_moveAmount = targetMoveAmount;
			//p_moveAmount = new Vector3(targetMoveAmount.x, p_moveAmount.y, targetMoveAmount.z);

			p_moveAmount.y += targetMoveAmount.y;
		}
	}

	private void ClimbSlopeBelow(ref Vector3 p_moveAmount, Vector3 p_horizontalVelocity)
	{
		Vector3 localXAxis = Vector3.Cross(p_horizontalVelocity, Vector3.up);
		Vector3 forwardRotation = Vector3.ProjectOnPlane(m_averageNormal, localXAxis);
		Quaternion upwardSlopeOffset = Quaternion.FromToRotation(Vector3.up, forwardRotation);
		Vector3 targetMoveAmount = (upwardSlopeOffset * p_horizontalVelocity);

		Vector3 normalCross = Vector3.Cross(Vector3.up, m_averageNormal);
		Vector3 movementSlopeCross = Vector3.Cross(normalCross, p_horizontalVelocity).normalized;

		if (Mathf.Sign(movementSlopeCross.y) > 0)
		{
			if (p_moveAmount.y <= targetMoveAmount.y)
			{
				p_moveAmount = new Vector3(targetMoveAmount.x, p_moveAmount.y, targetMoveAmount.z);
			}
		}
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (CheckCollisionLayer(m_groundMask, hit.collider.gameObject))
		{
			if (CheckCollisionLayer(m_slideMask, hit.collider.gameObject))
			{
				m_onSlideSurface = true;
			}
			else
			{
				m_onSlideSurface = false;
			}

			m_averageNormal = hit.normal;
		}
	}

	public bool CheckCollisionLayer(LayerMask p_layerMask, GameObject p_object)
	{
		if (p_layerMask == (p_layerMask | (1 << p_object.layer)))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	#endregion

	#region Public Bools
	public bool IsGrounded()
	{
		return m_isGrounded;
	}
	#endregion

	/*
	#region old


	private void SetHorizontalVelocity(ref Vector3 p_moveAmount, Vector3 p_newMovement)
	{
		p_moveAmount = new Vector3(p_newMovement.x, p_moveAmount.y, p_newMovement.z);
	}

	private Vector3 GetHorizontalMovement(Vector3 p_moveAmount)
	{
		return new Vector3(p_moveAmount.x, 0, p_moveAmount.z);
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

		//m_characterController.Move(p_moveAmount);

		if (m_logVelocityMag)
		{
			//Debug.Log(m_characterController.velocity.magnitude + " velocity mag");
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
					//targetMoveAmount.y += m_characterController.skinWidth;
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
					//targetMoveAmount.y -= m_characterController.skinWidth;
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
			//float castRadius = m_characterController.radius - m_castSkinWidth;
			float castRadius = 1;
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
					//targetMoveAmount.y -= m_characterController.skinWidth;
					p_moveAmount = targetMoveAmount;
					p_moveAmount.y += targetMoveAmount.y;
				}
			}
		}
	}
	#endregion

	#region Old Crap
	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		m_hitAnythingLastMovement = true;
		m_averageNormal += hit.normal;
		m_amountToAverage++;
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
	#endregion
	*/
}
