using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    public bool m_isEquipped = false;
    public bool m_inCraftingTable = false;
    public bool m_inCookingTable = false;
    public bool m_wasInEatingArea = false;
    public bool m_inEatingArea = false;
    public bool m_wasInEquipment;

    [HideInInspector] public Vector2Int m_previousGridPos;
    [HideInInspector] public Vector3 m_startingCoordPos;
    [HideInInspector] public Inventory_2DMenu.RotationType m_previousRotType;
    private Vector3 m_dragOffset;

    public int m_currentResourceAmount;

    public bool m_opensInventorySelectButton;
    public CanvasGroup m_canvasGroup;

    public Image m_iconHueImage;
    public Color m_resourceColor, m_edibleColor, m_toolColor;

    [Header("Number UI")]
    public Text m_numberText;
    public RectTransform m_numberTransform;

    private Transform m_parentTransform;

    public Vector2Int m_clickedIndex;
    public Vector2Int m_prevClickedIndex;
    public Vector2 m_debugOffset, m_clickedOffset;
    public float m_iconCellSize = 45;
    public Vector2 m_referenceScreenSize;

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
                m_numberTransform.anchoredPosition = new Vector2(0, 0);
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

        switch (p_heldData.m_resourceData.m_resourceType)
        {
            case ResourceData.ResourceType.Resource:
                m_iconHueImage.color = m_resourceColor;
                break;
            case ResourceData.ResourceType.Edible:
                m_iconHueImage.color = m_edibleColor;
                break;
            case ResourceData.ResourceType.Tool:
                m_iconHueImage.color = m_toolColor;
                break;
        }

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
    public bool CanAddFullAmount(int p_amount, out int p_amountLeft)
    {
        p_amountLeft = p_amount;
        if (m_currentResourceAmount >= m_itemData.m_resourceData.m_singleResourceAmount)
        {
            return false;
        }

        if (m_currentResourceAmount + p_amount <= m_itemData.m_resourceData.m_singleResourceAmount)
        {
            p_amountLeft = 0;
            m_currentResourceAmount += p_amount;
            UpdateIconNumber();
            return true;
        }

        p_amountLeft = p_amount - (m_itemData.m_resourceData.m_singleResourceAmount - m_currentResourceAmount);
        m_currentResourceAmount = m_itemData.m_resourceData.m_singleResourceAmount;
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
        #region Get ClickedIndex
        Vector2 rectSize = transform.GetComponent<RectTransform>().sizeDelta;

        switch (m_rotatedDir)
        {
            case Inventory_2DMenu.RotationType.Left:
                m_debugOffset = new Vector2((Input.mousePosition.x - transform.position.x) * (m_referenceScreenSize.x / Screen.width) / 2,
                            -(Input.mousePosition.y - transform.position.y) * (m_referenceScreenSize.y / Screen.height) / 2);

                break;
            case Inventory_2DMenu.RotationType.Right:
                m_debugOffset = new Vector2(-(Input.mousePosition.x - transform.position.x) * (m_referenceScreenSize.x / Screen.width) / 2,
                                            (Input.mousePosition.y - transform.position.y) * (m_referenceScreenSize.y / Screen.height) / 2);
                break;




            case Inventory_2DMenu.RotationType.Down:
                m_debugOffset = new Vector2(-(Input.mousePosition.y - transform.position.y) * (m_referenceScreenSize.x / Screen.width) / 2,
                                            -(Input.mousePosition.x - transform.position.x) * (m_referenceScreenSize.y / Screen.height) / 2);
                break;



            case Inventory_2DMenu.RotationType.Up:
                m_debugOffset = new Vector2((Input.mousePosition.y - transform.position.y) * (m_referenceScreenSize.x / Screen.width) / 2,
                                            (Input.mousePosition.x - transform.position.x) * (m_referenceScreenSize.y / Screen.height) / 2);
                break;
        }
        m_clickedOffset = m_debugOffset + (new Vector2(rectSize.x, rectSize.y) / 2);
        m_clickedIndex = new Vector2Int((int)(m_clickedOffset.x / (rectSize.x / m_itemData.m_resourceData.m_inventoryWeight.x)),
                                (int)(m_clickedOffset.y / (rectSize.y / m_itemData.m_resourceData.m_inventoryWeight.y)));

        #endregion
        StartCoroutine(WaitForMouseUp());

        m_previousRotType = m_rotatedDir;



        Inventory_2DMenu.Instance.IconTappedOn(this);
        if (m_inBackpack)
        {
            Inventory_2DMenu.Instance.ClearGridPosition(m_previousGridPos, m_prevClickedIndex, m_itemData.m_resourceData.m_inventoryWeight, m_rotatedDir);
        }
        else if (m_inCraftingTable)
        {
            Crafting_Table.CraftingTable.RemoveIconFromTable(this);
        }
        else if (m_inCookingTable)
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
        UpdateDraggingOffset();
        bool beingHeld = true;
        m_parentTransform = transform.parent;
        m_canvasGroup.alpha = .5f;
        m_iconHueImage.raycastTarget = false;
        while (beingHeld)
        {
            if (Input.GetMouseButtonUp(0))
            {
                beingHeld = false;
                m_canvasGroup.alpha = 1;
            }

            transform.position = Input.mousePosition;


            transform.localPosition += m_dragOffset;
            //transform.localPosition -= ;


            yield return null;
        }
        ///Do the raycast check here
        Inventory_2DMenu.Instance.CheckIconPlacePosition(this, m_clickedIndex);
        m_iconHueImage.raycastTarget = true;
    }
    private void UpdateDraggingOffset()
    {
        switch (m_rotatedDir)
        {
            case Inventory_2DMenu.RotationType.Left:
                m_dragOffset = new Vector3(-m_iconCellSize * ((float)m_clickedIndex.x - ((float)m_itemData.m_resourceData.m_inventoryWeight.x / 2) + 0.5f),
                                            m_iconCellSize * ((float)m_clickedIndex.y - ((float)m_itemData.m_resourceData.m_inventoryWeight.y / 2) + 0.5f));
                break;
            case Inventory_2DMenu.RotationType.Right:
                m_dragOffset = new Vector3(m_iconCellSize * ((float)m_clickedIndex.x - ((float)m_itemData.m_resourceData.m_inventoryWeight.x / 2) + 0.5f),
                                            -m_iconCellSize * ((float)m_clickedIndex.y - ((float)m_itemData.m_resourceData.m_inventoryWeight.y / 2) + 0.5f));
                break;
            case Inventory_2DMenu.RotationType.Down:
                m_dragOffset = new Vector3(m_iconCellSize * ((float)m_clickedIndex.y - ((float)m_itemData.m_resourceData.m_inventoryWeight.y / 2) + 0.5f),
                                            m_iconCellSize * ((float)m_clickedIndex.x - ((float)m_itemData.m_resourceData.m_inventoryWeight.x / 2) + 0.5f));
                break;
            case Inventory_2DMenu.RotationType.Up:
                m_dragOffset = new Vector3(-m_iconCellSize * ((float)m_clickedIndex.y - ((float)m_itemData.m_resourceData.m_inventoryWeight.y / 2) + 0.5f),
                                            -m_iconCellSize * ((float)m_clickedIndex.x - ((float)m_itemData.m_resourceData.m_inventoryWeight.x / 2) + 0.5f));
                break;
        }
    }

    public void ClosedWhileHolding()
    {
        StopAllCoroutines();
        m_canvasGroup.alpha = 1;
        m_iconHueImage.raycastTarget = true;
        transform.localPosition = m_startingCoordPos;
        ResetRotation();
        SetNumberRotation();

    }
    /// <summary>
    /// Called when the 2d menu is closed while still in the dragging sequence.
    /// Will stop the coroutine, and renenable the raycast target
    /// </summary>
    public void ForceIconDrop()
    {
        m_rotatedDir = m_previousRotType;
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
        UpdateDraggingOffset();
    }

    public void RotateToFaceDir(Inventory_2DMenu.RotationType p_newRotation)
    {
        m_rotatedDir = p_newRotation;
        switch (m_rotatedDir)
        {
            case Inventory_2DMenu.RotationType.Left:
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case Inventory_2DMenu.RotationType.Down:
                transform.eulerAngles = new Vector3(0, 0, -90);
                break;
            case Inventory_2DMenu.RotationType.Right:
                transform.eulerAngles = new Vector3(0, 0, -90);
                break;
            case Inventory_2DMenu.RotationType.Up:
                transform.eulerAngles = new Vector3(0, 0, -270);
                break;
        }

        SetNumberRotation();
        UpdateDraggingOffset();
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


    public void IconProperlyPlaced()
    {
        m_prevClickedIndex = m_clickedIndex;
    }
}
