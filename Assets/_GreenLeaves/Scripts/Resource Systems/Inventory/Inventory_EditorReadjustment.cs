using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Inventory_EditorReadjustment : MonoBehaviour
{
    public GameObject m_prefab;
    public Inventory_Grid m_grid;

    public Vector2Int m_newGridSize;
    public Vector2 m_slotSize = new Vector2(45, 45);


    [Button]
    public void AdjustGridSize()
    {
        int childCount = m_grid.transform.childCount;
        //Debug.Log("Child Count:" + m_grid.transform.childCount.ToString());
        for (int i = 0; i < childCount; i++)
        {
            DestroyImmediate(m_grid.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>());
            DestroyImmediate(m_grid.transform.GetChild(0).GetComponent<CanvasRenderer>());
            //Destroy(m_grid.transform.GetChild(0).GetComponent<RectTransform>());
            DestroyImmediate(m_grid.transform.GetChild(0).gameObject);
        }

        m_grid.m_itemGrids.Clear();
        m_grid.m_rowSlots.Clear();
        for (int y = 0; y < m_newGridSize.x; y++)
        {
            InventoryGrid currentSlot = new InventoryGrid();
            currentSlot.m_gridCells = new List<Transform>();
            InventoryRow currentRow = new InventoryRow();
            currentRow.m_itemGrids = new List<Inventory_Icon>();
            for (int x = 0; x < m_newGridSize.y; x++)
            {
                GameObject newSlot = GameObject.Instantiate(m_prefab);
                newSlot.transform.parent = m_grid.transform;
                currentSlot.m_gridCells.Add(newSlot.gameObject.transform);
                currentRow.m_itemGrids.Add(null);
                newSlot.GetComponent<RectTransform>().sizeDelta = m_slotSize;

                newSlot.GetComponent<RectTransform>().localPosition = new Vector3((x * m_slotSize.x - (m_slotSize.x / 2)), (y * m_slotSize.y - (m_slotSize.y / 2)), 0);
            }
            m_grid.m_rowSlots.Add(currentSlot);
            m_grid.m_itemGrids.Add(currentRow);
        }
    }
}
