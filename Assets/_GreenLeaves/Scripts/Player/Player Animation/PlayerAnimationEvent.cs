using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
	private PlayerVisualsController m_playerVisuals;
	private PlayerController m_playerController;

	private void Start()
	{
		m_playerVisuals = GetComponentInParent<PlayerVisualsController>();
		m_playerController = GetComponentInParent<PlayerController>();
	}

	public void FootStep(int p_stepSide)
	{
		SoundEmitter_FootSteps.Instance.PlaySound();

		m_playerVisuals.OnFootUpdate(p_stepSide);
	}

	public void SwingStart()
	{

	}

	public void SwingEnd()
	{
		m_playerController.SwingEnd();
	}

	public void PickHit()
	{
		m_playerController.OnPickHit();
	}
}
