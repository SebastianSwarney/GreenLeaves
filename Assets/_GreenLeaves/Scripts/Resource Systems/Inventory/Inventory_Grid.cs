using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The script used to handle the grid that the inventory icons are placed in.
/// </summary>
public class Inventory_Grid : MonoBehaviour
{

    /// <summary>
    /// This is a list of a data type that holds the transforms of each cell in the grid.
    /// Used to easily place the icons in the grid
    /// </summary>
    [Header("Grid Properties")]
    public List<InventoryGrid> m_rowSlots;

    public int m_currentAmount;
    public int m_maxCapacity;
    public Vector2 m_gridIconSize;

    /// <summary>
    /// The rows of the grids. This represents the Y when refering to grid position
    /// </summary>
    [Header("Contained Items")]
    public List<InventoryRow> m_itemGrids;



    private Vector2Int m_newPlacement;


    /// <summary>
    /// Called to initialize the grid. Since the object is disabled on start,
    /// this is called from the Inventory_2DMenu script, upon it's start function
    /// </summary>
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



    /// <summary>
    /// Called when trying to place an icon.
    /// Takes in the data of the resource, and utilizes it's weight vector2 in order to determine how many spots the
    /// icon will takeup. 
    /// If the icon will fit, returns true, if not, returns false.
    /// The rotation type is taken into account, as the item can be rotated, and the search function adjusts accordingly.
    /// 
    /// Called only when the icon is initially placed
    /// </summary>
    public bool CanAddToRow(ResourceData p_data, Inventory_2DMenu.RotationType p_currentRotationType)
    {
        if (m_currentAmount > m_maxCapacity) return false;
        int curX = 0, curY = 0;
        for (int y = 0; y < m_itemGrids.Count; y++)
        {
            curY = y;
            for (int x = 0; x < m_itemGrids[y].m_itemGrids.Count; x++)
            {
                bool canFit = true;

                curX = x;
                for (int i = 0; i < ((p_currentRotationType == Inventory_2DMenu.RotationType.Left) ? p_data.m_inventoryWeight.y : p_data.m_inventoryWeight.x); i++)
                {
                    if (y + i < m_itemGrids.Count)
                    {

                        for (int o = 0; o < ((p_currentRotationType == Inventory_2DMenu.RotationType.Left) ? p_data.m_inventoryWeight.x : p_data.m_inventoryWeight.y); o++)
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

    /// <summary>
    /// Sets up the icon for being placed into the grid.
    /// Calls the PlaceIcon script to actually place the icon on the grid properly
    /// </summary>

    public void AddNewIcon(ResourceData p_data, Inventory_2DMenu.RotationType p_rotationType, Inventory_Icon p_iconObject, out Vector2Int p_placement)
    {


        p_iconObject.transform.localScale = Vector3.one;
        p_iconObject.transform.localPosition = Vector3.zero;
        //p_iconObject.transform.localRotation = Quaternion.identity;


        p_placement = m_newPlacement;
        PlaceIcon(p_iconObject, p_placement, p_data, p_rotationType);



    }


    /// <summary>
    /// Used to place the icons into the grid.
    /// As the icon can be rotated in different ways, the placing function adjusts accoridingly.
    /// (Have fun if anyone else tries to go through this)
    /// </summary>
    public void PlaceIcon(Inventory_Icon p_icon, Vector2Int p_placement, ResourceData p_data, Inventory_2DMenu.RotationType p_rotateType)
    {
        Vector2 pos = m_rowSlots[p_placement.y].m_gridCells[p_placement.x].transform.position; ;
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
                pos = new Vector2(pos.x + (Mathf.Sign(pos.x)) * (m_gridIconSize.x * -(p_data.m_inventoryWeight.y - 1) * .5f),
                                  pos.y + (Mathf.Sign(-pos.y)) * (m_gridIconSize.y * (p_data.m_inventoryWeight.x - 1) * .5f));

                p_icon.transform.position = pos;

                for (int y = p_placement.y; y <= p_placement.y + (p_data.m_inventoryWeight.x - 1); y++)
                {
                    for (int x = p_placement.x; x >= p_placement.x - (p_data.m_inventoryWeight.y - 1); x--)
                    {

                        m_itemGrids[y].m_itemGrids[x] = p_icon.GetComponent<Inventory_Icon>();
                    }
                }
                break;
            #endregion

            #region Right Placement
            case Inventory_2DMenu.RotationType.Right:
                pos = new Vector2(pos.x + (Mathf.Sign(pos.x)) * (m_gridIconSize.x * -(p_data.m_inventoryWeight.x - 1) * .5f),
                                  pos.y + (Mathf.Sign(-pos.y)) * (m_gridIconSize.y * -(p_data.m_inventoryWeight.y - 1) * .5f));

                p_icon.transform.position = pos;

                for (int y = p_placement.y; y >= p_placement.y - (p_data.m_inventoryWeight.y - 1); y--)
                {
                    for (int x = p_placement.x; x >= p_placement.x - (p_data.m_inventoryWeight.x - 1); x--)
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

                for (int y = p_placement.y; y >= p_placement.y - (p_data.m_inventoryWeight.x - 1); y--)
                {
                    for (int x = p_placement.x; x <= p_placement.x + (p_data.m_inventoryWeight.y - 1); x++)
                    {

                        m_itemGrids[y].m_itemGrids[x] = p_icon.GetComponent<Inventory_Icon>();
                    }
                }
                break;
                #endregion
        }


    }

    /// <summary>
    /// Adds the weight of the item to the current backpack weight
    /// </summary>

    public void AddWeight(ResourceData p_data)
    {
        m_currentAmount += (p_data.m_inventoryWeight.x * p_data.m_inventoryWeight.y);
    }

    /// <summary>
    /// Removes the weight of the item from the current backpack weight
    /// </summary>

    public void RemoveWeight(ResourceData p_data)
    {
        m_currentAmount -= (p_data.m_inventoryWeight.x * p_data.m_inventoryWeight.y);
    }






    #endregion

    #region Grid Dragging

    /// <summary>
    /// Performs a check on whether the icon can be placed in the given area.
    /// As icons can be greater than 1x1, this function performs the check to see if the
    /// entire icon can fit into the space. The icon's size is represented by the Resource_Data's Inventory Weight type
    /// </summary>
    public bool CanPlaceHere(Vector2Int p_gridPos, Vector2Int p_gridWeight, Inventory_2DMenu.RotationType p_rotatedType)
    {
        switch (p_rotatedType)
        {

            #region Rotated Left
            case Inventory_2DMenu.RotationType.Left:
                if (p_gridPos.y + (p_gridWeight.y - 1) >= m_itemGrids.Count || p_gridPos.x + (p_gridWeight.x - 1) >= m_itemGrids[0].m_itemGrids.Count) return false;

                for (int y = p_gridPos.y; y <= p_gridPos.y + (p_gridWeight.y - 1); y++)
                {
                    for (int x = p_gridPos.x; x <= p_gridPos.x + (p_gridWeight.x - 1); x++)
                    {
                        if (m_itemGrids[y].m_itemGrids[x] != null)
                        {
                            return false;
                        }
                    }
                }
                return true;
                break;
            #endregion

            #region Rotated Down
            case Inventory_2DMenu.RotationType.Down:
                if (p_gridPos.y + (p_gridWeight.x - 1) >= m_itemGrids.Count || p_gridPos.x - (p_gridWeight.y - 1) < 0) return false;
                for (int y = p_gridPos.y; y <= p_gridPos.y + (p_gridWeight.x - 1); y++)
                {
                    for (int x = p_gridPos.x; x >= p_gridPos.x - (p_gridWeight.y - 1); x--)
                    {
                        if (m_itemGrids[y].m_itemGrids[x] != null)
                        {
                            return false;
                        }
                    }
                }
                return true;
                break;

            #endregion

            #region Rotated Right
            case Inventory_2DMenu.RotationType.Right:
                if (p_gridPos.y - (p_gridWeight.y - 1) < 0 || p_gridPos.x - (p_gridWeight.x - 1) < 0) return false;

                for (int y = p_gridPos.y; y >= p_gridPos.y - (p_gridWeight.y - 1); y--)
                {
                    for (int x = p_gridPos.x; x >= p_gridPos.x - (p_gridWeight.x - 1); x--)
                    {
                        if (m_itemGrids[y].m_itemGrids[x] != null)
                        {
                            return false;
                        }
                    }
                }
                return true;
                break;
            #endregion

            #region Rotated Up
            case Inventory_2DMenu.RotationType.Up:
                if (p_gridPos.y - (p_gridWeight.x - 1) < 0 || p_gridPos.x + (p_gridWeight.y - 1) >= m_itemGrids[0].m_itemGrids.Count) return false;

                for (int y = p_gridPos.y; y >= p_gridPos.y - (p_gridWeight.x - 1); y--)
                {
                    for (int x = p_gridPos.x; x <= p_gridPos.x + (p_gridWeight.y - 1); x++)
                    {
                        if (m_itemGrids[y].m_itemGrids[x] != null)
                        {
                            return false;
                        }
                    }
                }
                return true;
                break;
                #endregion
        }
        Debug.LogError("Null case found while trying to place icon");
        return false;

    }


    /// <summary>
    /// Clears the backpack' data of the current held icon. Changes it to null.
    /// Is called when the icon is tapped on, and dragging initialized
    /// </summary>
    public void ClearOldPos(Vector2Int p_gridPos, Vector2Int p_gridWeight, Inventory_2DMenu.RotationType p_rotatedDir)
    {

        switch (p_rotatedDir)
        {

            #region Rotated Left
            case Inventory_2DMenu.RotationType.Left:
                for (int y = p_gridPos.y; y <= p_gridPos.y + (p_gridWeight.y - 1); y++)
                {
                    for (int x = p_gridPos.x; x <= p_gridPos.x + (p_gridWeight.x - 1); x++)
                    {
                        m_itemGrids[y].m_itemGrids[x] = null;
                    }
                }
                break;
            #endregion


            #region Rotated Down
            case Inventory_2DMenu.RotationType.Down:
                for (int y = p_gridPos.y; y <= p_gridPos.y + (p_gridWeight.x - 1); y++)
                {
                    for (int x = p_gridPos.x; x >= p_gridPos.x - (p_gridWeight.y - 1); x--)
                    {
                        m_itemGrids[y].m_itemGrids[x] = null;
                    }
                }
                break;
            #endregion

            #region Rotated Right

            case Inventory_2DMenu.RotationType.Right:
                for (int y = p_gridPos.y; y >= p_gridPos.y - (p_gridWeight.y - 1); y--)
                {
                    for (int x = p_gridPos.x; x >= p_gridPos.x - (p_gridWeight.x - 1); x--)
                    {
                        m_itemGrids[y].m_itemGrids[x] = null;
                    }
                }
                break;
            #endregion

            #region Rotated Up

            case Inventory_2DMenu.RotationType.Up:
                for (int y = p_gridPos.y; y >= p_gridPos.y - (p_gridWeight.x - 1); y--)
                {
                    for (int x = p_gridPos.x; x <= p_gridPos.x + (p_gridWeight.y - 1); x++)
                    {
                        m_itemGrids[y].m_itemGrids[x] = null;
                    }
                }
                break;

                #endregion
        }

    }

    public List<Inventory_Icon> GetExistingIconsOfResource(ResourceData p_dataType)
    {
        List<Inventory_Icon> m_currentIcons = new List<Inventory_Icon>();
        for (int y = 0; y < m_itemGrids.Count; y++)
        {
            for (int x = 0; x < m_itemGrids[y].m_itemGrids.Count; x++)
            {
                if (m_itemGrids[y].m_itemGrids[x] == null) continue;
                if(m_itemGrids[y].m_itemGrids[x].m_itemData.m_resourceName == p_dataType.m_resourceName)
                {
                    if (!m_currentIcons.Contains(m_itemGrids[y].m_itemGrids[x]))
                    {
                        m_currentIcons.Add(m_itemGrids[y].m_itemGrids[x]);
                    }
                }
            }
        }

        return m_currentIcons;
    }
    #endregion

}

/// <summary>
/// A row in the grid. This is represented as the X variable when going through the grid
/// </summary>
[System.Serializable]
public class InventoryRow
{
    public int m_currentRowSize;
    public int m_maxRowAvailability;
    public List<Inventory_Icon> m_itemGrids;
}


/// <summary>
/// This is a container used to hold the transforms of the grid palcements.
/// Used to easily place an item on the grid.
/// </summary>
[System.Serializable]
public class InventoryGrid
{
    public List<Transform> m_gridCells;
}


