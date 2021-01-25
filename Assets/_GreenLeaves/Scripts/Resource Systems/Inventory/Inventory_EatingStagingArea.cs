using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_EatingStagingArea : MonoBehaviour
{
    public List<Inventory_Icon> m_edibles;

    public bool CanAddIconToEatingArea(Inventory_Icon p_icon)
    {
        if (!m_edibles.Contains(p_icon))
        {
            if (p_icon.m_itemData.m_isEdible)
            {
                m_edibles.Add(p_icon);
                return true;
            }
        }
        return false;
    }

    public void RemoveIcon(Inventory_Icon p_icon)
    {
        Debug.Log("Attempt Removal");
        if (m_edibles.Contains(p_icon))
        {
            Debug.Log("Removed");
            m_edibles.Remove(p_icon);
        }
    }

    public void EatEverything()
    {
        foreach(Inventory_Icon icon in m_edibles)
        {
            icon.m_itemData.UseItem(icon);
        }
        m_edibles.Clear();
    }


}
