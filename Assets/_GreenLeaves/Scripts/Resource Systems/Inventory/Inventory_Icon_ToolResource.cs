using UnityEngine;

/// <summary>
/// The icons that are used to hold the special resource used to craft the tools<br/>
/// This icon must be placed in the editor, as some other systems rely on references to this
/// </summary>
public class Inventory_Icon_ToolResource : Inventory_Icon
{
    public ResourceContainer_Equip.ToolType m_toolType;

    /// <summary>
    /// Used to hold the original placement of the tool icon in the belt
    /// </summary>
    public Vector3 m_toolBeltPlacement;
    private void Start()
    {
        m_startingCoordPos = transform.localPosition;
        m_toolBeltPlacement = transform.localPosition;
    }

    /// <summary>
    /// Instead of removing the icon, simply disable it
    /// </summary>
    public override void RemoveIcon()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// This reenables the icon, and places it back in it's starting position<br/>
    /// in the toolbelt
    /// </summary>
    public void ReEnableIcon()
    {
        Crafting_Table.CraftingTable.m_toolComponents.EnableToolResource(m_toolType);
    }

    /// <summary>
    /// This function is specifically used to remove the icon from the crafting table<br/>
    /// in the event that it is still there, but the player closes the crafting menu.
    /// </summary>
    public override void RemoveIconFromCraftingTableOnClose()
    {
        base.RemoveIconFromCraftingTableOnClose();
        ReEnableIcon();
    }
}
