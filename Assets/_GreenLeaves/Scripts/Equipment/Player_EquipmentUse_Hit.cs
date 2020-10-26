using UnityEngine;

/// <summary>
/// This class is used to detect hits with the bushes, logs, animals etc.
/// </summary>
public class Player_EquipmentUse_Hit : Player_EquipmentUse
{
    public LayerMask m_detectionMask;
    public float m_detectionRadius;
    public GameObject m_playerObject;
    
    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            UseEquipment();
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
        Collider[] cols = Physics.OverlapSphere(m_playerObject.transform.position, m_detectionRadius, m_detectionMask);
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
