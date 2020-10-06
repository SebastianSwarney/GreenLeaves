using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory : MonoBehaviour
{


    [Header("Pickup Raycast")]
    public KeyCode m_pickupKeycode;
    private bool m_isPickingUp;
    public LayerMask m_interactableLayer;
    //public float m_capsuleDetectHeight, m_capsuleCollectForward, m_capsuleCollectRadius;
    public float m_spherecastRadius;
    public KeyCode m_toggleMenu;

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


        /*Physics.OverlapCapsule(transform.position + transform.forward * m_capsuleCollectForward + Vector3.up * m_capsuleDetectHeight / 2,
                            transform.position + transform.forward * m_capsuleCollectForward + Vector3.up * -m_capsuleDetectHeight / 2,
                            m_capsuleCollectRadius, m_interactableLayer);*/

        /*if (cols.Length > 0)
        {
            p_detectedItem = cols[0].gameObject;
            return true;
        }*/


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
        //Debug.LogError("Hit Object: " + newItem.name, newItem);
        ResourceData newData = new ResourceData(newItem.GetComponent<Resource_Pickup>().m_myData);
        Inventory_2DMenu.Instance.AddItemToInventory(newItem);

        /*
        ObjectPooler.Instance.ReturnToPool(newItem);*/

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
        /*Gizmos.DrawWireSphere((transform.forward * m_capsuleCollectForward) + Vector3.up *  (m_capsuleDetectHeight/2), m_capsuleCollectRadius);
        Gizmos.DrawWireSphere((transform.forward * m_capsuleCollectForward) - Vector3.up * (m_capsuleDetectHeight / 2), m_capsuleCollectRadius);*/
    }
    #endregion
}



