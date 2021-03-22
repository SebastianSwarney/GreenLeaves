using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_EquipmentUse_Torch : Player_EquipmentUse
{
    public static Player_EquipmentUse_Torch Instance;

    public bool m_torchEquipped;
    private void Awake()
    {
        Instance = this;
    }

    public override void ObjectBroke()
    {
        base.ObjectBroke();
        m_torchEquipped = false;
    }


    public override void EquipObject(Inventory_Icon_Durability p_linkedIcon)
    {
        base.EquipObject(p_linkedIcon);
        m_torchEquipped = true;
    }
    public override void UnEquipObject()
    {
        base.UnEquipObject();
        m_torchEquipped = false;
    }

}
