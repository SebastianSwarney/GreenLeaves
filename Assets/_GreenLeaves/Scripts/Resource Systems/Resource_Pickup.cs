using UnityEngine;

/// <summary>
/// Used on the 3d object that is representing the objects. 
/// Simply holds a reference to the ResourceContainer datatype that this object
/// is related to.
/// </summary>
public class Resource_Pickup : MonoBehaviour
{
    /// <summary>
    /// The resource data that the object holds. Is a scriptable object
    /// </summary>
    public ResourceContainer m_resourceInfo;
    public bool m_canPickup = true;
    public int m_resourceAmount;

    public GenericWorldEvent m_resourcePickedUpEvent, m_objectSpawned;

    public float m_minDistanceFromPlayer, m_maxDistanceFromPlayer;
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
    public void ResetResourceAmount()
    {
        m_resourceAmount = 1;
        m_objectSpawned.Invoke();
    }
    public void NewResource()
    {
        TogglePickup(true);
        m_objectSpawned.Invoke();
    }
    public virtual void PickupResource()
    {
        m_resourceAmount = 0;
        TogglePickup(false);
        if (Map_LoadingManager.Instance.GetCurrentOccupiedMapArea().m_allResources.Contains(gameObject))
        {
            Map_LoadingManager.Instance.GetCurrentOccupiedMapArea().m_allResources.Remove(gameObject);
        }
    }

    public virtual void TogglePickup(bool p_newState)
    {
        m_canPickup = p_newState;
        if (!p_newState)
        {
            m_resourcePickedUpEvent.Invoke();
        }
    }

    public void ReturnToPool()
    {
        ObjectPooler.Instance.ReturnToPool(gameObject);
    }


}
