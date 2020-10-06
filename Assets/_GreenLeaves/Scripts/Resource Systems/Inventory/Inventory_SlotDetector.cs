using UnityEngine;

public class Inventory_SlotDetector : MonoBehaviour
{
    private int m_childIndex;
    public Inventory_Grid m_associatedRow;
    private void Start()
    {
        m_childIndex = transform.GetSiblingIndex();
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
