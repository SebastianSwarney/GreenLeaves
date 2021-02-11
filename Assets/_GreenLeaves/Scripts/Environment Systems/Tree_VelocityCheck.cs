using UnityEngine;
using System.Collections;

/// <summary>
/// <para>Used on the falling trees to determine when to poof them. <br/>
/// This scrip essentially tracks their velocity, and when the velocity reaches a sudden stop, <br/>
/// the m_objectHit event is invoked.</para>
/// Note: When tree hits something, the ovject is disabled. <br/>
/// Additional Note: This script must be the child of the falling tree <br/>
/// and must be setup on the original tree itself. It is assigned when the tree is sliced.
/// </summary>
public class Tree_VelocityCheck : MonoBehaviour
{
    public Rigidbody m_rb;
    public float m_prevVel;
    public Transform m_woodSpawn;
    public GameObject m_woodObject;
    public GenericWorldEvent m_objectHit;
    public float m_delayCheckTime;

    public bool m_debugCollision;

    public void AssignToNewTree(GameObject p_newTree)
    {
        transform.parent = p_newTree.transform;
        m_rb = p_newTree.GetComponent<Rigidbody>();
        m_rb.angularDrag = 0;
        enabled = true;

        Debug.DrawLine(Vector3.zero, m_rb.centerOfMass,Color.red, 4f);
        if (!m_debugCollision)
        {
            StartCoroutine(PerformCheck());
        }
    }

    private IEnumerator PerformCheck()
    {
        yield return new WaitForSeconds(m_delayCheckTime * transform.parent.localScale.y);
        bool explode = false;
        float timer = 5;
        while (!explode)
        {
            if (m_prevVel - m_rb.velocity.magnitude > 0)
            {
                explode = true;
            }
            m_prevVel = m_rb.velocity.magnitude;
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                explode = true;
            }
            yield return null;
        }
        m_objectHit.Invoke();
        transform.parent.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_debugCollision)
        {
            Debug.Log("Collided with : " + collision.gameObject.name);
        }
    }
}
