using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A crafting recipe that utilizes the canteen specifically.<br/>
/// Has a variable for the required water amount, to make sure the canteen has atleast that much.
/// </summary>
[CreateAssetMenu(fileName = "CraftingRecipe_Canteen_", menuName = "ScriptableObjects/CraftingRecipe/Canteen", order = 0)]
public class Crafting_Recipe_CanteenRecipes : Crafting_Recipe
{

    [Header("Canteen Specific")]
    public Crafting_Table.Crafting_ItemsContainer m_canteenRecipe;
    public float m_requiredCanteenAmount;
    public override bool CanCraft(List<Crafting_Table.Crafting_ItemsContainer> p_givenItems, out int p_matchAmount)
    {
        bool p_hasCanteen = false;
        Inventory_Icon_Durability canteenIcon = null;
        foreach (Crafting_Table.Crafting_ItemsContainer given in p_givenItems)
        {
            if (given.m_itemData.m_resourceData.m_resourceName == "Canteen")
            {
                p_hasCanteen = true;
                canteenIcon = given.m_relatedDurabilityIcon;
                break;
            }
        }
        if (!p_hasCanteen)
        {
            p_matchAmount = 0;
            return false;
        }


        if (canteenIcon.m_durabilityAmount < m_requiredCanteenAmount)
        {
            p_matchAmount = 0;
            return false;
        }
        return base.CanCraft(p_givenItems, out p_matchAmount);
    }
    public override List<Crafting_Table.Crafting_ItemsContainer> GetRecipe()
    {
        List<Crafting_Table.Crafting_ItemsContainer> returnRecipe = new List<Crafting_Table.Crafting_ItemsContainer>(base.GetRecipe());

        returnRecipe.Add(m_canteenRecipe);
        return returnRecipe;

    }
}
