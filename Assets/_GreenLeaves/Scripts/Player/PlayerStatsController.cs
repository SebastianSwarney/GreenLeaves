using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class PlayerStatsController : MonoBehaviour
{
    public static PlayerStatsController Instance;

    #region Energy Properties
    [FoldoutGroup("Energy")]
    public float m_energyDepletionRate;
    [FoldoutGroup("Energy")]
    public float m_energyGainPerHour;
    [FoldoutGroup("Energy")]
    public Image m_energyImage;
    [FoldoutGroup("Energy")]
    public UI_ShakeElement m_mainEnergyShake;
    [FoldoutGroup("Energy")]
    public float m_energyStartShakingAmount;

    [HideInInspector]
    public float m_currentEnergy;
    #endregion

    #region Stamina Properties
    [FoldoutGroup("Stamina")]
    public float m_staminaReplenishTime;
    [FoldoutGroup("Stamina")]
    public Image m_staminaImage;
    [FoldoutGroup("Stamina")]
    public UI_ShakeElement m_secondaryEnergyShake;
    [FoldoutGroup("Stamina")]
    public float m_staminaStartShakingAmount;

    [HideInInspector]
    public float m_currentStamina;
    #endregion

    #region Hunger Properties
    [FoldoutGroup("Hunger")]
    public float m_maxDrainMultiplier;
    [FoldoutGroup("Hunger")]
    public float m_hungerDepletionRate;

    [FoldoutGroup("Hunger")]
    public Image m_hungerImage;
    [FoldoutGroup("Hunger")]
    public UI_ShakeElement m_hungerShake;
    [FoldoutGroup("Hunger")]
    public float m_hungerStartShakingAmount;
    [FoldoutGroup("Hunger")]
    public Gradient m_hungerColorGradient;

    [FoldoutGroup("Hunger")]
    public float m_minCampfireHungerRestore = 25;

    private float m_currentDrainMultiplier;
    [HideInInspector]
    public float m_currentHunger;
    #endregion

    #region Stat Action Properties
    [FoldoutGroup("Stat Actions")]
    public float m_sprintStaminaDepletionTime;
    [FoldoutGroup("Stat Actions")]
    public float m_sprintEnergyDepletionTime;
    [FoldoutGroup("Stat Actions")]
    public Vector2 m_minMaxFallDistance;
    [FoldoutGroup("Stat Actions")]
    public Vector2 m_minMaxFallEnergyLossPercent;
    [FoldoutGroup("Stat Actions")]
    public Vector2 m_minMaxFallStaminaLossPercent;
    [FoldoutGroup("Stat Actions")]
    public int m_killHeight;
    #endregion

    [HideInInspector]
    public bool m_pauseStatDrain;

    private PlayerController m_playerController;
    private PlayerVisualsController m_playerVisuals;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetEnergyToMax();
        m_pauseStatDrain = false;

        m_playerController = GetComponent<PlayerController>();
        m_playerVisuals = GetComponent<PlayerVisualsController>();
    }

    private void Update()
    {
        m_currentDrainMultiplier = (int)Mathf.Lerp(m_maxDrainMultiplier, 1f, m_currentHunger / 100);

        RunPassiveEnergyDrain();
        RunPassiveHungerDrain();
        ReplenishSecondaryEnergy();

        if (!HasEnergy())
        {
            m_playerController.PassOut();
            m_playerVisuals.PauseTiredness();
        }

        if (m_currentStamina < 5)
        {
            m_playerVisuals.RunTiredness(m_currentEnergy / 100);
        }
        else
        {
            m_playerVisuals.PauseTiredness();
        }

        UpdateUI();
    }

	#region General Drain Functions
	public void DrainEnergyPercentage(float p_inputAmountToDrain)
    {
        p_inputAmountToDrain *= 0.01f;

        float energyToDeplete = 100 * p_inputAmountToDrain;

        DrainEnergySingleInstance(energyToDeplete);
    }

    private void DrainEnergySingleInstance(float p_inputAmountToDrain)
    {
        float totalAmountToDrain = p_inputAmountToDrain * m_currentDrainMultiplier;

        if (m_currentStamina > 0)
        {
            m_currentStamina -= totalAmountToDrain;

            if (m_currentStamina <= 0)
            {
                float spillOverAmount = Mathf.Abs(m_currentStamina);
                m_currentEnergy -= spillOverAmount;
            }
        }
        else if (m_currentEnergy > 0)
        {
            m_currentEnergy -= totalAmountToDrain;
        }
    }

    private void DrainEnergyOverTime(float p_inputAmoutnToDrain)
    {
        float depletionSpeed = 100 / p_inputAmoutnToDrain;
        float totalAmountToDrain = (depletionSpeed * Time.deltaTime) * m_currentDrainMultiplier;

        if (m_currentStamina > 0)
        {
            m_currentStamina -= totalAmountToDrain;
        }
        else if (m_currentEnergy > 0)
        {
            m_currentEnergy -= totalAmountToDrain;
        }
    }
    #endregion

    #region Drain Actions
    private void RunPassiveEnergyDrain()
    {
        if (!m_pauseStatDrain && !PauseStatsInMenu())
        {
            float depletionSpeed = 100 / m_energyDepletionRate;

            m_currentEnergy -= (depletionSpeed * Time.deltaTime) * m_currentDrainMultiplier;

            if (m_currentEnergy <= 0)
            {
                m_currentEnergy = 0;
            }
        }
    }

    private void RunPassiveHungerDrain()
    {
        if (!m_pauseStatDrain && !PauseStatsInMenu())
        {
            float depletionSpeed = 100 / m_hungerDepletionRate;

            m_currentHunger -= (depletionSpeed * Time.deltaTime);

            if (m_currentHunger <= 0)
            {
                m_currentHunger = 0;
            }
        }
    }

    public void EquipmentStatDrain(float p_mainEnergyAmount, float p_staminaAmount)
    {
        if (UsingMainEnergy())
        {
            DrainEnergySingleInstance(p_mainEnergyAmount);
        }
        else
        {
            DrainEnergySingleInstance(p_staminaAmount);
        }
    }

    public void SprintEnergyDrain()
    {
        if (UsingMainEnergy())
        {
            DrainEnergyOverTime(m_sprintEnergyDepletionTime);
        }
        else
        {
            DrainEnergyOverTime(m_sprintStaminaDepletionTime);
        }
    }

    public void CalculateFallDamage(float p_distanceFallen)
    {
        if (p_distanceFallen < m_minMaxFallDistance.x) return;

        float fallPercent = Mathf.InverseLerp(m_minMaxFallDistance.x, m_minMaxFallDistance.y, p_distanceFallen);

        float damageAmount = Mathf.Lerp(m_minMaxFallStaminaLossPercent.x, m_minMaxFallStaminaLossPercent.y, fallPercent);

        if (UsingMainEnergy())
        {
            damageAmount = Mathf.Lerp(m_minMaxFallEnergyLossPercent.x, m_minMaxFallEnergyLossPercent.y, fallPercent);
        }

		if (p_distanceFallen >= m_killHeight)
		{
            damageAmount = 200;
		}

        DrainEnergyPercentage(damageAmount);
    }
    #endregion

    #region Stat Adding
    public void SetEnergyToMax()
    {
        m_currentEnergy = 100;
        m_currentStamina = 100;
    }

    private void ReplenishSecondaryEnergy()
    {
        if (m_currentEnergy <= 0)
        {
            m_currentStamina = 0;
            return;
        }

        if (!m_pauseStatDrain && !PauseStatsInMenu())
        {
            if (m_currentStamina < 100)
            {
                float replenishSpeed = 100 / m_staminaReplenishTime;
                m_currentStamina += replenishSpeed * Time.deltaTime;
            }
        }
    }


    public void AddStatsFromCampfire(float p_hoursWaited)
    {
        if (m_currentHunger < m_minCampfireHungerRestore)
        {
            m_currentHunger = m_minCampfireHungerRestore;
        }
        float gainAmount = p_hoursWaited * m_energyGainPerHour;
        AddAmount(ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Energy, gainAmount);
        AddAmount(ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Stamina, 1000);
    }

    public void AddAmount(ResourceContainer_Cosume.TypeOfCosumption.ConsumeType p_typeOfCosumption, float p_amount, bool p_increasePastMax = false)
    {
        switch (p_typeOfCosumption)
        {
            #region Energy
            case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Energy:

                m_currentEnergy += p_amount;

                if (!p_increasePastMax)
                {
                    if (m_currentEnergy > 100)
                    {
                        m_currentEnergy = 100;
                    }
                }

                break;
            #endregion

            #region Hunger
            case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Hunger:

                m_currentHunger += p_amount;

                if (!p_increasePastMax)
                {
                    if (m_currentHunger > 100)
                    {
                        m_currentHunger = 100;
                    }
                }
                break;

            #endregion

            #region Stamina
            case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Stamina:

                m_currentStamina += p_amount;

                if (!p_increasePastMax)
                {
                    if (m_currentStamina > 100)
                    {
                        m_currentStamina = 100;
                    }
                }
                break;

                #endregion
        }

        UpdateUI();
    }
    #endregion

    public bool HasEnergy()
    {
        if (m_currentEnergy > 0)
        {
            return true;
        }

        return false;
    }

    public bool UsingMainEnergy()
    {
        if (m_currentStamina <= 0)
        {
            return true;
        }

        return false;
    }

    #region UI
    private void UpdateUI()
    {
        m_hungerImage.fillAmount = m_currentHunger / 100;
        m_hungerImage.color = m_hungerColorGradient.Evaluate(m_currentHunger / 100);

        m_staminaImage.fillAmount = m_currentStamina / 100;
        m_energyImage.fillAmount = m_currentEnergy / 100;

        UpdateUIShake();
    }

    public void UpdateUIShake()
    {
        m_mainEnergyShake.UpdateShakeAmount(Mathf.Lerp(1, 0, (m_currentEnergy / m_energyStartShakingAmount)));
        m_secondaryEnergyShake.UpdateShakeAmount(Mathf.Lerp(1, 0, (m_currentStamina / m_staminaStartShakingAmount)));
        m_hungerShake.UpdateShakeAmount(Mathf.Lerp(1, 0, (m_currentHunger / m_hungerStartShakingAmount)));
    }
    #endregion

    private bool PauseStatsInMenu()
	{
		if (PlayerUIManager.Instance.m_isPaused)
		{
            return true;
		}

		if (m_playerController.m_isCredits)
		{
            return true;
		}

        return false;
	}
}
