using UnityEngine;


[CreateAssetMenu(fileName = "ResourceData_Equip_", menuName = "ScriptableObjects/ResourceData_Equip", order = 0)]
public class ResourceContainer_Equip : ResourceContainer
{
    public override void UseItem(Inventory_Icon p_currentIcon)
    {
        GameObject newItem = ObjectPooler.Instance.NewObject(m_resourceData.m_resourcePrefab, Vector3.zero, Quaternion.identity);
        newItem.GetComponent<Player_EquipmentUse>().m_durability = p_currentIcon.GetComponent<Inventory_Icon_Durability>().m_durabilityAmount;
        Inventory_ItemUsage.Instance.EquipNewItem(p_currentIcon, this, newItem);
    }

    public override GameObject DropObject(Inventory_Icon p_icon, Vector3 p_pos, Quaternion p_rot)
    {
        GameObject dropped = base.DropObject(p_icon, p_pos, p_rot);
        dropped.GetComponent<Player_EquipmentUse>().enabled = false;

        if (p_icon.GetComponent<Inventory_Icon_Durability>() != null)
        {
            dropped.GetComponent<Player_EquipmentUse>().m_durability = p_icon.GetComponent<Inventory_Icon_Durability>().m_durabilityAmount;
        }

        return dropped;
    }

    /// <summary>
    /// Undo any stat changes here
    /// </summary>
    public void ItemUnequipped()
    {

    }

}
