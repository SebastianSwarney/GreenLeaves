using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// The script placed on the icons that appear on the grid.
/// </summary>
public class Inventory_Icon : MonoBehaviour
{
    public ResourceContainer m_itemData;
    public Image m_itemIcon;
    public RectTransform m_iconTransform;

    public Inventory_2DMenu.RotationType m_rotatedDir = Inventory_2DMenu.RotationType.Left;
    public bool m_inBackpack = false;
    public bool m_inCraftingTable = false;
    public bool m_inCookingTable = false;

    [HideInInspector] public Vector2Int m_previousGridPos;
    [HideInInspector] public Vector3 m_startingCoordPos;
    [HideInInspector] public Inventory_2DMenu.RotationType m_previousRotType;
    private Vector3 m_dragOffset;

    public int m_currentResourceAmount;

    public bool m_opensInventorySelectButton;

    [Header("Number UI")]
    public Text m_numberText;
    public RectTransform m_numberTransform;
    
    
    /// <summary>
    /// Changes the rotation of the icon to match it's current rotation type
    /// </summary>
    public void ResetRotation()
    {
        m_rotatedDir = m_previousRotType;
        switch (m_rotatedDir)
        {
            case Inventory_2DMenu.RotationType.Left:
                transform.localEulerAngles = new Vector3(0, 0, 0);
                m_numberTransform.localEulerAngles = new Vector3(0, 0, 0);
                m_numberTransform.anchoredPosition = new Vector2(0,0);
                break;
            case Inventory_2DMenu.RotationType.Down:
                transform.localEulerAngles = new Vector3(0, 0, -90);

                m_numberTransform.localEulerAngles = new Vector3(0, 0, -90);
                m_numberTransform.anchoredPosition = new Vector2(-m_iconTransform.sizeDelta.x, 0);
                break;
            case Inventory_2DMenu.RotationType.Right:
                transform.localEulerAngles = new Vector3(0, 0, 180);

                m_numberTransform.localEulerAngles = new Vector3(0, 0, -180);
                m_numberTransform.anchoredPosition = new Vector2(-m_iconTransform.sizeDelta.x, -m_iconTransform.sizeDelta.y);
                break;
            case Inventory_2DMenu.RotationType.Up:
                transform.localEulerAngles = new Vector3(0, 0, 90);
                m_numberTransform.localEulerAngles = new Vector3(0, 0, 90);
                m_numberTransform.anchoredPosition = new Vector2(0, -m_iconTransform.sizeDelta.y);
                break;
        }
        AdjustedDraggingOffset();
    }

    /// <summary>
    /// Used to rotate the resource amount number text element to always<br/>
    /// be on the bottom left corner, and rotated upright
    /// </summary>
    public void SetNumberRotation()
    {
        switch (m_rotatedDir)
        {
            case Inventory_2DMenu.RotationType.Left:
                m_numberTransform.localEulerAngles = new Vector3(0, 0, 0);
                m_numberTransform.anchoredPosition = new Vector2(0, 0);
                break;
            case Inventory_2DMenu.RotationType.Down:
                m_numberTransform.localEulerAngles = new Vector3(0, 0, 90);
                m_numberTransform.anchoredPosition = new Vector2(0, m_iconTransform.sizeDelta.y);
                break;
            case Inventory_2DMenu.RotationType.Right:
                m_numberTransform.localEulerAngles = new Vector3(0, 0, -180);
                m_numberTransform.anchoredPosition = new Vector2(-m_iconTransform.sizeDelta.x, m_iconTransform.sizeDelta.y);
                break;
            case Inventory_2DMenu.RotationType.Up:
                m_numberTransform.localEulerAngles = new Vector3(0, 0, -90);
                m_numberTransform.anchoredPosition = new Vector2(-m_iconTransform.sizeDelta.x, 0);

                break;
        }
    }
    
    
    /// <summary>
    /// Adjusts the offset from the mouse while being dragged
    /// The offset changes depending on the rotation type
    /// </summary>
    public void AdjustedDraggingOffset()
    {
        m_dragOffset = GetComponent<RectTransform>().sizeDelta;
        m_dragOffset = new Vector2(m_dragOffset.x / m_itemData.m_resourceData.m_inventoryWeight.x, m_dragOffset.y / m_itemData.m_resourceData.m_inventoryWeight.y);

        switch (m_rotatedDir)
        {
            case Inventory_2DMenu.RotationType.Left:
                m_dragOffset = new Vector2(m_dragOffset.x * (m_itemData.m_resourceData.m_inventoryWeight.x - 1) * .5f, m_dragOffset.y * -(m_itemData.m_resourceData.m_inventoryWeight.y - 1) * .5f);
                break;

            case Inventory_2DMenu.RotationType.Down:
                m_dragOffset = new Vector2(m_dragOffset.x * -(m_itemData.m_resourceData.m_inventoryWeight.y - 1) * .5f, m_dragOffset.y * -(m_itemData.m_resourceData.m_inventoryWeight.x - 1) * .5f);
                break;

            case Inventory_2DMenu.RotationType.Right:
                m_dragOffset = new Vector2(m_dragOffset.x * -(m_itemData.m_resourceData.m_inventoryWeight.x - 1) * .5f, m_dragOffset.y * (m_itemData.m_resourceData.m_inventoryWeight.y - 1) * .5f);
                break;

            case Inventory_2DMenu.RotationType.Up:
                m_dragOffset = new Vector2(m_dragOffset.x * (m_itemData.m_resourceData.m_inventoryWeight.y - 1) * .5f, m_dragOffset.y * (m_itemData.m_resourceData.m_inventoryWeight.x - 1) * .5f);
                break;
        }
    }

