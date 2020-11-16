using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsController : MonoBehaviour
{
    public static PlayerStatsController Instance;
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

    [Header("Hunger Properties")]

    public float m_hungerMax;
    public float m_maxDrainMultiplier;
    public Gradient m_hungerColorGradient;
    public float m_hungerDepletionRate;

    public Image m_hungerImage;

    public int m_hungerSegments;
    public GameObject m_hungerSegmentObject;
    public Transform m_hungerSegmentRoot;

    public Image m_hungerGainPreviewImage;

    public float m_currentHunger;
    private float m_currentDrainMultiplier;
    private List<Image> m_hungerSegmentImages;

    private bool m_isPreviewing;

    public float m_healthGainTest;

    private void Start()
    {
        Instance = this;
        SetEnergyToMax();

        m_hungerSegmentImages = new List<Image>();

        m_hungerSegmentImages.Add(m_hungerSegmentObject.GetComponent<Image>());

        for (int i = 0; i < m_hungerSegments - 1; i++)
		{
            GameObject newObject = Instantiate(m_hungerSegmentObject, m_hungerSegmentRoot);
            m_hungerSegmentImages.Add(newObject.GetComponent<Image>());
        }
    }

    private void Update()
    {
        ReplenishSecondaryEnergy();

        DrainHunger();
        DrawHugnerSegments();
    }

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

    private void DrainHunger()
	{
        float depletionSpeed = m_hungerMax / m_hungerDepletionRate;

        m_currentHunger -= depletionSpeed * Time.deltaTime;

        m_hungerImage.fillAmount = m_currentHunger / m_hungerMax;

		if (m_currentHunger <= 0)
		{
            m_currentHunger = 0;
		}
    }

    private void DrawHugnerSegments()
	{
        float segmentSpace = m_hungerMax / m_hungerSegmentImages.Count;

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
        }
    }

    private void SetHungerSegment(Image p_targetImage, float p_progress)
	{
        p_targetImage.color = m_hungerColorGradient.Evaluate(p_progress);
        m_currentDrainMultiplier = Mathf.Lerp(m_maxDrainMultiplier, 1f, p_progress);
    }

    private void ResetHungerSegment(Image p_targetImage)
	{
        p_targetImage.color = Color.white;
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

        p_currentEnergy -= (depletionSpeed * Time.deltaTime) * m_currentDrainMultiplier;

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

    public void PreviewStatGain(ResourceContainer_Cosume.TypeOfCosumption.ConsumeType p_typeOfCosumption, float p_amount, bool p_increasePastMax = false)
    {
		switch (p_typeOfCosumption)
		{
			case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Stamina:



				break;

			case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Hunger:

                float targetAddAmount = m_currentHunger + p_amount;

                m_hungerGainPreviewImage.fillAmount = targetAddAmount / m_hungerMax;


                break;

			case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Energy:



				break;
		}
	}

    private void ResetPreviews()
	{
        m_hungerGainPreviewImage.fillAmount = 0;
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
                Debug.Log("Refill Stamina");

                break;

                #endregion


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
}
