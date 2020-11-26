using UnityEngine;

/// <summary>
/// This class is used to detect hits with the bushes, logs, animals etc.
/// </summary>
public class Player_EquipmentUse_Hit : Player_EquipmentUse
{
    public LayerMask m_hitDetectionMask;
    public float m_hitDetectionRadius;
    public Transform m_playerObject;

    public int m_cutType;

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

    public override void UseEquipment()
    {
        Manipulation_HitObject hit = CheckBushRadius();
        if (hit != null)
        {
            hit.HitObject();
            ReduceDurability();
        }
    }

    public Manipulation_HitObject CheckBushRadius()
    {
        Collider[] cols = Physics.OverlapSphere(m_playerObject.position, m_hitDetectionRadius, m_hitDetectionMask);
        Manipulation_HitObject hit;
        foreach (Collider col in cols)
        {
            hit = col.gameObject.GetComponent<Manipulation_HitObject>();
            if (hit != null)
            {
                if (hit.m_canHit && hit.m_cutType == m_cutType)
                {
                    return hit;
                }
            }
        }
        return null;
    }

}
