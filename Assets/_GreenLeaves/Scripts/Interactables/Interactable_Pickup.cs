using UnityEngine;

/// <summary>
/// The class used for items that can be picked up<br/>
/// through the interaction menu.
/// </summary>
public class Interactable_Pickup : Interactable
{
    public Resource_Pickup m_pickupable;


    /// <summary>
    /// Called when the left button is pressed while the interaction menu is open.<br/>
    /// Calls the inventory to add the item to the menu inventory.
    /// </summary>
    public override void LeftButtonPressed()
    {
        ResourceData newData = new ResourceData(m_pickupable.m_resourceInfo.m_resourceData);
        int amount = m_pickupable.m_resourceAmount;
        Inventory_2DMenu.Instance.PickupItem(gameObject, amount);
        if (DisableMenu())
        {
            Interactable_Manager.Instance.HideButtonMenu(this, true);
        }
    }


    /// <summary>
    /// This function is used to determine whether the current interaction<br/>
    /// will hide the menu. <br/><br/>
    /// It's use was to prevent the menu from closing for the renewable items <br/>
    /// (which inherit from this), as soon as they are pressed, as they may still <br/>
    /// contain resources.
    /// </summary>

    public virtual bool DisableMenu()
    {
        return true;
    }


}
