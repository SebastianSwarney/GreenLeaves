using UnityEngine;

/// <summary>
/// Similar to the interactable_pickup script,<br/> 
/// this one is used on the renewable resources.
/// </summary>
public class Interactable_Pickup_Renewable : Interactable_Pickup
{
    public Resource_Pickup_Renewable m_currentRenewable;

    /// <summary>
    /// Only disables the menu if the resource cannot be picked up anymore.
    /// </summary>
    /// <returns></returns>
    public override bool DisableMenu()
    {
        m_canBeInteractedWith = m_currentRenewable.m_canPickup;
        return !m_currentRenewable.m_canPickup;
    }
}
