using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipe_", menuName = "ScriptableObjects/CraftingRecipe/Repair", order = 0)]
public class Crafting_Recipe_Tool_Repair : Crafting_Recipe
{

    public override int GetToolDurability(List<Crafting_Table.Crafting_ItemsContainer> p_givenItems)
    {
        int returnAmount = m_startingToolDurability;
        foreach (Crafting_Table.Crafting_ItemsContainer item in p_givenItems)
        {
            if (item.m_itemData.m_resourceData.m_resourceName == m_craftedItem.m_resourceData.m_resourceName)
            {
                returnAmount += item.m_relatedDurabilityIcon.m_durabilityAmount;
            }
        }
        return returnAmount;
    }
}
