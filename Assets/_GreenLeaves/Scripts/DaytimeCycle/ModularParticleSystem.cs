using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularParticleSystem : MonoBehaviour
{
    public ParticleSystem m_particles;

    public float m_stoppingRate = .75f, m_fullRate = .01f;
    public int m_initialEmitCount = 1;
    public float m_startingTime = 5, m_stoppingTime = 5;

    public bool m_isEmitting;
    public float m_emitRate = 1;
    public int m_emitCount;
    private float m_emitTimer = 0;

    private void Update()
    {
        if (m_isEmitting)
        {
            if (m_emitTimer > m_emitRate)
            {
                m_particles.Emit(m_emitCount);
                m_emitTimer = 0;
            }
            else
            {
                m_emitTimer += Time.deltaTime;
            }
        }
    }

    public void StartParticleSystem()
    {
        gameObject.SetActive(true);
        StartCoroutine(StartingParticles());
    }

    public void StartParticlesImmediately()
    {
        gameObject.SetActive(true);

        m_isEmitting = true;
        m_emitRate = m_fullRate;
        m_emitCount = m_initialEmitCount;
    }

    private IEnumerator StartingParticles()
    {
        m_isEmitting = true;
        m_emitRate = m_stoppingRate;
        m_emitCount = m_initialEmitCount;

        float timer = 0;
        while(timer < m_startingTime)
        {
            timer += Time.deltaTime;

            m_emitRate = Mathf.Lerp(m_stoppingRate, m_fullRate, timer / m_startingTime);

            yield return null;
        }
    }

    public void StopParticleSystem()
    {
        StartCoroutine(StoppingParticles());
    }
    private IEnumerator StoppingParticles()
    {
        float timer = 0;
        while (timer < m_stoppingTime)
        {
            timer += Time.deltaTime;

            m_emitRate = Mathf.Lerp(m_fullRate, m_stoppingRate, timer / m_startingTime);

            yield return null;
        }
        m_isEmitting = false;
        while (m_particles.particleCount > 0)
        {
            yield return null;
        }
        gameObject.SetActive(false);
    }


}
