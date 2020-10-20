using UnityEngine;

/// <summary>
/// Used on the 3d object that is representing the objects. 
/// Simply holds a reference to the ResourceContainer datatype that this object
/// is related to.
/// </summary>
public class Resource_Pickup : MonoBehaviour
{
    public ResourceContainer m_resourceInfo;
    public bool m_canPickup = true;
    public int m_resourceAmount;
    public virtual void PickupResource()
    {
        m_resourceAmount = 0;
        ObjectPooler.Instance.ReturnToPool(gameObject);
    }
}
