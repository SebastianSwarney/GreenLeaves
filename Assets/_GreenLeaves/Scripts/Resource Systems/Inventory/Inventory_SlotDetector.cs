using UnityEngine;

public class Inventory_SlotDetector : MonoBehaviour
{

    public Vector2Int m_gridPos;

    public void InitializeSlot(Vector2Int p_gridPos)
    {
        m_gridPos = p_gridPos;
    }

    public void UpdatePositioning(Inventory_Icon p_icon, Inventory_2DMenu.RotationType p_currentRotationType, out bool p_itemPlaced)
    {

        /*if (m_associatedRow.CanPlaceIcon(m_childIndex, p_icon.m_iconGridSize))
        {
            if (m_associatedRow.CanAddToRow(p_icon.m_itemData, p_currentRotationType))
            {
                p_icon.transform.parent = m_associatedRow.transform;
                p_icon.transform.SetSiblingIndex(m_childIndex);
                p_itemPlaced = true;
                return;
            }

        }*/
        p_itemPlaced = false;
        return;
    }
}
