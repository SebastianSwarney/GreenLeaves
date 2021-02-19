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

    private void Awake()
    {
        /*if (m_playerObject == null)
        {
            m_playerObject = PlayerStatsController.Instance.transform;
        }*/
    }
    public void Update()
    {

        if (Inventory_2DMenu.Instance.m_isOpen || PlayerUIManager.Instance.m_isPaused || Building_PlayerPlacement.Instance.m_isPlacing || Daytime_WaitMenu.Instance.m_isWaiting || Interactable_Readable_Menu.Instance.m_isOpen)
        {
            if (m_currentHit != null)
            {
                m_currentHit.m_durabilityUI.HideUI();
                m_currentHit = null;
            }
            return;
        }
        PerformCheck();
        if (Input.GetMouseButtonDown(0))
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
        }
        if (m_currentHit != null)
        {
            m_currentHit.m_durabilityUI.ShowUI();
        }
    }
    public override void EquipObject(Inventory_Icon_Durability p_linkedIcon)
    {
        base.EquipObject(p_linkedIcon);
        if(m_playerObject == null)
        {
            m_playerObject = PlayerStatsController.Instance.transform;
        }
    }

    public override void UseEquipment()
    {
        if (m_currentHit != null)
        {
            base.UseEquipment();
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

    public override void ObjectBroke()
    {
        base.ObjectBroke();
        if(m_currentHit != null)
        {
            m_currentHit.m_durabilityUI.HideUI();
        }
    }
}
