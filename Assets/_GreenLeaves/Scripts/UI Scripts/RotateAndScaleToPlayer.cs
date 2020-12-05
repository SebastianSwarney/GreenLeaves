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
    private Transform m_cameraRef;
    public Transform m_transformToScale;
    private void Start()
    {
        m_cameraRef = PlayerInputToggle.Instance.m_playerCamera;
        if(m_transformToScale == null)
        {
            m_transformToScale = transform;
        }
    }
    private void Update()
    {
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
        m_dir = Mathf.Sign(Vector3.Dot(camRef - myPos, Vector3.right));
        return -1 * m_dir * Vector3.Angle(Vector3.forward, direction);
    }

    public float m_dir;
    private float GetXRotation()
    {
        return m_cameraRef.eulerAngles.x;
    }

    private Vector3 GetDistanceAway()
    {
        return Vector3.one * (Vector3.Distance(transform.position, m_cameraRef.position) / m_startingDistance);
    }
}
