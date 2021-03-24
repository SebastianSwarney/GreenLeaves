using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;

public class PlayerVisualsController : MonoBehaviour
{
    public Transform m_modelTransform;

    #region Head effector properties
    [FoldoutGroup("Head Effector")]
    public Transform m_headEffector;
    [FoldoutGroup("Head Effector")]
    public float m_headEffectorRotateSpeed;
    #endregion

    #region Arm IK Properites
    [FoldoutGroup("Arm IK")]
    public Transform m_leftArmIKTarget, m_rightArmIKTarget;
    [FoldoutGroup("Arm IK")]
    public float m_armIKSmoothingTime;

    private float m_armIKSmoothingVelocity;
    private float m_currentArmIKWeight;
    private bool m_useArmIK;
    #endregion

    #region Grounder Properties
    [FoldoutGroup("Grounder")]
    public float m_grounderSmoothingTime;

    private float m_grounderSmoothingVelocity;
    private bool m_useGrounder = true;
    private float m_currentGrounderWeight;
    private GrounderFBBIK m_grounder;
    #endregion

    #region Slide Properties
    [FoldoutGroup("Slide")]
    public OffsetPoseBlend m_slidePose;
    [FoldoutGroup("Slide")]
    public OffsetPoseBlend m_shiftSlidePose;
	#endregion

	#region Climb Properties
	[FoldoutGroup("Climb")]
    public float m_climbMaxHeadRotation;
    #endregion

    #region Sprint Animation Properites
    [FoldoutGroup("Sprint Animation")]
    public float m_sprintTimeForSkidAnimation;

    private float m_sprintSkidTimer;
	#endregion

	#region Ground Turn Blend Properties
    [FoldoutGroup("Ground Turn Blend")]
	public float m_maximumTurnAngle;
    [FoldoutGroup("Ground Turn Blend")]
    public OffsetPoseBlend m_turnBlend;
    #endregion

    #region Tiredness Properties
    [FoldoutGroup("Tiredness")]
    public ParticleSystem m_sweatParticle;
    [FoldoutGroup("Tiredness")]
    public Vector2 m_minMaxSweatAmount;
    #endregion

	private PlayerController m_playerController;
    private FullBodyBipedIK m_fullBodyBipedIK;
    [HideInInspector]
    public Animator m_animator;

    private Cinemachine.CinemachineFreeLook m_freelookCam;

    private void Start()
	{
        m_playerController = GetComponent<PlayerController>();
        m_animator = GetComponentInChildren<Animator>();
        m_fullBodyBipedIK = GetComponentInChildren<FullBodyBipedIK>();
        m_grounder = GetComponentInChildren<GrounderFBBIK>();

        m_freelookCam = GetComponentInChildren<Cinemachine.CinemachineFreeLook>();

        m_fullBodyBipedIK.solver.leftHandEffector.positionWeight = 0f;
        m_fullBodyBipedIK.solver.leftHandEffector.rotationWeight = 0f;
        m_fullBodyBipedIK.solver.rightHandEffector.positionWeight = 0f;
        m_fullBodyBipedIK.solver.rightHandEffector.rotationWeight = 0f;

        m_useGrounder = true;
    }

	private void Update()
	{
        ArmIK();
        GrounderWeight();

		if (Player_EquipmentUse_Torch.Instance != null)
		{
            if (Player_EquipmentUse_Torch.Instance.m_torchEquipped)
            {
                m_animator.SetLayerWeight(1, 1);
            }
            else
            {
                m_animator.SetLayerWeight(1, 0);
            }
        }

		if (Input.GetMouseButtonDown(0))
		{
            //m_animator.SetTrigger("SwingAxe");
		}
    }


    public void OnFootUpdate(int p_footSide)
	{

	}

	#region General Animations
	public void SetGroundMovementAnimations(Vector3 p_horizontalVelocity, float p_sprintSpeed, float p_runSpeed, float p_walkSpeed)
	{
        float inverseLerpValue;
        float lerpValue;

        float speedValue = p_horizontalVelocity.magnitude;

        if (speedValue > p_runSpeed)
        {
            inverseLerpValue = Mathf.InverseLerp(p_runSpeed, p_sprintSpeed, speedValue);
            lerpValue = Mathf.Lerp(0.5f, 1, inverseLerpValue);
        }
        else
        {
            inverseLerpValue = Mathf.InverseLerp(p_walkSpeed, p_runSpeed, speedValue);
            lerpValue = Mathf.Lerp(0, 0.5f, inverseLerpValue);
        }

        m_animator.SetFloat("ForwardMovement", lerpValue);
    }

