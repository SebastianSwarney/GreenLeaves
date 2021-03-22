using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_EquipmentUse_Torch : Player_EquipmentUse
{
    public static Player_EquipmentUse_Torch Instance;

    public bool m_torchEquipped;

    public int m_timeToLoseOneDurability = 1;

    private float m_timer = 0;
    private void Awake()
    {
        Instance = this;
    }

    public override void ObjectBroke()
    {
        base.ObjectBroke();
        m_torchEquipped = false;
    }

    private void Update()
    {
        if (m_torchEquipped && !Inventory_2DMenu.Instance.m_isOpen && !PlayerUIManager.Instance.m_isPaused)
        {
            m_timer += Time.deltaTime;
            if(m_timer > m_timeToLoseOneDurability)
            {
                ReduceDurability();
                m_timer = 0;
            }
        }
    }
    public override void EquipObject(Inventory_Icon_Durability p_linkedIcon)
    {
        base.EquipObject(p_linkedIcon);
        m_torchEquipped = true;
        m_timer = 0;
    }
    public override void UnEquipObject()
    {
        base.UnEquipObject();
        m_torchEquipped = false;
    }


}
