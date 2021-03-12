﻿using System.Collections;
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
	#endregion

	#region Climb Properties
	[FoldoutGroup("Climb")]
    public float m_climbMaxHeadRotation;
	#endregion

	private CollisionController m_collisionController;
    private FullBodyBipedIK m_fullBodyBipedIK;
    private Animator m_animator;

    private void Start()
	{
        m_collisionController = GetComponent<CollisionController>();
        m_animator = GetComponentInChildren<Animator>();

        m_fullBodyBipedIK = GetComponentInChildren<FullBodyBipedIK>();

        m_fullBodyBipedIK.solver.leftHandEffector.positionWeight = 0f;
        m_fullBodyBipedIK.solver.leftHandEffector.rotationWeight = 0f;

        m_fullBodyBipedIK.solver.rightHandEffector.positionWeight = 0f;
        m_fullBodyBipedIK.solver.rightHandEffector.rotationWeight = 0f;

        m_grounder = GetComponentInChildren<GrounderFBBIK>();

        m_useGrounder = true;
    }

	private void Update()
	{
        ArmIK();
        GrounderWeight();
    }

	#region General Animations
	public void SetGroundMovementAnimations(Vector3 p_horizontalVelocity, float p_runSpeed, float p_sprintSpeed, float p_walkSpeed)
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

    public void SetJumpAnimations(float p_airMovement, bool p_hasJumped)
	{
        m_animator.SetFloat("JumpMovement", p_airMovement);
        ToggleGrounder(!p_hasJumped);
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
        m_grounder.weight = m_currentGrounderWeight;
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

        m_headEffector.rotation = Quaternion.RotateTowards(m_headEffector.rotation, Quaternion.LookRotation(targetMoveAmount, Vector3.up), m_headEffectorRotateSpeed * Time.deltaTime);
    }
	#endregion
}