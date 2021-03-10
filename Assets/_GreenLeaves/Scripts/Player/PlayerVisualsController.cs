using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class PlayerVisualsController : MonoBehaviour
{
    public float m_maxSlopeEffortSpeed;

    public Transform m_headEffector;

    public float m_headEffectorRotateSpeed;

    public float m_maxModelRotationSpeed;

    public float m_maxUpwardSlopeModelRotation;
    public float m_maxSidewaysSlopeModelRotation;

    public Vector2 m_minMaxModelRotationAngle;

    public Transform m_modelTransform;

    public Transform m_slopeTransform;

	private Animator m_animator;
    private CollisionController m_collisionController;

    public OffsetPoseBlend m_effortPose;

    public Transform m_leftArmIKTarget, m_rightArmIKTarget;

    private FullBodyBipedIK m_fullBodyBipedIK;

    private bool m_armIK;

    private float m_armIKSmoothingVelocity;

    public float m_armIKSmoothingTime;

    private float m_currentArmIKWeight;

    private GrounderFBBIK m_grounder;

    private bool m_useGrounder = true;
    public float m_grounderSmoothingTime;
    private float m_grounderSmoothingVelocity;
    private float m_currentGrounderWeight;

    public float m_climbMaxHeadRotation;

    public float m_maxClimbSpeedMultiplier;

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
        //SetAnimations();
        ArmIK();
        GrounderWeight();
    }

	#region Arm IK
	public void ToggleArmIK(bool p_ikState)
    {
        m_armIK = p_ikState;
    }

    private void ArmIK()
	{
        float weightTarget = 0;

		if (m_armIK)
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

    public void CenterHead()
	{
        Quaternion headRotation = Quaternion.Euler(0, 0, 0);
        m_headEffector.localRotation = headRotation;
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

    public void CalculateSlopeEffort(float p_currentSlope, float p_minSlope, float p_maxSlope)
	{
        float effortProgress = Mathf.InverseLerp(p_minSlope, p_maxSlope, p_currentSlope);

        float slopeEffortSpeed = Mathf.Lerp(1, m_maxSlopeEffortSpeed, effortProgress);

        m_animator.SetFloat("SlopeEffort", slopeEffortSpeed);

        //m_effortPose.SetDirectBlendValue(1);
    }

	public void SetAnimations(Vector3 p_horizontalVelocity, float p_runSpeed, float p_sprintSpeed, float p_walkSpeed)
    {
        m_animator.SetBool("IsGrounded", m_collisionController.IsGrounded());

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
        //m_animator.SetFloat("ForwardMovement", 0.5f);

        m_animator.SetBool("IsSliding", m_collisionController.m_playSlideAnimation);

        //m_animator.SetFloat("ForwardMovement", 0);
    }

    public void SetModelRotation(Vector3 p_horizontalDirection, Vector3 p_groundNormal, float p_slopeFacingDirection, float p_currentSlope, float p_minSlope, float p_maxSlope)
    {
        Vector3 localXAxis = Vector3.Cross(transform.forward, Vector3.up);
        Vector3 forwardRotation = Vector3.ProjectOnPlane(p_groundNormal, localXAxis);
        Quaternion upwardSlopeOffset = Quaternion.FromToRotation(Vector3.up, forwardRotation);
        Vector3 targetMoveAmount = (upwardSlopeOffset * transform.forward);

        m_headEffector.rotation = Quaternion.RotateTowards(m_headEffector.rotation, Quaternion.LookRotation(targetMoveAmount, Vector3.up), m_headEffectorRotateSpeed * Time.deltaTime);

        float angle = 0;

        float rotationProgress = Mathf.InverseLerp(m_minMaxModelRotationAngle.x, m_minMaxModelRotationAngle.y, p_currentSlope);

        if (p_slopeFacingDirection > 0)
		{
			float currentRotation = Mathf.Lerp(0, m_maxUpwardSlopeModelRotation, rotationProgress) * targetMoveAmount.y;

            angle = currentRotation;
		}

        //m_modelTransform.localRotation = Quaternion.AngleAxis(angle, Vector3.right);
        m_modelTransform.localRotation = Quaternion.RotateTowards(m_modelTransform.localRotation, Quaternion.AngleAxis(angle, Vector3.right), m_maxModelRotationSpeed * Time.deltaTime);


        //m_animator.SetLayerWeight(1, rotationProgress * targetMoveAmount.y);

        /*
        Vector3 horizontalCross = Vector3.Cross(m_slopeTransform.right, transform.right);
        m_horizontalSlopeBlend.SetBlendValue(Mathf.Abs(horizontalCross.y) * Mathf.Sign(-horizontalCross.y));

        m_modelTransform.localRotation = Quaternion.RotateTowards(m_modelTransform.localRotation, Quaternion.AngleAxis(angle, Vector3.forward), m_maxModelRotationSpeed * Time.deltaTime);
        */
    }
}