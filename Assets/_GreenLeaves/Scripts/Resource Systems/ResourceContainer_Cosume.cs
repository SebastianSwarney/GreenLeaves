using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The data container used for edible resources<br/>
/// IE. Berries, meat
/// </summary>
[CreateAssetMenu(fileName = "ResourceData_Consume_", menuName = "ScriptableObjects/ResourceData_Consume", order = 0)]
public class ResourceContainer_Cosume : ResourceContainer
{
    public List<TypeOfCosumption> m_consumeType;
    public override void UseItem(Inventory_Icon p_currentIcon)
    {
        Inventory_ItemUsage.Instance.ConsumeItem(p_currentIcon,m_consumeType);
    }

    [System.Serializable]
    public class TypeOfCosumption
    {
        public enum ConsumeType {Stamina, Hunger, Energy}
        public TypeOfCosumption.ConsumeType m_typeOfConsume;

        public float m_replenishAmount;
        public bool m_increasePastAmount;
    }
}

