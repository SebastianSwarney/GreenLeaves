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

    [Header("Stamina")]
    public float m_energyLossPerEquipmentUse;
    public float m_staminaLossPerEquipmentUse;

    public bool m_enableDurabilityUI;

    

    public virtual void EquipObject(Inventory_Icon_Durability p_linkedIcon)
    {
        m_linkedIcon = p_linkedIcon;
        m_durability = p_linkedIcon.m_durabilityAmount;



        gameObject.SetActive(true);
        enabled = true;
        if (m_enableDurabilityUI)
        {
            Player_EquipmentToolsUi.Instance.ToggleDurabilityUI(true);
            Player_EquipmentToolsUi.Instance.SetDurabilityText(m_durability);
        }
    }

    public virtual void UnEquipObject()
    {
        m_linkedIcon = null;
        m_durability = m_startingDurability;

        gameObject.SetActive(false);
        Player_EquipmentToolsUi.Instance.ToggleDurabilityUI(false);
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
        UpdateIconDurability();
        Player_EquipmentToolsUi.Instance.SetDurabilityText(m_durability);
    }

    public virtual void ObjectBroke()
    {
        Inventory_2DMenu.Instance.m_inventoryGrid.RemoveSingleIcon(m_linkedIcon);
        Player_Inventory.Instance.UnEquipCurrentTool();
        ReEnableToolComponent();
        Player_EquipmentToolsUi.Instance.ToggleDurabilityUI(false);
    }

    public virtual void ReEnableToolComponent()
    {
        Crafting_Table.CraftingTable.m_toolComponents.EnableToolResource(ResourceContainer_Equip.ToolType.Torch);
    }

    public virtual void UpdateIconDurability()
    {
        if (m_linkedIcon != null)
        {
            m_linkedIcon.UpdateDurability(m_durability);
        }
    }
    #endregion


    public virtual void PlayAnimation(string p_animName)
    {
        Debug.Log("Play Anim: " + p_animName, this.gameObject);
    }

    public virtual void UseEquipment()
    {
        PlayerStatsController.Instance.EquipmentStatDrain(m_energyLossPerEquipmentUse, m_staminaLossPerEquipmentUse);
    }
}
