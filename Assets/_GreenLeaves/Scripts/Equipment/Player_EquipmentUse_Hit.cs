using UnityEngine;

/// <summary>
/// This class is used to detect hits with the bushes, logs, animals etc.
/// </summary>
public class Player_EquipmentUse_Hit : Player_EquipmentUse
{
    public LayerMask m_detectionMask;
    public float m_detectionRadius;
    public Transform m_playerObject;

    public override void InitializeObject(Inventory_Icon_Durability p_linkedIcon)
    {
        base.InitializeObject(p_linkedIcon);
        if(m_playerObject == null)
        {
            m_playerObject = EnergyController.Instance.transform;
        }
    }
    public override void UseEquipment()
    {
        Manipulation_HitObject hit = CheckRadius();
        if (hit != null)
        {
            hit.HitObject();
            ReduceDurability();
        }
    }

    public Manipulation_HitObject CheckRadius()
    {
        Collider[] cols = Physics.OverlapSphere(m_playerObject.position, m_detectionRadius, m_detectionMask);
        Manipulation_HitObject hit;
        foreach (Collider col in cols)
        {
            hit = col.gameObject.GetComponent<Manipulation_HitObject>();
            if (hit != null)
            {
                if (hit.m_canHit)
                {
                    return hit;
                }
            }
        }
        return null;
    }
}
