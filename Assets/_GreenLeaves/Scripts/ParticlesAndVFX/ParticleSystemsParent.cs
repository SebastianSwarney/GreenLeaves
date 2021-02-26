using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemsParent : MonoBehaviour
{

    public List<ParticleSystem> m_allParticleSystems;
    public void ToggleParticleSystems(bool p_newState)
    {
        foreach (ParticleSystem sys in m_allParticleSystems)
        {
            if (sys.gameObject.activeSelf)
            {
                if (p_newState)
                {
                    if (sys.isPlaying) continue;
                    sys.Play();
                }
                else
                {
                    if (!sys.isPlaying) continue;
                    sys.Stop();
                }
            }
        }
    }
}
