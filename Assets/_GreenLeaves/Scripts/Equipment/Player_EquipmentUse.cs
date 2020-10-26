using UnityEngine;

/// <summary>
/// The base class used to interact with world items. This class will be called during animations, with the equipped Item
/// </summary>
public class Player_EquipmentUse : MonoBehaviour
{

    #region Durability
    public int m_startingDurability;
    public int m_durability;

    [Header("Events")]
    public GenericWorldEvent m_itemBrokeEffect;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            ReduceDurability();
        }
    }

    public void ResetDurability()
    {
        m_durability = m_startingDurability;
    }

    public void ReduceDurability(int p_durabilityUsed = 1)
    {
        m_durability -= p_durabilityUsed;
        if (m_durability <= 0)
        {
            m_itemBrokeEffect.Invoke();
            ObjectBroke();
        }
    }
    public void ObjectBroke()
    {
        Inventory_ItemUsage.Instance.HeldEquipmentBroke();
    }
    #endregion

    public virtual void UseEquipment()
    {
        Debug.Log("Place equipment usage code here");
    }
}
