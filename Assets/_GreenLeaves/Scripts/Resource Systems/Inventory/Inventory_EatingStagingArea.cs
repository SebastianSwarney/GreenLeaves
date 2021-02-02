using UnityEngine;
using UnityEngine.UI;

public class Inventory_EatingStagingArea : MonoBehaviour
{
    public Inventory_Icon m_currentEdible;
    public GameObject m_eatMenu;
    public Text m_txtEatAmount;
    private int m_currentEatAmount;

    public GameObject m_addButton, m_subtractButton;
    public Transform m_eatingArea;

    public GameObject m_addHunger, m_addStamina, m_addEnergy;
    public Text m_addHungerText, m_addStaminaText, m_addEnergyText;

    public ResourceContainer_Cosume m_edibleData;

    public void AdjustEatAmount(int p_dir)
    {
        m_currentEatAmount += p_dir;
        if (m_currentEatAmount == m_currentEdible.m_currentResourceAmount)
        {
            m_addButton.SetActive(false);
            if (m_currentEatAmount != 1)
            {
                m_subtractButton.SetActive(true);
            }
        }
        else if (m_currentEatAmount == 1)
        {
            m_subtractButton.SetActive(false);
            if(m_currentEdible.m_currentResourceAmount > 1)
            {
                m_addButton.SetActive(true);
            }
        }
        else
        {
            m_subtractButton.SetActive(true);
            m_addButton.SetActive(true);
        }
        UpdateStatsText();
        m_txtEatAmount.text = m_currentEatAmount.ToString();
    }
    
    private void UpdateStatsText()
    {

        if (m_edibleData == null) return;
        foreach(ResourceContainer_Cosume.TypeOfCosumption ty in m_edibleData.m_consumeType)
        {
            switch (ty.m_typeOfConsume)
            {
                case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Energy:
                    m_addEnergy.SetActive(true);
                    m_addEnergyText.text = "+" + m_currentEatAmount * ty.m_replenishAmount;
                    break;

                case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Hunger:
                    m_addHunger.SetActive(true);
                    m_addHungerText.text = "+" + m_currentEatAmount * ty.m_replenishAmount;
                    break;

                case ResourceContainer_Cosume.TypeOfCosumption.ConsumeType.Stamina:
                    m_addStamina.SetActive(true);
                    m_addStaminaText.text = "+" + m_currentEatAmount * ty.m_replenishAmount;
                    break;

            }
        }
    }
    public void EatEdibles()
    {

        m_currentEdible.m_currentResourceAmount -= m_currentEatAmount;
        m_currentEdible.UpdateIconNumber();
        for (int i = 0; i < m_currentEatAmount; i++)
        {
            m_currentEdible.m_itemData.UseItem(m_currentEdible);
        }
        if (m_currentEdible.m_currentResourceAmount == 0)
        {
            Inventory_2DMenu.Instance.RemoveSingleIcon(m_currentEdible);
            m_currentEdible = null;
            m_eatMenu.SetActive(false);
        }
        else
        {
            m_currentEatAmount = 1;
            m_subtractButton.SetActive(false);
            if (m_currentEdible.m_currentResourceAmount == 1)
            {
                m_addButton.SetActive(false);
            }
            else
            {
                m_addButton.SetActive(true);
            }
            m_txtEatAmount.text = "1";
        }

    }

    public void IconRemoved()
    {
        m_currentEdible = null;
        m_eatMenu.SetActive(false);

        m_addStamina.SetActive(false);
        m_addHunger.SetActive(false);
        m_addEnergy.SetActive(false);
        m_edibleData = null;
    }


    public bool CanAddIconToEatingArea(Inventory_Icon p_currentIcon)
    {
        if (!p_currentIcon.m_itemData.m_isEdible) return false;

        m_edibleData = p_currentIcon.m_itemData.AddToEdibleTable();

        if (m_currentEdible != null)
        {
            m_currentEdible.m_wasInEatingArea = false;
            m_currentEdible.m_inEatingArea = false;
            Inventory_2DMenu.Instance.CanAddToInventory(m_currentEdible, m_currentEdible.m_rotatedDir);

        }

        m_currentEdible = p_currentIcon;
        m_currentEdible.transform.localPosition = m_eatingArea.localPosition;
        m_currentEdible.m_startingCoordPos = m_currentEdible.transform.localPosition;
        m_currentEatAmount = 1;
        m_txtEatAmount.text = m_currentEatAmount.ToString();

        UpdateStatsText();

        m_subtractButton.SetActive(false);
        m_addButton.SetActive(true);

        m_eatMenu.SetActive(true);
        return true;
    }






}
