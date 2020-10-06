using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory_2DMenu : MonoBehaviour
{
    public static Inventory_2DMenu Instance;

    public Player_Inventory m_playerInventory;

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

    /// <summary>
    /// Used to determine where the player can drop the item until it snaps back automatically
    /// </summary>
    public GameObject m_backpackImageUi;
    public enum RotationType { Right, Down, Left, Up }
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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckInventoryMouseDown();
        }
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
            Inventory_Icon newIcon = ObjectPooler.Instance.NewObject(pickedUpResource.m_resourceIconPrefab, Vector3.zero, Quaternion.identity).GetComponent<Inventory_Icon>();
            newIcon.transform.parent = m_gameIconsParent;

            Vector2Int placedPos;
            m_inventoryGrid.AddNewIcon(pickedUpResource, m_currentRotationType, newIcon, out placedPos);
            newIcon.m_previousPosition = placedPos;
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

    public void CheckInventoryMouseDown()
    {
        List<RaycastResult> hitUI = new List<RaycastResult>();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = 1,
        };
        pointerData.position = Input.mousePosition;

        EventSystem.current.RaycastAll(pointerData, hitUI);

        
        foreach(RaycastResult res in hitUI)
        {
            if (res.gameObject.GetComponent<Inventory_Icon>() != null)
            {
                res.gameObject.GetComponent<Inventory_Icon>().IconTappedOn();
                return;
            }
        }
    }
    public void CheckIconPlacePosition(Inventory_Icon p_holdingIcon)
    {
        List<RaycastResult> hitUI = new List<RaycastResult>();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = 1,
        };
        pointerData.position = Input.mousePosition;

        EventSystem.current.RaycastAll(pointerData, hitUI);

        Vector2Int newPlace = Vector2Int.zero;
        bool placedIcon = false;

        bool snapBack = false;

        foreach (RaycastResult res in hitUI)
        {
            if (res.gameObject != p_holdingIcon)
            {

                ///If an item already exists in that position
                if (res.gameObject.GetComponent<Inventory_Icon>() != null)
                {
                    newPlace = Vector2Int.zero;
                    placedIcon = false;
                }

                ///If the player lets go on the grid
                if (res.gameObject.GetComponent<Inventory_SlotDetector>() != null)
                {

                    ///Checks if the item can fit there
                    if (m_inventoryGrid.CanPlaceHere(res.gameObject.GetComponent<Inventory_SlotDetector>().m_gridPos, p_holdingIcon.m_itemData.m_inventoryWeight))
                    {
                        newPlace = res.gameObject.GetComponent<Inventory_SlotDetector>().m_gridPos;
                        placedIcon = true;
                    }
                }

                ///If the player lets go while on the backpack
                if (res.gameObject == m_backpackImageUi)
                {
                    snapBack = true;
                }
            }
        }

        if (!placedIcon)
        {

            if (snapBack)
            {
                //Remember to re-rotate the icon back to it's og rotation
                m_inventoryGrid.PlaceIcon(p_holdingIcon, p_holdingIcon.m_previousPosition, p_holdingIcon.m_itemData, m_currentRotationType);
            }
            return;
        }

        m_inventoryGrid.PlaceIcon(p_holdingIcon, newPlace, p_holdingIcon.m_itemData, m_currentRotationType);
        p_holdingIcon.m_previousPosition = newPlace;


    }

    public void ClearGridPosition(Vector2Int p_gridPos, Vector2Int p_gridWeight, RotationType p_rotatedDir)
    {
        m_inventoryGrid.ClearOldPos(p_gridPos, p_gridWeight, p_rotatedDir);
    }
}


[System.Serializable]
public class BackpackInventory
{
    public List<ResourceData> m_itemsInBackpack;

    public List<HeldIcons> m_currentIcons;
}

[System.Serializable]
public class HeldIcons
{
    public Inventory_Icon m_currentIcon;
    public Vector2Int m_gridPosition;
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




