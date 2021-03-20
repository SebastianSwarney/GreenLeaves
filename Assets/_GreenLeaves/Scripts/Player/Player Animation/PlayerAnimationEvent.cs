using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
	private PlayerVisualsController m_playerVisuals;

	private void Start()
	{
		m_playerVisuals = GetComponentInParent<PlayerVisualsController>();
	}

	public void FootStep(int p_stepSide)
	{
		SoundEmitter_FootSteps.Instance.PlaySound();

		m_playerVisuals.OnFootUpdate(p_stepSide);
	}
}
