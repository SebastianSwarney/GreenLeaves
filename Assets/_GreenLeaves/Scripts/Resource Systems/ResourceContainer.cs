using UnityEngine;

/// <summary>
/// The data container that contains the data of the objects.<br/>
/// Includes edibles, equipables, and canteen
/// </summary>
[CreateAssetMenu(fileName = "ResourceData_", menuName = "ScriptableObjects/ResourceData", order = 0)]
public class ResourceContainer : ScriptableObject
{
    public ResourceData m_resourceData;
    public string m_itemUseButtonText;
    public bool m_showInventorySelectionButton;

    public virtual void UseItem(Inventory_Icon p_currentIcon)
    {
        Debug.Log("Use Item");
    }

    public virtual GameObject DropObject(Inventory_Icon p_icon, Vector3 p_pos, Quaternion p_rot)
    {
        GameObject dropped = ObjectPooler.Instance.NewObject(m_resourceData.m_resourcePrefab, p_pos, p_rot);

        dropped.GetComponent<Resource_Pickup>().m_canPickup = true;
        return dropped;
    }
}

/// <summary>
/// The main data container used to manage the resources.
/// </summary>
[System.Serializable]
public class ResourceData
{
    public string m_resourceName;
    [Tooltip("Determines how many of this resources corelates to a single icon. IE. 10 arrows = 1 icon")]
    public int m_singleResourceAmount = 1;
    public Sprite m_resourceSprite;
    public GameObject m_resourcePrefab;
    public Vector2Int m_inventoryWeight;

    //Used to determine which way the icon should be rotated when initially being placed.
    public Inventory_2DMenu.RotationType m_iconStartingRotation = Inventory_2DMenu.RotationType.Left;

    public ResourceData(ResourceData p_newData = null)
    {
        if (p_newData != null)
        {
            m_resourceName = p_newData.m_resourceName;
            m_resourceSprite = p_newData.m_resourceSprite;
            m_resourcePrefab = p_newData.m_resourcePrefab;
            m_inventoryWeight = p_newData.m_inventoryWeight;
        }
    }
}
