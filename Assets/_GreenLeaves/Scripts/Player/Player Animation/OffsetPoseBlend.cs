using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class OffsetPoseBlend : OffsetModifier
{
	[Tooltip("The OffsetPose components")]
	public OffsetPose positivePose, negativePose;

	public float m_blendSmoothTime;

	public float m_targetBlendValue;
	
	private float m_blendSmoothingVelocity;
	private float m_tiltF;
	private float m_blendValue;

	protected override void Start()
	{
		base.Start();
	}

	public void SetDirectBlendValue(float p_value)
	{
		positivePose.Apply(ik.solver, p_value);
	}

	public void SetBlendValue(float p_blendInput)
	{
		m_targetBlendValue = p_blendInput;
	}

	// Called by IKSolverFullBody before updating
	protected override void OnModifyOffset()
	{
		m_blendValue = Mathf.SmoothDamp(m_blendValue, m_targetBlendValue, ref m_blendSmoothingVelocity, m_blendSmoothTime);
		float clampedValue = Mathf.Clamp01(Mathf.Abs(m_blendValue));
		m_tiltF = clampedValue / 1f;

		if (m_blendValue < 0)
		{
			negativePose.Apply(ik.solver, m_tiltF);
		}
		else
		{
			positivePose.Apply(ik.solver, m_tiltF);
		}
	}
}
