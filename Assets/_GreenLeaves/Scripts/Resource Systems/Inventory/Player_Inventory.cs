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
    [Header("Pickup Raycast")]
    public KeyCode m_pickupKeycode;
    public LayerMask m_interactableLayer;
    public float m_spherecastRadius;
    public KeyCode m_toggleMenu;

    /// <summary>
    /// Will be used to delay pickup while in an animation
    /// </summary>
    private bool m_isPickingUp;

    /// <summary>
    /// The detection type for determining which item is picked up first in the radius
    /// </summary>
    public enum PickupType { ClosestToPlayer, ClosestToPlayerForward }
    [Tooltip("Changes how the system decides which item to pickup first if there is more than 1 item")]
    public PickupType m_currentPickupType;

    [Header("Equipable Tools")]
    public Player_EquipmentUse m_currentEquipedTool;
    public Player_EquipmentUse m_axeTool, m_canteenTool, m_torchTool, m_bootsTool, m_climbingAxeTool;

    [Header("Debugging")]
    public bool m_debugging;
    public Color m_debugColor = Color.red;

    

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (m_isPickingUp) return;
        if (Input.GetKeyDown(m_pickupKeycode))
        {
            Debug.Log("Item Pickup Input Here", this);
            GameObject hitObj;
            if (CheckForPickup(out hitObj))
            {
                Pickup(hitObj);
            }
        }
        if (Input.GetKeyDown(m_toggleMenu))
        {
            Debug.Log("Inv Toggle Input Here", this);
            Inventory_2DMenu.Instance.ToggleInventory();
        }
    }

    #region 3d pickup & drop Logic

    /// <summary>
    /// Checks around the player for items that can be picked up, and returns true if there is an availabe item.
    /// The logic for which item to pickup is in this function
    /// Returns p_detectItem as the item that can be picked up
    /// </summary>
    public bool CheckForPickup(out GameObject p_detectedItem)
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, m_spherecastRadius, m_interactableLayer);

        if (cols.Length > 0)
        {
            GameObject closest = null;
            for (int i = 0; i < cols.Length; i++)
            {
                if(cols[i].GetComponent<Resource_Pickup>() == null)
                {
                    continue;
                }
                if (!cols[i].GetComponent<Resource_Pickup>().m_canPickup)
                {
                    continue;
                }
                if (i == 0)
                {
                    closest = cols[0].transform.gameObject;
                    continue;
                }
                if(closest == null)
                {
                    closest = cols[i].gameObject;
                }

                switch (m_currentPickupType)
                {
                    case PickupType.ClosestToPlayer:
                        ///Use closest item to player
                        if (Vector3.Distance(closest.transform.position, transform.position) > Vector3.Distance(cols[i].transform.position, transform.position))
                        {
                            closest = cols[i].transform.gameObject;
                        }
                        break;

                    case PickupType.ClosestToPlayerForward:

                        ///Use closest to forward
                        if (Vector3.Angle(transform.forward, closest.transform.position - transform.position) > Vector3.Angle(transform.forward, cols[i].transform.position - transform.position))
                        {
                            closest = cols[i].transform.gameObject;
                        }
                        break;
                }

            }

            p_detectedItem = closest;
            if (p_detectedItem != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        p_detectedItem = null;
        return false;
    }

    /// <summary>
    /// Adds the item to the inventory
    /// This is where the animation should thereotically be called
    /// </summary>
    private void Pickup(GameObject newItem)
    {
        ResourceData newData = new ResourceData(newItem.GetComponent<Resource_Pickup>().m_resourceInfo.m_resourceData);
        int amount = newItem.GetComponent<Resource_Pickup>().m_resourceAmount;
        Inventory_2DMenu.Instance.PickupItem(newItem,amount);
    }

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
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (!m_debugging) return;

        Gizmos.color = m_debugColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, m_spherecastRadius);
    }
    #endregion



    #region Player Equipment Equiping
    public void EquipItem(Inventory_Icon p_icon, ResourceContainer_Equip.ToolType p_toolType)
    {
        UnEquipCurrentTool();
        switch (p_toolType)
        {
            case ResourceContainer_Equip.ToolType.Axe:
                m_currentEquipedTool = m_axeTool;
                break;

            case ResourceContainer_Equip.ToolType.Canteen:
                m_currentEquipedTool = m_canteenTool;
                break;

            case ResourceContainer_Equip.ToolType.Torch:
                m_currentEquipedTool = m_torchTool;
                break;

            case ResourceContainer_Equip.ToolType.Boots:
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

        m_currentEquipedTool.UnEquipObject();
        m_currentEquipedTool = null;
    }
    #endregion
}



