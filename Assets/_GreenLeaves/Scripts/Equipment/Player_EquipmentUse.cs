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

    public virtual void EquipObject(Inventory_Icon_Durability p_linkedIcon)
    {
        m_linkedIcon = p_linkedIcon;
        m_durability = p_linkedIcon.m_durabilityAmount;
        
        gameObject.SetActive(true);
        enabled = true;
    }

    public virtual void UnEquipObject()
    {
        m_linkedIcon = null;
        m_durability = m_startingDurability;
        
        gameObject.SetActive(false);
        enabled = false;
    }

    #region Durability

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
        Inventory_2DMenu.Instance.m_inventoryGrid.RemoveSingleIcon(m_linkedIcon);
        Player_Inventory.Instance.UnEquipCurrentTool();
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
}
