using UnityEngine;

public class Resource_Pickup_Renewable : Resource_Pickup
{
    [Tooltip("Toggle to determine whether the source is emptied when used.")]
    public bool m_useUp;
    public override void PickupResource()
    {
        if(m_useUp)
        TogglePickup(false);
    }
}
