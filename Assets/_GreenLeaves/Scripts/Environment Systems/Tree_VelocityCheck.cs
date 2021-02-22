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
    public GenericWorldEvent m_objectHit;

    private Rigidbody m_rb;
    public float m_maxVelocityDifference;
    private float m_previousVelocity;

    public void AssignTree()
    {
        m_rb = transform.parent.GetComponent<Rigidbody>();
        StartCoroutine(CheckVelocity());
    }

    private IEnumerator CheckVelocity()
    {
        while (true)
        {
            if(Mathf.Abs(m_rb.velocity.magnitude - m_previousVelocity) > m_maxVelocityDifference)
            {
                transform.parent.GetChild(0).gameObject.SetActive(false);
                m_objectHit.Invoke();
                yield break;

            }
            m_previousVelocity = m_rb.velocity.magnitude;
            yield return null;
        }
    }
}
