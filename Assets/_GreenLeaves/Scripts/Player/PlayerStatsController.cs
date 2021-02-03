using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class PlayerStatsController : MonoBehaviour
{
    public static PlayerStatsController Instance;
    [Header("Main Energy Properties")]
    public float m_mainEnergyMax;
    public Image m_mainEnergyImage;

    public float m_mainEnergyDepletionRate;

    public float m_energyGainPerHour;

    public float m_currentMainEnergy;

    private bool m_usingMainEnergy;
    public UI_ShakeElement m_mainEnergyShake;
    public float m_mainEnergyStartShakingAmount;

    [Header("Secondary Energy Properties")]
    public float m_secondaryEnergyMax;
    public Image m_secondaryEnergyImage;
    public float m_secondaryEnergyReplenishTime;
    public float m_secondaryEnergyReplenishWaitTime;

    private float m_secondaryEnergyReplenishWaitTimer;

    public float m_currentSecondaryEnergy;
    public UI_ShakeElement m_secondaryEnergyShake;
    public float m_secondaryEnergyStartShakingAmount;


    [Header("Hunger Properties")]
    public float m_hungerMax;
    public float m_maxDrainMultiplier;

    public float m_hungerDepletionRate;

    public Image m_hungerImage;

    public int m_hungerSegments;
    public float m_currentHunger;
    private float m_currentDrainMultiplier;

    public UI_ShakeElement m_hungerShake;
    public float m_hungerStartShakingAmount;

    [Header("Hunger Visual Properties")]
    public Gradient m_hungerColorGradient;

    [Header("Stat Drain Action Properties")]
    public float m_sprintSecondaryEnergyDepletionTime;
    public float m_sprintEnergyDepletionTime;

    public bool m_pauseStatDrain;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetEnergyToMax();

        m_pauseStatDrain = false;
    }

    private void Update()
    {
        ReplenishSecondaryEnergy();

        PassiveDrainStat(m_mainEnergyMax, m_mainEnergyDepletionRate, ref m_currentMainEnergy, m_mainEnergyImage, m_currentDrainMultiplier);
        PassiveDrainStat(m_hungerMax, m_hungerDepletionRate, ref m_currentHunger, m_hungerImage);

        DrawHungerSegments();
        UpdateUIShake();
    }

    #region Drain Functions
    public void EquipmentStatDrain(float p_mainEnergyAmount, float p_staminaAmount)
    {
        if (m_usingMainEnergy)
        {
            DepleteEnergySingle(p_mainEnergyAmount);
        }
        else
        {
            DepleteEnergySingle(p_staminaAmount);
        }
    }

    public void SprintEnergyDrain()
    {
        if (m_usingMainEnergy)
        {
            DepleteEnergy(m_sprintEnergyDepletionTime);
        }
        else
        {
            DepleteEnergy(m_sprintSecondaryEnergyDepletionTime);
        }
    }

    private void PassiveDrainStat(float p_statMax, float p_statDepletionRate, ref float p_currentStat, Image p_statImage, float p_drainMultiplier = 1)
    {
        if (!m_pauseStatDrain)
        {
            float depletionSpeed = p_statMax / p_statDepletionRate;

            p_currentStat -= (depletionSpeed * Time.deltaTime) * p_drainMultiplier;

            p_statImage.fillAmount = p_currentStat / p_statMax;

            if (p_currentStat <= 0)
            {
                p_currentStat = 0;
            }
        }
    }

    public void DepleteEnergy(float p_depletionAmount)
    {
        if (CheckEnergy(p_depletionAmount, m_secondaryEnergyImage, ref m_currentSecondaryEnergy, m_secondaryEnergyMax))
        {
            m_secondaryEnergyReplenishWaitTimer = 0;
            m_usingMainEnergy = false;
            return;
        }

        if (CheckEnergy(p_depletionAmount, m_mainEnergyImage, ref m_currentMainEnergy, m_mainEnergyMax))
        {
            m_secondaryEnergyReplenishWaitTimer = 0;
            m_usingMainEnergy = true;
            return;
        }
    }

    public void DepleteEnergySingle(float p_depletionAmount)
    {
        if (CheckEnergySingle(p_depletionAmount, m_secondaryEnergyImage, ref m_currentSecondaryEnergy, m_secondaryEnergyMax))
        {
            m_secondaryEnergyReplenishWaitTimer = 0;
            m_usingMainEnergy = false;
            return;
        }

        if (CheckEnergySingle(p_depletionAmount, m_mainEnergyImage, ref m_currentMainEnergy, m_mainEnergyMax))
        {
            m_secondaryEnergyReplenishWaitTimer = 0;
            m_usingMainEnergy = true;
            return;
        }
    }
    #endregion

    #region Public Booleans
    public bool HasEnergy()
    {
        if (m_currentSecondaryEnergy > 0 || m_currentMainEnergy > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsFullMainEnergy(out int p_depletedEnergyAmount)
    {
        p_depletedEnergyAmount = 0;

        if (m_currentMainEnergy >= m_mainEnergyMax)
        {
            return true;
        }

        p_depletedEnergyAmount = (int)(m_mainEnergyMax - m_currentMainEnergy);

        return false;
    }
    #endregion

    #region Hunger Code


    private void DrawHungerSegments()
    {
        /*float segmentSpace = m_hungerMax / m_hungerSegmentImages.Count;

        for (int i = 0; i < m_hungerSegmentImages.Count; i++)
        {
            float segmentMin = i * segmentSpace;
            float segmentMax = segmentMin + segmentSpace;

            if (m_currentHunger >= segmentMin && m_currentHunger <= segmentMax)
            {
                SetHungerSegment(m_hungerSegmentImages[i], (float)i / ((float)m_hungerSegmentImages.Count - 1f));
            }
            else
            {
                ResetHungerSegment(m_hungerSegmentImages[i]);
            }
        }*/




        SetHungerSegment(m_hungerImage, ((m_currentHunger / m_hungerMax)));
    }

    private void SetHungerSegment(Image p_targetImage, float p_progress)
    {
        p_targetImage.color = m_hungerColorGradient.Evaluate(p_progress);
        m_currentDrainMultiplier = (int)Mathf.Lerp(m_maxDrainMultiplier, 1f, p_progress);


    }

    private void ResetHungerSegment(Image p_targetImage)
    {
        p_targetImage.color = Color.white;
    }
    #endregion

    private void SetEnergyToMax()
    {
        m_currentMainEnergy = m_mainEnergyMax;
        m_currentSecondaryEnergy = m_secondaryEnergyMax;
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

        p_currentEnergy -= (depletionSpeed * Time.deltaTime) * m_currentDrainMultiplier;

        p_image.fillAmount = p_currentEnergy / p_maxEnergy;

        if (p_currentEnergy > 0)
        {
            return true;
        }

        p_currentEnergy = 0;

        return false;
    }

    private bool CheckEnergySingle(float p_depletionAmount, Image p_image, ref float p_currentEnergy, float p_maxEnergy)
    {
        p_currentEnergy -= (p_depletionAmount) * m_currentDrainMultiplier;

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

    public void AddStatsFromCampfire(float p_hoursWaited)
    {
        m_currentMainEnergy += p_hoursWaited * m_energyGainPerHour;

        if (m_currentMainEnergy > m_mainEnergyMax)
        {
            m_currentMainEnergy = m_mainEnergyMax;
        }
    }

    public void AddAmount(ResourceContainer_Cosume.TypeOfCosumption.ConsumeType p_typeOfCosumption, float p_amount, bool p_increasePastMax = false)
    {
        switch (p_typeOfCosumption)
        {
            #region Energy
            case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Energy:

                m_currentMainEnergy += p_amount;

                if (!p_increasePastMax)
                {
                    if (m_currentMainEnergy > m_mainEnergyMax)
                    {
                        m_currentMainEnergy = m_mainEnergyMax;
                    }
                }

                break;
            #endregion

            #region Hunger
            case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Hunger:

                m_currentHunger += p_amount;

                if (!p_increasePastMax)
                {
                    if (m_currentHunger > m_hungerMax)
                    {
                        m_currentHunger = m_hungerMax;
                    }
                }

                break;

            #endregion

            #region Stamina
            case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Stamina:

                m_currentSecondaryEnergy += p_amount;

                if (!p_increasePastMax)
                {
                    if (m_currentSecondaryEnergy > m_secondaryEnergyMax)
                    {
                        m_currentSecondaryEnergy = m_secondaryEnergyMax;
                    }
                }

                break;

                #endregion
        }
    }

    public float GetCurrentStat(ResourceContainer_Cosume.TypeOfCosumption.ConsumeType p_typeOfStat)
    {
        switch (p_typeOfStat)
        {
            case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Energy:
                return m_currentMainEnergy;
                break;
            case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Hunger:
                return m_currentHunger;
                break;
            case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Stamina:
                return m_currentSecondaryEnergy;
                break;
        }
        Debug.Log("Error in stats controller | No matching stat type for: " + p_typeOfStat, this);
        return 0;
    }

    public float GetMaxStat(ResourceContainer_Cosume.TypeOfCosumption.ConsumeType p_typeOfStat)
    {
        switch (p_typeOfStat)
        {
            case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Energy:
                return m_mainEnergyMax;
                break;
            case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Hunger:
                return m_hungerMax;
                break;
            case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Stamina:
                return m_secondaryEnergyMax;
                break;
        }
        Debug.Log("Error in stats controller | No matching stat type for: " + p_typeOfStat, this);
        return 0;
    }

    public void UpdateUIShake()
    {
        m_mainEnergyShake.UpdateShakeAmount(Mathf.Lerp(1, 0, (m_currentMainEnergy / m_mainEnergyStartShakingAmount)));
        m_secondaryEnergyShake.UpdateShakeAmount(Mathf.Lerp(1, 0, (m_currentSecondaryEnergy / m_secondaryEnergyStartShakingAmount)));
        m_hungerShake.UpdateShakeAmount(Mathf.Lerp(1, 0, (m_currentHunger / m_hungerStartShakingAmount)));
    }
}
