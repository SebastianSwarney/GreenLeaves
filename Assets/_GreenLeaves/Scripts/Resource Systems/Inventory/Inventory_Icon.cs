using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Inventory_Icon : MonoBehaviour
{
    public ResourceData m_itemData;
    public Image m_itemIcon;

    public Vector2Int m_previousPosition;
    public bool m_inBackpack = false;

    private Coroutine m_waitingForMouseUp;

    public Inventory_2DMenu.RotationType m_rotatedDir = Inventory_2DMenu.RotationType.Left;
    public void UpdateIcon(ResourceData p_heldData)
    {
        m_itemData = p_heldData;
        m_itemIcon.sprite = p_heldData.m_resourceSprite;
        m_previousPosition = Vector2Int.zero;
        
    }

    public void IconTappedOn()
    {
        Debug.Log("Tapping");
        StartCoroutine(WaitForMouseUp());
        Inventory_2DMenu.Instance.ClearGridPosition(m_previousPosition, m_itemData.m_inventoryWeight, m_rotatedDir);
        //Inventory_2DMenu.Instance.IconTappedOn(this);
    }

    private IEnumerator WaitForMouseUp()
    {
        bool beingHeld = true;
        m_itemIcon.raycastTarget = false;
        Vector2 offset = GetComponent<RectTransform>().sizeDelta;

        offset = new Vector2(offset.x / m_itemData.m_inventoryWeight.x, offset.y / m_itemData.m_inventoryWeight.y);
        offset = new Vector2(offset.x * (m_itemData.m_inventoryWeight.x - 1) * .5f, offset.y * -(m_itemData.m_inventoryWeight.y - 1) * .5f);
        while (beingHeld)
        {
            if (Input.GetMouseButtonUp(0))
            {
                beingHeld = false;
            }
            transform.position = Input.mousePosition + (Vector3)offset;
            yield return null;
        }

        Debug.Log("Tapping End");
        ///Do the raycast check here
        Inventory_2DMenu.Instance.CheckIconPlacePosition(this);
        m_itemIcon.raycastTarget = true;
    }

}
