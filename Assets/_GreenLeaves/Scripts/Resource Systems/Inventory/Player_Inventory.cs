using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is specifically for detecting and picking up the resources in the 3d world
/// This interacts with the Inventory_2DMenu singleton, and adds the items to the inventory
/// Additionally, this class is called to drop the items from the 2d item menu
/// </summary>
public class Player_Inventory : MonoBehaviour
{

    public static Player_Inventory Instance;
    [HideInInspector]
    public bool m_canOpenMenu = true;

    public KeyCode m_toggleMenu, m_secondaryToggle;

    [Header("Equipable Tools")]
    public Player_EquipmentUse m_currentEquipedTool;
    public Player_EquipmentUse m_knifeTool,m_axeTool, m_canteenTool, m_torchTool, m_bootsTool, m_climbingAxeTool;

    [Header("Debugging")]
    public bool m_debugging;
    public Color m_debugColor = Color.red;
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (Building_PlayerPlacement.Instance.m_isPlacing || Daytime_WaitMenu.Instance.m_isWaiting || Interactable_Readable_Menu.Instance.m_isOpen) return;
        if (Input.GetKeyDown(m_toggleMenu) || Input.GetKeyDown(m_secondaryToggle))
        {
            if (m_canOpenMenu)
            {
                Inventory_2DMenu.Instance.ToggleInventory(true);
            }
        }
    }

    public void ToggleOpenability(bool p_newState)
    {
        m_canOpenMenu = p_newState;
    }


    #region 3d pickup & drop Logic

    /// <summary>
    /// Drops the object into the physical game world
    /// </summary>
    public void DropObject(Inventory_Icon p_droppedIcon, bool p_dropInWorld)
    {
        if (m_currentEquipedTool != null)
        {
            if (p_droppedIcon == m_currentEquipedTool.m_linkedIcon)
            {
                Inventory_ItemUsage.Instance.UnEquipCurrent();
            }
        }
        if (p_dropInWorld)
        {
            GameObject newDropped = p_droppedIcon.m_itemData.DropObject(p_droppedIcon, transform.position + transform.forward * 2, Quaternion.identity);
            if (newDropped != null)
            {
                newDropped.GetComponent<Resource_Pickup>().m_resourceAmount = p_droppedIcon.m_currentResourceAmount;
                if (newDropped.GetComponentInChildren<Manipulation_HitObject>())
                {
                    newDropped.GetComponentInChildren<Manipulation_HitObject>().ObjectRespawn();
                }
                Map_LoadingManager.Instance.GetCurrentOccupiedMapArea().m_allResources.Add(newDropped);
            }
        }
    }
    #endregion



    #region Player Equipment Equiping
    public void EquipItem(Inventory_Icon p_icon, ResourceContainer_Equip.ToolType p_toolType)
    {
        UnEquipCurrentTool();
        switch (p_toolType)
        {
            case ResourceContainer_Equip.ToolType.Knife:
                m_currentEquipedTool = m_knifeTool;
                break;
            case ResourceContainer_Equip.ToolType.Axe:
                m_currentEquipedTool = m_axeTool;
                break;

            case ResourceContainer_Equip.ToolType.Canteen:
                m_currentEquipedTool = m_canteenTool;
                break;

            case ResourceContainer_Equip.ToolType.Torch:
                m_currentEquipedTool = m_torchTool;
                break;

            case ResourceContainer_Equip.ToolType.Bow:
                m_currentEquipedTool = m_bootsTool;
                break;

            case ResourceContainer_Equip.ToolType.ClimbingAxe:
                m_currentEquipedTool = m_climbingAxeTool;
                break;
        }

        Inventory_Icon_Durability newIcon = p_icon.GetComponent<Inventory_Icon_Durability>();
        m_currentEquipedTool.EquipObject(newIcon);
        newIcon.UpdateIconNumber();
    }

    public void UnEquipCurrentTool()
    {
        if (m_currentEquipedTool == null) return;
        Debug.Log("Unequip Current");
        m_currentEquipedTool.UnEquipObject();
        m_currentEquipedTool = null;
        Inventory_2DMenu.Instance.m_currentEquippedTool = null;
    }
    #endregion
}



