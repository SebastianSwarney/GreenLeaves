using UnityEngine;

/// <summary>
/// The base class used to interact with world items. This class will be called during animations, with the equipped Item
/// </summary>
public class Player_EquipmentUse : MonoBehaviour
{


    public int m_startingDurability;
    public int m_durability;

    public Inventory_Icon_Durability m_linkedIcon;

    [Header("Events")]
    public GenericWorldEvent m_itemBrokeEffect;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            UseEquipment();
        }
    }

    public virtual void InitializeObject(Inventory_Icon_Durability p_linkedIcon)
    {
        enabled = true;
        m_linkedIcon = p_linkedIcon;
        m_durability = p_linkedIcon.m_durabilityAmount;
    }
    #region Durability
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
        UpdateIconDurability(m_durability);
    }
    public virtual void ObjectBroke()
    {
        Inventory_ItemUsage.Instance.HeldEquipmentBroke();
    }

    public virtual void UpdateIconDurability(int p_newAmount)
    {
        m_linkedIcon.UpdateDurability(p_newAmount);
    }
    #endregion


    public virtual void PlayAnimation(string p_animName)
    {
        Debug.Log("Play Anim: " + p_animName, this.gameObject);
    }

    public virtual void UseEquipment()
    {
        Debug.Log("Place equipment usage code here", this);
    }

    private void OnDisable()
    {
        enabled = false;
    }
}
