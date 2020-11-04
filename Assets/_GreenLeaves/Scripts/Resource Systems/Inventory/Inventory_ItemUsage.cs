using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_ItemUsage : MonoBehaviour
{

    public static Inventory_ItemUsage Instance;


    private void Awake()
    {
        Instance = this;
    }

    #region Item Usage Functions

    public void ConsumeItem(Inventory_Icon p_currentIcon, List<ResourceContainer_Cosume.TypeOfCosumption> p_currentStats)
    {
        foreach (ResourceContainer_Cosume.TypeOfCosumption consume in p_currentStats)
        {
            if (EnergyController.Instance != null)
            {
                EnergyController.Instance.AddAmount(consume.m_typeOfConsume, consume.m_replenishAmount, consume.m_increasePastAmount);
            }
            else
            {
                Debug.LogError("Energy Controller singleton is not initailized");
            }
        }
        Inventory_2DMenu.Instance.ItemUsed(p_currentIcon);
    }
    

    public void EquipNewItem(Inventory_Icon p_currentIcon, ResourceContainer_Equip.ToolType p_toolType)
    {
        Player_Inventory.Instance.EquipItem(p_currentIcon,p_toolType);
        Inventory_2DMenu.Instance.ChangeSelectedButtonText("Unequip");
    }

    public void UnEquipCurrent()
    {
        Player_Inventory.Instance.UnEquipCurrentTool();
        Inventory_2DMenu.Instance.ChangeSelectedButtonText("Equip");
    }
    #endregion

}
