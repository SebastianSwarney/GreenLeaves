using System.Collections;
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
    public GameObject m_durabilityIconPrefab;


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



    [Header("Icon Selection Variables")]
    public float m_selectedBufferTime;

    public GameObject m_selectedMenuParent;
    public UnityEngine.UI.Text m_selectButtonText;

    public GameObject m_warningMessageObject;

    [HideInInspector]
    public Inventory_Icon m_currentSelectedIcon;

    //[HideInInspector]
    public Inventory_Icon m_currentBuldingIcon;


    [Header("Secondary Menu")]
    public GameObject m_craftingMenu;
    public GameObject m_cookingMenu;
    private bool m_craftingMenuOpened;

    [Tooltip("Used to determine where the player can drop the item to put it on the crafting table")]
    public GameObject m_craftingTableUI;
    public GameObject m_cookingTableUI;
    public Transform m_craftedIconPlacement;

    ///Used to toggle the menu open and closed.
    private bool m_isOpen;
    private bool m_isDraggingObject;
    private bool m_iconWasInCraftingTable = false;

    private bool m_canDropEverything;

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
            Debug.Log("UI Inventory Mouse Down Input Here", this);
            IconTapped();
            //IconMovementBuffer();
        }
        else if (Input.GetKeyDown(m_rotateKey))
        {
            Debug.Log("UI Inventory Rotate Input Here", this);
            if (m_currentSelectedIcon == null) return;
            m_currentSelectedIcon.transform.Rotate(Vector3.forward, -90);
            m_currentSelectedIcon.RotateDir();
        }
    }

    #region Inventory Toggle
    public void ToggleInventory(bool p_openCrafting)
    {
        if (m_isOpen)
        {

            m_craftingMenu.SetActive(false);
            m_cookingMenu.SetActive(false);

            if (m_isDraggingObject)
            {
                m_currentSelectedIcon.ForceIconDrop();
                CloseWhileHoldinObject(m_currentSelectedIcon);
            }

            CloseInventoryMenu();
            Interactable_Manager.Instance.CheckReopen();
        }
        else
        {
            PlayerInputToggle.Instance.ToggleInput(false);
            m_isOpen = true;

            Interactable_Manager.Instance.ForceCloseMenu();
            OpenInventoryMenu();
            m_craftingMenuOpened = p_openCrafting;

            m_craftingMenu.SetActive(p_openCrafting);
            m_cookingMenu.SetActive(!p_openCrafting);

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

    public void CloseInventoryMenu(bool p_skipWarning = false)
    {
        m_isOpen = false;
        m_canvasObject.SetActive(false);
        m_selectedMenuParent.SetActive(false);
        m_currentSelectedIcon = null;
        DropAnyOutsideIcons(p_skipWarning);
        if (m_craftingMenuOpened)
        {
            Crafting_Table.CraftingTable.ClearIconList();
        }
        else
        {
            Crafting_Table.CookingTable.ClearIconList();
        }
    }

    /// <summary>
    /// Used to finalize the closing of the menu <br/>
    /// This is not in the Toggle Inventory Function, as the closing may be stalled by the warning message.
    /// </summary>
    private void FinalCloseInventory()
    {
        PlayerInputToggle.Instance.ToggleInput(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }
    #endregion

    #region Inventory Menu 
    /// <summary>
    /// Adds the item to the inventory & spawns a new icon for it
    /// Calls the grid to add the icon to the grid
    /// </summary>
    public void PickupItem(GameObject p_pickedUp, int p_amount)
    {
        ResourceContainer pickedUpResource = p_pickedUp.GetComponent<Resource_Pickup>().m_resourceInfo;


        p_pickedUp.GetComponent<Resource_Pickup>().PickupResource();
        AddToInventory(pickedUpResource, p_amount);
    }

    public void AddToInventory(ResourceContainer pickedUpResource, int p_amount)
    {
        ///Determine if there are any existing items like this in the inventory
        ///Used for items that can have more than 1 item in a slot IE. Arrows
        #region Existing Item Check

        int existingAmount = p_amount;
        if (pickedUpResource.m_resourceData.m_singleResourceAmount > 1)
        {
            List<Inventory_Icon> icons = m_inventoryGrid.GetExistingIconsOfResource(pickedUpResource.m_resourceData);
            if (icons.Count > 0)
            {
                foreach (Inventory_Icon icon in icons)
                {
                    if (icon.m_currentResourceAmount < pickedUpResource.m_resourceData.m_singleResourceAmount)
                    {
                        icon.m_currentResourceAmount += existingAmount;

                        if (icon.m_currentResourceAmount > pickedUpResource.m_resourceData.m_singleResourceAmount)
                        {
                            existingAmount = icon.m_currentResourceAmount - pickedUpResource.m_resourceData.m_singleResourceAmount;
                            icon.m_currentResourceAmount = pickedUpResource.m_resourceData.m_singleResourceAmount;
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



        #region Icon orientation setup

        RotationType iconRotationType = pickedUpResource.m_resourceData.m_iconStartingRotation;
        if (pickedUpResource.m_resourceData.m_inventoryWeight.x < pickedUpResource.m_resourceData.m_inventoryWeight.y)
        {
            pickedUpResource.m_resourceData.m_inventoryWeight = new Vector2Int(pickedUpResource.m_resourceData.m_inventoryWeight.y, pickedUpResource.m_resourceData.m_inventoryWeight.x);
        }

        #endregion



        ///Determines if the icon can be placed in the grid
        #region Icon Grid Positioning

        Inventory_Icon newIcon = CreateIcon(pickedUpResource, iconRotationType, existingAmount, false, 0);

        newIcon.m_opensInventorySelectButton = pickedUpResource.m_showInventorySelectionButton;

        ///If it can fit
        if (m_inventoryGrid.CanAddToRow(pickedUpResource.m_resourceData, iconRotationType))
        {
            newIcon.m_inBackpack = true;
            Vector2Int placedPos;
            m_inventoryGrid.AddNewIcon(pickedUpResource.m_resourceData, iconRotationType, newIcon, out placedPos);
            newIcon.m_previousGridPos = placedPos;
            newIcon.m_startingCoordPos = newIcon.transform.localPosition;

        }

        ///If it cant fit, open the menu.
        else
        {

            newIcon.m_inBackpack = false;
            newIcon.transform.localPosition = m_cantFitIconPos.localPosition;
            newIcon.m_startingCoordPos = newIcon.transform.localPosition;
            ToggleInventory(true);
        }
        #endregion
    }


    /// <summary>
    /// Creates and returns a new inventory icon
    /// </summary>
    private Inventory_Icon CreateIcon(ResourceContainer p_pickedUpResource, RotationType p_rotationType, int p_resourceAmount, bool p_isTool, int p_durabilityAmount = 1)
    {


        ///Determines which way the icon should be orientated


        ///Sets the icon's script up, assigning proper values
        #region Icon Script Initialization


        Inventory_Icon newIcon = null;


        if (p_isTool)
        {
            newIcon = ObjectPooler.Instance.NewObject(m_durabilityIconPrefab, Vector3.zero, Quaternion.identity).GetComponent<Inventory_Icon>();
            newIcon.GetComponent<Inventory_Icon_Durability>().m_durabilityAmount = p_durabilityAmount;
        }
        else
        {
            newIcon = ObjectPooler.Instance.NewObject(m_mainIconPrefab, Vector3.zero, Quaternion.identity).GetComponent<Inventory_Icon>();
        }

        newIcon.transform.parent = m_gameIconsParent;
        newIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(m_inventoryGrid.m_gridIconSize.x * p_pickedUpResource.m_resourceData.m_inventoryWeight.x, m_inventoryGrid.m_gridIconSize.y * p_pickedUpResource.m_resourceData.m_inventoryWeight.y);
        newIcon.UpdateIcon(p_pickedUpResource, p_rotationType);
        newIcon.m_currentResourceAmount = p_resourceAmount;
        newIcon.UpdateIconNumber();



        BackpackSlot newSlot = new BackpackSlot();

        #endregion

        ///Sets up the slot for this item. Adds it to the backpack
        #region Backpack Slot Initialization

        newSlot.m_currentData = p_pickedUpResource;
        newSlot.m_associatedIcon = newIcon;
        m_backpack.m_itemsInBackpack.Add(newSlot);
        m_inventoryGrid.AddWeight(p_pickedUpResource.m_resourceData);

        #endregion


        return newIcon;
    }



    public void CraftNewIcon(Crafting_Recipe p_recipe)
    {
        RotationType iconRotationType = p_recipe.m_craftedItem.m_resourceData.m_iconStartingRotation;
        if (p_recipe.m_craftedItem.m_resourceData.m_inventoryWeight.x < p_recipe.m_craftedItem.m_resourceData.m_inventoryWeight.y)
        {
            p_recipe.m_craftedItem.m_resourceData.m_inventoryWeight = new Vector2Int(p_recipe.m_craftedItem.m_resourceData.m_inventoryWeight.y, p_recipe.m_craftedItem.m_resourceData.m_inventoryWeight.x);
        }
        Inventory_Icon newIcon = CreateIcon(p_recipe.m_craftedItem, iconRotationType, p_recipe.m_craftedAmount, p_recipe.m_isToolRecipe, p_recipe.m_startingToolDurability);

        newIcon.m_opensInventorySelectButton = p_recipe.m_craftedItem.m_showInventorySelectionButton;

        newIcon.transform.localPosition = m_craftedIconPlacement.localPosition;
        newIcon.m_startingCoordPos = m_craftedIconPlacement.localPosition;
        newIcon.m_inBackpack = false;
        newIcon.m_inCraftingTable = false;
        newIcon.m_inCookingTable = false;

        newIcon.UpdateIconNumber();

    }


    #region Dropping Items
    /// <summary>
    /// Gathers the icons that are not within the grid, and removes them from the backpack.
    /// Spawns the items in the 3d world by calling the Player_Inventory script
    /// This is called when the 2d menu is closed
    /// </summary>
    public void DropAnyOutsideIcons(bool p_skipWarning)
    {
        ///Goes through all the existing icons, and adds those not in the backpack to a list
        List<int> removeList = new List<int>();

        ///Used for the current building's icon, if there is one.
        ///The drop behaviour for building is different, if it is attempting to be build,
        ///which requires this check
        int buildingIconIndex = 0;


        for (int i = 0; i < m_backpack.m_itemsInBackpack.Count; i++)
        {
            if (m_currentBuldingIcon != null && m_backpack.m_itemsInBackpack[i].m_associatedIcon == m_currentBuldingIcon)
            {
                buildingIconIndex = i;

            }

            if (!m_backpack.m_itemsInBackpack[i].m_associatedIcon.m_inBackpack || m_backpack.m_itemsInBackpack[i].m_associatedIcon.m_inCraftingTable || m_backpack.m_itemsInBackpack[i].m_associatedIcon.m_inCookingTable)
            {
                removeList.Add(i);
            }
        }
        ///Reverses the list, and removes the items properly
        removeList.Reverse();
        m_canDropEverything = true;

        ///Determines if the warning message should popup
        for (int i = 0; i < removeList.Count; i++)
        {
            if (m_backpack.m_itemsInBackpack[removeList[i]].m_currentData.m_stopDropping)
            {
                m_canDropEverything = false;
            }
        }

        StartCoroutine(DropItemsOutside(removeList, buildingIconIndex, p_skipWarning));

    }

    /// <summary>
    /// Used to wait until the player confirms to drop everything. <br/>
    /// If a tool is selected to drop, displays a message first, then waits. <br/>
    /// If no tool is selected to drop, performs the drop without waiting.
    /// </summary>
    private IEnumerator DropItemsOutside(List<int> p_removeOrder, int p_buildIconIndex, bool p_skipWarning)
    {
        if (p_skipWarning)
        {
            m_canDropEverything = true;
        }
        if (!m_canDropEverything)
        {
            m_warningMessageObject.SetActive(true);
        }

        while (!m_canDropEverything)
        {
            yield return null;
        }


        for (int i = 0; i < p_removeOrder.Count; i++)
        {

            ///Since the building system closes the inventory, but it should stay in the inventory if its being used, it "drops" the item,
            ///but changes the DropFunction's dropInWorld boolean to false so that no physical item is created, but the logic is still performed.
            if (i == p_buildIconIndex && m_currentBuldingIcon != null)
            {
                m_playerInventory.DropObject(m_currentBuldingIcon, false);
            }
            else
            {
                m_playerInventory.DropObject(m_backpack.m_itemsInBackpack[p_removeOrder[i]].m_associatedIcon, true);
            }


            m_backpack.m_itemsInBackpack[p_removeOrder[i]].m_associatedIcon.m_currentResourceAmount = 0;

            m_inventoryGrid.RemoveWeight(m_backpack.m_itemsInBackpack[p_removeOrder[i]].m_currentData.m_resourceData);
            ObjectPooler.Instance.ReturnToPool(m_backpack.m_itemsInBackpack[p_removeOrder[i]].m_associatedIcon.gameObject);
            m_backpack.m_itemsInBackpack.RemoveAt(p_removeOrder[i]);

        }
        m_currentBuldingIcon = null;

        FinalCloseInventory();
    }

    public void ConfirmDrop()
    {
        m_warningMessageObject.SetActive(false);
        m_canDropEverything = true;
    }

    public void CancelDrop()
    {
        m_warningMessageObject.SetActive(false);
        m_canDropEverything = false;
        StopAllCoroutines();
        m_canvasObject.SetActive(true);
    }

    #endregion

    /// <summary>
    /// This function is called when the player uses the mouse to tap down on the ui while this menu is open
    /// Detects for inventory icons, and calls their IconTappedOn function in their Inventory_Icon script
    /// </summary>
    private IEnumerator IconMovementBuffer()
    {
        yield return new WaitForSeconds(m_selectedBufferTime);
        //IconTapped();
    }

    private void IconTapped()
    {
        //m_mouseBufferCoroutine = null;

        List<RaycastResult> hitUI = new List<RaycastResult>();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = 1,
        };
        pointerData.position = Input.mousePosition;

        EventSystem.current.RaycastAll(pointerData, hitUI);



        foreach (RaycastResult res in hitUI)
        {
            if (res.gameObject == m_selectedMenuParent.gameObject)
            {
                return;
            }
            if (res.gameObject.GetComponent<Inventory_Icon>() != null)
            {
                m_currentSelectedIcon = res.gameObject.GetComponent<Inventory_Icon>();
                m_currentSelectedIcon.IconTappedOn();

                if (m_currentSelectedIcon.m_opensInventorySelectButton)
                {
                    m_selectedMenuParent.gameObject.SetActive(true);
                }

                if (Player_Inventory.Instance.m_currentEquipedTool != null)
                {
                    if (m_currentSelectedIcon == Player_Inventory.Instance.m_currentEquipedTool.m_linkedIcon)
                    {
                        ChangeSelectedButtonText("Unequip");
                    }
                    else
                    {
                        ChangeSelectedButtonText(m_currentSelectedIcon.m_itemData.m_itemUseButtonText);
                    }
                }
                else
                {
                    ChangeSelectedButtonText(m_currentSelectedIcon.m_itemData.m_itemUseButtonText);
                }

                return;
            }
        }
        m_currentSelectedIcon = null;
        m_selectedMenuParent.gameObject.SetActive(false);
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
            p_holdingIcon.m_inCraftingTable = false;
            p_holdingIcon.m_inCookingTable = false;

            //Remember to re-rotate the icon back to it's og rotation
            m_inventoryGrid.PlaceIcon(p_holdingIcon, p_holdingIcon.m_previousGridPos, p_holdingIcon.m_itemData.m_resourceData, p_holdingIcon.m_rotatedDir);
        }
        else if (p_holdingIcon.m_inCraftingTable)
        {
            p_holdingIcon.m_inCraftingTable = false;
            p_holdingIcon.m_inBackpack = false;
        }
        else if (p_holdingIcon.m_inCookingTable)
        {
            p_holdingIcon.m_inCookingTable = false;
            p_holdingIcon.m_inBackpack = false;
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
    ///     If it is over the crafting table ui, leave it there, and add it to the crafting system
    ///     
    /// Additionally, the logic changes when the icon is a tool resource.
    ///     If it is not over the crafting table UI, snap the icon back to it's original placement
    ///     If it is on the crafting table ui, leave it there, and set it to be in the crafting table
    /// </summary>
    public void CheckIconPlacePosition(Inventory_Icon p_holdingIcon)
    {
        m_isDraggingObject = false;

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
        bool crafting = false;
        bool cooking = false;

        ///Used to have a cached reference on whether this is a tool resource icon
        bool iconIsToolResource = p_holdingIcon.GetComponent<Inventory_Icon_ToolResource>() != null;
        snapBack = iconIsToolResource;

        foreach (RaycastResult res in hitUI)
        {
            ///If the hit object isnt the held icon
            if (res.gameObject != p_holdingIcon)
            {
                if (!iconIsToolResource)
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
                        if (m_inventoryGrid.CanPlaceHere(res.gameObject.GetComponent<Inventory_SlotDetector>().m_gridPos, p_holdingIcon.m_itemData.m_resourceData.m_inventoryWeight, p_holdingIcon.m_rotatedDir))
                        {
                            newPlace = res.gameObject.GetComponent<Inventory_SlotDetector>().m_gridPos;
                            placedIcon = true;
                        }
                        else
                        {
                            if (m_inventoryGrid.GetIcon(res.gameObject.GetComponent<Inventory_SlotDetector>().m_gridPos) != null)
                            {
                                Inventory_Icon currentIcon = m_inventoryGrid.GetIcon(res.gameObject.GetComponent<Inventory_SlotDetector>().m_gridPos);

                                if (currentIcon.m_itemData.m_resourceData.m_resourceName == p_holdingIcon.m_itemData.m_resourceData.m_resourceName)
                                {
                                    int amountLeft = 0;


                                    ///If the current icon can completely fit in the new icon, remove the current icon from the inventory
                                    if (currentIcon.CanAddFullAmount(p_holdingIcon.m_currentResourceAmount, out amountLeft))
                                    {
                                        p_holdingIcon.m_inCookingTable = false;
                                        p_holdingIcon.m_inCraftingTable = false;
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
                    else if (res.gameObject == m_craftingTableUI)
                    {
                        snapBack = false;
                        crafting = true;
                    }
                    else if (res.gameObject == m_cookingTableUI)
                    {
                        snapBack = false;
                        cooking = true;
                    }
                }

                ///Logic change for tool resource
                ///Basically, if its over anything except for the crafting table UI, snap it back to the tool belt
                else
                {

                    if (res.gameObject == m_craftingTableUI)
                    {
                        snapBack = false;
                        crafting = true;
                    }
                    else if (res.gameObject == m_cookingTableUI)
                    {
                        snapBack = false;
                        cooking = true;
                    }
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
                    p_holdingIcon.m_inCraftingTable = false;
                    p_holdingIcon.m_inCookingTable = false;
                    p_holdingIcon.m_inBackpack = true;
                    //Remember to re-rotate the icon back to it's og rotation
                    p_holdingIcon.ResetRotation();
                    m_inventoryGrid.PlaceIcon(p_holdingIcon, p_holdingIcon.m_previousGridPos, p_holdingIcon.m_itemData.m_resourceData, p_holdingIcon.m_rotatedDir);
                }

                ///If snapping back and the item was previously not in the backpack, return it to the outer, and it's original orientation
                else
                {
                    if (p_holdingIcon.m_inCraftingTable)
                    {
                        Crafting_Table.CraftingTable.AddIconToTable(p_holdingIcon);
                    }
                    else if (p_holdingIcon.m_inCookingTable)
                    {
                        Crafting_Table.CookingTable.AddIconToTable(p_holdingIcon);
                    }
                    else
                    {
                        if (iconIsToolResource)
                        {
                            p_holdingIcon.m_inCookingTable = false;
                            p_holdingIcon.m_inCraftingTable = false;
                            p_holdingIcon.m_startingCoordPos = p_holdingIcon.GetComponent<Inventory_Icon_ToolResource>().m_toolBeltPlacement;
                        }
                    }
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

                if (crafting)
                {
                    p_holdingIcon.m_inCraftingTable = true;
                    Crafting_Table.CraftingTable.AddIconToTable(p_holdingIcon);
                }
                else if (cooking)
                {
                    p_holdingIcon.m_inCookingTable = true;
                    Crafting_Table.CookingTable.AddIconToTable(p_holdingIcon);
                }
            }
            return;
        }

        ///If the item is over a placeable spot, place it properly in the grid
        p_holdingIcon.m_inCraftingTable = false;
        p_holdingIcon.m_inCookingTable = false;
        p_holdingIcon.m_inBackpack = true;
        m_inventoryGrid.PlaceIcon(p_holdingIcon, newPlace, p_holdingIcon.m_itemData.m_resourceData, p_holdingIcon.m_rotatedDir);
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

    #endregion

    #region Menu Selected Button
    /// <summary>
    /// Called from the UI button. Used to perform the functionality of the element
    /// </summary>
    public void TapSelectedButton()
    {
        if (m_currentSelectedIcon == null)
        {
            Debug.Log("No Selected icon");
            return;
        }

        if (!m_currentSelectedIcon.m_opensInventorySelectButton)
        {
            Debug.LogError("Button appeared, although it shouldnt for: " + m_currentSelectedIcon.m_itemData.m_resourceData.m_resourceName);
            return;
        }
        if (Player_Inventory.Instance.m_currentEquipedTool != null)
        {
            if (m_currentSelectedIcon == Player_Inventory.Instance.m_currentEquipedTool.m_linkedIcon)
            {
                Inventory_ItemUsage.Instance.UnEquipCurrent();
                ChangeSelectedButtonText("Equip");
            }
            else
            {
                m_currentSelectedIcon.m_itemData.UseItem(m_currentSelectedIcon);
            }
        }
        else
        {
            m_currentSelectedIcon.m_itemData.UseItem(m_currentSelectedIcon);
        }
    }

    /// <summary>
    /// Changes the selection button's text.
    /// Most likely to either Consume, or Equip
    /// </summary>
    public void ChangeSelectedButtonText(string p_newText)
    {
        m_selectButtonText.text = p_newText;
    }


    /// <summary>
    /// Updates the resource amount on the icon.
    /// Called from consumables
    /// </summary>
    public void ItemUsed(Inventory_Icon p_usedIcon)
    {
        p_usedIcon.m_currentResourceAmount--;
        p_usedIcon.UpdateIconNumber();
        if (p_usedIcon.m_currentResourceAmount <= 0)
        {
            m_currentSelectedIcon = null;
            m_selectedMenuParent.gameObject.SetActive(false);
            m_inventoryGrid.RemoveSingleIcon(p_usedIcon);
        }

        for (int i = 0; i < m_backpack.m_itemsInBackpack.Count; i++)
        {
            if (m_backpack.m_itemsInBackpack[i].m_associatedIcon == p_usedIcon)
            {
                m_backpack.m_itemsInBackpack.RemoveAt(i);
                return;
            }
        }
    }

    /// <summary>
    /// Used to remove the resource entirely from the inventory.<br/>
    /// This includes removing the icon, Removing the icon data from the inventory, and repooling the icon.
    /// </summary>
    public void RemoveIconFromInventory(Inventory_Icon p_removeMe)
    {
        m_currentSelectedIcon = null;
        m_selectedMenuParent.gameObject.SetActive(false);
        m_inventoryGrid.RemoveSingleIcon(p_removeMe);
        for (int i = 0; i < m_backpack.m_itemsInBackpack.Count; i++)
        {
            if (m_backpack.m_itemsInBackpack[i].m_associatedIcon == p_removeMe)
            {
                m_backpack.m_itemsInBackpack.RemoveAt(i);
                return;
            }
        }
    }
    #endregion
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
    public ResourceContainer m_currentData;
    public Inventory_Icon m_associatedIcon;
}


