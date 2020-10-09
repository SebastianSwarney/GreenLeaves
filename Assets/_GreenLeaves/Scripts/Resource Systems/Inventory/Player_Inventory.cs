using UnityEngine;

/// <summary>
/// This class is specifically for detecting and picking up the resources in the 3d world
/// This interacts with the Inventory_2DMenu singleton, and adds the items to the inventory
/// Additionally, this class is called to drop the items from the 2d item menu
/// </summary>
public class Player_Inventory : MonoBehaviour
{


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
                if (i == 0)
                {
                    closest = cols[0].transform.gameObject;
                    continue;
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
            return true;
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
        Inventory_2DMenu.Instance.AddItemToInventory(newItem);

        ObjectPooler.Instance.ReturnToPool(newItem);
    }

    /// <summary>
    /// Drops the object into the physical game world
    /// </summary>
    public void DropObject(GameObject p_dropItem)
    {
        ObjectPooler.Instance.NewObject(p_dropItem, transform.position + transform.forward * 2, Quaternion.identity);
    }


    private void OnDrawGizmos()
    {
        if (!m_debugging) return;

        Gizmos.color = m_debugColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, m_spherecastRadius);
    }
    #endregion
}



