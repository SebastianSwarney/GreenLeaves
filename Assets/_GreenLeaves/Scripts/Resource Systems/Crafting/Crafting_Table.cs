﻿using System.Collections.Generic;
using UnityEngine;

public class Crafting_Table : MonoBehaviour
{
    public static Crafting_Table Instance;
    public Crafting_ToolComponents m_toolComponents;


    private Crafting_Recipe m_currentRecipe;
    public List<Crafting_Recipe> m_allRecipes;

    public GameObject m_craftButton;

    [System.Serializable]
    public class Crafting_ItemsContainer
    {
        public ResourceContainer m_itemData;
        public int m_itemAmount;
    }

    public List<Inventory_Icon> m_iconsOnTable;

    private void Awake()
    {
        Instance = this;
    }


    public void AddIconToTable(Inventory_Icon p_currentIcon)
    {
        m_iconsOnTable.Add(p_currentIcon);
        m_craftButton.SetActive(CheckTableRecipe());
    }

    public void RemoveIconFromTable(Inventory_Icon p_currentIcon)
    {
        if (m_iconsOnTable.Contains(p_currentIcon))
        {
            p_currentIcon.m_inCraftingTable = false;
            m_iconsOnTable.Remove(p_currentIcon);
        }
        m_craftButton.SetActive(CheckTableRecipe());
    }

    public void ClearIconList()
    {
        foreach (Inventory_Icon cur in m_iconsOnTable)
        {
            cur.RemoveIconFromCraftingTableOnClose();
        }
        m_iconsOnTable.Clear();
    }

    /// <summary>
    /// <para>Uses the icons currently on the table, and determines whether a recipe <br/>
    /// can be used.</para>
    /// Note: Doesnt craft the recipe in this function
    /// </summary>
    public bool CheckTableRecipe()
    {
        List<Crafting_ItemsContainer> currentItems = GatherCurrentItems();

        foreach (Crafting_Recipe recip in m_allRecipes)
        {
            if (recip.CanCraft(currentItems))
            {
                m_currentRecipe = recip;
                return true;
            }
        }
        m_currentRecipe = null;
        return false;
    }

    private List<Crafting_ItemsContainer> GatherCurrentItems()
    {
        List<Crafting_ItemsContainer> currentItems = new List<Crafting_ItemsContainer>();

        bool newIcon = true;
        foreach (Inventory_Icon icon in m_iconsOnTable)
        {
            newIcon = true;
            foreach (Crafting_ItemsContainer curr in currentItems)
            {
                if (curr.m_itemData.m_resourceData.m_resourceName == icon.m_itemData.m_resourceData.m_resourceName)
                {
                    curr.m_itemAmount += icon.m_currentResourceAmount;
                    newIcon = false;
                    break;
                }
            }
            if (!newIcon) continue;
            Crafting_ItemsContainer newContainer = new Crafting_ItemsContainer();
            newContainer.m_itemAmount = icon.m_currentResourceAmount;
            newContainer.m_itemData = icon.m_itemData;
            currentItems.Add(newContainer);
        }
        return currentItems;
    }


    /// <summary>
    /// Called from the button. Used to perform the logic of crafting the item <br/>
    /// and adding it to the inventory.
    /// </summary>
    public void CraftItem()
    {
        Inventory_2DMenu.Instance.CraftNewIcon(m_currentRecipe);
        AdjustResources();

    }

    public void AdjustResources()
    {
        Crafting_Recipe currentRecipe = new Crafting_Recipe();
        currentRecipe.m_recipe = new List<Crafting_ItemsContainer>();
        foreach (Crafting_ItemsContainer cont in m_currentRecipe.m_recipe)
        {
            Crafting_ItemsContainer newItem = new Crafting_ItemsContainer();
            newItem.m_itemData = cont.m_itemData;
            newItem.m_itemAmount = cont.m_itemAmount;
            currentRecipe.m_recipe.Add(newItem);
        }

        List<Inventory_Icon> removeIcons = new List<Inventory_Icon>();

        foreach(Inventory_Icon currentIcon in m_iconsOnTable)
        {
            if (currentIcon.m_currentResourceAmount <= 0) continue;
            if (currentIcon.GetComponent<Inventory_Icon_ToolResource>() != null)
            {
                foreach (Crafting_ItemsContainer cont in currentRecipe.m_recipe)
                {
                    if (cont.m_itemData.m_resourceData.m_resourceName == currentIcon.m_itemData.m_resourceData.m_resourceName)
                    {
                        removeIcons.Add(currentIcon);
                    }
                }
                continue;
            }
            foreach(Crafting_ItemsContainer cont in currentRecipe.m_recipe)
            {
                if (cont.m_itemAmount <= 0) continue;
                if(cont.m_itemData.m_resourceData.m_resourceName == currentIcon.m_itemData.m_resourceData.m_resourceName)
                {
                    currentIcon.m_currentResourceAmount -= cont.m_itemAmount;
                    cont.m_itemAmount = 0;
                    if (currentIcon.m_currentResourceAmount <= 0)
                    {
                        if (currentIcon.m_currentResourceAmount < 0)
                        {
                            cont.m_itemAmount = Mathf.Abs(currentIcon.m_currentResourceAmount);
                            currentIcon.m_currentResourceAmount = 0;
                        }
                        removeIcons.Add(currentIcon);
                    }
                }
            }
        }

        for (int i = 0; i < removeIcons.Count; i++)
        {
            m_iconsOnTable.Remove(removeIcons[i]);
            removeIcons[i].RemoveIcon();
        }

        foreach (Inventory_Icon icon in m_iconsOnTable)
        {
            icon.UpdateIconNumber();
        }
        m_craftButton.SetActive(CheckTableRecipe());
    }
}

