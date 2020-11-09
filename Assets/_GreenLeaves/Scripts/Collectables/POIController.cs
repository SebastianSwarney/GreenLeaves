using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POIController : MonoBehaviour
{
	public LayerMask m_playerMask;

    public GameObject m_poiFoundCam;

    private bool m_discovered;

    private void OnObjectEnter(GameObject p_object)
	{
        if (CheckCollisionLayer(m_playerMask, p_object))
        {
            if (!m_discovered)
            {
                OnPlayerDiscorvered();
            }
        }
	}

    private void OnObjectLeave(GameObject p_object)
	{
        if (CheckCollisionLayer(m_playerMask, p_object))
        {
            if (m_discovered)
            {
                m_poiFoundCam.SetActive(false);
            }
        }
    }

    private void OnPlayerDiscorvered()
	{
        m_discovered = true;
        Debug.Log("Player found point");
        m_poiFoundCam.SetActive(true);
    }

	private void OnTriggerEnter(Collider other)
	{
        OnObjectEnter(other.gameObject);
	}

	private void OnTriggerExit(Collider other)
	{
        OnObjectLeave(other.gameObject);
    }

	public bool CheckCollisionLayer(LayerMask p_layerMask, GameObject p_object)
    {
        if (p_layerMask == (p_layerMask | (1 << p_object.layer)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
