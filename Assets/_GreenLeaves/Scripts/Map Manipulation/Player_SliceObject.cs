using UnityEngine;

public class Player_SliceObject : MonoBehaviour
{
    public bool m_debug;
    public Color m_debugColor;

    [Header("SliceDetection")]
    public GameObject m_playerObject;
    public float m_sliceHeight;
    public float m_detectionRadius;
    public LayerMask m_detectionMask;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SelfSlice sliceMe = CheckRadius();
            if(sliceMe != null)
            {
                SliceObject(sliceMe, new Vector3(0,m_playerObject.transform.position.y,0), Vector3.up, m_playerObject.transform.forward);
            }
        }
    }

    public SelfSlice CheckRadius()
    {
        Collider[] cols = Physics.OverlapSphere(m_playerObject.transform.position, m_detectionRadius, m_detectionMask);
        foreach(Collider col in cols)
        {
            if(col.gameObject.GetComponent<SelfSlice>() != null)
            {
                return col.gameObject.GetComponent<SelfSlice>();
            }
        }
        return null;
    }

    public void SliceObject(SelfSlice p_slice, Vector3 p_position, Vector3 p_upVector, Vector3 p_forwardVector)
    {
        p_slice.SliceMe(p_position, p_upVector, p_forwardVector);
    }
}
