using System.Collections.Generic;
using UnityEngine;

public class Crafting_Table : MonoBehaviour
{
    public static Crafting_Table CraftingTable, CookingTable;
    public Crafting_ToolComponents m_toolComponents;

    public bool m_isCraftingTable;
    private Crafting_Recipe m_currentRecipe;
    public List<Crafting_Recipe> m_allRecipes;
    private List<int> m_matchingAmount = new List<int>();

    public GameObject m_craftButton;

    public UnityEngine.UI.Text m_craftedItemText;

    [System.Serializable]
    public class Crafting_ItemsContainer
    {
        public Inventory_Icon m_relatedIcon;
        public Inventory_Icon_Durability m_relatedDurabilityIcon;
        public ResourceContainer m_itemData;
        public int m_itemAmount;
    }

    public List<Inventory_Icon> m_iconsOnTable;

    private void Awake()
    {
        if (m_isCraftingTable)
        {
            CraftingTable = this;
        }
        else
        {
            CookingTable = this;
        }
        for (int i = 0; i < m_allRecipes.Count; i++)
        {
            m_matchingAmount.Add(0);
        }
    }

    private void OnEnable()
    {
        m_craftButton.SetActive(CheckTableRecipe());
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
            if (m_isCraftingTable)
            {
                p_currentIcon.m_inCraftingTable = false;
            }
            else
            {
                p_currentIcon.m_inCookingTable = false;
            }
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
        m_currentRecipe = null;
        for (int i = 0; i < m_matchingAmount.Count; i++)
        {
            m_matchingAmount[i] = 0;
        }
        List<Crafting_ItemsContainer> currentItems = GatherCurrentItems();

        int newAmount = 0;
        for (int i = 0; i < m_allRecipes.Count; i++)
        {
            newAmount = 0;
            if (m_allRecipes[i].CanCraft(currentItems, out newAmount))
            {
                m_matchingAmount[i] = newAmount;
                //                m_currentRecipe = recip;

            }
        }

        int currentHighest = 0;
        for (int i = 0; i < m_allRecipes.Count; i++)
        {
            if (m_matchingAmount[i] == 0) continue;
            if (currentHighest < m_matchingAmount[i])
            {
                currentHighest = m_matchingAmount[i];
                m_currentRecipe = m_allRecipes[i];
            }
        }
        if (m_currentRecipe != null)
        {
            m_craftedItemText.text = m_currentRecipe.m_craftedItem.m_resourceData.m_resourceName;
        }
        m_craftedItemText.transform.parent.gameObject.SetActive(m_currentRecipe != null);
        return m_currentRecipe != null;
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
            newContainer.m_relatedIcon = icon;
            if (icon.GetComponent<Inventory_Icon_Durability>() != null)
            {
                newContainer.m_relatedDurabilityIcon = icon.GetComponent<Inventory_Icon_Durability>();
            }

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
        RecipeBook.Instance.UnlockRecipe(m_currentRecipe.m_recipeBookIndex);
        Crafting_Recipe tempRecipe = m_currentRecipe;
        if (tempRecipe.m_isBuilding)
        {
            tempRecipe.m_craftedItem.UseItem(null);
        }
        else
        {
            Inventory_2DMenu.Instance.CraftNewIcon(tempRecipe, GatherCurrentItems());
        }
        AdjustResources();

    }

    public void AdjustResources()
    {
        Crafting_Recipe currentRecipe = new Crafting_Recipe();
        currentRecipe.m_recipe = new List<Crafting_ItemsContainer>();
        foreach (Crafting_ItemsContainer cont in m_currentRecipe.GetRecipe())
        {
            Crafting_ItemsContainer newItem = new Crafting_ItemsContainer();
            newItem.m_itemData = cont.m_itemData;
            newItem.m_itemAmount = cont.m_itemAmount;
            currentRecipe.m_recipe.Add(newItem);
        }

        List<Inventory_Icon> removeIcons = new List<Inventory_Icon>();

        foreach (Inventory_Icon currentIcon in m_iconsOnTable)
        {
            if (currentIcon.m_currentResourceAmount <= 0) continue;
            if (currentIcon.GetComponent<Inventory_Icon_ToolResource>() != null)
            {
                foreach (Crafting_ItemsContainer cont in currentRecipe.GetRecipe())
                {
                    if (cont.m_itemData.m_resourceData.m_resourceName == currentIcon.m_itemData.m_resourceData.m_resourceName)
                    {
                        removeIcons.Add(currentIcon);
                    }
                }
                continue;
            }
            foreach (Crafting_ItemsContainer cont in currentRecipe.GetRecipe())
            {
                if (cont.m_itemAmount <= 0) continue;
                if (cont.m_itemData.m_resourceData.m_resourceName == currentIcon.m_itemData.m_resourceData.m_resourceName)
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

