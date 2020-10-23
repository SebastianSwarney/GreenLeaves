using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float m_lifeSpan;
    private ObjectPooler m_pooler;
    private float m_lifeTime;
    void Awake()
    {
        m_pooler = ObjectPooler.Instance;
    }
    private void OnEnable()
    {
        m_lifeTime = 0;
    }
    private void Update()
    {

        m_lifeTime += Time.deltaTime;
        if (m_lifeTime > m_lifeSpan)
        {
            m_pooler.ReturnToPool(gameObject);
        }
    }
}
