
public class Interactable_Pickup_ToolComponent : Interactable
{

    public ResourceContainer_Equip.ToolType m_toolType;

    public GenericWorldEvent m_objectPickedUp;
    /// <summary>
    /// Called when the left button is pressed while the interaction menu is open.<br/>
    /// Calls the inventory to add the item to the menu inventory.
    /// </summary>
    public override void LeftButtonPressed()
    {
        if (m_toolType == ResourceContainer_Equip.ToolType.Map)
        {
            PlayerUIManager.Instance.m_mapUnlocked = true;
        }
        else
        {
            Inventory_2DMenu.Instance.m_toolComponents.EnableToolResource(m_toolType);
        }
        m_objectPickedUp.Invoke();
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
        m_canBeInteractedWith = false;
        return true;
    }
}
