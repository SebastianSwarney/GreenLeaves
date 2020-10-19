using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// The script placed on the icons that appear on the grid.
/// </summary>
public class Inventory_Icon : MonoBehaviour
{
    public ResourceData m_itemData;
    public Image m_itemIcon;
    public RectTransform m_iconTransform;

    public Inventory_2DMenu.RotationType m_rotatedDir = Inventory_2DMenu.RotationType.Left;
    public bool m_inBackpack = false;

    [HideInInspector] public Vector2Int m_previousGridPos;
    [HideInInspector] public Vector3 m_startingCoordPos;
    [HideInInspector] public Inventory_2DMenu.RotationType m_previousRotType;
    private Vector3 m_dragOffset;

    public int m_currentResourceAmount;

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
                transform.localEulerAngles = new Vector3(0, 0, 90);

                m_numberTransform.localEulerAngles = new Vector3(0, 0, -90);
                m_numberTransform.anchoredPosition = new Vector2(-m_iconTransform.sizeDelta.x, 0);
                break;
            case Inventory_2DMenu.RotationType.Right:
                transform.localEulerAngles = new Vector3(0, 0, 180);

                m_numberTransform.localEulerAngles = new Vector3(0, 0, -180);
                m_numberTransform.anchoredPosition = new Vector2(-m_iconTransform.sizeDelta.x, -m_iconTransform.sizeDelta.y);
                break;
            case Inventory_2DMenu.RotationType.Up:
                transform.localEulerAngles = new Vector3(0, 0, 270);
                m_numberTransform.localEulerAngles = new Vector3(0, 0, 90);
                m_numberTransform.anchoredPosition = new Vector2(0, -m_iconTransform.sizeDelta.y);
                break;
        }
        AdjustedDraggingOffset();
    }


    public void SetNumberRotation()
    {
        switch (m_rotatedDir)
        {
            case Inventory_2DMenu.RotationType.Left:
                m_numberTransform.localEulerAngles = new Vector3(0, 0, 0);
                m_numberTransform.anchoredPosition = new Vector2(0, 0);
                break;
            case Inventory_2DMenu.RotationType.Down:
                m_numberTransform.localEulerAngles = new Vector3(0, 0, -90);
                m_numberTransform.anchoredPosition = new Vector2(-m_iconTransform.sizeDelta.x, 0);
                break;
            case Inventory_2DMenu.RotationType.Right:
                m_numberTransform.localEulerAngles = new Vector3(0, 0, -180);
                m_numberTransform.anchoredPosition = new Vector2(-m_iconTransform.sizeDelta.x, m_iconTransform.sizeDelta.y);
                break;
            case Inventory_2DMenu.RotationType.Up:
                m_numberTransform.localEulerAngles = new Vector3(0, 0, 90);
                m_numberTransform.anchoredPosition = new Vector2(0, m_iconTransform.sizeDelta.y);
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
        m_dragOffset = new Vector2(m_dragOffset.x / m_itemData.m_inventoryWeight.x, m_dragOffset.y / m_itemData.m_inventoryWeight.y);

        switch (m_rotatedDir)
        {
            case Inventory_2DMenu.RotationType.Left:
                m_dragOffset = new Vector2(m_dragOffset.x * (m_itemData.m_inventoryWeight.x - 1) * .5f, m_dragOffset.y * -(m_itemData.m_inventoryWeight.y - 1) * .5f);
                break;

            case Inventory_2DMenu.RotationType.Down:
                m_dragOffset = new Vector2(m_dragOffset.x * -(m_itemData.m_inventoryWeight.y - 1) * .5f, m_dragOffset.y * -(m_itemData.m_inventoryWeight.x - 1) * .5f);
                break;

            case Inventory_2DMenu.RotationType.Right:
                m_dragOffset = new Vector2(m_dragOffset.x * -(m_itemData.m_inventoryWeight.x - 1) * .5f, m_dragOffset.y * (m_itemData.m_inventoryWeight.y - 1) * .5f);
                break;

            case Inventory_2DMenu.RotationType.Up:
                m_dragOffset = new Vector2(m_dragOffset.x * (m_itemData.m_inventoryWeight.y - 1) * .5f, m_dragOffset.y * (m_itemData.m_inventoryWeight.x - 1) * .5f);
                break;
        }
    }

    /// <summary>
    /// Called to update the icon, and it's data
    /// This is called when the icon is initially created by the Inventory_2DMenu.
    /// </summary>
    public void UpdateIcon(ResourceData p_heldData, Inventory_2DMenu.RotationType p_startingRotation)
    {
        m_itemData = p_heldData;
        m_itemIcon.sprite = p_heldData.m_resourceSprite;
        m_previousGridPos = Vector2Int.zero;
        m_startingCoordPos = Vector3.zero;
        m_rotatedDir = m_previousRotType = p_startingRotation;

        ResetRotation();
    }

    public void UpdateIconNumber()
    {
        m_numberText.text = "x" + m_currentResourceAmount.ToString();
        Debug.Log("Current Amount: " + m_currentResourceAmount);
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
            Inventory_2DMenu.Instance.ClearGridPosition(m_previousGridPos, m_itemData.m_inventoryWeight, m_rotatedDir);
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

}
