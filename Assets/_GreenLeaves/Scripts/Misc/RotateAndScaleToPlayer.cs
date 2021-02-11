using UnityEngine;

/// <summary>
/// A small class used to rotate an object to the player, and <br/>
/// also maintain it's scaling on the screen, no matter how far it is.
/// </summary>
public class RotateAndScaleToPlayer : MonoBehaviour
{
    public float m_startingDistance;

    public bool m_rotate;
    public bool m_scale;
    public bool m_xAxisRotation;
    public bool m_followPlayer;
    public bool m_adjustToParentScale;
    public bool m_adjustToParentTransform;

    private Transform m_cameraRef;
    public Transform m_transformToScale;
    private Transform m_transformToFollow;
    public Vector3 m_transformOffset;

    public Vector3 m_localPosOffset;
    
    private void Start()
    {
        m_cameraRef = PlayerInputToggle.Instance.m_playerCamera;
        if(m_transformToScale == null)
        {
            m_transformToScale = transform;
        }
        if (m_followPlayer)
        {
            m_transformToFollow = PlayerInputToggle.Instance.transform;
            transform.parent = m_transformToFollow;
            transform.localPosition = m_transformOffset;
            
        }
        m_localPosOffset = transform.localPosition;
    }
    
    
    private void Update()
    {
        if (m_rotate)
        {
            transform.eulerAngles = new Vector3((m_xAxisRotation ? GetXRotation() : 0), GetYRotation(), 0);
        }
        if (m_scale)
        {
            if (m_adjustToParentScale)
            {
                Vector3 currentDis = GetDistanceAway();
                Vector3 newScale = new Vector3(currentDis.x * (1 / transform.parent.localScale.x), currentDis.y * (1 / transform.parent.localScale.y), currentDis.z * (1 / transform.parent.localScale.z));
                m_transformToScale.localScale = newScale;
            }
            else
            {
                m_transformToScale.localScale = GetDistanceAway();
            }
            if (m_adjustToParentTransform)
            {
                transform.localPosition = new Vector3(m_localPosOffset.x * (1 / transform.parent.localScale.x), m_localPosOffset.y * (1 / transform.parent.localScale.y), m_localPosOffset.z * (1 / transform.parent.localScale.z));
            }
        }
    }
    public void ForceUpdate()
    {
        if(m_cameraRef == null)
        {
            m_cameraRef = PlayerInputToggle.Instance.m_playerCamera;
        }
        if (m_rotate)
        {
            transform.eulerAngles = new Vector3((m_xAxisRotation ? GetXRotation() : 0), GetYRotation(), 0);
        }
        if (m_scale)
        {
            m_transformToScale.localScale = GetDistanceAway();
        }
    }

    private float GetYRotation()
    {
        Vector3 camRef = new Vector3(m_cameraRef.position.x, 0, m_cameraRef.position.z);
        Vector3 myPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 direction = (myPos - camRef).normalized;
        float dir = Mathf.Sign(Vector3.Dot(camRef - myPos, Vector3.right));
        return -1 * dir * Vector3.Angle(Vector3.forward, direction);
    }

    private float GetXRotation()
    {
        Vector3 dirToPrompt = transform.position - m_cameraRef.position;
        float dir = Mathf.Sign(Vector3.Dot(dirToPrompt, Vector3.down));
        
        return Vector3.Angle(new Vector3(dirToPrompt.x, 0, dirToPrompt.z), dirToPrompt) * dir;
        //return m_cameraRef.eulerAngles.x;
    }

    private Vector3 GetDistanceAway()
    {
        return Vector3.one * (Vector3.Distance(transform.position, m_cameraRef.position) / m_startingDistance);
    }
}
