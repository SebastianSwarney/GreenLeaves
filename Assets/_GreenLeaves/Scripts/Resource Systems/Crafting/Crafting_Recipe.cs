using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipe_", menuName = "ScriptableObjects/CraftingRecipe", order = 0)]

///The scriptable object for the recipies that can be crafted<br/>
///The logic for the crafting comparision is done here
public class Crafting_Recipe : ScriptableObject
{
    public List<Crafting_Table.Crafting_ItemsContainer> m_recipe;
    [Header("Crafted Item")]
    public ResourceContainer m_craftedItem;
    public int m_craftedAmount;

    [Header("Tool Stuff")]
    public bool m_isToolRecipe;
    public int m_startingToolDurability;

    /// <summary>
    /// Called from the crafting table everytime a resource is placed/removed from the tables <br/>
    /// This determines whether this recipe can be used, by comparing the requirements to the given list<br/>
    /// Returns true if it can be crafted | False if it cant be crafted.
    /// </summary>
    public bool CanCraft(List<Crafting_Table.Crafting_ItemsContainer> p_givenItems, out int p_matchAmount)
    {
        p_matchAmount = 0;
        foreach (Crafting_Table.Crafting_ItemsContainer required in m_recipe)
        {
            int requiredAmount = required.m_itemAmount;
            foreach(Crafting_Table.Crafting_ItemsContainer given in p_givenItems)
            {
                if(given.m_itemData.m_resourceData.m_resourceName == required.m_itemData.m_resourceData.m_resourceName)
                {
                    requiredAmount -= given.m_itemAmount;
                    p_matchAmount++;
                }
            }

            if(requiredAmount > 0) return false;
            

        }
        return true;
    }
}
