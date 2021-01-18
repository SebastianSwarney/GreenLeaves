using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire_Manager : MonoBehaviour
{
    public GameObject m_fireParticle;

    private bool m_canCook;

    public void ActivateCampfire()
    {
        m_fireParticle.SetActive(true);
        m_canCook = true;
    }

    public void DeactivateCampfire()
    {
        m_fireParticle.SetActive(false);
        m_canCook = false;
    }

    public void KillFire()
    {
        m_fireParticle.SetActive(false);
        m_canCook = false;
    }
}
