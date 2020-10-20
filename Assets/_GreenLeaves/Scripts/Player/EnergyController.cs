using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyController : MonoBehaviour
{
	[Header("Main Energy Properties")]
	public float m_mainEnergyMax;
	public Image m_mainEnergyImage;

	private float m_currentMainEnergy;

	[Header("Secondary Energy Properties")]
	public float m_secondaryEnergyMax;
	public Image m_secondaryEnergyImage;
	public float m_secondaryEnergyReplenishTime;
	public float m_secondaryEnergyReplenishWaitTime;

	private float m_secondaryEnergyReplenishWaitTimer;

	private float m_currentSecondaryEnergy;

	[HideInInspector]
	public bool m_hasEnergy;

	private void Start()
	{
		SetEnergyToMax();
	}

	private void Update()
	{
		ReplenishSecondaryEnergy();

		if (m_currentSecondaryEnergy > 0 || m_currentMainEnergy > 0)
		{
			m_hasEnergy = true;
		}
		else
		{
			m_hasEnergy = false;
		}

	}

	private void SetEnergyToMax()
	{
		m_currentMainEnergy = m_mainEnergyMax;
		m_currentSecondaryEnergy = m_secondaryEnergyMax;
	}

	public void DepleteEnergy(float p_depletionAmount)
	{
		if (CheckEnergy(p_depletionAmount, m_secondaryEnergyImage, ref m_currentSecondaryEnergy, m_secondaryEnergyMax))
		{
			m_secondaryEnergyReplenishWaitTimer = 0;
			return;
		}

		if (CheckEnergy(p_depletionAmount, m_mainEnergyImage, ref m_currentMainEnergy, m_mainEnergyMax))
		{
			return;
		}
	}

	private void ReplenishSecondaryEnergy()
	{
		m_secondaryEnergyReplenishWaitTimer += Time.deltaTime;

		if (m_secondaryEnergyReplenishWaitTimer >= m_secondaryEnergyReplenishWaitTime)
		{
			if (m_currentSecondaryEnergy < m_secondaryEnergyMax)
			{
				float replenishSpeed = m_secondaryEnergyMax / m_secondaryEnergyReplenishTime;
				m_currentSecondaryEnergy += replenishSpeed * Time.deltaTime;
				SetEnergyImage(m_secondaryEnergyImage, m_currentSecondaryEnergy, m_secondaryEnergyMax);
			}
		}
	}

	private bool CheckEnergy(float p_depletionAmount, Image p_image, ref float p_currentEnergy, float p_maxEnergy)
	{
		float depletionSpeed = m_secondaryEnergyMax / p_depletionAmount;

		p_currentEnergy -= depletionSpeed * Time.deltaTime;

		p_image.fillAmount = p_currentEnergy / p_maxEnergy;

		if (p_currentEnergy > 0)
		{
			return true;
		}

		p_currentEnergy = 0;

		return false;
	}

	private void SetEnergyImage(Image p_image, float p_currentEnergy, float p_maxEnergy)
	{
		p_image.fillAmount = p_currentEnergy / p_maxEnergy;
	}
}
