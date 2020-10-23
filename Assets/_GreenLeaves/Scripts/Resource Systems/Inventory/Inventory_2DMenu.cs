using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The script used to manage the Inventory's 2d menu. 
/// All the data for the inventory, including the items held in it are 
/// in this script.
/// </summary>
public class Inventory_2DMenu : MonoBehaviour
{
    public static Inventory_2DMenu Instance;
    ///Used to determine which way the icon is currently facing
    public enum RotationType { Left, Right, Down, Up }

    [Header("World References")]
    public Player_Inventory m_playerInventory;
    public GameObject m_canvasObject;
    public GameObject m_inventorySlotPrefab;



    [Header("Icon")]
    public GameObject m_mainIconPrefab;


    [Header("Inventory")]
    public BackpackInventory m_backpack;
    public KeyCode m_rotateKey = KeyCode.R;

    [Header("UI Elements")]
    public Inventory_Grid m_inventoryGrid;
    public Transform m_gameIconsParent;
    public Transform m_cantFitIconPos;
    public Transform m_itemTransferParent;

    [Tooltip("Used to determine where the player can drop the item until it snaps back automatically")]
    public GameObject m_backpackImageUi;


    [HideInInspector] public Inventory_Icon m_currentSelectedIcon;

    ///Used to toggle the menu open and closed.
    private bool m_isOpen;
    private bool m_isDraggingObject;

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
        else if (Input.GetKeyDown(m_rotateKey))
        {
            if (m_currentSelectedIcon == null) return;
            m_currentSelectedIcon.transform.Rotate(Vector3.forward, 90);
            m_currentSelectedIcon.RotateDir();
        }



    }

    #region Inventory Toggle
    public void ToggleInventory()
    {
        if (m_isOpen)
        {
            m_isOpen = false;
            if (m_isDraggingObject)
            {
                m_currentSelectedIcon.ForceIconDrop();
                CloseWhileHoldinObject(m_currentSelectedIcon);
            }

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
        m_isDraggingObject = false;
        m_canvasObject.SetActive(true);
    }



    public void CloseInventoryMenu()
    {
        DropAnyOutsideIcons();
        m_canvasObject.SetActive(false);
    }
    #endregion

    /// <summary>
    /// Adds the item to the inventory & spawns a new icon for it
    /// Calls the grid to add the icon to the grid
    /// </summary>
    public void AddItemToInventory(GameObject p_pickedUp, int p_amount)
    {
        ResourceData pickedUpResource = p_pickedUp.GetComponent<Resource_Pickup>().m_resourceInfo.m_resourceData;
        BackpackSlot newSlot = new BackpackSlot();

        p_pickedUp.GetComponent<Resource_Pickup>().PickupResource();
        ///Determine if there are any existing items like this in the inventory
        ///Used for items that can have more than 1 item in a slot IE. Arrows
        #region Existing Item Check

        int existingAmount = p_amount;
        if (pickedUpResource.m_singleResourceAmount > 1)
        {
            List<Inventory_Icon> icons = m_inventoryGrid.GetExistingIconsOfResource(pickedUpResource);
            if (icons.Count > 0)
            {
                foreach (Inventory_Icon icon in icons)
                {
                    if (icon.m_currentResourceAmount < pickedUpResource.m_singleResourceAmount)
                    {
                        icon.m_currentResourceAmount += existingAmount;

                        if (icon.m_currentResourceAmount > pickedUpResource.m_singleResourceAmount)
                        {
                            existingAmount = icon.m_currentResourceAmount - pickedUpResource.m_singleResourceAmount;
                            icon.m_currentResourceAmount = pickedUpResource.m_singleResourceAmount;
                            icon.UpdateIconNumber();
                        }
                        else
                        {
                            icon.UpdateIconNumber();
                            return;
                        }
                    }
                }
            }

        }
        #endregion

        ///Determines which way the icon should be orientated
        #region Icon orientation setup

        RotationType iconRotationType = pickedUpResource.m_iconStartingRotation;
        if (pickedUpResource.m_inventoryWeight.x < pickedUpResource.m_inventoryWeight.y)
        {
            pickedUpResource.m_inventoryWeight = new Vector2Int(pickedUpResource.m_inventoryWeight.y, pickedUpResource.m_inventoryWeight.x);
        }

        #endregion

        ///Sets the icon's script up, assigning proper values
        #region Icon Script Initialization

        Inventory_Icon newIcon = ObjectPooler.Instance.NewObject(m_mainIconPrefab, Vector3.zero, Quaternion.identity).GetComponent<Inventory_Icon>();
        newIcon.transform.parent = m_gameIconsParent;
        newIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(m_inventoryGrid.m_gridIconSize.x * pickedUpResource.m_inventoryWeight.x, m_inventoryGrid.m_gridIconSize.y * pickedUpResource.m_inventoryWeight.y);
        newIcon.UpdateIcon(pickedUpResource, iconRotationType);
        newIcon.m_currentResourceAmount = existingAmount;
        newIcon.UpdateIconNumber();

        #endregion


        ///Sets up the slot for this item. Adds it to the backpack
        #region Backpack Slot Initialization

        newSlot.m_currentData = pickedUpResource;
        newSlot.m_associatedIcon = newIcon;
        m_backpack.m_itemsInBackpack.Add(newSlot);
        m_inventoryGrid.AddWeight(pickedUpResource);

        #endregion


        ///Determines if the icon can be placed in the grid
        #region Icon Grid Positioning

        ///If it can fit
        if (m_inventoryGrid.CanAddToRow(pickedUpResource, iconRotationType))
        {
            newIcon.m_inBackpack = true;
            Vector2Int placedPos;
            m_inventoryGrid.AddNewIcon(pickedUpResource, iconRotationType, newIcon, out placedPos);
            newIcon.m_previousGridPos = placedPos;
            newIcon.m_startingCoordPos = newIcon.transform.localPosition;

        }

        ///If it cant fit, open the menu.
        else
        {

            newIcon.m_inBackpack = false;
            newIcon.transform.localPosition = m_cantFitIconPos.localPosition;
            newIcon.m_startingCoordPos = newIcon.transform.localPosition;
            ToggleInventory();
        }
        #endregion
    }

    /// <summary>
    /// Gathers the icons that are not within the grid, and removes them from the backpack.
    /// Spawns the items in the 3d world by calling the Player_Inventory script
    /// This is called when the 2d menu is closed
    /// </summary>
    public void DropAnyOutsideIcons()
    {
        ///Goes through all the existing icons, and adds those not in the backpack to a list
        List<int> removeList = new List<int>();
        for (int i = 0; i < m_backpack.m_itemsInBackpack.Count; i++)
        {
            if (!m_backpack.m_itemsInBackpack[i].m_associatedIcon.m_inBackpack)
            {
                removeList.Add(i);
            }
        }

        ///Reverses the list, and removes the items properly
        removeList.Reverse();

        for (int i = 0; i < removeList.Count; i++)
        {
            m_playerInventory.DropObject(m_backpack.m_itemsInBackpack[removeList[i]].m_currentData.m_resourcePrefab, m_backpack.m_itemsInBackpack[removeList[i]].m_associatedIcon.m_currentResourceAmount);
            m_backpack.m_itemsInBackpack[removeList[i]].m_associatedIcon.m_currentResourceAmount = 0;

            m_inventoryGrid.RemoveWeight(m_backpack.m_itemsInBackpack[removeList[i]].m_currentData);
            ObjectPooler.Instance.ReturnToPool(m_backpack.m_itemsInBackpack[removeList[i]].m_associatedIcon.gameObject);
            m_backpack.m_itemsInBackpack.RemoveAt(removeList[i]);
        }
    }


    /// <summary>
    /// This function is called when the player uses the mouse to tap down on the ui while this menu is open
    /// Detects for inventory icons, and calls their IconTappedOn function in their Inventory_Icon script
    /// </summary>
    public void CheckInventoryMouseDown()
    {
        List<RaycastResult> hitUI = new List<RaycastResult>();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = 1,
        };
        pointerData.position = Input.mousePosition;

        EventSystem.current.RaycastAll(pointerData, hitUI);


        foreach (RaycastResult res in hitUI)
        {
            if (res.gameObject.GetComponent<Inventory_Icon>() != null)
            {
                res.gameObject.GetComponent<Inventory_Icon>().IconTappedOn();
                return;
            }
        }
    }


    /// <summary>
    /// Called from the icon, when it is tapped on
    /// Initializes the dragging state for the 2d menu
    /// </summary>
    public void IconTappedOn(Inventory_Icon p_tappedOn)
    {
        p_tappedOn.transform.parent = m_itemTransferParent;
        m_currentSelectedIcon = p_tappedOn;
        m_isDraggingObject = true;
    }


    /// <summary>
    /// Called when the player is still dragging an icon, but they exit out the menu.
    /// Properly returns the dragged icon, and resets it.
    /// </summary>
    public void CloseWhileHoldinObject(Inventory_Icon p_holdingIcon)
    {
        m_isDraggingObject = false;
        m_currentSelectedIcon = null;

        if (p_holdingIcon.m_inBackpack)
        {
            p_holdingIcon.m_inBackpack = true;
            //Remember to re-rotate the icon back to it's og rotation
            m_inventoryGrid.PlaceIcon(p_holdingIcon, p_holdingIcon.m_previousGridPos, p_holdingIcon.m_itemData, p_holdingIcon.m_rotatedDir);
        }
        else
        {
            p_holdingIcon.m_inBackpack = false;

            p_holdingIcon.transform.localPosition = p_holdingIcon.m_startingCoordPos;
        }
    }

    /// <summary>
    /// Called when the icon is dropped by the player lifiting the mouse.
    /// Called in the Inventory_Icon script, in it's coroutine
    /// This function checks to see if the player's cursor is over any ui elements. 
    ///     If it is over a grid slot, it will check if it can be placed
    ///     If it is not over a grid slot, but it is over the backpack ui, it will return the object to where it was
    ///     If it is over nothing, it will toggle the icon to no longer being in the bag, and drop it upon closing the item menu
    /// </summary>
    public void CheckIconPlacePosition(Inventory_Icon p_holdingIcon)
    {
        m_isDraggingObject = false;
        m_currentSelectedIcon = null;

        #region Perform the UI Raycast using the events system

        List<RaycastResult> hitUI = new List<RaycastResult>();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = 1,
        };
        pointerData.position = Input.mousePosition;

        EventSystem.current.RaycastAll(pointerData, hitUI);

        #endregion

        #region Determine what ui elements are being hovered over
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
                    if (m_inventoryGrid.CanPlaceHere(res.gameObject.GetComponent<Inventory_SlotDetector>().m_gridPos, p_holdingIcon.m_itemData.m_inventoryWeight, p_holdingIcon.m_rotatedDir))
                    {
                        newPlace = res.gameObject.GetComponent<Inventory_SlotDetector>().m_gridPos;
                        placedIcon = true;
                    }
                    else
                    {
                        if (m_inventoryGrid.GetIcon(res.gameObject.GetComponent<Inventory_SlotDetector>().m_gridPos) != null)
                        {
                            Inventory_Icon currentIcon = m_inventoryGrid.GetIcon(res.gameObject.GetComponent<Inventory_SlotDetector>().m_gridPos);

                            if (currentIcon.m_itemData.m_resourceName == p_holdingIcon.m_itemData.m_resourceName)
                            {
                                int amountLeft = 0;


                                ///If the current icon can completely fit in the new icon, remove the current icon from the inventory
                                if (currentIcon.CanAddFullAmount(p_holdingIcon.m_currentResourceAmount, out amountLeft))
                                {
                                    p_holdingIcon.m_inBackpack = false;
                                    ObjectPooler.Instance.ReturnToPool(p_holdingIcon.gameObject);

                                    
                                    for (int i = 0; i < m_backpack.m_itemsInBackpack.Count; i++)
                                    {
                                        if (m_backpack.m_itemsInBackpack[i].m_associatedIcon != p_holdingIcon) continue;
                                        m_backpack.m_itemsInBackpack.RemoveAt(i);
                                        return;
                                    }
                                }

                                ///If there is still remainder, reset the held icon
                                else
                                {
                                    p_holdingIcon.m_currentResourceAmount = amountLeft;
                                    p_holdingIcon.UpdateIconNumber();
                                }
                            }
                        }
                    }
                }

                ///If the player lets go while on the backpack
                if (res.gameObject == m_backpackImageUi)
                {
                    snapBack = true;
                }
            }
        }

        #endregion

        #region Determine what to do with the icon given the findings

        p_holdingIcon.transform.parent = m_gameIconsParent;
        if (!placedIcon)
        {

            if (snapBack)
            {

                ///If snapping back and the item was previously in the backpack, return it to the backpack, and it's original orientation
                if (p_holdingIcon.m_inBackpack)
                {
                    p_holdingIcon.m_inBackpack = true;
                    //Remember to re-rotate the icon back to it's og rotation
                    p_holdingIcon.ResetRotation();
                    m_inventoryGrid.PlaceIcon(p_holdingIcon, p_holdingIcon.m_previousGridPos, p_holdingIcon.m_itemData, p_holdingIcon.m_rotatedDir);
                }

                ///If snapping back and the item was previously not in the backpack, return it to the outer, and it's original orientation
                else
                {
                    p_holdingIcon.m_inBackpack = false;
                    p_holdingIcon.ForceIconDrop();
                    p_holdingIcon.ResetRotation();
                    p_holdingIcon.transform.localPosition = p_holdingIcon.m_startingCoordPos;
                }
            }

            ///If the item is outside the backpack, just leave it there
            else
            {
                p_holdingIcon.m_inBackpack = false;
                p_holdingIcon.m_startingCoordPos = p_holdingIcon.transform.localPosition;
            }
            return;
        }

        ///If the item is over a placeable spot, place it properly in the grid
        p_holdingIcon.m_inBackpack = true;
        m_inventoryGrid.PlaceIcon(p_holdingIcon, newPlace, p_holdingIcon.m_itemData, p_holdingIcon.m_rotatedDir);
        p_holdingIcon.m_previousGridPos = newPlace;
        p_holdingIcon.m_startingCoordPos = p_holdingIcon.transform.localPosition;

        #endregion
    }

    /// <summary>
    /// Called to clear the grid of the current lifeted icon
    /// Called when the icon is being dragged.
    /// </summary>
    public void ClearGridPosition(Vector2Int p_gridPos, Vector2Int p_gridWeight, RotationType p_rotatedDir)
    {
        m_inventoryGrid.ClearOldPos(p_gridPos, p_gridWeight, p_rotatedDir);
    }
}


