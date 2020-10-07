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

        for (int y = 0; y < m_rowSlots.Count; y++)
        {
            for (int x = 0; x < m_rowSlots[y].m_gridCells.Count; x++)
            {
                m_rowSlots[y].m_gridCells[x].GetComponent<Inventory_SlotDetector>().m_gridPos = new Vector2Int(x, y);
            }
        }
    }



    public bool CanAddToRow(ResourceData p_data, Inventory_2DMenu.RotationType p_currentRotationType)
    {
        if (m_currentAmount > m_maxCapacity) return false;
        //if (p_data.m_inventoryWeight.x * p_data.m_inventoryWeight.y + m_currentAmount > m_maxCapacity + 1) return false;



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

    public void AddNewIcon(ResourceData p_data, Inventory_2DMenu.RotationType p_rotationType, Inventory_Icon p_iconObject, out Vector2Int p_placement)
    {


        p_iconObject.transform.localScale = Vector3.one;
        p_iconObject.transform.localPosition = Vector3.zero;
        p_iconObject.transform.localRotation = Quaternion.identity;


        p_placement = m_newPlacement;
        PlaceIcon(p_iconObject,p_placement,p_data, p_rotationType);

        

    }

    public void AddWeight(ResourceData p_data)
    {
        m_currentAmount += (p_data.m_inventoryWeight.x * p_data.m_inventoryWeight.y);
    }

    public void RemoveWeight(ResourceData p_data)
    {
        m_currentAmount -= (p_data.m_inventoryWeight.x * p_data.m_inventoryWeight.y);
    }


    public void PlaceIcon(Inventory_Icon p_icon, Vector2Int p_placement,ResourceData p_data, Inventory_2DMenu.RotationType p_rotateType)
    {
        Vector2 pos =  m_rowSlots[p_placement.y].m_gridCells[p_placement.x].transform.position; ;
        switch (p_rotateType)
        {

            #region Left Placement
            case Inventory_2DMenu.RotationType.Left:
                pos = new Vector2(pos.x + (Mathf.Sign(pos.x)) * (m_gridIconSize.x * (p_data.m_inventoryWeight.x - 1) * .5f),
                                  pos.y + (Mathf.Sign(-pos.y)) * (m_gridIconSize.y * (p_data.m_inventoryWeight.y - 1) * .5f));

                p_icon.transform.position = pos;

                for (int y = p_placement.y; y < p_placement.y + p_data.m_inventoryWeight.y; y++)
                {
                    for (int x = p_placement.x; x < p_placement.x + p_data.m_inventoryWeight.x; x++)
                    {

                        m_itemGrids[y].m_itemGrids[x] = p_icon.GetComponent<Inventory_Icon>();
                    }
                }
                break;

            #endregion

            #region Down Placement
            case Inventory_2DMenu.RotationType.Down:
                pos = new Vector2(pos.x + (Mathf.Sign(pos.x)) * (m_gridIconSize.x * (p_data.m_inventoryWeight.y - 1) * .5f),
                  pos.y + (Mathf.Sign(-pos.y)) * (m_gridIconSize.y * (p_data.m_inventoryWeight.x - 1) * .5f));

                p_icon.transform.position = pos;

                for (int y = p_placement.y; y < p_placement.y + p_data.m_inventoryWeight.x; y++)
                {
                    for (int x = p_placement.x; x < p_placement.x + p_data.m_inventoryWeight.y; x++)
                    {

                        m_itemGrids[y].m_itemGrids[x] = p_icon.GetComponent<Inventory_Icon>();
                    }
                }
                break;
            #endregion

            #region Right Placement
            case Inventory_2DMenu.RotationType.Right:
                Debug.LogError("Hit");
                pos = new Vector2(pos.x + (Mathf.Sign(pos.x)) * (m_gridIconSize.x * -(p_data.m_inventoryWeight.x - 1) * .5f),
                                  pos.y + (Mathf.Sign(-pos.y)) * (m_gridIconSize.y * (p_data.m_inventoryWeight.y - 1) * .5f));

                p_icon.transform.position = pos;

                for (int y = p_placement.y; y < p_placement.y - p_data.m_inventoryWeight.y; y--)
                {
                    for (int x = p_placement.x; x < p_placement.x - p_data.m_inventoryWeight.x; x--)
                    {

                        m_itemGrids[y].m_itemGrids[x] = p_icon.GetComponent<Inventory_Icon>();
                    }
                }
                break;

            #endregion

            #region Up Placement
            case Inventory_2DMenu.RotationType.Up:
                pos = new Vector2(pos.x + (Mathf.Sign(pos.x)) * (m_gridIconSize.x * (p_data.m_inventoryWeight.y - 1) * .5f),
                  pos.y + (Mathf.Sign(-pos.y)) * (m_gridIconSize.y * -(p_data.m_inventoryWeight.x - 1) * .5f));

                p_icon.transform.position = pos;

                for (int y = p_placement.y; y < p_placement.y - p_data.m_inventoryWeight.x; y--)
                {
                    for (int x = p_placement.x; x < p_placement.x - p_data.m_inventoryWeight.y; x--)
                    {

                        m_itemGrids[y].m_itemGrids[x] = p_icon.GetComponent<Inventory_Icon>();
                    }
                }
                break;
                #endregion
        }


    }
    
    
    
    #endregion

    #region Grid Dragging

    public bool CanPlaceHere(Vector2Int p_gridPos, Vector2Int p_gridWeight, Inventory_2DMenu.RotationType p_rotatedType)
    {
        switch (p_rotatedType)
        {
            case Inventory_2DMenu.RotationType.Left:
                if (p_gridPos.y + p_gridWeight.y-1 > m_itemGrids.Count || p_gridPos.x + p_gridWeight.x-1 > m_itemGrids[0].m_itemGrids.Count) return false;

                for (int y = p_gridPos.y; y < p_gridPos.y + p_gridWeight.y; y++)
                {
                    for (int x = p_gridPos.x; x < p_gridPos.x + p_gridWeight.x; x++)
                    {
                        if (m_itemGrids[y].m_itemGrids[x] != null)
                        {
                            return false;
                        }
                    }
                }
                return true;
                break;
            case Inventory_2DMenu.RotationType.Down:
                if (p_gridPos.y + p_gridWeight.x -1> m_itemGrids.Count || p_gridPos.x + p_gridWeight.y -1> m_itemGrids[0].m_itemGrids.Count) return false;

                for (int y = p_gridPos.y; y < p_gridPos.y + p_gridWeight.x; y++)
                {
                    for (int x = p_gridPos.x; x < p_gridPos.x + p_gridWeight.y; x++)
                    {
                        if (m_itemGrids[y].m_itemGrids[x] != null)
                        {
                            return false;
                        }
                    }
                }
                return true;
                break;

            case Inventory_2DMenu.RotationType.Right:
               if (p_gridPos.y - (p_gridWeight.y-1) < 0 || p_gridPos.x - (p_gridWeight.x-1) < 0) return false;

                for (int y = p_gridPos.y; y < p_gridPos.y - p_gridWeight.y; y--)
                {
                    for (int x = p_gridPos.x; x < p_gridPos.x - p_gridWeight.x; x--)
                    {
                        if (m_itemGrids[y].m_itemGrids[x] != null)
                        {
                            return false;
                        }
                    }
                }
                return true;


                break;
            case Inventory_2DMenu.RotationType.Up:
                if (p_gridPos.y + p_gridWeight.x -1> m_itemGrids.Count || p_gridPos.x + p_gridWeight.y -1> m_itemGrids[0].m_itemGrids.Count) return false;

                for (int y = p_gridPos.y; y < p_gridPos.y - p_gridWeight.x; y--)
                {
                    for (int x = p_gridPos.x; x < p_gridPos.x - p_gridWeight.y; x--)
                    {
                        if (m_itemGrids[y].m_itemGrids[x] != null)
                        {
                            return false;
                        }
                    }
                }
                return true;
                break;
        }
        Debug.LogError("Null case found while trying to place icon");
        return false;

    }

    public void ClearOldPos(Vector2Int p_gridPos, Vector2Int p_gridWeight, Inventory_2DMenu.RotationType p_rotatedDir)
    {
        for (int y = p_gridPos.y; y < p_gridPos.y + p_gridWeight.y; y++)
        {
            for (int x = p_gridPos.x; x < p_gridPos.x + p_gridWeight.x; x++)
            {
                Debug.Log("Placement: " + x + " | " + y);
                Debug.LogError("Out of range index here");
                m_itemGrids[y].m_itemGrids[x] = null;
            }
        }
    }

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

