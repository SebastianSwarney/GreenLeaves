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
        m_txtEatAmount.text = m_currentEatAmount.ToString();
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
    }


    public bool CanAddIconToEatingArea(Inventory_Icon p_currentIcon)
    {
        if (!p_currentIcon.m_itemData.m_isEdible) return false;

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

        m_subtractButton.SetActive(false);
        m_addButton.SetActive(true);

        m_eatMenu.SetActive(true);
        return true;
    }






}
