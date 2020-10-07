using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory_2DMenu : MonoBehaviour
{
    public static Inventory_2DMenu Instance;

    public Player_Inventory m_playerInventory;

    public GameObject m_canvasObject;
    public GameObject m_inventorySlotPrefab;

    public Inventory_Icon m_currentSelectedIcon;

    [Header("Inventory")]
    public BackpackInventory m_backpack;
    public enum RotationType { Right, Down, Left, Up }
    public RotationType m_currentRotationType = RotationType.Left;
    public KeyCode m_rotateKey = KeyCode.R;
    public bool m_isDraggingObject;

    private bool m_isOpen;

    [Header("UI Elements")]
    public Inventory_Grid m_inventoryGrid;
    public Transform m_gameIconsParent;
    public Transform m_cantFitIconPos;
    public Transform m_itemTransferParent;

    /// <summary>
    /// Used to determine where the player can drop the item until it snaps back automatically
    /// </summary>
    public GameObject m_backpackImageUi;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        m_inventoryGrid.Initialize();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckInventoryMouseDown();
        }else if (Input.GetKeyDown(m_rotateKey))
        {
            if (m_currentSelectedIcon == null) return;
            m_currentSelectedIcon.transform.Rotate(Vector3.forward, 90);
            m_currentSelectedIcon.RotateDir();
        }
        

        
    }

    #region Inventory Toggle
    public void ToggleInventory()
    {
        if (m_isOpen)
        {
            m_isOpen = false;
            if (m_isDraggingObject)
            {
                m_currentSelectedIcon.ForceIconDrop();
                CloseWhileHoldinObject(m_currentSelectedIcon);
            }

            CloseInventoryMenu();
            PlayerInputToggle.Instance.ToggleInput(true);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
        else
        {
            PlayerInputToggle.Instance.ToggleInput(false);
            m_isOpen = true;
            OpenInventoryMenu();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    public void OpenInventoryMenu()
    {
        m_currentSelectedIcon = null;
        m_isDraggingObject = false;
        m_canvasObject.SetActive(true);
    }



    public void CloseInventoryMenu()
    {
        DropAnyOutsideIcons();
        m_canvasObject.SetActive(false);
    }
    #endregion

    /// <summary>
    /// Adds the item to the inventory & spawns a new icon for it
    /// </summary>
    public void AddItemToInventory(GameObject p_pickedUp)
    {

        ResourceData pickedUpResource = p_pickedUp.GetComponent<Resource_Pickup>().m_myData;

        m_currentRotationType = RotationType.Left;


        BackpackSlot newSlot = new BackpackSlot();
        Inventory_Icon newIcon = ObjectPooler.Instance.NewObject(pickedUpResource.m_resourceIconPrefab, Vector3.zero, Quaternion.identity).GetComponent<Inventory_Icon>();
        newIcon.m_rotatedDir = RotationType.Left;
        newIcon.transform.parent = m_gameIconsParent;
        newIcon.UpdateIcon(pickedUpResource);

        
        newSlot.m_currentData = pickedUpResource;
        newSlot.m_associatedIcon = newIcon;
        m_backpack.m_itemsInBackpack.Add(newSlot);
        m_inventoryGrid.AddWeight(pickedUpResource);


        if (m_inventoryGrid.CanAddToRow(pickedUpResource, RotationType.Left))
        {
            Debug.Log("Fit Item");
            newIcon.m_inBackpack = true;
            Vector2Int placedPos;
            m_inventoryGrid.AddNewIcon(pickedUpResource, m_currentRotationType, newIcon, out placedPos);
            newIcon.m_previousGridPos = placedPos;
            newIcon.m_startingCoordPos = newIcon.transform.localPosition;
            
        }
        else
        {
            Debug.Log("Cant Fit Item: " + newIcon.gameObject.name, newIcon.gameObject);
            newIcon.m_inBackpack = false;
            newIcon.transform.localPosition = m_cantFitIconPos.localPosition;
            newIcon.m_startingCoordPos = newIcon.transform.localPosition;
            ToggleInventory();
        }
    }

    
    public void DropAnyOutsideIcons()
    {
        List<int> removeList = new List<int>();

        for (int i = 0; i < m_backpack.m_itemsInBackpack.Count; i++)
        {
            if (!m_backpack.m_itemsInBackpack[i].m_associatedIcon.m_inBackpack)
            {
                removeList.Add(i);
            }
        }

        removeList.Reverse();

        for (int i = 0; i < removeList.Count; i++)
        {
            m_playerInventory.DropObject(m_backpack.m_itemsInBackpack[removeList[i]].m_currentData.m_resourcePrefab);
            ObjectPooler.Instance.ReturnToPool(m_backpack.m_itemsInBackpack[removeList[i]].m_associatedIcon.gameObject);

            m_inventoryGrid.RemoveWeight(m_backpack.m_itemsInBackpack[removeList[i]].m_currentData);

            m_backpack.m_itemsInBackpack.RemoveAt(removeList[i]);
        }
    }



    public void EquipItem()
    {
        Debug.Log("Equip Item");
    }


    public void IconTappedOn(Inventory_Icon p_tappedOn)
    {
        p_tappedOn.transform.parent = m_itemTransferParent;
        m_currentSelectedIcon = p_tappedOn;
        m_isDraggingObject = true;
    }

    public void CheckInventoryMouseDown()
    {
        List<RaycastResult> hitUI = new List<RaycastResult>();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = 1,
        };
        pointerData.position = Input.mousePosition;

        EventSystem.current.RaycastAll(pointerData, hitUI);


        foreach (RaycastResult res in hitUI)
        {
            if (res.gameObject.GetComponent<Inventory_Icon>() != null)
            {
                res.gameObject.GetComponent<Inventory_Icon>().IconTappedOn();
                return;
            }
        }
    }


    public void CloseWhileHoldinObject(Inventory_Icon p_holdingIcon)
    {
        m_isDraggingObject = false;
        m_currentSelectedIcon = null;

        if (p_holdingIcon.m_inBackpack)
        {
            p_holdingIcon.m_inBackpack = true;
            //Remember to re-rotate the icon back to it's og rotation
            m_inventoryGrid.PlaceIcon(p_holdingIcon, p_holdingIcon.m_previousGridPos, p_holdingIcon.m_itemData, p_holdingIcon.m_rotatedDir);
        }
        else
        {
            p_holdingIcon.m_inBackpack = false;

            p_holdingIcon.transform.localPosition = p_holdingIcon.m_startingCoordPos;
        }
    }

    public void CheckIconPlacePosition(Inventory_Icon p_holdingIcon)
    {
        m_isDraggingObject = false;
        m_currentSelectedIcon = null;
        List<RaycastResult> hitUI = new List<RaycastResult>();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = 1,
        };
        pointerData.position = Input.mousePosition;

        EventSystem.current.RaycastAll(pointerData, hitUI);

        Vector2Int newPlace = Vector2Int.zero;
        bool placedIcon = false;

        bool snapBack = false;



        foreach (RaycastResult res in hitUI)
        {
            if (res.gameObject != p_holdingIcon)
            {

                ///If an item already exists in that position
                if (res.gameObject.GetComponent<Inventory_Icon>() != null)
                {
                    newPlace = Vector2Int.zero;
                    placedIcon = false;
                }

                ///If the player lets go on the grid
                if (res.gameObject.GetComponent<Inventory_SlotDetector>() != null)
                {
                    
                    ///Checks if the item can fit there
                    if (m_inventoryGrid.CanPlaceHere(res.gameObject.GetComponent<Inventory_SlotDetector>().m_gridPos, p_holdingIcon.m_itemData.m_inventoryWeight, p_holdingIcon.m_rotatedDir))
                    {
                        newPlace = res.gameObject.GetComponent<Inventory_SlotDetector>().m_gridPos;
                        Debug.Log("Icon Hit");
                        placedIcon = true;
                    }
                }

                ///If the player lets go while on the backpack
                if (res.gameObject == m_backpackImageUi)
                {
                    snapBack = true;

                }
            }
        }

        p_holdingIcon.transform.parent = m_gameIconsParent;
        if (!placedIcon)
        {

            if (snapBack)
            {
                if (p_holdingIcon.m_inBackpack)
                {
                    p_holdingIcon.m_inBackpack = true;
                    //Remember to re-rotate the icon back to it's og rotation
                    Debug.LogError("Start Here");
                    m_inventoryGrid.PlaceIcon(p_holdingIcon, p_holdingIcon.m_previousGridPos, p_holdingIcon.m_itemData, p_holdingIcon.m_rotatedDir);
                }
                else
                {
                    Debug.LogError("Failed");
                    p_holdingIcon.m_inBackpack = false;
                    p_holdingIcon.ForceIconDrop();
                    p_holdingIcon.transform.localPosition = p_holdingIcon.m_startingCoordPos;
                }
            }
            else
            {
                Debug.LogError("Failed");
                p_holdingIcon.m_inBackpack = false;
            }
            return;
        }

        p_holdingIcon.m_inBackpack = true;
        Debug.LogError("Start Here");
        m_inventoryGrid.PlaceIcon(p_holdingIcon, newPlace, p_holdingIcon.m_itemData, p_holdingIcon.m_rotatedDir);
        p_holdingIcon.m_previousGridPos = newPlace;
        p_holdingIcon.m_startingCoordPos = p_holdingIcon.transform.localPosition;


    }

    public void ClearGridPosition(Vector2Int p_gridPos, Vector2Int p_gridWeight, RotationType p_rotatedDir)
    {
        m_inventoryGrid.ClearOldPos(p_gridPos, p_gridWeight, p_rotatedDir);
    }
}


[System.Serializable]
public class BackpackInventory
{
    public List<BackpackSlot> m_itemsInBackpack;
}

[System.Serializable]
public class BackpackSlot
{
    public ResourceData m_currentData;
    public Inventory_Icon m_associatedIcon;
}

[System.Serializable]
public class HeldIcons
{
    public Inventory_Icon m_currentIcon;
    public Vector2Int m_gridPosition;
}
[System.Serializable]
public class ResourceData
{
    public string m_resourceName;
    public string m_resourceDetails;
    public Sprite m_resourceSprite;
    public GameObject m_resourcePrefab;
    public GameObject m_resourceIconPrefab;
    public Vector2Int m_inventoryWeight;

    public ResourceData(ResourceData p_newData = null)
    {
        if (p_newData != null)
        {
            m_resourceName = p_newData.m_resourceName;
            m_resourceSprite = p_newData.m_resourceSprite;
            m_resourcePrefab = p_newData.m_resourcePrefab;
            m_resourceIconPrefab = p_newData.m_resourceIconPrefab;
            m_inventoryWeight = p_newData.m_inventoryWeight;
        }
    }
}




