using UnityEngine;

/// <summary>
/// Used on the 3d object that is representing the objects. 
/// Simply holds a reference to the ResourceContainer datatype that this object
/// is related to.
/// </summary>
public class Resource_Pickup : MonoBehaviour
{
    /// <summary>
    /// The resource data that the object holds. Is a scriptable object
    /// </summary>
    public ResourceContainer m_resourceInfo;
    public bool m_canPickup = true;
    public int m_resourceAmount;

    public GenericWorldEvent m_resourcePickedUpEvent, m_objectSpawned;

    public void ResetResourceAmount()
    {
        m_resourceAmount = 1;
        m_objectSpawned.Invoke();
    }
    public void NewResource()
    {
        TogglePickup(true);
        m_objectSpawned.Invoke();
    }
    public virtual void PickupResource()
    {
        m_resourceAmount = 0;
        TogglePickup(false);
        if (Map_LoadingManager.Instance.GetCurrentOccupiedMapArea().m_allResources.Contains(gameObject))
        {
            Map_LoadingManager.Instance.GetCurrentOccupiedMapArea().m_allResources.Remove(gameObject);
        }
    }

    public virtual void TogglePickup(bool p_newState)
    {
        m_canPickup = p_newState;
        if (!p_newState)
        {
            m_resourcePickedUpEvent.Invoke();
        }
    }

    public void ReturnToPool()
    {
        ObjectPooler.Instance.ReturnToPool(gameObject);
    }
}
