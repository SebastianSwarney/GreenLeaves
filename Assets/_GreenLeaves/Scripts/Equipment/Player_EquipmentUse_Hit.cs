using UnityEngine;

/// <summary>
/// This class is used to detect hits with the bushes, logs, animals etc.
/// </summary>
public class Player_EquipmentUse_Hit : Player_EquipmentUse
{
    public LayerMask m_hitDetectionMask;
    public float m_hitDetectionRadius;
    public Transform m_playerObject;


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            UseEquipment();
        }
    }
    public override void EquipObject(Inventory_Icon_Durability p_linkedIcon)
    {
        base.EquipObject(p_linkedIcon);
        if(m_playerObject == null)
        {
            m_playerObject = EnergyController.Instance.transform;
        }
    }
    
}
