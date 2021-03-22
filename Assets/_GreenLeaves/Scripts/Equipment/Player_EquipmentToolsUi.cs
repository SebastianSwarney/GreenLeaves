using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_EquipmentToolsUi : MonoBehaviour
{
    public static Player_EquipmentToolsUi Instance;
    // Start is called before the first frame update

    public GameObject m_canteenUI;
    public UnityEngine.UI.Image m_canteenFill;
    public GameObject m_standardDurabilityUI;
    public UnityEngine.UI.Text m_durabilityAmount;
    void Awake()
    {
        Instance = this;
    }

    public void AdjustCanteenUI(float p_newPercent)
    {
        if (m_canteenFill == null) return;
        m_canteenFill.fillAmount = p_newPercent;
    }
    public void ToggleCanteenUi(bool p_newState)
    {
        if (m_canteenFill == null) return;
        m_canteenUI.SetActive(p_newState);
    }

    public void ToggleDurabilityUI(bool p_newState)
    {
        m_standardDurabilityUI.SetActive(p_newState);
    }
    public void SetDurabilityText(float p_amount)
    {
        m_durabilityAmount.text = "x" + p_amount.ToString();
    }
}
