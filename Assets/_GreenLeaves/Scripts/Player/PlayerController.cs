using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RootMotion.FinalIK;
using System;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[Serializable]
public class PlayerControllerEvent : UnityEvent { }

public class PlayerController : MonoBehaviour
{
	public Transform m_viewCameraTransform;
	private Vector3 m_horizontalDirection;
	private Vector3 m_horizontalVelocity;
	private Vector2 m_movementInput;
	private Vector3 m_horizontalInput;

	private PlayerVisualsController m_playerVisuals;

	#region General Collision
	[FoldoutGroup("General Collision")]
	public LayerMask m_groundMask;

	private CharacterController m_characterController;
	private Vector3 m_averageNormal;
	private Vector3 m_playerTop, m_playerBottom;
	private bool m_blockGroundSnapping;
	#endregion

	#region Ground Movement Properties
	[FoldoutGroup("Ground Movement")]
	public float m_playerTurnSpeed = 0.1f;
	[FoldoutGroup("Ground Movement")]
	public float m_runSpeed;
	[FoldoutGroup("Ground Movement")]
	public float m_sprintSpeed;
	[FoldoutGroup("Ground Movement")]
	public float m_accelerationTime;

	private bool m_sprinting;
	private Vector3 m_groundMovementVelocity;
	private Vector3 m_groundMovementVelocitySmoothing;
	private float m_playerTurnSmoothingVelocity;
	#endregion

	#region Jumping Properties
	[FoldoutGroup("Jumping")]
	public Vector2 m_minMaxJumpHeight;
	[FoldoutGroup("Jumping")]
	public float m_timeToJumpApex;

	private float m_gravity;
	private float m_maxJumpVelocity;
	private float m_minJumpVelocity;
	private bool m_forceGravity;
	private Vector3 m_gravityVelocity;
	private bool m_hasJumped;
	#endregion

	#region Slide Properties
	[FoldoutGroup("Sliding")]
	public LayerMask m_slopeMask;
	[FoldoutGroup("Sliding")]
	public Transform m_slopeTransform;
	[FoldoutGroup("Sliding")]
	public Vector2 m_minMaxSlideSpeed;
	[FoldoutGroup("Sliding")]
	public float m_slideStartAngle;
	[FoldoutGroup("Sliding")]
	public float m_slideEndAngle;
	[FoldoutGroup("Sliding")]
	public AnimationCurve m_slopeSlowCurve;
	[FoldoutGroup("Sliding")]
	public float m_minimumSlopeSpeedPercent;

	private Vector3 m_slopeVelocity;
	private float m_slopeFacingDirection;
	private bool m_onSlideSurface;
	private float m_currentSlopeAngle;
	private float m_currentSlownessFactor;
	private bool m_sliding;
	#endregion

	#region Misc Slide
	private float m_preSlideTime;
	private AnimationCurve m_preSlideCurve;
	private float m_slopeSpeedSlowStartAngle;
	private float m_slideRecoveryTime;
	private AnimationCurve m_endSlideCruve;
	private bool m_runningPreSlide;
	#endregion

	#region Climb Properties
	[FoldoutGroup("Climbing")]
	public LayerMask m_climbMask;
	[FoldoutGroup("Climbing")]
	public Transform m_climbTransform;
	[FoldoutGroup("Climbing")]
	public float m_climbSpeed;
	[FoldoutGroup("Climbing")]
	public float m_climbAcceleration;
	[FoldoutGroup("Climbing")]
	public float m_climbStartDistance;
	[FoldoutGroup("Climbing")]
	public float m_wallAlignmentToStartClimb;
	[FoldoutGroup("Climbing")]
	public float m_clamberSpeed;
	[FoldoutGroup("Climbing")]
	public AnimationCurve m_clamberCurve;
	[FoldoutGroup("Climbing")]
	public float m_clamberEndTime;
	[FoldoutGroup("Climbing")]
	public float m_clamberEndSpeed;
	[FoldoutGroup("Climbing")]
	public AnimationCurve m_clamberEndCurve;
	[FoldoutGroup("Climbing")]
	public float m_groundCancelTime;
	[FoldoutGroup("Climbing")]
	public float m_armWidth;
	[FoldoutGroup("Climbing")]
	public float m_handHeight;

	private bool m_clamber;
	private bool m_groundCancel;
	private bool m_climbing;
	private bool m_onClimbSurface;
	private Vector3 m_climbVelocity;
	private Vector3 m_climbMovementSmoothingVelocity;
	private Vector3 m_wallStickVelocity;
	private Vector3 m_horizontalWallDirection;
	#endregion

