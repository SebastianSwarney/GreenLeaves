using UnityEngine;

/// <summary>
/// This class is used to detect hits with the bushes, logs, animals etc.
/// </summary>
public class Player_EquipmentUse_Hit : Player_EquipmentUse
{
    public LayerMask m_hitDetectionMask;
    public float m_hitDetectionRadius;
    public Transform m_playerObject;

    private Manipulation_HitObject m_currentHit;
    public void Update()
    {
        PerformCheck();
        if (Input.GetMouseButtonDown(1))
        {
            UseEquipment();
        }
    }

    private void PerformCheck()
    {
        Manipulation_HitObject currentHittable = m_currentHit;

        m_currentHit = CheckBushRadius();
        if (m_currentHit == currentHittable) return;
        if(m_currentHit != currentHittable && currentHittable != null)
        {
            currentHittable.m_durabilityUI.HideUI();
            currentHittable.m_durabilityUI.ShowControlUI(false);
        }
        if (m_currentHit != null)
        {
            m_currentHit.m_durabilityUI.ShowControlUI(true);
            m_currentHit.m_durabilityUI.ShowUI();
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
        if (m_currentHit != null)
        {
            m_currentHit.HitObject();
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
                if (hit.m_canHit && hit.m_cutType == 0 || hit.m_canHit && hit.m_cutType == 2)
                {
                    return hit;
                }
            }
        }
        return null;
    }

}
