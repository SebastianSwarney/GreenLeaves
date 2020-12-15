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
    #region Player States
    public enum MovementControllState { MovementEnabled, MovementDisabled }
    public enum GravityState { GravityEnabled, GravityDisabled }
    public enum ControllerState { ControllerEnabled, ControllerDisabled }

    [System.Serializable]
    public struct PlayerState
    {
        public MovementControllState m_movementControllState;
        public GravityState m_gravityControllState;
        public ControllerState m_controllerState;
    }

    public PlayerState m_states;
    #endregion

    #region Player Events
    [System.Serializable]
    public struct PlayerControllerEvents
    {
        [Header("Basic Events")]
        public PlayerControllerEvent m_onLandedEvent;
        public PlayerControllerEvent m_onJumpEvent;
    }

    public PlayerControllerEvents m_events;
    #endregion

    #region Camera Properties
    [System.Serializable]
    public struct CameraProperties
    {
        public Transform m_viewCameraTransform;
    }

    [Header("Camera Properties")]
    public CameraProperties m_cameraProperties;
    #endregion

    #region Base Movement Properties
    [System.Serializable]
    public struct BaseMovementProperties
    {
        public float m_runSpeed;
        public float m_walkSpeed;
        public float m_sprintSpeed;
        public float m_runEnergyDepletionTime;
        public float m_playerTurnSpeed;
        public float m_accelerationTimeGrounded;
        public float m_accelerationTimeAir;
    }

    private BaseMovementProperties m_baseMovementProperties;

    [Header("Base Movement Properties")]
    [InlineEditor(InlineEditorModes.GUIAndPreview)]
    public PlayerBaseMovementSettings m_currentBaseMovementSettings;

    private Vector3 m_groundMovementVelocity;
    private Vector3 m_groundMovementVelocitySmoothing;
    private CharacterController m_characterController;
    private Coroutine m_jumpBufferCoroutine;
    private Coroutine m_graceBufferCoroutine;
    private float m_playerTurnSmoothingVelocity;
    private bool m_isSprinting;
    private bool m_isWalking;
    #endregion

    #region Jumping Properties
    [System.Serializable]
    public struct JumpingProperties
    {
        [Header("Jump Properties")]
        public float m_maxJumpHeight;
        public float m_minJumpHeight;
        public float m_timeToJumpApex;

        [Header("Jump Buffer Properties")]
        public float m_graceTime;
        public float m_jumpBufferTime;
    }

    private JumpingProperties m_jumpingProperties;

    [Header("Jumping Properties")]
    [InlineEditor(InlineEditorModes.GUIAndPreview)]
    public PlayerJumpingSettings m_currentJumpingSettings;

    private bool m_hasJumped;

    private float m_graceTimer;
    private float m_jumpBufferTimer;

    private float m_gravity;
    private float m_maxJumpVelocity;
    private float m_minJumpVelocity;
    private bool m_isLanded;
    private bool m_offLedge;

    private Vector3 m_gravityVelocity;
    #endregion

    #region Slide Properties
    [System.Serializable]
    public struct SlideProperties
    {
        [Header("Slope Slide Properties")]
        public float m_slideAngleBoostMax;
        public float m_slopeSlideAccelerationTime;
        public float m_slopeTolerence;

        [Header("Slope Side Shift Properties")]
        public float m_slideSideShiftMaxSpeed;
        public float m_slideSideShiftAcceleration;

        public float m_slideStartAngle;
        public float m_maximumSlopeAngle;

        public float m_additionalSlopeDecendSpeed;
        public float m_additionalSlopeAccendSpeed;

        public float m_speedToStartSlide;
    }

    private SlideProperties m_slideProperties;

    [Header("Slide Properties")]
    [InlineEditor(InlineEditorModes.GUIAndPreview)]
    public PlayerSlidingSettings m_currentSlidingSettings;

    public Transform m_slopeTransform;

    private float m_slopeAngle;
    private bool m_isSliding;
    private float m_slideTimer;
    private Vector3 m_slideVelocity;
    private Vector3 m_slopeVelocity;
    private Vector3 m_slopeShiftVelocity;
    private float m_currentHorizontalMovementSpeed;
    private float m_currentHorizontalAccelerationSpeed;
    #endregion

    #region Animation Properties
    private Vector2 m_animInput;
    private Vector2 m_animInputSmoothing;
    private Vector2 m_animInputTarget;
    private Animator m_playerAnimator;
    #endregion

    #region Debug Properties
    private float m_flyInput;
	#endregion

	#region Misc
	[FoldoutGroup("Misc")]
    public LayerMask m_groundMask;
    private Vector2 m_movementInput;

    private Rigidbody m_rigidbody;
    private CapsuleCollider m_capsuleCollider;

    [FoldoutGroup("Misc")]
    private bool m_controllerEnabled;

    [FoldoutGroup("Misc")]
    public CollisionInfo m_collisions;

    #region Climb Properties
    [FoldoutGroup("Misc")]
    public float m_climbSpeed;
    [FoldoutGroup("Misc")]
    public float m_maxClimbAcceleration;

	private Vector3 m_climbVelocity;
    private bool m_isClimbing;
    #endregion

    [FoldoutGroup("Misc")]
    public Transform m_wallTransform;
    
    [FoldoutGroup("Misc")]
    public Transform m_modelTransform;

    [FoldoutGroup("Misc")]
    public Transform m_headEffectorRootTransform;

    [FoldoutGroup("Misc")]
    public AnimationCurve m_turnOffsetCurve;
    
    [FoldoutGroup("Misc")]
    public OffsetPoseBlend m_turnBlend;

    [FoldoutGroup("Misc")]
    public OffsetPoseBlend m_verticalSlopeBlend;
    [FoldoutGroup("Misc")]
    public OffsetPoseBlend m_horizontalSlopeBlend;
    [FoldoutGroup("Misc")]
    public float m_maximumBlendTurnAngle;
    [FoldoutGroup("Misc")]
    public float m_maxVerticalSlopePercent;
    [FoldoutGroup("Misc")]
    public float m_landedSlideSpeed;
    [FoldoutGroup("Misc")]
    public float m_landedSlideTime;
    [FoldoutGroup("Misc")]
    public AnimationCurve m_landedSlideCurve;

    private PlayerStatsController m_playerStats;
    [FoldoutGroup("Misc")]
    public bool m_aimMovement;

    private float m_fallDistance;

    private float m_fallPositionY;
    private bool m_startedFallRecording;
    private bool m_isFalling;
    #endregion

    public LayerMask m_waterMask;

	private void Start()
    {
        m_characterController = GetComponent<CharacterController>();
        m_playerAnimator = GetComponentInChildren<Animator>();

        m_baseMovementProperties = m_currentBaseMovementSettings.m_baseMovementSettings;
        m_jumpingProperties = m_currentJumpingSettings.m_jumpingSettings;
        m_slideProperties = m_currentSlidingSettings.m_slidingSettings;

        m_jumpBufferTimer = m_jumpingProperties.m_jumpBufferTime;

        m_capsuleCollider = GetComponent<CapsuleCollider>();
        m_capsuleCollider.height = m_characterController.height;
        m_capsuleCollider.radius = m_characterController.radius;

        m_rigidbody = GetComponent<Rigidbody>();

        CalculateJump();

        m_controllerEnabled = true;

        OnControllerEnabled();

        m_playerStats = GetComponent<PlayerStatsController>();
    }

    private void OnValidate()
    {
        CalculateJump();
    }

	private void Update()
	{
        SetAnimations();

		if (Input.GetKeyDown(KeyCode.J))
		{
            m_controllerEnabled = !m_controllerEnabled;

			if (m_controllerEnabled)
			{
                OnControllerEnabled();
			}
			else
			{
                OnControllerDisabled();
                //m_rigidbody.AddForce(transform.forward * 50f, ForceMode.Impulse);
            }
		}
	}

	private void FixedUpdate()
	{
		if (m_controllerEnabled)
		{
            PerformController();
		}
		else
		{
            RunClimbLoop();
        }
    }

	public void PerformController()
    {
		CalculateVelocity();

        SetSlideSlopeVariables();
        OnSlideStart();

        HitWater();

        CaculateTotalVelocity();
        SlopePhysics();
        CheckFall();
        CheckLanded();
        ZeroVelocityOnGround();
    }

	#region Animation Code

	private void SetAnimations()
	{

        m_playerAnimator.SetBool("IsGrounded", IsGrounded());

        m_playerAnimator.SetBool("IsWalking", m_isWalking);
        m_playerAnimator.SetBool("IsSprinting", m_isSprinting);

        Vector3 relativeVelocity = transform.InverseTransformDirection(m_groundMovementVelocity);

        m_animInputTarget = new Vector2(relativeVelocity.x / m_baseMovementProperties.m_sprintSpeed, relativeVelocity.z / m_baseMovementProperties.m_sprintSpeed);

		if (m_isWalking)
		{
            m_animInputTarget = new Vector2(relativeVelocity.x, relativeVelocity.z) / m_baseMovementProperties.m_walkSpeed;
        }
		else if (m_isSprinting)
		{
            m_animInputTarget = new Vector2(relativeVelocity.x, relativeVelocity.z) / m_baseMovementProperties.m_sprintSpeed;
        }
		else
		{
            m_animInputTarget = new Vector2(relativeVelocity.x, relativeVelocity.z) / m_baseMovementProperties.m_runSpeed;
        }

        m_animInput = Vector2.SmoothDamp(m_animInput, m_animInputTarget, ref m_animInputSmoothing, 0.1f);
        m_playerAnimator.SetFloat("ForwardMovement", m_animInput.y);
        m_playerAnimator.SetFloat("SideMovement", m_animInput.x);

        SetModelRotation();
    }

    private void SetModelRotation()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, m_groundMask))
        {
            Vector3 localXAxis = Vector3.Cross(transform.forward, Vector3.up);
            Vector3 forwardRotation = Vector3.ProjectOnPlane(hit.normal, localXAxis);
            Quaternion upwardSlopeOffset = Quaternion.FromToRotation(Vector3.up, forwardRotation);
            Vector3 targetMoveAmount = (upwardSlopeOffset * transform.forward);

            m_wallTransform.rotation = Quaternion.LookRotation(hit.normal);

            m_headEffectorRootTransform.rotation = Quaternion.RotateTowards(m_headEffectorRootTransform.rotation, Quaternion.LookRotation(targetMoveAmount, Vector3.up), 150f * Time.deltaTime);


            float veritcalLerp = Mathf.InverseLerp(0, 1, Mathf.Abs(targetMoveAmount.y) / m_maxVerticalSlopePercent);
            m_verticalSlopeBlend.SetBlendValue(veritcalLerp * Mathf.Sign(targetMoveAmount.y));

            Vector3 horizontalCross = Vector3.Cross(m_wallTransform.right, transform.right);
            //m_horizontalSlopeBlend.SetBlendValue(Mathf.Abs(horizontalCross.y) * Mathf.Sign(-horizontalCross.y));
        }
    }

    #endregion

    #region Controller State Code

    private void OnControllerDisabled()
	{
        m_characterController.enabled = false;
        m_states.m_movementControllState = MovementControllState.MovementDisabled;
        m_states.m_gravityControllState = GravityState.GravityDisabled;

        m_rigidbody.isKinematic = false;
	}

    private void OnControllerEnabled()
	{
        m_characterController.enabled = true;
        m_states.m_movementControllState = MovementControllState.MovementEnabled;
        m_states.m_gravityControllState = GravityState.GravityEnabled;

        m_rigidbody.isKinematic = true;
    }

	#endregion

	#region Input Code
	public void SetMovementInput(Vector2 p_input)
    {
        m_movementInput = p_input;
    }

    public void OnSprintButtonDown()
    {
        m_isSprinting = !m_isSprinting;

		if (m_isSprinting)
		{
            m_isWalking = false;
		}
    }

    public void OnSprintButtonUp()
    {
        m_isSprinting = false;
    }

    public void OnWalkButtonDown()
    {
        m_isWalking = !m_isWalking;

        if (m_isWalking)
        {
            m_isSprinting = false;
        }
    }

    public void SetFlyInput(float p_input)
    {
        m_flyInput = p_input;
    }

    #endregion

    #region Input Buffering Code

    private bool CheckBuffer(ref float p_bufferTimer, ref float p_bufferTime, Coroutine p_bufferTimerRoutine)
    {
        if (p_bufferTimer < p_bufferTime)
        {
            if (p_bufferTimerRoutine != null)
            {
                StopCoroutine(p_bufferTimerRoutine);
            }

            p_bufferTimer = p_bufferTime;

            return true;
        }
        else if (p_bufferTimer >= p_bufferTime)
        {
            return false;
        }

        return false;
    }

    private bool CheckOverBuffer(ref float p_bufferTimer, ref float p_bufferTime, Coroutine p_bufferTimerRoutine)
    {
        if (p_bufferTimer >= p_bufferTime)
        {
            p_bufferTimer = p_bufferTime;

            return true;
        }

        return false;
    }

    //Might want to change this so it does not feed the garbage collector monster
    private IEnumerator RunBufferTimer(System.Action<float> m_bufferTimerRef, float p_bufferTime)
    {
        float t = 0;

        while (t < p_bufferTime)
        {
            t += Time.deltaTime;
            m_bufferTimerRef(t);
            yield return null;
        }

        m_bufferTimerRef(p_bufferTime);
    }
    #endregion

    #region Collision Checks
    public bool IsGrounded()
    {
        RaycastHit hit;

        float rayLength = (m_characterController.height / 2) + m_characterController.skinWidth;

        if (Physics.SphereCast(transform.position, m_characterController.radius, Vector3.down, out hit, rayLength, m_groundMask))
        {
            return true;
        }

        if (m_characterController.isGrounded)
        {
            return true;
        }

        return false;
    }

    public bool HitCeiling()
    {
        if ((m_characterController.collisionFlags & CollisionFlags.Above) != 0)
        {
            return true;
        }

        return false;
    }

    public bool HitSide()
    {
        if ((m_characterController.collisionFlags & CollisionFlags.Sides) != 0)
        {
            return true;
        }

        return false;
    }

    public void HitWater()
	{
        Vector3 top = transform.position + m_characterController.center + Vector3.up * m_characterController.height * 0.5F;

        RaycastHit hit;

		if (Physics.Raycast(top, Vector3.down, out hit, Mathf.Infinity, m_waterMask))
		{
            /*
            Debug.Log(hit.distance);

            Debug.DrawLine(top, hit.point, Color.blue, 0f, false);

            DebugExtension.DebugArrow(hit.point, m_groundMovementVelocity.normalized, Color.green, 0f, false);

            Vector3 localXAxis = Vector3.Cross(m_groundMovementVelocity, Vector3.up);
            Vector3 forwardRotation = Vector3.ProjectOnPlane(hit.normal, localXAxis);
            Quaternion upwardSlopeOffset = Quaternion.FromToRotation(Vector3.up, forwardRotation);
            Vector3 targetMoveAmount = (upwardSlopeOffset * m_groundMovementVelocity);

            DebugExtension.DebugArrow(hit.point, targetMoveAmount, Color.red, 0f, false);
            */
        }
    }

    #endregion

    #region Physics Calculation Code
    private void CaculateTotalVelocity()
    {
        Vector3 velocity = Vector3.zero;

        velocity += m_groundMovementVelocity;
        velocity += m_gravityVelocity;
        velocity += m_slideVelocity;
        velocity += m_slopeVelocity;

        m_characterController.Move(velocity * Time.fixedDeltaTime);
    }

    private void ZeroVelocityOnGround()
    {
        if (m_characterController.isGrounded)
        {
            m_gravityVelocity.y = 0;
        }
    }

    private void CheckOffLedge()
    {
        if (!IsGrounded() && !m_offLedge)
        {
            OnOffLedge();
        }
        if (IsGrounded())
        {
            m_offLedge = false;
        }
    }

    private void OnOffLedge()
    {
        m_offLedge = true;
    }

    private void CheckFall()
	{
		if (m_gravityVelocity.y < 0 && !IsGrounded() && !m_isFalling)
		{
            m_isFalling = true;
            m_fallPositionY = transform.position.y;
		}
	}

    private void CheckLanded()
    {
        float fallDistance = 0;

		if (m_isFalling)
		{
            fallDistance = m_fallPositionY - transform.position.y;
        }

        if (IsGrounded() && !m_isLanded)
        {
            OnLanded(fallDistance);
        }

        if (!IsGrounded())
        {
            m_isLanded = false;
        }
    }

    private void OnLanded(float p_fallDistance)
    {
        m_isLanded = true;
        m_isFalling = false;
        m_hasJumped = false;

        if (CheckBuffer(ref m_jumpBufferTimer, ref m_jumpingProperties.m_jumpBufferTime, m_jumpBufferCoroutine))
        {
            //JumpMaxVelocity();
        }

        m_events.m_onLandedEvent.Invoke();
    }

    private void StartLandedSlide()
	{
		if (!m_isSliding)
		{
            StartCoroutine(LandedSlide());
		}
	}
    private IEnumerator LandedSlide()
	{
        m_isSliding = true;

        float t = 0;

		while (t < m_landedSlideTime)
		{
            t += Time.deltaTime;
            float progress = m_landedSlideCurve.Evaluate(t / m_landedSlideTime);
            float currentSlideSpeed = Mathf.Lerp(m_landedSlideSpeed, 0, progress);
            m_slideVelocity = transform.forward * currentSlideSpeed;
            yield return null;
		}

        m_slideVelocity = Vector3.zero;

        m_isSliding = false;
	}
    #endregion

    #region Slope Code
    private struct SlopeInfo
    {
        public bool m_onSlope;
        public float m_slopeAngle;
        public Vector3 m_slopeNormal;
    }

    private SlopeInfo OnSlope()
    {
        SlopeInfo slopeInfo = new SlopeInfo { };

        RaycastHit hit;

        Vector3 bottom = m_characterController.transform.position - new Vector3(0, m_characterController.height / 2, 0);

        if (Physics.Raycast(bottom, Vector3.down, out hit, 2f, m_groundMask))
        {
            if (hit.normal != Vector3.up)
            {
                slopeInfo.m_onSlope = true;
                slopeInfo.m_slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
                slopeInfo.m_slopeNormal = hit.normal;

                return slopeInfo;
            }
        }

        return slopeInfo;
    }

    private void SlopePhysics()
    {
        SlopeInfo slopeInfo = OnSlope();

        if (slopeInfo.m_onSlope == true)
        {
            if (m_gravityVelocity.y > 0)
            {
                return;
            }

            if (m_hasJumped)
            {
                return;
            }

            RaycastHit hit;

            Vector3 bottom = m_characterController.transform.position - new Vector3(0, m_characterController.height / 2, 0);

            if (Physics.Raycast(bottom, Vector3.down, out hit))
            {
                if (slopeInfo.m_slopeAngle > m_characterController.slopeLimit)
                {
                    //m_slideVelocity.x += (1f - hit.normal.y) * hit.normal.x * (0.9f);
                    //m_slideVelocity.z += (1f - hit.normal.y) * hit.normal.z * (0.9f);
				}

                m_characterController.Move(new Vector3(0, -(hit.distance), 0));
            }
        }
    }
    #endregion

    #region Basic Movement Code
    private void CalculateVelocity()
    {
        if (m_states.m_gravityControllState == GravityState.GravityEnabled)
        {
            m_gravityVelocity.y += m_gravity * Time.deltaTime;
		}
		else
		{
            m_gravityVelocity.y = 0;
		}

        PlayerMovement();

    }

    private void PlayerMovement()
	{
        if (m_states.m_movementControllState == MovementControllState.MovementEnabled)
        {
            GroundMovement();
        }
        else
        {
            m_groundMovementVelocity = Vector3.zero;
        }
    }

    private void FindTreeLookPoint()
	{

        /*
        float dist = Vector3.Distance(transform.position, m_treeTransform.position);

        //sphereCol.radius = dist;

        MeshCollider treeCOl = m_treeTransform.GetComponentInChildren<MeshCollider>();

        m_targetLookTransform.position = treeCOl.ClosestPoint(transform.position);
        */


        /*
        RaycastHit firstGroundHit;

        if (Physics.Raycast(transform.position, Vector3.down, out firstGroundHit, Mathf.Infinity, m_groundMask))
        {
            Vector3 lookDir = m_treeTransform.position - transform.position;
            lookDir.y = 0;

            Vector3 localXAxis = Vector3.Cross(lookDir, Vector3.up);
            Vector3 forwardRotation = Vector3.ProjectOnPlane(firstGroundHit.normal, localXAxis);
            Quaternion upwardSlopeOffset = Quaternion.FromToRotation(Vector3.up, forwardRotation);
            Vector3 targetMoveAmount = (upwardSlopeOffset * lookDir);

        }
        */
    }

    public bool CheckSphereExtra(Collider target_collider, SphereCollider sphere_collider, out Vector3 closest_point, out Vector3 surface_normal)
    {
        closest_point = Vector3.zero;
        surface_normal = Vector3.zero;
        float surface_penetration_depth = 0;

        Vector3 sphere_pos = sphere_collider.transform.position;
        if (Physics.ComputePenetration(target_collider, target_collider.transform.position, target_collider.transform.rotation, sphere_collider, sphere_pos, Quaternion.identity, out surface_normal, out surface_penetration_depth))
        {
            closest_point = sphere_pos + (surface_normal * (sphere_collider.radius - surface_penetration_depth));

            surface_normal = -surface_normal;

            return true;
        }

        return false;
    }

    private void TargetMovement()
	{
        /*
        FindTreeLookPoint();
        float targetAngle = m_cameraProperties.m_viewCameraTransform.eulerAngles.y;

        RaycastHit hit;

		if (Physics.Raycast(m_treeTransform.position + (Vector3.up * 10), -m_treeTransform.up, out hit, Mathf.Infinity, m_groundMask))
		{
            //m_targetLookTransform.position = hit.point + (Vector3.up * m_targetCameraHeightOffset);
        }

        Debug.DrawLine(transform.position, m_targetLookTransform.position);

        Vector3 lookPos = m_targetLookTransform.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos, Vector3.up);

        float horizontalSpeed = m_baseMovementProperties.m_runSpeed;
        float currentAcceleration = m_baseMovementProperties.m_accelerationTimeGrounded;

        Vector3 forwardMovement = transform.forward * m_movementInput.y;
        Vector3 rightMovement = transform.right * m_movementInput.x;

        Vector3 targetHorizontalMovement = (Vector3.ClampMagnitude(forwardMovement + rightMovement, 1.0f) * horizontalSpeed);
        Vector3 horizontalMovement = Vector3.SmoothDamp(m_groundMovementVelocity, targetHorizontalMovement, ref m_groundMovementVelocitySmoothing, currentAcceleration);

        m_groundMovementVelocity = new Vector3(horizontalMovement.x, 0, horizontalMovement.z);



        Vector3 targetMovementRotation = Quaternion.Euler(0, targetAngle, 0f) * Vector3.forward;
        float movementAngle = Vector3.Angle(transform.forward, targetMovementRotation);
        float progress = m_turnOffsetCurve.Evaluate(movementAngle / m_maximumBlendTurnAngle);
        float moveDir = -Mathf.Sign(Vector3.Cross(targetMovementRotation, transform.forward).y);
        m_turnBlend.SetBlendValue(progress * moveDir);

        */

    }

    private void GroundMovement()
	{
        Vector3 horizontalInput = Vector3.ClampMagnitude(new Vector3(m_movementInput.x, 0, m_movementInput.y), 1f);

        float targetAngle = transform.eulerAngles.y;

        if (horizontalInput.magnitude > 0)
        {
            targetAngle = Mathf.Atan2(horizontalInput.x, horizontalInput.z) * Mathf.Rad2Deg + m_cameraProperties.m_viewCameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref m_playerTurnSmoothingVelocity, m_baseMovementProperties.m_playerTurnSpeed);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            if (m_isSprinting)
            {
                PlayerStatsController.Instance.SprintEnergyDrain();
            }
        }

        float horizontalSpeed = m_baseMovementProperties.m_runSpeed;

        if (m_isWalking)
        {
            horizontalSpeed = m_baseMovementProperties.m_walkSpeed;
        }

        if (m_isSprinting)
        {
            horizontalSpeed = m_baseMovementProperties.m_sprintSpeed;
        }

        float currentAcceleration = m_baseMovementProperties.m_accelerationTimeGrounded;


        Vector3 targetMovementRotation = Quaternion.Euler(0, targetAngle, 0f) * Vector3.forward;

        Vector3 actualMovementDir = Quaternion.Euler(0, targetAngle, 0f) * Vector3.forward;

        //Vector3 targetHorizontalMovement = (transform.forward * horizontalSpeed) * horizontalInput.magnitude;
        Vector3 targetHorizontalMovement = (actualMovementDir * horizontalSpeed) * horizontalInput.magnitude;
        Vector3 horizontalMovement = Vector3.SmoothDamp(m_groundMovementVelocity, targetHorizontalMovement, ref m_groundMovementVelocitySmoothing, currentAcceleration);

        float movementAngle = Vector3.Angle(transform.forward, targetMovementRotation);
        float progress = m_turnOffsetCurve.Evaluate(movementAngle / m_maximumBlendTurnAngle);
        float moveDir = -Mathf.Sign(Vector3.Cross(targetMovementRotation, transform.forward).y);
        m_turnBlend.SetBlendValue(progress * moveDir);

        if (m_states.m_gravityControllState == GravityState.GravityEnabled)
        {
            m_groundMovementVelocity = new Vector3(horizontalMovement.x, 0, horizontalMovement.z);
        }
        else
        {
            Vector3 flyAmount = transform.up * (horizontalSpeed + m_currentHorizontalMovementSpeed) * m_flyInput;

            m_groundMovementVelocity = horizontalMovement + flyAmount;
        }
    }

    private void AimMovement()
	{
        Vector3 horizontalInput = Vector3.ClampMagnitude(new Vector3(m_movementInput.x, 0, m_movementInput.y), 1f);

        float targetAngle = m_cameraProperties.m_viewCameraTransform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref m_playerTurnSmoothingVelocity, m_baseMovementProperties.m_playerTurnSpeed);
        transform.rotation = Quaternion.Euler(0, angle, 0);

        float horizontalSpeed = m_baseMovementProperties.m_runSpeed;
        float currentAcceleration = m_baseMovementProperties.m_accelerationTimeGrounded;

        Vector3 targetMovementRotation = Quaternion.Euler(0, targetAngle, 0f) * Vector3.forward;

        Vector3 forwardMovement = transform.forward * m_movementInput.y;
        Vector3 rightMovement = transform.right * m_movementInput.x;

        Vector3 targetHorizontalMovement = (Vector3.ClampMagnitude(forwardMovement + rightMovement, 1.0f) * horizontalSpeed);
        Vector3 horizontalMovement = Vector3.SmoothDamp(m_groundMovementVelocity, targetHorizontalMovement, ref m_groundMovementVelocitySmoothing, currentAcceleration);


        float movementAngle = Vector3.Angle(transform.forward, targetMovementRotation);
        float progress = m_turnOffsetCurve.Evaluate(movementAngle / m_maximumBlendTurnAngle);
        float moveDir = -Mathf.Sign(Vector3.Cross(targetMovementRotation, transform.forward).y);
        m_turnBlend.SetBlendValue(progress * moveDir);

        m_groundMovementVelocity = new Vector3(horizontalMovement.x, 0, horizontalMovement.z);
    }
    #endregion

    #region Jump Code
    public void OnJumpInputDown()
    {
        m_jumpBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_jumpBufferTimer = (x), m_jumpingProperties.m_jumpBufferTime));

        if (CheckBuffer(ref m_graceTimer, ref m_jumpingProperties.m_graceTime, m_graceBufferCoroutine) && !IsGrounded() && m_gravityVelocity.y <= 0f)
        {
            //GroundJump();
            //return;
        }

        if (IsGrounded() && !m_isSliding)
        {
            GroundJump();
            return;
        }

    }

    public void OnJumpInputUp()
    {
        if (m_gravityVelocity.y > m_minJumpVelocity)
        {
            JumpMinVelocity();
        }
    }

    private void CalculateJump()
    {
        m_gravity = -(2 * m_jumpingProperties.m_maxJumpHeight) / Mathf.Pow(m_jumpingProperties.m_timeToJumpApex, 2);
        m_maxJumpVelocity = Mathf.Abs(m_gravity) * m_jumpingProperties.m_timeToJumpApex;
        m_minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(m_gravity) * m_jumpingProperties.m_minJumpHeight);
    }

    private void GroundJump()
    {
        m_events.m_onJumpEvent.Invoke();
        JumpMaxVelocity();
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

    private void JumpMaxMultiplied(float p_force)
    {
        m_gravityVelocity.y = m_maxJumpVelocity * p_force;
    }

    #endregion

    #region Slide Code
    private void CalculateSlopeSpeed()
    {
        if (OnSlope().m_onSlope)
        {
            float slopeCross = Vector3.Cross(m_slopeTransform.right, transform.forward).y;

            if (slopeCross < 0)
            {
                float currentSlopePercent = Mathf.InverseLerp(0, 45, m_slopeAngle);
                float currentSlopeSpeed = Mathf.Lerp(0, m_slideProperties.m_additionalSlopeDecendSpeed, currentSlopePercent);
                m_currentHorizontalMovementSpeed = currentSlopeSpeed;

                float currentSlopeAccel = Mathf.Lerp(m_baseMovementProperties.m_accelerationTimeGrounded, 0.4f, currentSlopePercent);
                m_currentHorizontalAccelerationSpeed = currentSlopeAccel;
            }
            else if (slopeCross > 0)
            {
                float currentSlopePercent = Mathf.InverseLerp(0, 45, m_slopeAngle);
                float currentSlopeSpeed = Mathf.Lerp(0, m_slideProperties.m_additionalSlopeAccendSpeed, currentSlopePercent);
                m_currentHorizontalMovementSpeed = currentSlopeSpeed;

                float currentSlopeAccel = Mathf.Lerp(m_baseMovementProperties.m_accelerationTimeGrounded, 0.07f, currentSlopePercent);
                m_currentHorizontalAccelerationSpeed = currentSlopeAccel;
            }


        }
        else
        {
            m_currentHorizontalMovementSpeed = 0;
            m_currentHorizontalAccelerationSpeed = m_baseMovementProperties.m_accelerationTimeGrounded;
        }
    }

    private void OnSlideStart()
    {
        if (!m_isSliding)
        {
            if (OnSlope().m_onSlope)
            {
                if (m_slopeAngle > m_slideProperties.m_slideStartAngle)
				{
                    StartCoroutine(RunSlide());
                }
            }
        }
    }

    private void StopSlide()
    {

    }

    private void SetSlideSlopeVariables()
	{
        SlopeInfo slopeInfo = OnSlope();

        if (slopeInfo.m_onSlope)
        {
            float normalX = slopeInfo.m_slopeNormal.x > 0 ? slopeInfo.m_slopeNormal.x : slopeInfo.m_slopeNormal.x * -1;
            float normalZ = slopeInfo.m_slopeNormal.z > 0 ? slopeInfo.m_slopeNormal.z : slopeInfo.m_slopeNormal.z * -1;

            Vector3 slopeDir = new Vector3(normalX * Mathf.Sign(slopeInfo.m_slopeNormal.x), 0, normalZ * Mathf.Sign(slopeInfo.m_slopeNormal.z));

            m_slopeTransform.rotation = Quaternion.LookRotation(slopeDir);
            m_slopeAngle = Vector3.Angle(Vector3.up, slopeInfo.m_slopeNormal);
        }
    }

    private IEnumerator RunSlide()
    {
        m_isSliding = true;

        //m_states.m_movementControllState = MovementControllState.MovementDisabled;

        Vector3 slopeVelocitySmoothing = Vector3.zero;
        Vector3 slopeShiftVelocitySmoothing = Vector3.zero;

        while (m_slopeAngle > m_slideProperties.m_slideStartAngle && IsGrounded())
        {
            #region Slope Slide Code

            SlopeInfo slopeInfo = OnSlope();

            if (slopeInfo.m_onSlope)
            {
                float currentSlopePercent = Mathf.InverseLerp(m_slideProperties.m_slideStartAngle, m_slideProperties.m_maximumSlopeAngle, m_slopeAngle);
                float currentSlopeSpeed = Mathf.Lerp(m_baseMovementProperties.m_runSpeed, m_slideProperties.m_slideAngleBoostMax, currentSlopePercent);
                Vector3 targetSlopeVelocity = m_slopeTransform.forward * currentSlopeSpeed;
                m_slopeVelocity = targetSlopeVelocity;

                /*
                //Vector3 targetShiftVelocity = m_slideProperties.m_slopeTransform.right * m_movementInput.x * m_slideProperties.m_slideSideShiftMaxSpeed;

                Vector3 rightInput = m_slopeTransform.right * m_movementInput.x;
                Vector3 forwardInput = m_slopeTransform.forward * m_movementInput.y;

                Vector3 fullInput = Vector3.ClampMagnitude(rightInput + forwardInput, 1f);

                Vector3 targetMovement = fullInput * m_slideProperties.m_slideSideShiftMaxSpeed;

                Vector3 shiftMovement = Vector3.SmoothDamp(m_slopeShiftVelocity, targetMovement, ref slopeShiftVelocitySmoothing, m_slideProperties.m_slideSideShiftAcceleration);
                m_slopeShiftVelocity = shiftMovement;

                transform.rotation = Quaternion.LookRotation(m_slopeVelocity);
                */
            }
            #endregion

            yield return new WaitForFixedUpdate();
        }

        //m_groundMovementVelocity = m_slopeVelocity + m_slopeShiftVelocity;

        m_slopeVelocity = Vector3.zero;
        m_slideVelocity = Vector3.zero;
        m_slopeShiftVelocity = Vector3.zero;

        m_states.m_movementControllState = MovementControllState.MovementEnabled;

        m_isSliding = false;
    }
    #endregion

    #region Climb Code

    private void RunClimbLoop()
	{
        UpdateState();
        m_climbVelocity = m_rigidbody.velocity;

        AdjustClimbVelocity();

		if (m_isClimbing)
		{
            //Debug.Log("trying to stick");
            m_climbVelocity -= m_collisions.m_climbNormal * (m_maxClimbAcceleration * 0.9f * Time.deltaTime);
		}
		else
		{
            RaycastHit hit;

			if (Physics.Raycast(transform.position, -m_collisions.m_lastClimbNormal, out hit, Mathf.Infinity, m_groundMask))
			{
                m_climbVelocity -= hit.normal * (m_maxClimbAcceleration * 0.9f * Time.deltaTime);
                //Debug.Log(hit.normal * (m_maxClimbAcceleration * 0.2f * Time.deltaTime));

                Debug.DrawLine(transform.position, hit.point, Color.red);

            }
        }

        m_rigidbody.velocity = m_climbVelocity;
        m_collisions.Reset();
        //ClearState();
    }

    private void UpdateState()
	{
        if (m_collisions.m_climbContactCount >= 1)
        {
            m_isClimbing = true;
        }
        else
        {
            m_isClimbing = false;
        }
    }

    private void AdjustClimbVelocity()
    {
        float acceleration, speed;
        Vector3 xAxis, zAxis;

        acceleration = m_maxClimbAcceleration;
        speed = m_climbSpeed;
        xAxis = Vector3.Cross(m_collisions.m_climbNormal, Vector3.up);
        zAxis = Vector3.up;

        xAxis = ProjectDirectionOnPlane(xAxis, m_collisions.m_climbNormal);
        zAxis = ProjectDirectionOnPlane(zAxis, m_collisions.m_climbNormal);

        Vector3 relativeVelocity = m_climbVelocity;

        Vector3 adjustment = Vector3.zero;
        adjustment.x = m_movementInput.x * speed - Vector3.Dot(relativeVelocity, xAxis);
        adjustment.z = m_movementInput.y * speed - Vector3.Dot(relativeVelocity, zAxis);

        adjustment = Vector3.ClampMagnitude(adjustment, acceleration * Time.fixedDeltaTime);

        m_climbVelocity += xAxis * adjustment.x + zAxis * adjustment.z;
    }

    private void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void EvaluateCollision(Collision p_collision)
    {
        for (int i = 0; i < p_collision.contactCount; i++)
        {
            Vector3 normal = p_collision.GetContact(i).normal;

            if (CheckCollisionLayer(m_groundMask, p_collision.gameObject))
            {
                m_collisions.m_climbContactCount += 1;
                m_collisions.m_climbNormal += normal;
                m_collisions.m_lastClimbNormal = m_collisions.m_climbNormal;
            }
        }
    }

    public struct CollisionInfo
    {
        public Vector3 m_climbNormal, m_lastClimbNormal;
        public int m_climbContactCount;

        public void Reset()
		{
            m_climbNormal = Vector3.zero;
            m_climbContactCount = 0;
        }
    }

    private Vector3 ProjectDirectionOnPlane(Vector3 p_direction, Vector3 p_normal)
    {
        return (p_direction - p_normal * Vector3.Dot(p_direction, p_normal)).normalized;
    }

	#endregion

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
}