	private void Start()
	{
		m_characterController = GetComponent<CharacterController>();
		m_playerVisuals = GetComponent<PlayerVisualsController>();
	}

	private void OnValidate()
	{
		CalculateJump();
	}

	private void Update()
	{
		m_playerVisuals.SetGroundMovementAnimations(m_groundMovementVelocity, m_sprintSpeed, m_runSpeed, m_runSpeed * 0.5f);
		m_playerVisuals.SetJumpAnimations(Mathf.InverseLerp(-m_maxJumpVelocity, m_maxJumpVelocity, m_gravityVelocity.y), m_hasJumped);
		m_playerVisuals.SetGrounded(m_characterController.isGrounded);

		m_playerVisuals.SetSlideAnimations(m_sliding);

		m_playerVisuals.ToggleClimbAnimation(m_climbing, m_clamber);

		if (!m_climbing && m_characterController.isGrounded)
		{
			m_playerVisuals.SetGroundHeadRotation(m_averageNormal);
		}
	}

	private void FixedUpdate()
	{
		UpdatePlayerCastPoints();

		CalculateSlopeVariables();

		GroundMovement();

		RunGravity();

		ClimbLoop();

		CheckSlide();

		CaculateTotalVelocity();
		DecendSlopeBelow(m_characterController.velocity * Time.fixedDeltaTime);

		if (!CheckGravityConditions())
		{
			m_gravityVelocity.y = 0;
			return;
		}
	}

	private void UpdatePlayerCastPoints()
	{
		m_playerBottom = transform.position + (Vector3.down * (m_characterController.height / 2));
		m_playerTop = transform.position + (Vector3.up * (m_characterController.height / 2));
	}

	public void SetMovementInput(Vector2 p_input)
	{
		m_movementInput = p_input;
	}


	private void CaculateTotalVelocity()
	{
		Vector3 velocity = Vector3.zero;

		velocity += m_groundMovementVelocity;
		velocity *= m_currentSlownessFactor;

		velocity += m_slopeVelocity;

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
	}

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
		if (m_sliding)
		{
			return false;
		}

		if (m_climbing)
		{
			return false;
		}

