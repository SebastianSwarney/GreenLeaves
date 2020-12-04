using UnityEngine;

/// <summary>
/// This class is used to interact with objects that can have their mesh sliced. <br/>
/// When the Use Equipment function is called on this, a radius will be scanned for any <br/>
/// objects that contain the Manipulation_SelfSlice script on it. It will then call the Slice <br/>
/// function on that object.
/// </summary>
public class Player_EquipmentUse_MeshSlice : Player_EquipmentUse
{
    public bool m_debug;
    public Color m_debugColor;

    [Header("SliceDetection")]
    public Transform m_playerObject;
    public float m_detectionRadius;
    public LayerMask m_detectionMask;

    [Header("Equipment shake detection")]
    public LayerMask m_hitDetectionMask;
    public float m_hitDetectionRadius;

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
        if (m_playerObject == null)
        {
            m_playerObject = Player_Inventory.Instance.transform;
        }
    }
    public override void UseEquipment()
    {
        Manipulation_SelfSlice sliceMe = CheckTreeRadius();

        ///If an object in the radius can be sliced, call their slice method
        if (sliceMe != null)
        {
            ///The parameters will determine the angle, and position of the slice
            sliceMe.SliceMe(new Vector3(0, m_playerObject.transform.position.y, 0), Vector3.up, m_playerObject.transform.forward);
            ReduceDurability();
        }
        else
        {
            Manipulation_HitObject hit = CheckBushRadius();
            if (hit != null)
            {
                hit.HitObject();
                ReduceDurability();
            }
        }
    }

    /// <summary>
    /// Returns a variable if there is one in the radius, and if it can be sliced.
    /// </summary>
    /// <returns></returns>
    public Manipulation_SelfSlice CheckTreeRadius()
    {
        Collider[] cols = Physics.OverlapSphere(m_playerObject.transform.position, m_detectionRadius, m_detectionMask);
        foreach (Collider col in cols)
        {
            if (col.gameObject.GetComponent<Manipulation_SelfSlice>() != null)
            {
                return col.gameObject.GetComponent<Manipulation_SelfSlice>();
            }
        }
        return null;
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
                if (hit.m_cutType == 1)
                {
                    if (hit.m_canHit)
                    {
                        return hit;
                    }
                }
            }
        }
        return null;
    }

    public override void ReEnableToolComponent()
    {
        Crafting_Table.CraftingTable.m_toolComponents.EnableToolResource(ResourceContainer_Equip.ToolType.Axe);
    }
    private void OnDrawGizmos()
    {
        if (!m_debug) return;
        Gizmos.color = m_debugColor;
        if(m_playerObject != null)
        {
            Gizmos.DrawWireSphere(m_playerObject.position, m_detectionRadius);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, m_detectionRadius);
        }
    }
}
