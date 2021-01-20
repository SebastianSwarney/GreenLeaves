using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_EquipmentToolsUi : MonoBehaviour
{
    public static Player_EquipmentToolsUi Instance;
    // Start is called before the first frame update

    public GameObject m_canteenUI;
    public UnityEngine.UI.Image m_canteenFill;
    void Awake()
    {
        Instance = this;
    }

    public void AdjustCanteenUI(float p_newPercent)
    {
        m_canteenFill.fillAmount = p_newPercent;
    }
    public void ToggleCanteenUi(bool p_newState)
    {
        m_canteenUI.SetActive(p_newState);
    }
}