		return true;
	}

	public bool CheckSprintConditions()
	{
		if (m_characterController.isGrounded && m_sprinting)
		{
			return true;
		}

		return false;
	}

	private void GroundMovement()
	{
		if (!CheckGroundMovement())
		{
			m_groundMovementVelocity = Vector3.zero;
			m_groundMovementVelocitySmoothing = Vector3.zero;
			return;
		}

		Vector3 newHorizontalInput = Vector3.ClampMagnitude(new Vector3(m_movementInput.x, 0, m_movementInput.y), 1f);

		m_playerVisuals.EvaluateSkidCondition(m_sprinting, newHorizontalInput, m_horizontalInput);

		m_horizontalInput = newHorizontalInput;


		float targetAngle = transform.eulerAngles.y;

		if (m_horizontalInput.magnitude > 0)
		{
			targetAngle = Mathf.Atan2(m_horizontalInput.x, m_horizontalInput.z) * Mathf.Rad2Deg + m_viewCameraTransform.eulerAngles.y;
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref m_playerTurnSmoothingVelocity, m_playerTurnSpeed);
			transform.rotation = Quaternion.Euler(0, angle, 0);
		}

		float horizontalSpeed;
		float currentAcceleration = m_accelerationTime;

		if (CheckSprintConditions())
		{
			horizontalSpeed = m_sprintSpeed;
		}
		else
		{
			horizontalSpeed = m_runSpeed;
		}

		Vector3 actualMovementDir = Quaternion.Euler(0, targetAngle, 0f) * Vector3.forward;
		Vector3 targetHorizontalMovement = (actualMovementDir * horizontalSpeed) * m_horizontalInput.magnitude;
		Vector3 horizontalMovement = Vector3.SmoothDamp(m_groundMovementVelocity, targetHorizontalMovement, ref m_groundMovementVelocitySmoothing, currentAcceleration);

		m_groundMovementVelocity = new Vector3(horizontalMovement.x, 0, horizontalMovement.z);

		//m_playerVisuals.SetTurnBlendValue(targetAngle);
	}
	#endregion

	#region Jump and Gravity
	private void CalculateJump()
	{
		m_gravity = -(2 * m_minMaxJumpHeight.y) / Mathf.Pow(m_timeToJumpApex, 2);
		m_maxJumpVelocity = Mathf.Abs(m_gravity) * m_timeToJumpApex;
		m_minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(m_gravity) * m_minMaxJumpHeight.x);
	}

	private bool CheckGravityConditions()
	{
		if (m_forceGravity)
		{
			return true;
		}

		if (m_climbing)
		{
			return false;
		}

		if (m_gravityVelocity.y > 0)
		{
			return true;
		}

		if (m_hasJumped)
		{
			return true;
		}

		if (m_characterController.isGrounded)
		{
			return false;
		}
		return true;
	}

	private void RunGravity()
	{
		if (CheckGravityConditions())
		{
			m_gravityVelocity.y += m_gravity * Time.fixedDeltaTime;
		}
	}

	public void OnJumpInputDown()
	{
		if (m_characterController.isGrounded)
		{
			JumpMaxVelocity();
		}
	}

	public void OnJumpInputUp()
	{
		if (m_gravityVelocity.y > m_minJumpVelocity)
		{
			JumpMinVelocity();
		}
	}

	private void JumpMaxVelocity()
	{
		m_hasJumped = true;
		m_gravityVelocity.y = m_maxJumpVelocity;
	}

	private void JumpMinVelocity()
	{
		m_gravityVelocity.y = m_minJumpVelocity;
	}
	#endregion

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

		if (m_onClimbSurface && wallFacingDot >= m_wallAlignmentToStartClimb && m_movementInput.y > 0 && !m_climbing && m_characterController.isGrounded)
		{
			return true;
		}

		return false;
	}

	private IEnumerator RunClimb()
	{
		m_climbing = true;
		float targetClamberHeight = 0;
		Vector3 ledgeTopPos = Vector3.zero;

		m_playerVisuals.ToggleGrounder(false);

		while (m_onClimbSurface && m_climbing && !m_clamber && !m_groundCancel)
		{
			ClimbAnimations();
			ClimbMovement();

			Vector3 localClimbVelocity = transform.InverseTransformDirection(m_climbVelocity);
			Vector2 normalClimbVel = new Vector2(Mathf.InverseLerp(-m_climbSpeed, m_climbSpeed, localClimbVelocity.x), Mathf.InverseLerp(-m_climbSpeed, m_climbSpeed, localClimbVelocity.y));
			m_playerVisuals.ClimbHeadAnimations(normalClimbVel);

			if (CheckGroundClimbCancel())
			{
				m_groundCancel = true;
			}

			if (CheckForClamber(ref ledgeTopPos, ref targetClamberHeight))
			{
				m_clamber = true;
			}

			yield return new WaitForFixedUpdate();
		}

		m_climbMovementSmoothingVelocity = Vector3.zero;
		m_climbVelocity = Vector3.zero;
		m_wallStickVelocity = Vector3.zero;

		if (m_clamber)
		{
			StartCoroutine(ClamberLoop(ledgeTopPos, targetClamberHeight));
		}
		else if (m_groundCancel)
		{
			StartCoroutine(GroundClimbCancel());
		}
		else
		{
			ResetClimb();
		}
	}

	private IEnumerator GroundClimbCancel()
	{
		m_playerVisuals.ToggleGrounder(true);

		float t = 0;

		while (t < m_groundCancelTime)
		{
			t += Time.fixedDeltaTime;

			ClimbAnimations();

			float progress = t / m_groundCancelTime;

			yield return new WaitForFixedUpdate();
		}

		ResetClimb();
	}

	private bool CheckGroundClimbCancel()
	{
		if (m_movementInput.y < 0 && m_characterController.isGrounded)
		{
			return true;
		}

		return false;
	}

	private IEnumerator ClamberLoop(Vector3 p_ledgePosition, float p_targetClamberHeight)
	{
		float startHeight = m_playerBottom.y;
		float lastSpeed = 0;
		ResetClimbVelocitys();

		m_playerVisuals.ToggleArmIK(true);
		m_playerVisuals.ToggleGrounder(true);
		m_playerVisuals.CenterHead();

		if (p_ledgePosition == Vector3.zero)
		{
			p_ledgePosition = transform.forward * 0.5f;
		}

		while (m_clamber)
		{
			Vector3 handMidPos = p_ledgePosition + (Vector3.up * m_handHeight);
			m_playerVisuals.SetArmTargetPosition(handMidPos + (m_climbTransform.right * m_armWidth), handMidPos - (m_climbTransform.right * m_armWidth));

			float heightProgress = Mathf.InverseLerp(startHeight, p_targetClamberHeight, m_playerBottom.y);

			float currentClamberSpeed = Mathf.Lerp(m_climbSpeed, m_clamberSpeed, m_clamberCurve.Evaluate(heightProgress));
			m_climbVelocity = Vector3.up * currentClamberSpeed;
			m_wallStickVelocity = -m_climbTransform.forward * m_climbSpeed;

			lastSpeed = currentClamberSpeed;

			if (CheckForClamberEnd())
			{
				m_clamber = false;
			}

			//Might want to make this a public variable but its working fine for now and might just be clutter if public
			if (heightProgress >= 0.4f)
			{
				m_playerVisuals.ToggleArmIK(false);
			}

			m_playerVisuals.SetClamberProgress(heightProgress);

			yield return new WaitForFixedUpdate();
		}

		ResetClimbVelocitys();

		m_playerVisuals.ToggleGrounder(true);
		m_playerVisuals.ToggleArmIK(false);

		m_forceGravity = true;
		float clamberForwardTimer = 0;

		while (clamberForwardTimer < m_clamberEndTime)
		{
			clamberForwardTimer += Time.fixedDeltaTime;
			float progress = m_clamberEndCurve.Evaluate(clamberForwardTimer / m_clamberEndTime);

			float currentClamberEndSpeed = Mathf.Lerp(lastSpeed, m_clamberEndSpeed, progress);
			m_climbVelocity = transform.forward * currentClamberEndSpeed;

			yield return new WaitForFixedUpdate();
		}

		m_forceGravity = false;
		ResetClimb();
	}

	private bool CheckForClamber(ref Vector3 p_ledgePosition, ref float p_targetClamberHeight)
	{
		RaycastHit clamberHit;

		if (Physics.Raycast(m_playerTop, transform.forward, out clamberHit, m_climbStartDistance, m_climbMask))
		{
			if (Vector3.Angle(clamberHit.normal, Vector3.up) < 90)
			{
				p_targetClamberHeight = m_playerTop.y;
				p_ledgePosition = clamberHit.point;
				return true;
			}

			p_ledgePosition = clamberHit.point;
		}
		else
		{
			p_targetClamberHeight = m_playerTop.y;

			return true;
		}

		return false;
	}

	private bool CheckForClamberEnd()
	{
		RaycastHit clamberHit;

		if (Physics.Raycast(m_playerBottom, transform.forward, out clamberHit, m_climbStartDistance, m_climbMask))
		{
			if (Vector3.Angle(clamberHit.normal, Vector3.up) < 90)
			{
				Debug.DrawRay(m_playerBottom, transform.forward);
				return true;
			}
		}
		else
		{
			Debug.DrawRay(m_playerBottom, transform.forward);
			return true;
		}

		return false;
	}

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

	private void ResetClimb()
	{
		m_climbing = false;
		m_groundCancel = false;
		m_clamber = false;

		m_playerVisuals.ToggleGrounder(true);
		m_playerVisuals.CenterHead();

		ResetClimbVelocitys();
	}

	private void ResetClimbVelocitys()
	{
		m_climbMovementSmoothingVelocity = Vector3.zero;
		m_climbVelocity = Vector3.zero;
		m_wallStickVelocity = Vector3.zero;
	}

	private void ClimbMovement()
	{
		Vector3 verticalClimbMovment = m_climbTransform.up * m_movementInput.y;
		Vector3 horizontalClimbMovment = m_climbTransform.right * -m_movementInput.x;
		Vector3 targetClimbMovement = Vector3.ClampMagnitude(verticalClimbMovment + horizontalClimbMovment, 1) * m_climbSpeed;
		Vector3 climbMovement = Vector3.SmoothDamp(m_climbVelocity, targetClimbMovement, ref m_climbMovementSmoothingVelocity, m_climbAcceleration);
		m_climbVelocity = climbMovement;

		m_wallStickVelocity = -m_climbTransform.forward * m_climbSpeed;

		transform.rotation = Quaternion.LookRotation(m_horizontalWallDirection);
	}

	private void ClimbAnimations()
	{
		Vector3 localClimbVelocity = transform.InverseTransformDirection(m_climbVelocity);
		Vector2 normalClimbVel = new Vector2(Mathf.InverseLerp(-m_climbSpeed, m_climbSpeed, localClimbVelocity.x), Mathf.InverseLerp(-m_climbSpeed, m_climbSpeed, localClimbVelocity.y));
		Vector2 animValue = new Vector2(Mathf.Lerp(-1, 1, normalClimbVel.x), Mathf.Lerp(-1, 1, normalClimbVel.y));
		m_playerVisuals.ClimbAnimations(animValue.x, animValue.y);
	}
	#endregion

	#region Slide
	private void CalculateSlopeVariables()
	{
		m_currentSlopeAngle = Vector3.Angle(m_averageNormal, Vector3.up);
		m_slopeTransform.rotation = Quaternion.LookRotation(m_averageNormal);
		Vector3 normalCross = Vector3.Cross(Vector3.up, m_averageNormal);
		Vector3 movementSlopeCross = Vector3.Cross(normalCross, m_horizontalDirection).normalized;
		m_slopeFacingDirection = Mathf.Sign(movementSlopeCross.y);
	}

	private void CheckSlide()
	{
		if (m_slopeFacingDirection < 0)
		{
			StartSlideLoop(m_slopeFacingDirection);
			return;
		}

		m_currentSlownessFactor = 1;

		if (m_slopeFacingDirection > 0 && m_currentSlopeAngle >= m_slideEndAngle && !m_sliding)
		{
			float slopeInverse = m_slopeSlowCurve.Evaluate(Mathf.InverseLerp(m_slideEndAngle, m_slideStartAngle, m_currentSlopeAngle));
			float currentSlownessFactor = Mathf.Lerp(1, m_minimumSlopeSpeedPercent, slopeInverse);

			m_currentSlownessFactor = currentSlownessFactor;
		}
		else
		{
			m_currentSlownessFactor = 1;
		}

		if (m_currentSlopeAngle >= m_slideStartAngle)
		{
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
		if (!m_sliding && CheckSlideConditions())
		{
			StartCoroutine(RunSlide(p_facingDir));
		}
	}

	private IEnumerator RunSlide(float p_facingDir)
	{
		m_sliding = true;

		while (CheckSlideConditions())
		{
			float currentSlopePercent = Mathf.InverseLerp(m_slideEndAngle, 90f, m_currentSlopeAngle);
			float currentSlopeSpeed = Mathf.Lerp(m_minMaxSlideSpeed.x, m_minMaxSlideSpeed.y, currentSlopePercent);
			Vector3 targetSlopeVelocity = -m_slopeTransform.up * currentSlopeSpeed;
			m_slopeVelocity = targetSlopeVelocity;

			SlideRotation(p_facingDir);

			m_playerVisuals.SetSlideOffsetPose(currentSlopePercent);

			yield return new WaitForFixedUpdate();
		}

		m_slopeVelocity = Vector3.zero;
		m_sliding = false;
	}

	private void SlideRotation(float p_facingDir)
	{
		Vector3 horizontalSlopeDirection = Mathf.Sign(-p_facingDir) * new Vector3(m_slopeTransform.forward.x, 0, m_slopeTransform.forward.z);
		transform.rotation = Quaternion.LookRotation(horizontalSlopeDirection);
	}

	#region No Usey
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

	private IEnumerator SlideEndPush(float p_facingDir)
	{
		float t = 0;

		while (t < 0.1f)
		{
			t += Time.fixedDeltaTime;

			float progress = m_endSlideCruve.Evaluate(t / 0.1f);

			float currentSpeed = Mathf.Lerp(m_minMaxSlideSpeed.x, 0, progress);

			Vector3 targetSlopeVelocity = -m_slopeTransform.up * currentSpeed;
			m_slopeVelocity = targetSlopeVelocity;

			SlideRotation(p_facingDir);

			yield return new WaitForFixedUpdate();
		}

		m_slopeVelocity = Vector3.zero;

		t = 0;

		while (t < m_slideRecoveryTime)
		{
			t += Time.fixedDeltaTime;

			yield return new WaitForFixedUpdate();
		}

		m_sliding = false;
	}
	#endregion

	#endregion

	#region Collision
	private bool ResetGroundSnap()
	{
		if (m_characterController.isGrounded && !m_climbing)
		{
			return true;
		}

		return false;
	}

	private bool CanGroundSnap()
	{
		if (m_blockGroundSnapping)
		{
			return false;
		}

		if (m_climbing)
		{
			return false;
		}

		if (m_hasJumped)
		{
			return false;
		}

		if (m_gravityVelocity.y > 0)
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
			m_hasJumped = false;
		}

		if (!CanGroundSnap())
		{
			return;
		}

		RaycastHit hit;

		float rayLength = 2f; //may want to make this not hard set but its working for now

		if (Physics.Raycast(m_playerBottom, Vector3.down, out hit, rayLength))
		{
			m_characterController.Move(new Vector3(0, -(hit.distance), 0));
		}
		else
		{
			m_blockGroundSnapping = true;
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
			if (CheckCollisionLayer(m_slopeMask, hit.collider.gameObject))
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
		return m_characterController.isGrounded;
	}
	#endregion
}