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
            GameObject hitObj;
            if (CheckForPickup(out hitObj))
            {
                Pickup(hitObj);
            }
        }
        if (Input.GetKeyDown(m_toggleMenu))
        {
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
        Inventory_2DMenu.Instance.AddItemToInventory(newItem,amount);
    }

    /// <summary>
    /// Drops the object into the physical game world
    /// </summary>
    public void DropObject(Inventory_Icon p_droppedIcon)
    {
        if(p_droppedIcon == Inventory_ItemUsage.Instance.m_currentEquippedIcon)
        {
            Inventory_ItemUsage.Instance.UnEquipCurrent();
        }
        p_droppedIcon.m_itemData.DropObject(p_droppedIcon,transform.position + transform.forward * 2, Quaternion.identity).GetComponent<Resource_Pickup>().m_resourceAmount = p_droppedIcon.m_currentResourceAmount;
    }


    private void OnDrawGizmos()
    {
        if (!m_debugging) return;

        Gizmos.color = m_debugColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, m_spherecastRadius);
    }
    #endregion


    public Transform m_equipmentParent;
    public GameObject m_currentEquipped;
    #region Player Equipment Placement
    public void EquipItem(GameObject p_equipment)
    {
        p_equipment.transform.parent = m_equipmentParent;
        p_equipment.transform.localPosition = Vector3.zero;
        p_equipment.transform.localRotation = Quaternion.identity;
        if(p_equipment.GetComponent<Rigidbody>()!= null)
        {
            p_equipment.GetComponent<Rigidbody>().isKinematic = true;
            p_equipment.GetComponent<Collider>().enabled = false;
        }
        m_currentEquipped = p_equipment;
    }

    public void UnEquipItem()
    {
        if (m_currentEquipped == null) return;
        if (m_currentEquipped.GetComponent<Rigidbody>() != null)
        {
            m_currentEquipped.GetComponent<Rigidbody>().isKinematic = true;
            m_currentEquipped.GetComponent<Collider>().enabled = false;
        }
        m_currentEquipped.transform.parent = ObjectPooler.Instance.transform;
        m_currentEquipped = null;
    }
    #endregion
}



