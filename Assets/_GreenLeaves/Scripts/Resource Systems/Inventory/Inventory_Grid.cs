using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Grid : MonoBehaviour
{


    public List<InventoryGrid> m_rowSlots;
    [Header("Contained Items")]
    public List<InventoryRow> m_itemGrids;

    public Vector2Int m_newPlacement;

    public int m_currentAmount;
    public int m_maxCapacity;
    public Vector2 m_gridIconSize;
    public void Initialize()
    {
        m_maxCapacity = m_rowSlots[0].m_gridCells.Count * m_rowSlots.Count;
        m_gridIconSize = m_rowSlots[0].m_gridCells[0].GetComponent<RectTransform>().sizeDelta;
    }



    public bool CanAddToRow(ResourceData p_data, Inventory_2DMenu.RotationType p_currentRotationType)
    {
        if (m_currentAmount == m_maxCapacity) return false;
        if (p_data.m_inventoryWeight.x * p_data.m_inventoryWeight.y + m_currentAmount > m_maxCapacity) return false;



        int curX = 0, curY = 0;
        for (int y = 0; y < m_itemGrids.Count; y++)
        {
            curY = y;
            for (int x = 0; x < m_itemGrids[y].m_itemGrids.Count; x++)
            {
                bool canFit = true;

                curX = x;
                for (int i = 0; i < p_data.m_inventoryWeight.y; i++)
                {
                    if (y + i < m_itemGrids.Count)
                    {


                        for (int o = 0; o < p_data.m_inventoryWeight.x; o++)
                        {
                            if (o + x < m_itemGrids[y].m_itemGrids.Count)
                            {
                                if (m_itemGrids[y + i].m_itemGrids[x + o] != null)
                                {
                                    canFit = false;
                                    break;
                                }
                            }
                            else
                            {
                                canFit = false;
                            }
                        }
                        if (!canFit)
                        {
                            break;
                        }
                    }
                    else
                    {
                        canFit = false;
                    }

                }
                if (canFit)
                {
                    m_newPlacement = new Vector2Int(curX, curY);
                    return true;
                }
            }
        }
        return false;
    }

    #region Adding a new icon
    public void AddNewIcon(ResourceData p_data, Inventory_2DMenu.RotationType p_rotationType, Transform p_iconParent)
    {

        Inventory_Icon createdIcon = ObjectPooler.Instance.NewObject(p_data.m_resourceIconPrefab, Vector3.zero, Quaternion.identity).GetComponent<Inventory_Icon>();
        createdIcon.transform.parent = p_iconParent;
        createdIcon.UpdateIcon(p_data);

        createdIcon.transform.parent = transform;
        createdIcon.transform.localScale = Vector3.one;
        createdIcon.transform.localPosition = Vector3.zero;
        createdIcon.transform.localRotation = Quaternion.identity;



        Vector2 pos = m_rowSlots[m_newPlacement.y].m_gridCells[m_newPlacement.x].transform.position;

        pos = new Vector2(pos.x +(Mathf.Sign(pos.x))* (m_gridIconSize.x * (p_data.m_inventoryWeight.x - 1) * .5f), 
                          pos.y + (Mathf.Sign(-pos.y)) * (m_gridIconSize.y * (p_data.m_inventoryWeight.y - 1) * .5f));

        createdIcon.transform.position = pos;

        for (int y = m_newPlacement.y; y < m_newPlacement.y+ p_data.m_inventoryWeight.y; y++)
        {
            for (int x = m_newPlacement.x; x < m_newPlacement.x + p_data.m_inventoryWeight.x; x++)
            {
                m_itemGrids[y].m_itemGrids[x] = createdIcon.GetComponent<Inventory_Icon>();
            }
        }

        m_currentAmount += (p_data.m_inventoryWeight.x * p_data.m_inventoryWeight.y);

    }

    #endregion

    #region Grid Dragging

    

    #endregion
}

[System.Serializable]
public class InventoryRow
{
    public int m_currentRowSize;
    public int m_maxRowAvailability;
    public List<Inventory_Icon> m_itemGrids;
}

[System.Serializable]
public class InventoryGrid
{
    public List<Transform> m_gridCells;
}

