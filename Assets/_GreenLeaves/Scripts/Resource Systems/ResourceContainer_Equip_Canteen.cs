
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceData_Equip_Canteen", menuName = "ScriptableObjects/ResourceData_Equip_Canteen", order = 0)]
public class ResourceContainer_Equip_Canteen : ResourceContainer_Equip
{
    public enum CanteenFillType { Water, BerryJuice, Tea}
    public CanteenFillType m_fillType;
}
