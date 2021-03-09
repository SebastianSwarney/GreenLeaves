using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceData_Equip_Canteen", menuName = "ScriptableObjects/ResourceData_Equip_Canteen", order = 0)]
public class ResourceContainer_Equip_Canteen : ResourceContainer_Equip
{

    public List<Player_EquipmentUse_Canteen.RefillType> m_energyRefilType;
    public bool m_specialDrink;
    public override void UseItem(Inventory_Icon p_currentIcon)
    {
        base.UseItem(p_currentIcon);
        Player_EquipmentUse_Canteen.Instance.ChangeEdibleType(m_energyRefilType, m_specialDrink);

    }
}