    public void SetAimMovementAnimations(Vector3 p_horizontalVelocity, float p_walkSpeed)
	{
        Vector3 localVelocity = transform.InverseTransformDirection(p_horizontalVelocity);

        float forwardInverse = Mathf.InverseLerp(p_walkSpeed, -p_walkSpeed, localVelocity.z);
        float sideInverse = Mathf.InverseLerp(-p_walkSpeed, p_walkSpeed, localVelocity.x);

        float forwardValue = Mathf.Lerp(1, -1, forwardInverse);
        float sideValue = Mathf.Lerp(-1, 1, sideInverse);

		if (sideValue == 0 && forwardValue == 0)
		{
            float axisValue = m_freelookCam.m_XAxis.m_InputAxisValue;

            if (axisValue != 0)
            {
                if (axisValue < 0)
                {
                    sideValue = -1;
                }
                else
                {
                    sideValue = 1;
                }
            }
        }

        m_animator.SetFloat("ForwardMovement", forwardValue);
        m_animator.SetFloat("SideMovement", sideValue);

    }

    public void SetAiming(bool p_aim)
	{
        m_animator.SetBool("Aim", p_aim);
    }

    public void SetJumpAnimations(float p_airMovement, bool p_hasJumped)
	{
        //m_animator.SetFloat("JumpMovement", p_airMovement);
        //ToggleGrounder(!p_hasJumped);
    }

    public void SetGrounded(bool p_groundedState)
	{
        m_animator.SetBool("IsGrounded", p_groundedState);
    }
	#endregion

	#region Climb Animations
    public void ToggleClimbAnimation(bool p_climbState, bool p_clamberState)
	{
        m_animator.SetBool("Climb", p_climbState);
        m_animator.SetBool("Clamber", p_clamberState);
    }

    public void SetClamberProgress(float p_clamberProgress)
	{
        m_animator.SetFloat("ClamberProgress", p_clamberProgress);
    }

    public void ClimbHeadAnimations(Vector2 p_animationVector)
    {
        float horizontalRotation = Mathf.Lerp(-m_climbMaxHeadRotation, m_climbMaxHeadRotation, p_animationVector.x);
        float verticalRotation = Mathf.Lerp(m_climbMaxHeadRotation, -m_climbMaxHeadRotation, p_animationVector.y);
        Quaternion headRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);

