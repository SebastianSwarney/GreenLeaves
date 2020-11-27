using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A crafting recipe that utilizes the canteen specifically.<br/>
/// Has a variable for the required water amount, to make sure the canteen has atleast that much.
/// </summary>
[CreateAssetMenu(fileName = "CraftingRecipe_Canteen_", menuName = "ScriptableObjects/CraftingRecipe/Canteen", order = 0)]
public class Crafting_Recipe_CanteenRecipes : Crafting_Recipe
{
    public float m_requiredCanteenAmount;
    public override bool CanCraft(List<Crafting_Table.Crafting_ItemsContainer> p_givenItems, out int p_matchAmount)
    {
        bool p_hasCanteen = false;
        foreach(Crafting_Table.Crafting_ItemsContainer given in p_givenItems)
        {
            if(given.m_itemData.name == "Canteen")
            {
                p_hasCanteen = true;
                break;
            }
        }
        if (!p_hasCanteen)
        {
            p_matchAmount = 0;
            return false;
        }

        if(Player_EquipmentUse_Canteen.Instance == null)
        {
            Debug.Log("Tyler: Null Check for canteen here, fix this later");
            if (Player_Inventory.Instance.m_canteenTool == null)
            {
                p_matchAmount = 0;
                return false;
            }
            Player_EquipmentUse_Canteen.Instance = Player_Inventory.Instance.m_canteenTool.GetComponent<Player_EquipmentUse_Canteen>();
        }
        
        if(Player_EquipmentUse_Canteen.Instance.m_durability < m_requiredCanteenAmount)
        {
            p_matchAmount = 0;
            return false;
        }
        return base.CanCraft(p_givenItems, out p_matchAmount);
    }
}