/// <summary>
/// A container script to display the info better in the unity editor.
/// More items may be added to this later.
/// </summary>
[System.Serializable]
public class BackpackInventory
{
    public List<BackpackSlot> m_itemsInBackpack;
}

/// <summary>
/// A slot to hold an item. Each slot holds one single item.
/// This is used to get the associated icon with a resource data type.
/// </summary>
[System.Serializable]
public class BackpackSlot
{
    public ResourceData m_currentData;
    public Inventory_Icon m_associatedIcon;
}


/// <summary>
/// The main data container used to manage the resources.
/// </summary>
[System.Serializable]
public class ResourceData
{
    public string m_resourceName;
    [Tooltip("Determines how many of this resources corelates to a single icon. IE. 10 arrows = 1 icon")]
    public int m_singleResourceAmount = 1;
    public Sprite m_resourceSprite;
    public GameObject m_resourcePrefab;
    public Vector2Int m_inventoryWeight;

    //Used to determine which way the icon should be rotated when initially being placed.
    public Inventory_2DMenu.RotationType m_iconStartingRotation = Inventory_2DMenu.RotationType.Left;

    public ResourceData(ResourceData p_newData = null)
    {
        if (p_newData != null)
        {
            m_resourceName = p_newData.m_resourceName;
            m_resourceSprite = p_newData.m_resourceSprite;
            m_resourcePrefab = p_newData.m_resourcePrefab;
            m_inventoryWeight = p_newData.m_inventoryWeight;
        }
    }
}