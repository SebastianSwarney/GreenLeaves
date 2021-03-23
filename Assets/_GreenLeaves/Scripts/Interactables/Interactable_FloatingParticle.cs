using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_FloatingParticle : MonoBehaviour
{
    public float m_minDistanceFromPlayer = 10, m_maxDistanceFromPlayer = 50;
    public float m_currentDis;
    public GameObject m_particlePrefab;
    public GameObject m_currentParticle;
    private Daytime_TimeBasedParticles m_particleSystemsParent;
    private bool m_toggleOff;
    private void Update()
    {
        m_currentDis = Vector3.Distance(transform.position, PlayerInputToggle.Instance.transform.position);
        if (m_currentDis > m_minDistanceFromPlayer && m_currentDis < m_maxDistanceFromPlayer)
        {
            if (m_currentParticle == null)
            {
                m_currentParticle = ObjectPooler.Instance.NewObject(m_particlePrefab, transform.position, Quaternion.identity);
                m_particleSystemsParent = m_currentParticle.GetComponent<Daytime_TimeBasedParticles>();
                m_particleSystemsParent.ToggleParticles(true);
                m_toggleOff = false;
            }
            else
            {
                if (m_particleSystemsParent != null)
                {
                    m_particleSystemsParent.ToggleParticles(true);
                }
                m_toggleOff = false;
            }
        }
        else
        {
            if (m_currentParticle != null)
            {
                if (m_currentDis < m_minDistanceFromPlayer)
                {
                    if (!m_toggleOff)
                    {
                        m_toggleOff = true;
                        if (m_particleSystemsParent != null)
                        {
                            m_particleSystemsParent.ToggleParticles(false);
                        }
                    }
                }
                else
                {

                    ObjectPooler.Instance.ReturnToPool(m_currentParticle);
                    m_currentParticle = null;
                    if (m_particleSystemsParent != null)
                    {
                        m_particleSystemsParent.ToggleParticles(true);
                    }
                    m_particleSystemsParent = null;
                }
            }

        }
    }

    public void TurnOffParticle()
    {
        if(m_currentParticle != null && m_particleSystemsParent != null)
        {
            m_particleSystemsParent.ToggleParticles(false);
            enabled = false;
        }
    }
}
