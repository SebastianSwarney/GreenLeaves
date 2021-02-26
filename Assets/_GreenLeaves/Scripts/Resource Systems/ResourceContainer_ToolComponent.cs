using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceData_ToolComponent_", menuName = "ScriptableObjects/ResourceData_ToolComponent", order = 0)]
public class ResourceContainer_ToolComponent : ResourceContainer
{

    //public enum ToolType { Axe, Canteen, Torch, Boots, ClimbingAxe }
    public ResourceContainer_Equip.ToolType m_currentToolType;


    public override GameObject DropObject(Inventory_Icon p_icon, Vector3 p_pos, Quaternion p_rot, bool p_returnObject = true)
    {
        Crafting_Table.CraftingTable.m_toolComponents.EnableToolResource(m_currentToolType);
        return null;
    }
}
