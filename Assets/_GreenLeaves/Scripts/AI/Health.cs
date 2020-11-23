using UnityEngine;


public class Health : MonoBehaviour
{
    public int m_startingHealth;
    public int m_currentHealth;
    public bool m_isDead;

    [Header("Health Event")]
    public GenericWorldEvent m_hurtEvent;
    public GenericWorldEvent m_diedEvent;
    public void Respawn()
    {
        m_isDead = false;
        m_currentHealth = m_startingHealth;
    }

    public void TakeDamage(int p_takenDamage)
    {
        if (m_isDead) return;
        m_currentHealth -= p_takenDamage;
        if(m_currentHealth > 0)
        {
            m_hurtEvent.Invoke();
        }
        else
        {
            m_diedEvent.Invoke();
        }
    }
}
