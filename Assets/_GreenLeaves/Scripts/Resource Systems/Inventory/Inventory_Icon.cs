using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Inventory_Icon : MonoBehaviour
{
    public ResourceData m_itemData;
    public Image m_itemIcon;

    public Vector2Int m_previousGridPos;
    public Vector3 m_startingCoordPos;
    public bool m_inBackpack = false;

    private Coroutine m_waitingForMouseUp;

    public Inventory_2DMenu.RotationType m_rotatedDir = Inventory_2DMenu.RotationType.Left;

    public Inventory_2DMenu.RotationType m_previousRotType;

    Vector3 m_dragOffset;
    public void AdjustedDraggingOffset()
    {
        m_dragOffset = GetComponent<RectTransform>().sizeDelta;
        switch (m_rotatedDir)
        {
            case Inventory_2DMenu.RotationType.Left:
                
                m_dragOffset = new Vector2(m_dragOffset.x / m_itemData.m_inventoryWeight.x, m_dragOffset.y / m_itemData.m_inventoryWeight.y);
                m_dragOffset = new Vector2(m_dragOffset.x * (m_itemData.m_inventoryWeight.x - 1) * .5f, m_dragOffset.y * -(m_itemData.m_inventoryWeight.y - 1) * .5f);
                
                break;

            case Inventory_2DMenu.RotationType.Down:
                m_dragOffset = new Vector2(m_dragOffset.x / m_itemData.m_inventoryWeight.y, m_dragOffset.y / m_itemData.m_inventoryWeight.x);
                m_dragOffset = new Vector2(m_dragOffset.x * (m_itemData.m_inventoryWeight.y - 1) * .5f, m_dragOffset.y * -(m_itemData.m_inventoryWeight.x + 1) * .75f);
                break;

            case Inventory_2DMenu.RotationType.Right:
                m_dragOffset = new Vector2(m_dragOffset.x / m_itemData.m_inventoryWeight.x, m_dragOffset.y / m_itemData.m_inventoryWeight.y);
                m_dragOffset = new Vector2(m_dragOffset.x * -(m_itemData.m_inventoryWeight.x - 1) * .5f, m_dragOffset.y * (m_itemData.m_inventoryWeight.y - 1) * .5f);
                break;

            case Inventory_2DMenu.RotationType.Up:
                m_dragOffset = new Vector2(m_dragOffset.x / m_itemData.m_inventoryWeight.y, m_dragOffset.y / m_itemData.m_inventoryWeight.x);
                m_dragOffset = new Vector2(m_dragOffset.x * -(m_itemData.m_inventoryWeight.y - 1) * .5f, m_dragOffset.y * (m_itemData.m_inventoryWeight.x + 1)* .75f);
                break;
        }
    }


    public void UpdateIcon(ResourceData p_heldData)
    {
        m_itemData = p_heldData;
        m_itemIcon.sprite = p_heldData.m_resourceSprite;
        m_previousGridPos = Vector2Int.zero;
        m_startingCoordPos = Vector3.zero;
        m_rotatedDir = Inventory_2DMenu.RotationType.Left;
        AdjustedDraggingOffset();
    }

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

    public void ForceIconDrop()
    {
        m_rotatedDir = m_previousRotType;
        AdjustedDraggingOffset();
        StopAllCoroutines();
        m_itemIcon.raycastTarget = true;
    }
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
        AdjustedDraggingOffset();
    }
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

}