    /// <summary>
    /// Called to update the icon, and it's data
    /// This is called when the icon is initially created by the Inventory_2DMenu.
    /// </summary>
    public void UpdateIcon(ResourceContainer p_heldData, Inventory_2DMenu.RotationType p_startingRotation)
    {
        m_itemData = p_heldData;
        m_itemIcon.sprite = p_heldData.m_resourceData.m_resourceSprite;
        m_previousGridPos = Vector2Int.zero;
        m_startingCoordPos = Vector3.zero;
        m_rotatedDir = m_previousRotType = p_startingRotation;

        ResetRotation();
        SetNumberRotation();
    }

    /// <summary>
    /// Used to update the icon's resource amount ui text to match
    /// </summary>
    public virtual void UpdateIconNumber()
    {
        m_numberText.text = "x" + m_currentResourceAmount.ToString();
    }

    /// <summary>
    /// <para>Used to add resource amounts to items that are added to each other while dragging.<br/>
    /// IE. if the player has 2 wood icons, 1 icon has 2 wood, the other has 3 wood, adds the 3 wood to the 2 wood when it is dropped on it<br/>
    /// If the new amount is more than the limit, still adds until the limit, and returns the amount left.</para>
    /// 
    /// Returns true if there is no remainder. | Returns false if the full amount cant be added
    /// </summary>
    public bool CanAddFullAmount(int p_amount,out int p_amountLeft)
    {
        p_amountLeft = p_amount;
        if(m_currentResourceAmount >= m_itemData.m_resourceData.m_singleResourceAmount)
        {
            return false;
        }

        if(m_currentResourceAmount + p_amount <= m_itemData.m_resourceData.m_singleResourceAmount)
        {
            p_amountLeft = 0;
            m_currentResourceAmount += p_amount;
            UpdateIconNumber();
            return true;
        }

        p_amountLeft = p_amount - (m_itemData.m_resourceData.m_singleResourceAmount - m_currentResourceAmount);
        m_currentResourceAmount += p_amount;
        UpdateIconNumber();
        return false;
    }

    /// <summary>
    /// Called from the Inventory_2DMenu, when the player taps down on the mouse
    /// Starts the coroutine that moves the icon with the mouse. 
    /// Also calls the Inventory_2DMenu to initialize the tapping sequence.
    /// </summary>
    public void IconTappedOn()
    {
        StartCoroutine(WaitForMouseUp());

        m_previousRotType = m_rotatedDir;

        Inventory_2DMenu.Instance.IconTappedOn(this);
        if (m_inBackpack)
        {
            Inventory_2DMenu.Instance.ClearGridPosition(m_previousGridPos, m_itemData.m_resourceData.m_inventoryWeight, m_rotatedDir);
        }
        else if (m_inCraftingTable)
        {
            Crafting_Table.CraftingTable.RemoveIconFromTable(this);
        } else if (m_inCookingTable)
        {
            Crafting_Table.CookingTable.RemoveIconFromTable(this);
        }

    }
    /// <summary>
    /// The corourtine used to move the icon with the mouse.
    /// Disables the raycast target on the image component.
    /// When the mouse is let go, calls the Inventory_2DMenu to stop the dragging sequence.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForMouseUp()
    {
        bool beingHeld = true;
        m_itemIcon.raycastTarget = false;
        while (beingHeld)
        {
            if (Input.GetMouseButtonUp(0))
            {
                beingHeld = false;
            }
            transform.position = Input.mousePosition + (Vector3)m_dragOffset;


            yield return null;
        }
        ///Do the raycast check here
        Inventory_2DMenu.Instance.CheckIconPlacePosition(this);
        m_itemIcon.raycastTarget = true;
    }

    /// <summary>
    /// Called when the 2d menu is closed while still in the dragging sequence.
    /// Will stop the coroutine, and renenable the raycast target
    /// </summary>
    public void ForceIconDrop()
    {
        m_rotatedDir = m_previousRotType;
        AdjustedDraggingOffset();
        StopAllCoroutines();
        m_itemIcon.raycastTarget = true;
    }

    /// <summary>
    /// Called to change the direction of the rotation type while dragging the object
    /// </summary>
    public void RotateDir()
    {
        switch (m_rotatedDir)
        {
            case Inventory_2DMenu.RotationType.Left:
                m_rotatedDir = Inventory_2DMenu.RotationType.Down;
                break;
            case Inventory_2DMenu.RotationType.Down:
                m_rotatedDir = Inventory_2DMenu.RotationType.Right;
                break;
            case Inventory_2DMenu.RotationType.Right:
                m_rotatedDir = Inventory_2DMenu.RotationType.Up;
                break;
            case Inventory_2DMenu.RotationType.Up:
                m_rotatedDir = Inventory_2DMenu.RotationType.Left;
                break;
        }
        SetNumberRotation();
        AdjustedDraggingOffset();
    }

    /// <summary>
    /// Removes the icon from the inventory<br/>
    /// This is a virtual, as the Tool Resource variant uses this to not remove itself <br/>
    /// from the inventory but rather disable itself
    /// </summary>
    public virtual void RemoveIcon()
    {
        Inventory_2DMenu.Instance.RemoveIconFromInventory(this);
    }

    /// <summary>
    /// Called when the icon is being cleared from the crafting table when it is closed.<br/>
    /// This is a virtual function, as the inventory icon Tool Resource variant needs a way <br/>to know when the crafting menu is closed
    /// </summary>
    public virtual void RemoveIconFromCraftingTableOnClose()
    {
        m_inCraftingTable = false;
        m_inCookingTable = false;
    }
}
