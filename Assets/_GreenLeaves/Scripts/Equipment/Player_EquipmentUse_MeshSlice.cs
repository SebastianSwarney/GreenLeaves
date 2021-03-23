using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to interact with objects that can have their mesh sliced. <br/>
/// When the Use Equipment function is called on this, a radius will be scanned for any <br/>
/// objects that contain the Manipulation_SelfSlice script on it. It will then call the Slice <br/>
/// function on that object.
/// </summary>
public class Player_EquipmentUse_MeshSlice : Player_EquipmentUse
{
    public static Player_EquipmentUse_MeshSlice Instance;
    public bool m_debug;
    public Color m_debugColor;

    [Header("SliceDetection")]
    public Transform m_playerObject;
    public float m_detectionRadius;
    public LayerMask m_detectionMask;

    [Header("Equipment shake detection")]
    public LayerMask m_hitDetectionMask;
    public float m_hitDetectionRadius;

    private List<GameObject> m_objectsInRange = new List<GameObject>();
    private Manipulation_SelfSlice m_currentTarget;
    private Manipulation_HitObject m_currentHittable;

    public bool m_axeEquipped;

    public void AssignSingleton()
    {
        Instance = this;
    }

    public void Update()
    {
        if (Inventory_2DMenu.Instance.m_isOpen || PlayerUIManager.Instance.m_isPaused || Building_PlayerPlacement.Instance.m_isPlacing || Daytime_WaitMenu.Instance.m_isWaiting || Interactable_Readable_Menu.Instance.m_isOpen)
        {
            if (m_currentTarget != null)
            {
                m_currentTarget.HideUI();
                m_currentTarget = null;
            }
            if (m_currentHittable != null)
            {
                m_currentHittable.HideUI();
                m_currentHittable = null;
            }
            return;
        }
        DetectCurrentSlicables();
        if (Input.GetMouseButtonDown(0))
        {
            UseEquipment();
        }
    }

    public void DetectCurrentSlicables()
    {


        Manipulation_SelfSlice currentSlice = m_currentTarget;

        m_currentTarget = CheckTreeRadius();
        if (m_currentTarget != null && m_currentHittable != null)
        {
            m_currentHittable.HideUI();
            m_currentHittable = null;
        }

        if (currentSlice == m_currentTarget && currentSlice != null) return;

        if (currentSlice != null && m_currentTarget == null)
        {
            currentSlice.HideUI();
        }

        if (m_currentTarget != null)
        {
            if (currentSlice != null)
            {
                currentSlice.HideUI();
            }
            m_currentTarget.ShowUI();
        }
        else
        {
            Manipulation_HitObject currentHit = m_currentHittable;

            m_currentHittable = CheckBushRadius();

            if (currentHit != null && m_currentHittable == null)
            {
                currentHit.HideUI();
                m_currentHittable = null;
            }

            if (currentHit == m_currentHittable && currentHit != null) return;
            if (m_currentHittable != null)
            {
                if (currentHit != null)
                {
                    currentHit.HideUI();
                }
                m_currentHittable.ShowUI();
            }
        }
    }
    public override void EquipObject(Inventory_Icon_Durability p_linkedIcon)
    {
        m_axeEquipped = true;
        base.EquipObject(p_linkedIcon);
        if (m_playerObject == null)
        {
            m_playerObject = Player_Inventory.Instance.transform;
        }
    }

    public override void UnEquipObject()
    {
        base.UnEquipObject();
        m_axeEquipped = false;
    }
    public override void UseEquipment()
    {
        Chop(m_playerObject.transform.position);


    }

    public void Chop(Vector3 p_axeWorldPos)
    {
        ///If an object in the radius can be sliced, call their slice method
        if (m_currentTarget != null)
        {
            base.UseEquipment();
            ///The parameters will determine the angle, and position of the slice
            m_currentTarget.SliceMe(new Vector3(0, p_axeWorldPos.y, 0), m_playerObject.transform.forward);
            ReduceDurability();
        }
        else if (m_currentHittable != null)
        {
            base.UseEquipment();
            m_currentHittable.HitObject();
            ReduceDurability();
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
                if (hit.m_cutType == 1 || hit.m_cutType == 2)
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
        if (m_playerObject != null)
        {
            Gizmos.DrawWireSphere(m_playerObject.position, m_detectionRadius);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, m_detectionRadius);
        }
    }
    public override void ObjectBroke()
    {
        m_axeEquipped = false;
        base.ObjectBroke();
        if (m_currentHittable != null)
        {
            m_currentHittable.HideUI();
        }
        if (m_currentTarget != null)
        {
            m_currentTarget.HideUI();
        }
        PlayerEquipmentBreak.Instance.ShowUI();
    }
}
