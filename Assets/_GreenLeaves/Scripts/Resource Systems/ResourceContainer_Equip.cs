using UnityEngine;


[CreateAssetMenu(fileName = "ResourceData_Equip_", menuName = "ScriptableObjects/ResourceData_Equip", order = 0)]
public class ResourceContainer_Equip : ResourceContainer
{
    public bool m_toggleEquipmentOnPlayer;

    public enum ToolType { Axe, Canteen, Torch, Bow, ClimbingAxe, Knife }
    public ToolType m_currentToolType;
    public override void UseItem(Inventory_Icon p_currentIcon)
    {
        Inventory_ItemUsage.Instance.EquipNewItem(p_currentIcon, m_currentToolType);
    }

    public override GameObject DropObject(Inventory_Icon p_icon, Vector3 p_pos, Quaternion p_rot, bool p_returnObject = true)
    {
        Crafting_Table.CraftingTable.m_toolComponents.EnableToolResource(m_currentToolType);
        return null;
    }


}
