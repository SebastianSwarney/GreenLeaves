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
    public UnityEngine.UI.Text m_heldItemText;
    public GameObject m_craftingRecipeMenu, m_cookingRecipeMenu;
    public GameObject m_craftingRecipeButton, m_cookingRecipeButton;

    [Header("Icon Selection Variables")]
    public float m_selectedBufferTime;

    public GameObject m_warningMessageObject;

    [HideInInspector]
    public Inventory_Icon m_currentSelectedIcon;

    //[HideInInspector]
    //public bool m_currentBuldingIcon;


    [Header("Secondary Menu")]
    public GameObject m_craftingMenu;
    public GameObject m_cookingMenu;
    public Crafting_ToolComponents m_toolComponents;

    private bool m_craftingMenuOpened;

    [Tooltip("Used to determine where the player can drop the item to put it on the crafting table")]
    public GameObject m_craftingTableUI;
    public GameObject m_cookingTableUI;
    public GameObject m_dropArea;
    public GameObject m_equipArea;
    public GameObject m_eatingArea;
    public GameObject m_toolComponentArea;
    public GameObject m_toolSlotArea;
    public Inventory_EatingStagingArea m_eatingStagingArea;

    public Transform m_craftedIconPlacement;
    public Inventory_Icon m_currentEquippedTool;
    public Inventory_Icon m_currentToolSlot;

    ///Used to toggle the menu open and closed.
    //[HideInInspector]
    public bool m_isOpen;
    private bool m_isDraggingObject;

    private bool m_canDropEverything;

    [Header("Tutorial Bools")]
    public bool m_canClose = true;
    public bool m_canTap = true;

    [Header("Special Key")]
    public bool m_containsKey;
    private void Awake()
    {
        Instance = this;
        m_heldItemText.text = "";
    }

    private void Start()
    {
        m_inventoryGrid.Initialize();
    }

    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.M))
        {
            ClearInventory();
        }
        if (!m_isOpen) return;
        if (Input.GetMouseButtonDown(0) && m_canTap)
        {
            IconTapped();
            //IconMovementBuffer();
        }
        else if (Input.GetKeyDown(m_rotateKey) && m_isDraggingObject)
        {
            if (m_currentSelectedIcon == null) return;
            m_currentSelectedIcon.transform.Rotate(Vector3.forward, -90);
            m_currentSelectedIcon.RotateDir();

            Inventory_Tutorial.Instance.RotateTut();
        }
    }

    #region Inventory Toggle
    public void ToggleInventory(bool p_openCrafting)
    {
        if (!m_canClose) return;

        if (m_isOpen)
        {

            m_heldItemText.text = "";
            m_craftingMenu.SetActive(false);
            m_cookingMenu.SetActive(false);
            Inventory_Tutorial.Instance.EndTutorial();
            if (m_isDraggingObject)
            {
                m_currentSelectedIcon.ForceIconDrop();
                CloseWhileHoldinObject(m_currentSelectedIcon);
            }

            CloseInventoryMenu();
            //Interactable_Manager.Instance.CheckReopen();
            Interactable_Manager.Instance.SearchForInteractable();
            PlayerStatsController.Instance.m_pauseStatDrain = false;
        }
        else
        {
            PlayerUIManager.Instance.ToggleCameraMode(false);
            m_heldItemText.text = "";
            PlayerInputToggle.Instance.ToggleInput(false);
            m_isOpen = true;

            Interactable_Manager.Instance.ForceCloseMenu();
            OpenInventoryMenu();
            m_craftingMenuOpened = p_openCrafting;

            m_craftingMenu.SetActive(p_openCrafting);
            m_cookingMenu.SetActive(!p_openCrafting);

            PlayerStatsController.Instance.m_pauseStatDrain = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Inventory_Tutorial.Instance.m_showTutorial)
        {
            Inventory_Tutorial.Instance.StartInventory();
        }
    }

    public void OpenInventoryMenu()
    {
        StopAllCoroutines();
        m_warningMessageObject.SetActive(false);
        if (DaytimeCycle_Update.Instance != null)
        {
            DaytimeCycle_Update.Instance.ToggleDaytimePause(true);
        }
        m_currentSelectedIcon = null;
        m_isDraggingObject = false;
        m_canvasObject.SetActive(true);
    }

    public void CloseInventoryMenu(bool p_skipWarning = false)
    {
        if (p_skipWarning)
        {
            StopAllCoroutines();
            m_warningMessageObject.SetActive(false);
        }
        if (DaytimeCycle_Update.Instance != null)
        {
            DaytimeCycle_Update.Instance.ToggleDaytimePause(false);
        }
        m_isOpen = false;
        m_canvasObject.SetActive(false);
        m_currentSelectedIcon = null;

        m_craftingRecipeMenu.SetActive(false);
        m_craftingRecipeButton.SetActive(true);
        m_cookingRecipeMenu.SetActive(false);
        m_cookingRecipeButton.SetActive(true);
        DropAnyOutsideIcons(p_skipWarning);
        if (m_craftingMenuOpened)
        {
            if (Crafting_Table.CraftingTable != null)
                Crafting_Table.CraftingTable.ClearIconList();
        }
        else
        {
            if (Crafting_Table.CookingTable != null)
                Crafting_Table.CookingTable.ClearIconList();
        }
    }

    /// <summary>
    /// Used to finalize the closing of the menu <br/>
    /// This is not in the Toggle Inventory Function, as the closing may be stalled by the warning message.
    /// </summary>
    private void FinalCloseInventory()
    {
        m_eatingStagingArea.IconRemoved();
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

        if (pickedUpResource.m_resourceData.m_resourceType == ResourceData.ResourceType.Tool)
        {
            AddToInventory(pickedUpResource, p_amount, true, p_pickedUp.GetComponent<Resource_Pickup_UsedEquipment>().m_startingDurability);
        }
        else
        {
            AddToInventory(pickedUpResource, p_amount);
        }
    }

    public void AddToInventory(ResourceContainer pickedUpResource, int p_amount, bool p_isTool = false, int p_toolDurability = 0)
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


        Inventory_Icon newIcon = CreateIcon(pickedUpResource, iconRotationType, existingAmount, p_isTool, p_toolDurability);


        newIcon.m_opensInventorySelectButton = pickedUpResource.m_showInventorySelectionButton;

        if (!CanAddToInventory(newIcon, iconRotationType) || Inventory_Tutorial.Instance.m_showTutorial)
        {
            ToggleInventory(true);

        }


        #endregion
    }

    public bool CanAddToInventory(Inventory_Icon p_newIcon, RotationType p_iconRotationType)
    {
        if (m_inventoryGrid.CanAddToRow(p_newIcon.m_itemData.m_resourceData, p_iconRotationType))
        {
            m_inventoryGrid.AddWeight(p_newIcon.m_itemData.m_resourceData);
            p_newIcon.m_inBackpack = true;
            Vector2Int placedPos;
            m_inventoryGrid.AddNewIcon(p_newIcon.m_itemData.m_resourceData, p_iconRotationType, p_newIcon, out placedPos);
            p_newIcon.m_previousGridPos = placedPos;
            p_newIcon.m_startingCoordPos = p_newIcon.transform.localPosition;
            return true;
        }

        ///If it cant fit, , check different directions open the menu.
        else
        {

            if (p_iconRotationType == RotationType.Down || p_iconRotationType == RotationType.Up)
            {
                if (m_inventoryGrid.CanAddToRow(p_newIcon.m_itemData.m_resourceData, RotationType.Left))
                {
                    m_inventoryGrid.AddWeight(p_newIcon.m_itemData.m_resourceData);
                    p_newIcon.m_inBackpack = true;
                    p_newIcon.RotateToFaceDir(RotationType.Left);
                    Vector2Int placedPos;
                    m_inventoryGrid.AddNewIcon(p_newIcon.m_itemData.m_resourceData, RotationType.Left, p_newIcon, out placedPos);
                    p_newIcon.m_previousGridPos = placedPos;
                    p_newIcon.m_startingCoordPos = p_newIcon.transform.localPosition;
                    return true;
                }
            }
            else
            {
                if (m_inventoryGrid.CanAddToRow(p_newIcon.m_itemData.m_resourceData, RotationType.Down))
                {
                    m_inventoryGrid.AddWeight(p_newIcon.m_itemData.m_resourceData);
                    p_newIcon.m_inBackpack = true;
                    p_newIcon.RotateToFaceDir(RotationType.Down);
                    Vector2Int placedPos;
                    m_inventoryGrid.AddNewIcon(p_newIcon.m_itemData.m_resourceData, RotationType.Down, p_newIcon, out placedPos);
                    p_newIcon.m_previousGridPos = placedPos;
                    p_newIcon.m_startingCoordPos = p_newIcon.transform.localPosition;
                    return true;
                }
            }

            p_newIcon.m_inBackpack = false;
            p_newIcon.transform.localPosition = m_cantFitIconPos.localPosition;
            p_newIcon.m_startingCoordPos = p_newIcon.transform.localPosition;
            return false;

        }
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

        newIcon.GetComponent<RectTransform>().SetParent(m_gameIconsParent, false);
        newIcon.transform.localScale = Vector3.one;
        newIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(m_inventoryGrid.m_gridIconSize.x * p_pickedUpResource.m_resourceData.m_inventoryWeight.x, m_inventoryGrid.m_gridIconSize.y * p_pickedUpResource.m_resourceData.m_inventoryWeight.y);
        newIcon.UpdateIcon(p_pickedUpResource, p_rotationType);
        newIcon.m_currentResourceAmount = p_resourceAmount;
        newIcon.m_inCookingTable = false;
        newIcon.m_inCraftingTable = false;

        newIcon.UpdateIconNumber();



        BackpackSlot newSlot = new BackpackSlot();

        #endregion

        ///Sets up the slot for this item. Adds it to the backpack
        #region Backpack Slot Initialization

        newSlot.m_currentData = p_pickedUpResource;
        newSlot.m_associatedIcon = newIcon;
        m_backpack.m_itemsInBackpack.Add(newSlot);

        #endregion


        return newIcon;
    }



    public void CraftNewIcon(Crafting_Recipe p_recipe, List<Crafting_Table.Crafting_ItemsContainer> p_givenItems)
    {
        RotationType iconRotationType = p_recipe.m_craftedItem.m_resourceData.m_iconStartingRotation;
        if (p_recipe.m_craftedItem.m_resourceData.m_inventoryWeight.x < p_recipe.m_craftedItem.m_resourceData.m_inventoryWeight.y)
        {
            p_recipe.m_craftedItem.m_resourceData.m_inventoryWeight = new Vector2Int(p_recipe.m_craftedItem.m_resourceData.m_inventoryWeight.y, p_recipe.m_craftedItem.m_resourceData.m_inventoryWeight.x);
        }
        Inventory_Icon newIcon = CreateIcon(p_recipe.m_craftedItem, iconRotationType, p_recipe.m_craftedAmount, p_recipe.m_isToolRecipe, p_recipe.GetToolDurability(p_givenItems));

        newIcon.m_opensInventorySelectButton = p_recipe.m_craftedItem.m_showInventorySelectionButton;

        newIcon.transform.localPosition = m_craftedIconPlacement.localPosition;

        newIcon.m_startingCoordPos = m_craftedIconPlacement.localPosition;
        newIcon.m_inBackpack = false;
        newIcon.m_inCraftingTable = false;
        newIcon.m_inCookingTable = false;

        newIcon.UpdateIconNumber();

        if (Crafting_Table.CraftingTable.enabled)
        {
            newIcon.m_inCraftingTable = true;
            Crafting_Table.CraftingTable.AddIconToTable(newIcon);
        }
        else
        {
            newIcon.m_inCookingTable = true;
            Crafting_Table.CookingTable.AddIconToTable(newIcon);
        }

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
            /*if (m_currentBuldingIcon != null && m_backpack.m_itemsInBackpack[i].m_associatedIcon == m_currentBuldingIcon)
            {
                buildingIconIndex = i;

            }*/
            if (m_backpack.m_itemsInBackpack[i].m_associatedIcon.m_isEquipped || m_backpack.m_itemsInBackpack[i].m_associatedIcon.m_inToolSlot)
            {
                continue;
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
            Player_Inventory.Instance.DropObject(m_backpack.m_itemsInBackpack[p_removeOrder[i]].m_associatedIcon, true);


            m_backpack.m_itemsInBackpack[p_removeOrder[i]].m_associatedIcon.m_currentResourceAmount = 0;


            ObjectPooler.Instance.ReturnToPool(m_backpack.m_itemsInBackpack[p_removeOrder[i]].m_associatedIcon.gameObject);
            m_backpack.m_itemsInBackpack.RemoveAt(p_removeOrder[i]);

        }
        //m_currentBuldingIcon = null;

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
        m_craftingMenu.SetActive(true);
        m_isOpen = true;
        m_craftingMenuOpened = true;

    }

    #endregion

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
            if (res.gameObject.GetComponent<Inventory_Icon>() != null)
            {
                m_currentSelectedIcon = res.gameObject.GetComponent<Inventory_Icon>();

                m_currentSelectedIcon.IconTappedOn();
                m_heldItemText.text = m_currentSelectedIcon.m_itemData.m_resourceData.m_resourceName;
                return;
            }
        }
        m_currentSelectedIcon = null;
    }


    /// <summary>
    /// Called from the icon, when it is tapped on
    /// Initializes the dragging state for the 2d menu
    /// </summary>
    public void IconTappedOn(Inventory_Icon p_tappedOn)
    {
        p_tappedOn.GetComponent<RectTransform>().SetParent(m_itemTransferParent, false);
        m_currentSelectedIcon = p_tappedOn;
        m_isDraggingObject = true;
        if (p_tappedOn.m_isEquipped)
        {
            Inventory_ItemUsage.Instance.UnEquipCurrent();
            p_tappedOn.m_isEquipped = false;
            p_tappedOn.m_wasInEquipment = true;
            m_currentEquippedTool = null;
        }
        else if (p_tappedOn.m_inToolSlot)
        {
            p_tappedOn.m_inToolSlot = false;
            p_tappedOn.m_wasInToolSlot = true;
            p_tappedOn.m_wasInEquipment = false;
            m_currentToolSlot.m_itemData.SecondaryUseItem(false);
            m_currentToolSlot = null;
        }
        else
        {
            p_tappedOn.m_wasInToolSlot = false;
            p_tappedOn.m_wasInEquipment = false;
        }

        if (p_tappedOn.m_inEatingArea)
        {
            p_tappedOn.m_wasInEatingArea = true;
            m_eatingStagingArea.IconRemoved();
        }
        else
        {
            p_tappedOn.m_wasInEatingArea = false;
        }

        if (p_tappedOn.gameObject.GetComponent<Inventory_Icon_ToolResource>() == null && p_tappedOn.m_inBackpack)
        {
            m_inventoryGrid.RemoveWeight(p_tappedOn.m_itemData.m_resourceData);
        }


        Inventory_Tutorial.Instance.PickUpTutorial();
    }


    /// <summary>
    /// Called when the player is still dragging an icon, but they exit out the menu.
    /// Properly returns the dragged icon, and resets it.
    /// </summary>
    public void CloseWhileHoldinObject(Inventory_Icon p_holdingIcon)
    {

        m_isDraggingObject = false;
        m_currentSelectedIcon.ClosedWhileHolding();
        if (m_currentSelectedIcon.m_inBackpack)
        {
            m_inventoryGrid.AddWeight(m_currentSelectedIcon.m_itemData.m_resourceData);
        }

        m_currentSelectedIcon = null;

        if (p_holdingIcon.m_inBackpack)
        {
            p_holdingIcon.m_inBackpack = true;
            p_holdingIcon.m_inCraftingTable = false;
            p_holdingIcon.m_inCookingTable = false;

            //Remember to re-rotate the icon back to it's og rotation
            m_inventoryGrid.PlaceIcon(p_holdingIcon, p_holdingIcon.m_previousGridPos, Vector2Int.zero, p_holdingIcon.m_itemData.m_resourceData, p_holdingIcon.m_rotatedDir);
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
        else if (p_holdingIcon.m_wasInEquipment)
        {
            p_holdingIcon.m_isEquipped = true;
            p_holdingIcon.m_itemData.UseItem(p_holdingIcon);
            m_currentEquippedTool = p_holdingIcon;
            p_holdingIcon.m_wasInEquipment = false;
        }
        else if (p_holdingIcon.m_wasInToolSlot)
        {
            m_currentToolSlot = p_holdingIcon;
            p_holdingIcon.m_inToolSlot = true;
            p_holdingIcon.m_wasInToolSlot = false;
            p_holdingIcon.m_itemData.SecondaryUseItem(true);
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
    public void CheckIconPlacePosition(Inventory_Icon p_holdingIcon, Vector2Int p_clickedOffset)
    {

        m_heldItemText.text = "";
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

        bool snapBack = true;
        bool crafting = false;
        bool cooking = false;
        bool equip = false;
        bool eating = false;
        bool toolSlot = false;

        ///Used to have a cached reference on whether this is a tool resource icon
        bool iconIsToolResource = p_holdingIcon.GetComponent<Inventory_Icon_ToolResource>() != null;


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
                        if (m_inventoryGrid.CanPlaceHere(res.gameObject.GetComponent<Inventory_SlotDetector>().m_gridPos, p_clickedOffset, p_holdingIcon.m_itemData.m_resourceData.m_inventoryWeight, p_holdingIcon.m_rotatedDir))
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
                                        p_holdingIcon.m_wasInCookingTable = false;
                                        p_holdingIcon.m_wasInCraftingTable = false;
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

                    if (res.gameObject == m_craftingTableUI)
                    {
                        snapBack = false;
                        crafting = true;
                    }
                    else if (res.gameObject == m_cookingTableUI)
                    {
                        snapBack = false;
                        cooking = true;
                        p_holdingIcon.m_wasInCookingTable = false;
                        p_holdingIcon.m_wasInCraftingTable = false;
                    }
                    else if (res.gameObject == m_dropArea)
                    {
                        snapBack = false;
                        crafting = false;
                        cooking = false;
                        p_holdingIcon.m_wasInCookingTable = false;
                        p_holdingIcon.m_wasInCraftingTable = false;
                    }
                    else if (res.gameObject == m_equipArea)
                    {
                        snapBack = false;
                        equip = true;
                        crafting = false;
                        p_holdingIcon.m_wasInCookingTable = false;
                        p_holdingIcon.m_wasInCraftingTable = false;
                    }
                    else if (res.gameObject == m_eatingArea)
                    {
                        snapBack = false;
                        crafting = false;
                        cooking = false;
                        eating = true;
                        p_holdingIcon.m_wasInCookingTable = false;
                        p_holdingIcon.m_wasInCraftingTable = false;
                    }
                    else if (res.gameObject == m_toolSlotArea)
                    {
                        p_holdingIcon.m_wasInCookingTable = false;
                        p_holdingIcon.m_wasInCraftingTable = false;
                        toolSlot = true;
                        crafting = false;
                        snapBack = false;
                        cooking = false;
                        equip = false;
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
                        p_holdingIcon.m_wasInCookingTable = false;
                        p_holdingIcon.m_wasInCraftingTable = false;
                    }
                    else if (res.gameObject == m_cookingTableUI)
                    {
                        snapBack = false;
                        cooking = true;
                        p_holdingIcon.m_wasInCookingTable = false;
                        p_holdingIcon.m_wasInCraftingTable = false;
                    }
                    else if (res.gameObject == m_toolComponentArea || res.gameObject.GetComponent<Inventory_SlotDetector>() != null)
                    {
                        snapBack = true;
                        cooking = false;
                        crafting = false;
                        p_holdingIcon.m_wasInCookingTable = false;
                        p_holdingIcon.m_wasInCraftingTable = false;
                    }
                }
            }
        }

        #endregion

        #region Determine what to do with the icon given the findings

        p_holdingIcon.GetComponent<RectTransform>().SetParent(m_gameIconsParent, false);
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
                    p_holdingIcon.m_isEquipped = false;

                    //Remember to re-rotate the icon back to it's og rotation
                    p_holdingIcon.ResetRotation();
                    p_holdingIcon.SetNumberRotation();
                    m_inventoryGrid.PlaceIcon(p_holdingIcon, p_holdingIcon.m_previousGridPos, p_holdingIcon.m_prevClickedIndex, p_holdingIcon.m_itemData.m_resourceData, p_holdingIcon.m_rotatedDir);
                }

                ///If snapping back and the item was previously not in the backpack, return it to the outer, and it's original orientation
                else
                {
                    if (p_holdingIcon.m_inCraftingTable || p_holdingIcon.m_wasInCraftingTable)
                    {
                        p_holdingIcon.m_inCraftingTable = true;
                        p_holdingIcon.m_isEquipped = false;
                        p_holdingIcon.m_inEatingArea = false;
                        Crafting_Table.CraftingTable.AddIconToTable(p_holdingIcon);
                    }
                    else if (p_holdingIcon.m_inCookingTable || p_holdingIcon.m_wasInCookingTable)
                    {
                        p_holdingIcon.m_inCookingTable = true;
                        p_holdingIcon.m_isEquipped = false;
                        p_holdingIcon.m_inEatingArea = false;
                        Crafting_Table.CookingTable.AddIconToTable(p_holdingIcon);
                    }
                    else if (p_holdingIcon.m_isEquipped || p_holdingIcon.m_wasInEquipment)
                    {
                        m_currentEquippedTool = p_holdingIcon;
                        m_currentEquippedTool.m_itemData.UseItem(m_currentEquippedTool);
                        m_currentEquippedTool.m_isEquipped = true;
                    }
                    else if (p_holdingIcon.m_wasInToolSlot || p_holdingIcon.m_inToolSlot)
                    {
                        m_currentToolSlot = p_holdingIcon;
                        p_holdingIcon.m_inToolSlot = true;
                        p_holdingIcon.m_itemData.SecondaryUseItem(true);

                    }
                    else if (p_holdingIcon.m_wasInEatingArea)
                    {
                        if (m_eatingStagingArea.CanAddIconToEatingArea(p_holdingIcon))
                        {
                            p_holdingIcon.m_inEatingArea = true;
                        }
                    }
                    else
                    {
                        p_holdingIcon.m_isEquipped = false;
                        p_holdingIcon.m_inEatingArea = false;

                        if (iconIsToolResource)
                        {
                            p_holdingIcon.m_inCookingTable = false;
                            p_holdingIcon.m_inCraftingTable = false;
                            p_holdingIcon.m_startingCoordPos = p_holdingIcon.GetComponent<Inventory_Icon_ToolResource>().m_toolBeltPlacement;
                        }
                    }
                    p_holdingIcon.m_wasInCookingTable = p_holdingIcon.m_wasInCraftingTable = false;
                    p_holdingIcon.m_inBackpack = false;
                    p_holdingIcon.ForceIconDrop();
                    p_holdingIcon.ResetRotation();
                    p_holdingIcon.SetNumberRotation();
                    p_holdingIcon.transform.localPosition = p_holdingIcon.m_startingCoordPos;
                }
            }

            ///If the item is outside the backpack, just leave it there
            else
            {
                p_holdingIcon.m_inBackpack = false;
                p_holdingIcon.m_isEquipped = false;
                p_holdingIcon.m_startingCoordPos = p_holdingIcon.transform.localPosition;

                if (crafting)
                {
                    p_holdingIcon.m_inCraftingTable = true;
                    p_holdingIcon.m_inEatingArea = false;
                    p_holdingIcon.m_wasInEatingArea = false;
                    p_holdingIcon.m_wasInEquipment = false;
                    p_holdingIcon.m_wasInToolSlot = false;
                    p_holdingIcon.m_isEquipped = false;
                    Crafting_Table.CraftingTable.AddIconToTable(p_holdingIcon);
                }
                else if (cooking)
                {
                    p_holdingIcon.m_inEatingArea = false;
                    p_holdingIcon.m_wasInEatingArea = false;
                    p_holdingIcon.m_wasInToolSlot = false;
                    p_holdingIcon.m_wasInEquipment = false;
                    p_holdingIcon.m_isEquipped = false;
                    p_holdingIcon.m_inCookingTable = true;
                    Crafting_Table.CookingTable.AddIconToTable(p_holdingIcon);
                }
                else if (equip || toolSlot)
                {
                    if (p_holdingIcon.GetComponent<Inventory_Icon_Durability>())
                    {

                        if (toolSlot)
                        {
                            if (m_currentToolSlot != null)
                            {
                                if (!CanAddToInventory(m_currentToolSlot, m_currentToolSlot.m_rotatedDir))
                                {
                                    m_currentToolSlot.m_inBackpack = false;
                                }
                                m_currentToolSlot.m_itemData.SecondaryUseItem(false);
                                m_currentToolSlot.m_inToolSlot = false;
                            }

                            m_currentToolSlot = p_holdingIcon;
                            p_holdingIcon.m_inToolSlot = true;
                            p_holdingIcon.RotateToFaceDir(RotationType.Left);
                            p_holdingIcon.transform.localPosition = m_toolSlotArea.transform.localPosition;
                            p_holdingIcon.m_startingCoordPos = p_holdingIcon.transform.localPosition;
                            p_holdingIcon.m_itemData.SecondaryUseItem(true);
                        }
                        else
                        {
                            if (m_currentEquippedTool != null)
                            {
                                if (!CanAddToInventory(m_currentEquippedTool, m_currentEquippedTool.m_rotatedDir))
                                {
                                    m_currentEquippedTool.m_inBackpack = false;
                                }

                                m_currentEquippedTool.m_isEquipped = false;
                                Inventory_ItemUsage.Instance.UnEquipCurrent();

                            }
                            m_currentEquippedTool = p_holdingIcon;
                            m_currentEquippedTool.RotateToFaceDir(RotationType.Left);
                            m_currentEquippedTool.m_isEquipped = true;

                            m_currentEquippedTool.m_itemData.UseItem(m_currentEquippedTool);
                            p_holdingIcon.transform.localPosition = m_equipArea.transform.localPosition;
                            p_holdingIcon.m_startingCoordPos = p_holdingIcon.transform.localPosition;
                        }
                    }
                    else
                    {
                        p_holdingIcon.m_inCookingTable = false;
                        p_holdingIcon.m_inCraftingTable = false;
                        p_holdingIcon.m_inEatingArea = false;
                        p_holdingIcon.m_wasInEatingArea = false;
                        p_holdingIcon.m_wasInEquipment = false;
                        p_holdingIcon.m_isEquipped = false;
                        p_holdingIcon.m_wasInToolSlot = false;
                        p_holdingIcon.m_inToolSlot = false;

                        p_holdingIcon.m_inBackpack = true;
                        //Remember to re-rotate the icon back to it's og rotation
                        p_holdingIcon.ResetRotation();
                        p_holdingIcon.SetNumberRotation();
                        if (!CanAddToInventory(p_holdingIcon, p_holdingIcon.m_rotatedDir))
                        {
                            Debug.Log("Cant Fit Properly");
                        }
                    }
                }
                else if (eating)
                {
                    if (m_eatingStagingArea.CanAddIconToEatingArea(p_holdingIcon))
                    {
                        p_holdingIcon.m_wasInEatingArea = false;
                        p_holdingIcon.m_inEatingArea = true;
                    }
                    else
                    {
                        if (p_holdingIcon.m_wasInEquipment)
                        {
                            m_currentEquippedTool = p_holdingIcon;
                            m_currentEquippedTool.RotateToFaceDir(RotationType.Left);
                            m_currentEquippedTool.m_isEquipped = true;
                            m_currentEquippedTool.m_itemData.UseItem(m_currentEquippedTool);
                            p_holdingIcon.transform.localPosition = m_equipArea.transform.localPosition;
                            p_holdingIcon.m_startingCoordPos = p_holdingIcon.transform.localPosition;
                        }
                        else if (p_holdingIcon.m_wasInToolSlot)
                        {
                            m_currentToolSlot = p_holdingIcon;
                            p_holdingIcon.m_inToolSlot = true;
                            m_currentToolSlot.RotateToFaceDir(RotationType.Left);
                            p_holdingIcon.m_wasInToolSlot = false;
                            p_holdingIcon.transform.localPosition = m_toolSlotArea.transform.localPosition;
                            p_holdingIcon.m_startingCoordPos = p_holdingIcon.transform.localPosition;
                            p_holdingIcon.m_itemData.SecondaryUseItem(true);
                        }
                        else
                        {
                            Debug.Log("Cant Eat");
                            p_holdingIcon.m_inCraftingTable = false;
                            p_holdingIcon.m_inCookingTable = false;
                            p_holdingIcon.m_inEatingArea = false;
                            p_holdingIcon.m_inBackpack = true;
                            //Remember to re-rotate the icon back to it's og rotation
                            p_holdingIcon.ResetRotation();
                            p_holdingIcon.SetNumberRotation();
                            if (!CanAddToInventory(p_holdingIcon, p_holdingIcon.m_rotatedDir))
                            {
                                Debug.Log("Cant Fit Properly");
                            }
                        }
                    }
                }
            }
            return;
        }

        ///If the item is over a placeable spot, place it properly in the grid
        p_holdingIcon.m_inCraftingTable = false;
        p_holdingIcon.m_inCookingTable = false;
        p_holdingIcon.m_wasInEquipment = false;
        p_holdingIcon.m_isEquipped = false;
        p_holdingIcon.m_wasInToolSlot = false;
        p_holdingIcon.m_inToolSlot = false;
        p_holdingIcon.m_wasInEatingArea = false;
        p_holdingIcon.m_inEatingArea = false;
        p_holdingIcon.m_inBackpack = true;
        m_inventoryGrid.PlaceIcon(p_holdingIcon, newPlace, p_clickedOffset, p_holdingIcon.m_itemData.m_resourceData, p_holdingIcon.m_rotatedDir);
        m_inventoryGrid.AddWeight(p_holdingIcon.m_itemData.m_resourceData);
        p_holdingIcon.m_previousGridPos = newPlace;
        p_holdingIcon.m_startingCoordPos = p_holdingIcon.transform.localPosition;
        p_holdingIcon.IconProperlyPlaced();

        Inventory_Tutorial.Instance.LetGoTutorial();

        #endregion
    }

    /// <summary>
    /// Called to clear the grid of the current lifeted icon
    /// Called when the icon is being dragged.
    /// </summary>
    public void ClearGridPosition(Vector2Int p_gridPos, Vector2Int p_offset, Vector2Int p_gridWeight, RotationType p_rotatedDir)
    {
        m_inventoryGrid.ClearOldPos(p_gridPos, p_offset, p_gridWeight, p_rotatedDir);
    }

    #endregion

    #region Menu Selected Button


    /*/// <summary>
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
*/
    public void RemoveSingleIcon(Inventory_Icon p_usedIcon)
    {
        p_usedIcon.m_currentResourceAmount = 0;
        for (int i = 0; i < m_backpack.m_itemsInBackpack.Count; i++)
        {
            if (m_backpack.m_itemsInBackpack[i].m_associatedIcon == p_usedIcon)
            {
                m_backpack.m_itemsInBackpack.RemoveAt(i);
                break;
            }
        }
        m_inventoryGrid.RemoveSingleIcon(p_usedIcon);
    }



    /// <summary>
    /// Used to remove the resource entirely from the inventory.<br/>
    /// This includes removing the icon, Removing the icon data from the inventory, and repooling the icon.
    /// </summary>
    public void RemoveIconFromInventory(Inventory_Icon p_removeMe)
    {
        m_currentSelectedIcon = null;
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

    #region ItemGetterFunctions
    [Header("Berry Stats")]
    public ResourceContainer m_hungerBerry;
    public ResourceContainer m_staminaBerry, m_energyBerry;
    public int GetAmountOfHungerBerries()
    {
        int amount = 0;
        foreach (BackpackSlot slot in m_backpack.m_itemsInBackpack)
        {
            if (slot.m_currentData.m_resourceData.m_resourceName == m_hungerBerry.m_resourceData.m_resourceName)
            {
                amount += slot.m_associatedIcon.m_currentResourceAmount;
            }
        }
        return amount;
    }

    public int GetAmountOfStaminaBerries()
    {
        int amount = 0;
        foreach (BackpackSlot slot in m_backpack.m_itemsInBackpack)
        {
            if (slot.m_currentData.m_resourceData.m_resourceName == m_staminaBerry.m_resourceData.m_resourceName)
            {
                amount += slot.m_associatedIcon.m_currentResourceAmount;
            }
        }
        return amount;
    }

    public int GetAmountOfEnergyBerries()
    {
        int amount = 0;
        foreach (BackpackSlot slot in m_backpack.m_itemsInBackpack)
        {
            if (slot.m_currentData.m_resourceData.m_resourceName == m_energyBerry.m_resourceData.m_resourceName)
            {
                amount += slot.m_associatedIcon.m_currentResourceAmount;
            }
        }
        return amount;
    }

    #endregion


    #region Inventory Clear Code
    public void ClearInventory()
    {
        if (m_currentToolSlot != null)
        {
            m_currentToolSlot.m_itemData.SecondaryUseItem(false);
            m_currentToolSlot.m_inBackpack = false;
            m_currentToolSlot.m_inToolSlot = false;
            m_currentToolSlot = null;
        }


        List<int> removeIndexes = new List<int>();
        foreach (BackpackSlot slot in m_backpack.m_itemsInBackpack)
        {
            if (slot.m_associatedIcon.GetComponent<Inventory_Icon_Durability>())
            {
                Player_Inventory.Instance.DropObject(slot.m_associatedIcon, false);
                ObjectPooler.Instance.ReturnToPool(slot.m_associatedIcon.gameObject);
                slot.m_associatedIcon.m_itemData.DropObject(slot.m_associatedIcon, Vector3.zero, Quaternion.identity, false);
                removeIndexes.Add(m_backpack.m_itemsInBackpack.IndexOf(slot));
            }
        }

        removeIndexes.Sort();
        removeIndexes.Reverse();
        for (int i = 0; i < removeIndexes.Count; i++)
        {
            m_backpack.m_itemsInBackpack.RemoveAt(removeIndexes[i]);
        }

        for (int y = 0; y < m_inventoryGrid.m_itemGrids.Count; y++)
        {
            for (int x = 0; x < m_inventoryGrid.m_itemGrids[y].m_itemGrids.Count; x++)
            {
                if (m_inventoryGrid.m_itemGrids[y].m_itemGrids[x] != null)
                {
                    if (!m_inventoryGrid.m_itemGrids[y].m_itemGrids[x].gameObject.activeSelf || !m_inventoryGrid.m_itemGrids[y].m_itemGrids[x].gameObject.activeInHierarchy)
                    {
                        m_inventoryGrid.m_itemGrids[y].m_itemGrids[x] = null;
                        m_inventoryGrid.m_currentAmount--;
                    }
                }
                //m_inventoryGrid.m_itemGrids[y].m_itemGrids[x] = null;
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