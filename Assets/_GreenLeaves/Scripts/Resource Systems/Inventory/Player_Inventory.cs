using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory : MonoBehaviour
{


    [Header("Pickup Raycast")]
    public KeyCode m_pickupKeycode;
    private bool m_isPickingUp;
    public LayerMask m_interactableLayer;
    public float m_capsuleDetectHeight, m_capsuleCollectForward, m_capsuleCollectRadius;

    public KeyCode m_dropKeycode;

    [Header("Inventory")]
    public BackpackInventory m_backpack;

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
        if (Input.GetKeyDown(m_dropKeycode))
        {
            DropObject();
        }
    }

    public bool CheckForPickup(out GameObject p_detectedItem)
    {
        Collider[] cols = Physics.OverlapCapsule(transform.position + transform.forward * m_capsuleCollectForward + Vector3.up * m_capsuleDetectHeight / 2,
                                transform.position + transform.forward * m_capsuleCollectForward + Vector3.up * -m_capsuleDetectHeight / 2,
                                m_capsuleCollectRadius, m_interactableLayer);

        if (cols.Length > 0)
        {
            p_detectedItem = cols[0].gameObject;
            return true;
        }
        p_detectedItem = null;
        return false;
    }

    private void Pickup(GameObject newItem)
    {
        ResourceData newData = new ResourceData(newItem.GetComponent<Resource_Pickup>().m_myData);
        m_backpack.AddNewResource(newData);
        ObjectPooler.Instance.ReturnToPool(newItem);
    }
    private void DropObject(int p_backpackSlot = 0)
    {
        if (m_backpack.CanDropItem(p_backpackSlot))
        {
            ObjectPooler.Instance.NewObject(m_backpack.DropItem(p_backpackSlot), transform.position + transform.forward * m_capsuleCollectForward, Quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        if (!m_debugging) return;

        Gizmos.color = m_debugColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere((transform.forward * m_capsuleCollectForward) + Vector3.up *  (m_capsuleDetectHeight/2), m_capsuleCollectRadius);
        Gizmos.DrawWireSphere((transform.forward * m_capsuleCollectForward) - Vector3.up * (m_capsuleDetectHeight / 2), m_capsuleCollectRadius);
    }
}


[System.Serializable]
public class BackpackInventory
{
    public List<ResourceBackpackSlot> m_resourceBackpackSlots;
    public void AddNewResource(ResourceData p_newResource)
    {
        foreach (ResourceBackpackSlot slot in m_resourceBackpackSlots)
        {
            if (slot.m_heldResource.m_resourceName == p_newResource.m_resourceName)
            {
                slot.m_quantity++;
                return;
            }
        }

        m_resourceBackpackSlots.Add(new ResourceBackpackSlot(p_newResource));
    }

    public GameObject DropItem(ResourceBackpackSlot p_selectedSlot)
    {
        GameObject retrievedItem = p_selectedSlot.m_heldResource.m_resourceObject;
        if (p_selectedSlot.RemoveResource())
        {
            m_resourceBackpackSlots.Remove(p_selectedSlot);
        }
        return retrievedItem;
    }

    public GameObject DropItem(int p_selectedSlot)
    {
        GameObject retrievedItem = m_resourceBackpackSlots[p_selectedSlot].m_heldResource.m_resourceObject;
        if (m_resourceBackpackSlots[p_selectedSlot].RemoveResource())
        {
            m_resourceBackpackSlots.Remove(m_resourceBackpackSlots[p_selectedSlot]);
        }
        return retrievedItem;
    }

    public bool CanDropItem(int p_selectedSlot)
    {
        if (m_resourceBackpackSlots.Count > p_selectedSlot)
        {
            if(m_resourceBackpackSlots[p_selectedSlot] != null)
            {
                if (m_resourceBackpackSlots[p_selectedSlot].m_quantity > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }


}

[System.Serializable]
public class ResourceData
{
    public string m_resourceName;
    public string m_resourceDetails;
    public Sprite m_resourceSprite;
    public GameObject m_resourceObject;

    public ResourceData(ResourceData p_newData = null)
    {
        if (p_newData != null)
        {
            m_resourceName = p_newData.m_resourceName;
            m_resourceSprite = p_newData.m_resourceSprite;
            m_resourceObject = p_newData.m_resourceObject;
        }
    }
}

[System.Serializable]
public class ResourceBackpackSlot
{
    public ResourceData m_heldResource;
    public int m_quantity;

    public ResourceBackpackSlot(ResourceData p_heldResource)
    {
        m_heldResource = p_heldResource;
        m_quantity = 1;
    }

    public bool RemoveResource()
    {
        m_quantity -= 1;
        if (m_quantity == 0)
        {
            return true;
        }
        return false;
    }
}



