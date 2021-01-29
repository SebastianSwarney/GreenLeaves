using UnityEngine;


[CreateAssetMenu(fileName = "ResourceData_Building_", menuName = "ScriptableObjects/ResourceData_Building", order = 0)]
public class ResourceContainer_Building : ResourceContainer
{
    public GameObject m_buildingPrefab;
    public override void UseItem(Inventory_Icon p_currentIcon)
    {
        //Inventory_2DMenu.Instance.m_currentBuldingIcon = p_currentIcon;
        Inventory_2DMenu.Instance.CloseInventoryMenu(true);
        //Inventory_2DMenu.Instance.ItemUsed(p_currentIcon);
        Building_PlayerPlacement.Instance.StartPlacement(m_buildingPrefab,this);
    }
}
