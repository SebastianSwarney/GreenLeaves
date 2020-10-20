using UnityEngine;

public class Resource_Pickup_Renewable : Resource_Pickup
{
    public GameObject m_resourceObject;
    [Tooltip("Toggle to determine whether the source is emptied when used.")]
    public bool m_useUp;
    public override void PickupResource()
    {
        if(m_useUp)
        TogglePickup(false);
    }

    public void TogglePickup(bool p_newState)
    {
        m_canPickup = p_newState;
        m_resourceObject.SetActive(p_newState);
    }
}
