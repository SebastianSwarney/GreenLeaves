using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_EquipmentUse_Pick : Player_EquipmentUse
{
    public static Player_EquipmentUse_Pick Instance;
    public bool m_canClimb;

    public void AssignSingleton()
    {
        Instance = this;
    }
    public override void EquipObject(Inventory_Icon_Durability p_linkedIcon)
    {
        base.EquipObject(p_linkedIcon);
        m_canClimb = true;
    }

    public override void UnEquipObject()
    {
        base.UnEquipObject();
        m_canClimb = false;
    }
    public override void UseEquipment()
    {
        base.UseEquipment();
        ReduceDurability();
    }
    public override void ObjectBroke()
    {
        base.ObjectBroke();
        m_canClimb = false;
        PlayerEquipmentBreak.Instance.ShowUI();
    }
}