        m_headEffector.localRotation = Quaternion.RotateTowards(m_headEffector.localRotation, headRotation, m_headEffectorRotateSpeed * Time.deltaTime);
    }

    public void ClimbAnimations(float p_horizontalValue, float p_verticalValue)
    {
        m_animator.SetFloat("ClimbHorizontal", p_horizontalValue);
        m_animator.SetFloat("ClimbVertical", p_verticalValue);
    }
    #endregion

    #region Slide Animations
    public void SetSlideAnimations(bool m_sliding)
    {
        m_animator.SetBool("IsSliding", m_sliding);
    }

    public void SetSlideOffsetPose(float p_value)
    {
        m_slidePose.SetBlendValue(p_value);
    }

    public void SetSlideRotation(float p_slopeAngle)
	{
        //m_modelTransform.localRotation = Quaternion.AngleAxis(p_slopeAngle, Vector3.right);
	}

    public void SetSlideShiftTilt(float p_value)
	{
        m_shiftSlidePose.SetBlendValue(p_value);
    }
    #endregion

    #region Arm IK
    public void ToggleArmIK(bool p_ikState)
    {
        m_useArmIK = p_ikState;
    }

    private void ArmIK()
	{
        float weightTarget = 0;

		if (m_useArmIK)
		{
            weightTarget = 1f;
        }

        m_currentArmIKWeight = Mathf.SmoothDamp(m_currentArmIKWeight, weightTarget, ref m_armIKSmoothingVelocity, m_armIKSmoothingTime);

        m_fullBodyBipedIK.solver.leftHandEffector.positionWeight = m_currentArmIKWeight;
        m_fullBodyBipedIK.solver.leftHandEffector.rotationWeight = m_currentArmIKWeight;

        m_fullBodyBipedIK.solver.rightHandEffector.positionWeight = m_currentArmIKWeight;
        m_fullBodyBipedIK.solver.rightHandEffector.rotationWeight = m_currentArmIKWeight;
    }

    public void SetArmTargetPosition(Vector3 p_leftArm, Vector3 p_rightArm)
    {
        m_leftArmIKTarget.position = p_leftArm;
        m_rightArmIKTarget.position = p_rightArm;
    }
    #endregion

    #region Grounder
    private void GrounderWeight()
	{
        float weightTarget = 0;

        if (m_useGrounder)
        {
            weightTarget = 1f;
        }

        m_currentGrounderWeight = Mathf.SmoothDamp(m_currentGrounderWeight, weightTarget, ref m_grounderSmoothingVelocity, m_grounderSmoothingTime);
        //m_grounder.weight = m_currentGrounderWeight;
    }

    public void ToggleGrounder(bool p_grounderState)
	{
        m_useGrounder = p_grounderState;
	}
	#endregion

	#region Head
	public void CenterHead()
    {
        Quaternion headRotation = Quaternion.Euler(0, 0, 0);
        m_headEffector.localRotation = headRotation;
    }

    public void SetGroundHeadRotation(Vector3 p_groundNormal)
    {
        Vector3 localXAxis = Vector3.Cross(transform.forward, Vector3.up);
        Vector3 forwardRotation = Vector3.ProjectOnPlane(p_groundNormal, localXAxis);
        Quaternion upwardSlopeOffset = Quaternion.FromToRotation(Vector3.up, forwardRotation);
        Vector3 targetMoveAmount = (upwardSlopeOffset * transform.forward);

        //m_headEffector.rotation = Quaternion.RotateTowards(m_headEffector.rotation, Quaternion.LookRotation(targetMoveAmount, Vector3.up), m_headEffectorRotateSpeed * Time.deltaTime);
    }
	#endregion

    public void EvaluateSkidCondition(bool p_sprinting, Vector3 p_newHorizontalInput, Vector3 p_oldHorizontalInput)
	{
		if (p_sprinting && p_oldHorizontalInput.magnitude == 1)
		{
            m_sprintSkidTimer += Time.deltaTime;
        }

        if (p_sprinting && p_newHorizontalInput.magnitude < 1 && p_oldHorizontalInput.magnitude == 1 && m_sprintSkidTimer >= m_sprintTimeForSkidAnimation)
        {
            m_animator.SetTrigger("Skid");

            m_sprintSkidTimer = 0;
        }

		if (!p_sprinting || p_newHorizontalInput.magnitude < 1)
		{
            m_sprintSkidTimer = 0;
        }
    }

    public void SetTurnBlendValue(float p_targetAngle, bool p_zeroValue = false)
	{
        Vector3 targetMovementRotation = Quaternion.Euler(0, p_targetAngle, 0f) * Vector3.forward;
        float betweenMovementAngle = Vector3.Angle(transform.forward, targetMovementRotation);
        float progress = betweenMovementAngle / m_maximumTurnAngle;
        float moveDir = -Mathf.Sign(Vector3.Cross(targetMovementRotation, transform.forward).y);
		
		if (p_zeroValue)
		{
            m_turnBlend.SetBlendValue(0);
		}
		else
		{
            m_turnBlend.SetBlendValue(progress * moveDir);
        }

        float t = Mathf.InverseLerp(-1, 1, progress * moveDir);
        float horizontalRotation = Mathf.Lerp(-90, 90, t);
        Quaternion headRotation = Quaternion.Euler(0, horizontalRotation, 0);

		if (!p_zeroValue)
		{
            m_headEffector.localRotation = Quaternion.RotateTowards(m_headEffector.localRotation, headRotation, m_headEffectorRotateSpeed * Time.deltaTime);
        }
    }

    public void RunTiredness(float p_currentTiredness)
	{
        if (!m_sweatParticle.isPlaying)
        {
            m_sweatParticle.Play();
        }

        float sweatAmount = Mathf.Lerp(m_minMaxSweatAmount.y, m_minMaxSweatAmount.x, p_currentTiredness);

        ParticleSystem.EmissionModule emmision = m_sweatParticle.emission;
        emmision.rateOverTime = sweatAmount;
    }

    public void PauseTiredness()
	{
        if (m_sweatParticle.isPlaying) m_sweatParticle.Stop();
    }
}