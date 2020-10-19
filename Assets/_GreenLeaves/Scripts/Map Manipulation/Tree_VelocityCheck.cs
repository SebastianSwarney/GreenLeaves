using UnityEngine;

public class Tree_VelocityCheck : MonoBehaviour
{
    public Rigidbody m_rb;
    public float m_prevVel;
    public Transform m_woodSpawn;
    public GameObject m_woodObject;
    public GameObject m_poofParticle;
    private void Update()
    {
        if(m_prevVel - m_rb.velocity.magnitude > 0)
        {
            DoThePoof();
        }
        m_prevVel = m_rb.velocity.magnitude;
    }

    public void DoThePoof()
    {
        GameObject newWood = ObjectPooler.Instance.NewObject(m_woodObject, m_woodSpawn.position, Quaternion.identity);
        m_poofParticle.transform.parent = null;
        m_poofParticle.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void AssignToNewTree(GameObject p_newTree)
    {
        Tree_VelocityCheck newTree = p_newTree.AddComponent<Tree_VelocityCheck>();
        newTree.m_rb = p_newTree.GetComponent<Rigidbody>();
        newTree.m_woodSpawn = m_woodSpawn;
        newTree.m_woodObject = m_woodObject;
        newTree.m_poofParticle = m_poofParticle;    
    }
}
