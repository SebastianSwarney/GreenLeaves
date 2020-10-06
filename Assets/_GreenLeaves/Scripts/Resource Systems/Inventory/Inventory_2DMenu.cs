using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_2DMenu : MonoBehaviour
{
    public static Inventory_2DMenu Instance;

    public Player_Inventory m_playerInventory;
    public BackpackInventory m_playerBackpack;

    public GameObject m_canvasObject;
    public GameObject m_inventorySlotPrefab;

    public Inventory_Icon m_currentSelectedIcon;

    [Header("Inventory")]
    public BackpackInventory m_backpack;
    public enum MenuType { None, Resource, Equipment }
    public MenuType m_currentMenuType;


    [Header("UI Elements")]
    public Inventory_Grid m_inventoryGrid;
    public Transform m_gameIconsParent;
    public enum RotationType { Right, Down, Left, Up}
    public RotationType m_currentRotationType = RotationType.Left;

    

    private bool m_isOpen;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        m_inventoryGrid.Initialize();
    }

    #region Inventory Toggle
    public void ToggleInventory()
    {
        if (m_isOpen)
        {
            m_isOpen = false;
            CloseInventoryMenu();
            PlayerInputToggle.Instance.ToggleInput(true);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
        else
        {
            PlayerInputToggle.Instance.ToggleInput(false);
            m_isOpen = true;
            OpenInventoryMenu();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    public void OpenInventoryMenu()
    {
        m_currentSelectedIcon = null;
        m_currentMenuType = MenuType.None;
        m_canvasObject.SetActive(true);
        UpdateInventoryUI();
    }

    /// <summary>
    /// Updates the UI for the Inventory Menu
    /// </summary>
    public void UpdateInventoryUI()
    {
        switch (m_currentMenuType)
        {
            case MenuType.None:
                break;
            case MenuType.Equipment:
                break;
            case MenuType.Resource:
                break;
        }
    }

    public void CloseInventoryMenu()
    {
        m_canvasObject.SetActive(false);
    }
    #endregion

    /// <summary>
    /// Adds the item to the inventory & spawns a new icon for it
    /// </summary>
    public void AddItemToInventory(GameObject p_pickedUp)
    {

        ResourceData pickedUpResource = p_pickedUp.GetComponent<Resource_Pickup>().m_myData;

        m_currentRotationType = RotationType.Left;
        if (m_inventoryGrid.CanAddToRow(pickedUpResource, m_currentRotationType))
        {
            m_backpack.m_itemsInBackpack.Add(pickedUpResource);
            m_inventoryGrid.AddNewIcon(pickedUpResource, m_currentRotationType, m_gameIconsParent);
        }
        else
        {
            Debug.Log("Cannot Pickup");
        }
    }

    /// <summary>
    /// Drops the item into the world
    /// </summary>
    public void DropItem()
    {
        m_playerInventory.DropObject(m_currentSelectedIcon.m_itemData.m_resourcePrefab);
        ObjectPooler.Instance.ReturnToPool(m_currentSelectedIcon.gameObject);
        m_currentSelectedIcon = null;
        m_currentMenuType = MenuType.None;
        UpdateInventoryUI();
    }

    public void EquipItem()
    {
        Debug.Log("Equip Item");
    }


    public void IconTappedOn(Inventory_Icon p_tappedOn)
    {
        m_currentSelectedIcon = p_tappedOn;
    }

}


[System.Serializable]
public class BackpackInventory
{
    public List<ResourceData> m_itemsInBackpack;
}

[System.Serializable]
public class ResourceData
{
    public string m_resourceName;
    public string m_resourceDetails;
    public Sprite m_resourceSprite;
    public GameObject m_resourcePrefab;
    public GameObject m_resourceIconPrefab;
    public Vector2Int m_inventoryWeight;

    public ResourceData(ResourceData p_newData = null)
    {
        if (p_newData != null)
        {
            m_resourceName = p_newData.m_resourceName;
            m_resourceSprite = p_newData.m_resourceSprite;
            m_resourcePrefab = p_newData.m_resourcePrefab;
            m_resourceIconPrefab = p_newData.m_resourceIconPrefab;
            m_inventoryWeight = p_newData.m_inventoryWeight;
        }
    }
}




