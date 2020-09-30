using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class PlayerControllerEvent : UnityEvent { }

public class PlayerController : MonoBehaviour
{
    #region Player States
    public enum MovementControllState { MovementEnabled, MovementDisabled }
    public enum GravityState { GravityEnabled, GravityDisabled }

    [System.Serializable]
    public struct PlayerState
    {
        public MovementControllState m_movementControllState;
        public GravityState m_gravityControllState;
    }

    public PlayerState m_states;
    #endregion

    #region Player Events
    public PlayerControllerEvents m_events;
    [System.Serializable]
    public struct PlayerControllerEvents
    {
        [Header("Basic Events")]
        public PlayerControllerEvent m_onLandedEvent;
        public PlayerControllerEvent m_onJumpEvent;
        public PlayerControllerEvent m_onRespawnEvent;

        [Header("Wall Run Events")]
        public PlayerControllerEvent m_onWallRunBeginEvent;
        public PlayerControllerEvent m_onWallRunEndEvent;
        public PlayerControllerEvent m_onWallRunJumpEvent;

        [Header("Wall Climb Events")]
        public PlayerControllerEvent m_onWallClimbBeginEvent;
        public PlayerControllerEvent m_onWallClimbEndEvent;
        public PlayerControllerEvent m_onWallClimbJumpEvent;

        [Header("Wall Jump Events")]
        public PlayerControllerEvent m_onWallJumpEvent;

        [Header("Leap Events")]
        public PlayerControllerEvent m_onLeapEvent;

    }
    #endregion

    #region Camera Properties
    [System.Serializable]
    public struct CameraProperties
    {
        public float m_mouseSensitivity;
        public float m_maxCameraAng;
        public bool m_inverted;
        public Camera m_camera;
        public Transform m_cameraMain;
    }

    [Header("Camera Properties")]
    public CameraProperties m_cameraProperties;
    #endregion

    #region Base Movement Properties
    [System.Serializable]
    public struct BaseMovementProperties
    {
        public float m_baseMovementSpeed;
        public float m_accelerationTimeGrounded;
        public float m_accelerationTimeAir;
        public float m_slopeFriction;
    }

    [Header("Base Movement Properties")]
    public BaseMovementProperties m_baseMovementProperties;

    private float m_currentMovementSpeed;
    [HideInInspector]
    public Vector3 m_groundMovementVelocity;
    private Vector3 m_velocitySmoothing;
    private CharacterController m_characterController;
    private Coroutine m_jumpBufferCoroutine;
    private Coroutine m_graceBufferCoroutine;
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

