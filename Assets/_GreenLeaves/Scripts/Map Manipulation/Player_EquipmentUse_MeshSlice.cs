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
    public GameObject m_playerObject;
    public float m_detectionRadius;
    public LayerMask m_detectionMask;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UseEquipment();
        }
    }


    public override void UseEquipment()
    {
        Manipulation_SelfSlice sliceMe = CheckRadius();

        ///If an object in the radius can be sliced, call their slice method
        if (sliceMe != null)
        {
            ///The parameters will determine the angle, and position of the slice
            sliceMe.SliceMe(new Vector3(0, m_playerObject.transform.position.y, 0), Vector3.up, m_playerObject.transform.forward);
        }
    }

    /// <summary>
    /// Returns a variable if there is one in the radius, and if it can be sliced.
    /// </summary>
    /// <returns></returns>
    public Manipulation_SelfSlice CheckRadius()
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
}
