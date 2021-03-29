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

    

    public void EquipNewItem(Inventory_Icon p_currentIcon, ResourceContainer_Equip.ToolType p_toolType)
    {
        Player_Inventory.Instance.EquipItem(p_currentIcon,p_toolType);
    }


    public void ToolSlotItem(ResourceContainer_Equip.ToolType p_toolType, bool p_toggleType)
    {
        Player_Inventory.Instance.ToggleItemOnBackpack(p_toolType, p_toggleType);
    }
    public void UnEquipCurrent()
    {
        Player_Inventory.Instance.UnEquipCurrentTool();
    }
    #endregion

}