    [Header("Jumping Properties")]
    public JumpingProperties m_jumpingProperties;

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
        public Transform m_slopeTransform;
    }

    [Header("Slide Properties")]
    public SlideProperties m_slideProperties;

    private bool m_isSliding;
    private float m_slideTimer;
    private Vector3 m_slideVelocity;
    private Vector3 m_slopeVelocity;
    private Vector3 m_slopeShiftVelocity;

    private Coroutine m_slideCooldownCoroutine;
    private float m_slideCooldownTimer;
    #endregion

    [HideInInspector]
    public Vector2 m_movementInput;
    private Vector2 m_lookInput;

    public Transform referenceCamera;

    public float turnSpeed;

    private float turnSmoothingVelocity;

    public Animator playerAnimator;

    public Transform m_modelTransform;
    public float m_maxTiltAngle;

    private float m_slopeAngle;

    public float m_slideStartAngle;
    public float m_maximumSlopeAngle;

    public float m_maxSlopeDecendSpeed;
    public float m_maxSlopeAccendSpeed;

    private float m_currentHorizontalMovementSpeed;
    private float m_currentHorizontalAccelerationSpeed;

    private void Start()
    {
        m_characterController = GetComponent<CharacterController>();

        CalculateJump();

        m_currentMovementSpeed = m_baseMovementProperties.m_baseMovementSpeed;
        m_jumpBufferTimer = m_jumpingProperties.m_jumpBufferTime;
    }

    private void OnValidate()
    {
        CalculateJump();
    }

    private void Update()
    {
        PerformController();
        SetSlideSlopeVariables();
        OnSlideStart();
        CalculateSlope();

        playerAnimator.SetFloat("ForwardMovement", m_groundMovementVelocity.magnitude);
        //playerAnimator.SetFloat("AirMovement", -m_gravityVelocity.y);
        //playerAnimator.SetBool("IsGrounded", IsGrounded());
        playerAnimator.SetBool("IsSliding", m_isSliding);
    }

    public void PerformController()
    {
        CalculateVelocity();
        CaculateTotalVelocity();

        CheckLanded();
        SlopePhysics();

        ZeroOnGroundCeiling();
    }

    private void CalculateVelocity()
    {
        if (m_states.m_gravityControllState == GravityState.GravityEnabled)
        {
            m_gravityVelocity.y += m_gravity * Time.deltaTime;
        }

        if (m_states.m_movementControllState == MovementControllState.MovementEnabled)
        {
            Vector3 targetHorizontalMovementDirection = new Vector3(m_movementInput.x, 0, m_movementInput.y);

            Vector3 targetHorizontalMovement = Vector3.zero;

            if (targetHorizontalMovementDirection.magnitude != 0)
			{
                float targetAngle = Mathf.Atan2(targetHorizontalMovementDirection.x, targetHorizontalMovementDirection.z) * Mathf.Rad2Deg + referenceCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothingVelocity, turnSpeed);
                transform.rotation = Quaternion.Euler(0, angle, 0);

                Vector3 actualMovementDir = Quaternion.Euler(0, targetAngle, 0f) * Vector3.forward;

                targetHorizontalMovement = actualMovementDir * m_currentHorizontalMovementSpeed;
            }

            //float currentAcceleration = IsGrounded() ? m_baseMovementProperties.m_accelerationTimeGrounded : m_baseMovementProperties.m_accelerationTimeAir;
            float currentAcceleration = m_currentHorizontalAccelerationSpeed;
            Vector3 horizontalMovement = Vector3.SmoothDamp(m_groundMovementVelocity, targetHorizontalMovement, ref m_velocitySmoothing, currentAcceleration);
            m_groundMovementVelocity = new Vector3(horizontalMovement.x, 0, horizontalMovement.z);
		}
		else
		{
            m_groundMovementVelocity = Vector3.zero;
		}

    }

    private void CalculateSlope()
	{
		if (OnSlope().m_onSlope)
		{
            float slopeCross = Vector3.Cross(m_slideProperties.m_slopeTransform.right, transform.forward).y;

            if (slopeCross < 0)
            {
                float currentSlopePercent = Mathf.InverseLerp(0, 45, m_slopeAngle);
                float currentSlopeSpeed = Mathf.Lerp(m_baseMovementProperties.m_baseMovementSpeed, 20f, currentSlopePercent);
                m_currentHorizontalMovementSpeed = currentSlopeSpeed;

                float currentSlopeAccel = Mathf.Lerp(m_baseMovementProperties.m_accelerationTimeGrounded, 0.4f, currentSlopePercent);
                m_currentHorizontalAccelerationSpeed = currentSlopeAccel;
            }
            else if (slopeCross > 0)
            {
                float currentSlopePercent = Mathf.InverseLerp(0, 45, m_slopeAngle);
                float currentSlopeSpeed = Mathf.Lerp(m_baseMovementProperties.m_baseMovementSpeed, 10f, currentSlopePercent);
                m_currentHorizontalMovementSpeed = currentSlopeSpeed;

                float currentSlopeAccel = Mathf.Lerp(m_baseMovementProperties.m_accelerationTimeGrounded, 0.07f, currentSlopePercent);
                m_currentHorizontalAccelerationSpeed = currentSlopeAccel;
            }
		}
		else
		{
            m_currentHorizontalMovementSpeed = m_baseMovementProperties.m_baseMovementSpeed;
            m_currentHorizontalAccelerationSpeed = m_baseMovementProperties.m_accelerationTimeGrounded;
        }
    }

    #region Input Code
    public void SetMovementInput(Vector2 p_input)
    {
        m_movementInput = p_input;
    }

    public void SetLookInput(Vector2 p_input, float p_sensitivity)
    {
        m_lookInput = p_input;
        m_cameraProperties.m_mouseSensitivity = p_sensitivity;
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

    #region Physics Calculation Code

    private void CaculateTotalVelocity()
    {
        Vector3 velocity = Vector3.zero;

        velocity += m_groundMovementVelocity;
        velocity += m_gravityVelocity;
        velocity += m_slopeVelocity;
        velocity += m_slopeShiftVelocity;

        m_characterController.Move(velocity * Time.deltaTime);
    }

    public bool IsGrounded()
    {
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

    private void OnOffLedge()
    {
        m_offLedge = true;

        m_graceBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_graceTimer = (x), m_jumpingProperties.m_graceTime));

    }

    private void ZeroOnGroundCeiling()
    {
        if (IsGrounded())
        {
            m_gravityVelocity.y = 0;
        }

        if (HitCeiling() && m_gravityVelocity.y > 0)
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

    private void CheckLanded()
    {
        if (IsGrounded() && !m_isLanded)
        {
            OnLanded();
        }

        if (!IsGrounded())
        {
            m_isLanded = false;
        }
    }

    private void OnLanded()
    {
        m_isLanded = true;
        m_hasJumped = false;

        if (CheckBuffer(ref m_jumpBufferTimer, ref m_jumpingProperties.m_jumpBufferTime, m_jumpBufferCoroutine))
        {
            JumpMaxVelocity();
        }

        m_events.m_onLandedEvent.Invoke();
    }

    public void PhysicsSeekTo(Vector3 p_targetPosition)
    {
        Vector3 deltaPosition = p_targetPosition - transform.position;
        m_groundMovementVelocity = deltaPosition / Time.deltaTime;
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

        if (Physics.Raycast(bottom, Vector3.down, out hit, 1f))
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
                    //m_velocity.x += (1f - hit.normal.y) * hit.normal.x * (m_baseMovementProperties.m_slopeFriction);
                    //m_velocity.z += (1f - hit.normal.y) * hit.normal.z * (m_baseMovementProperties.m_slopeFriction);
                }

                m_characterController.Move(new Vector3(0, -(hit.distance), 0));
            }
        }
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

        if (IsGrounded() || OnSlope().m_onSlope)
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
    private void OnSlideStart()
    {
        if (!m_isSliding)
        {
            if (IsGrounded() || OnSlope().m_onSlope)
            {
                if (m_slopeAngle > m_slideStartAngle)
				{
                    float slopeCross = Vector3.Cross(m_slideProperties.m_slopeTransform.right, transform.forward).y;

                    if (slopeCross < -0.8f)
					{
                        //StartCoroutine(RunSlide());
                    }
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

            m_slideProperties.m_slopeTransform.rotation = Quaternion.LookRotation(slopeDir);
            m_slopeAngle = Vector3.Angle(Vector3.up, slopeInfo.m_slopeNormal);
        }
    }

    private IEnumerator RunSlide()
    {
        m_isSliding = true;

        m_states.m_movementControllState = MovementControllState.MovementDisabled;

        Vector3 slopeVelocitySmoothing = Vector3.zero;
        Vector3 slopeShiftVelocitySmoothing = Vector3.zero;

        while (m_slopeAngle > m_slideStartAngle && IsGrounded())
        {
            #region Slope Slide Code

            SlopeInfo slopeInfo = OnSlope();

            if (slopeInfo.m_onSlope)
            {
                float currentSlopePercent = Mathf.InverseLerp(m_slideStartAngle, m_maximumSlopeAngle, m_slopeAngle);
                float currentSlopeSpeed = Mathf.Lerp(m_baseMovementProperties.m_baseMovementSpeed, m_slideProperties.m_slideAngleBoostMax, currentSlopePercent);
                Vector3 targetSlopeVelocity = m_slideProperties.m_slopeTransform.forward * currentSlopeSpeed;
                m_slopeVelocity = targetSlopeVelocity;
                
                //Vector3 targetShiftVelocity = m_slideProperties.m_slopeTransform.right * m_movementInput.x * m_slideProperties.m_slideSideShiftMaxSpeed;

                Vector3 rightInput = m_slideProperties.m_slopeTransform.right * m_movementInput.x;
                Vector3 forwardInput = m_slideProperties.m_slopeTransform.forward * m_movementInput.y;

                Vector3 fullInput = Vector3.ClampMagnitude(rightInput + forwardInput, 1f);

                Vector3 targetMovement = fullInput * m_slideProperties.m_slideSideShiftMaxSpeed;

                Vector3 shiftMovement = Vector3.SmoothDamp(m_slopeShiftVelocity, targetMovement, ref slopeShiftVelocitySmoothing, m_slideProperties.m_slideSideShiftAcceleration);
                m_slopeShiftVelocity = shiftMovement;

                transform.rotation = Quaternion.LookRotation(m_slopeVelocity);
                m_modelTransform.localRotation = Quaternion.AngleAxis(m_slopeAngle, Vector3.right);

                //Debug.Log(m_slopeVelocity.magnitude);

                if (m_slopeVelocity.magnitude > 25)
                {
                    Debug.LogError("Fall over fool");
                }

                if (m_slopeAngle > m_maximumSlopeAngle)
				{

				}
            }
            #endregion

            yield return new WaitForFixedUpdate();
        }

        m_slopeVelocity = Vector3.zero;
        m_slideVelocity = Vector3.zero;
        m_slopeShiftVelocity = Vector3.zero;

        m_modelTransform.localRotation = Quaternion.identity;
        m_states.m_movementControllState = MovementControllState.MovementEnabled;

        m_isSliding = false;
    }
    #endregion

    float Map(float min, float max, float t)
    {
        return min + t * (max - min);
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
}