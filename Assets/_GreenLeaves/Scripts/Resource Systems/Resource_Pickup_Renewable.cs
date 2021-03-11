using System.Collections.Generic;
using UnityEngine;

public class Resource_Pickup_Renewable : Resource_Pickup
{
    [Tooltip("Toggle to determine whether the source is emptied when used.")]
    public bool m_useUp;

    [Header("Renewable Pickup")]
    [Tooltip("How many times this resource can be harvested until it's used up")]
    public int m_amountOfHarvestable;
    public int m_currentAmount;
    public List<GameObject> m_resourceVisuals;

    public VFX_SpawnParticle m_spawnParticle;
    private bool m_setFromSave = false;
    private void Start()
    {
        if (!m_setFromSave)
        {
            m_currentAmount = m_amountOfHarvestable;
            TogglePickup(true);
        }
        else
        {
            if(m_currentAmount > 0)
            {
                TogglePickup(true);
            }
            else
            {
                foreach(GameObject vis in m_resourceVisuals)
                {
                    vis.SetActive(false);
                }
                TogglePickup(false);
                gameObject.GetComponent<Interactable_Pickup_Renewable>().m_canBeInteractedWith = false;
            }
        }
    }

    public void SetBerryAmount(int p_newAmount)
    {
        m_setFromSave = true;
        m_currentAmount = p_newAmount;
        if (m_currentAmount == m_amountOfHarvestable) return;
        for (int i = 0; i < m_resourceVisuals.Count; i++)
        {
            m_resourceVisuals[i].SetActive(true);
        }
        for (int i = m_amountOfHarvestable; i > m_currentAmount; i--)
        {

            m_resourceVisuals[i].SetActive(false);
        }
        if(m_currentAmount <= 0)
        {
            TogglePickup(false);
            ToggleAllResourceVisuals(false);
        }
    }
    public void ResetAmount()
    {
        m_currentAmount = m_amountOfHarvestable;
        TogglePickup(true);
        ToggleAllResourceVisuals(true);
    }
    public override void PickupResource()
    {
        if (!m_useUp) return;

        m_currentAmount--;
        if (m_currentAmount <= 0)
        {
            m_spawnParticle.SpawnParticlePrefab(m_resourceVisuals[m_currentAmount].transform.position);
            TogglePickup(false);
            ToggleAllResourceVisuals(false);
        }
        else
        {
            if (m_currentAmount < m_resourceVisuals.Count)
            {
                m_spawnParticle.SpawnParticlePrefab(m_resourceVisuals[m_currentAmount].transform.position);
                m_resourceVisuals[m_currentAmount].SetActive(false);
            }
        }
    }


    public void ToggleAllResourceVisuals(bool p_newState)
    {
        foreach (GameObject visual in m_resourceVisuals)
        {
            visual.SetActive(p_newState);
        }
    }
}
