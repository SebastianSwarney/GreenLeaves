using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float m_lifeSpan;
    private ObjectPooler m_pooler;
    private ParticleSystem m_partSys;
    void Awake()
    {
        m_pooler = ObjectPooler.Instance;
        m_partSys = GetComponent<ParticleSystem>();
    }

    private void Update()
    {

        //m_lifeTime += Time.deltaTime;
        //if (m_lifeTime > m_lifeSpan)
        if(!m_partSys.IsAlive())
        {
            m_pooler.ReturnToPool(gameObject);
        }
    }
}
