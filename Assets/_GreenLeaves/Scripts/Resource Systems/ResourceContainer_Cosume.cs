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
        foreach (TypeOfCosumption consume in m_consumeType)
        {
            Debug.Log("Here is where the consuming is.");
            if (PlayerStatsController.Instance != null)
            {

                PlayerStatsController.Instance.AddAmount(consume.m_typeOfConsume, consume.m_replenishAmount, consume.m_increasePastAmount);
            }
            else
            {
                Debug.LogError("Energy Controller singleton is not initailized");
            }
        }
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

