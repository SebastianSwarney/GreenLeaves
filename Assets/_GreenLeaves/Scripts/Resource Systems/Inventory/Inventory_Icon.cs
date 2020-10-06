using UnityEngine;
using UnityEngine.UI;
public class Inventory_Icon : MonoBehaviour
{
    public ResourceData m_itemData;
    public Image m_itemIcon;

    public Inventory_Grid m_previousRow;
    public int m_iconGridSize = 1;
    public void UpdateIcon(ResourceData p_heldData)
    {
        m_itemData = p_heldData;
        m_itemIcon.sprite = p_heldData.m_resourceSprite;
    }

    public void IconTappedOn()
    {
        Inventory_2DMenu.Instance.IconTappedOn(this);
    }


}
