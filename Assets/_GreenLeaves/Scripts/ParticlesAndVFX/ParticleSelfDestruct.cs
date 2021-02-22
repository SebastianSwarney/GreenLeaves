using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This script is to be placed on particles that are reused within the object pooling system
/// It will wait until the particle has run it's lifetime, then disable and return it to the
/// object pooler
/// </summary>
public class ParticleSelfDestruct : MonoBehaviour
{
    private ParticleSystem m_attachedParticalSystem;
    public bool m_returnToPool = true;
    private void Start()
    {
        m_attachedParticalSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!m_attachedParticalSystem.IsAlive())
        {
            if (m_returnToPool)
            {
                ObjectPooler.Instance.ReturnToPool(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}