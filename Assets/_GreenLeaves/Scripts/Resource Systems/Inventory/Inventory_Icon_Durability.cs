using UnityEngine.UI;
using UnityEngine;

public class Inventory_Icon_Durability : Inventory_Icon
{
    [Header("Durability Exclusive")]
    public Text m_durabilityText;

    public int m_durabilityAmount;

    public void UpdateDurability(int p_newDurabilityAmount)
    {
        m_durabilityAmount = p_newDurabilityAmount;
        m_durabilityText.text = "x" + m_durabilityAmount.ToString();
    }
}
