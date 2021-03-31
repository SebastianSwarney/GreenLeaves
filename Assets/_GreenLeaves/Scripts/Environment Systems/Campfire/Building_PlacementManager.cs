
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Building_PlacementManager : MonoBehaviour
{


    public Building_PlacementDetection m_buildingPlacer;

    public ParticleSystem m_fireParticle;
    public Vector3 m_placement;


    public GenericWorldEvent m_objectPlacedEvent, m_objectDiedEvent;

    public Interactable m_interactable;

    public GameObject m_activeModel, m_burntModel;
    public bool m_rotateWithLand = true;
    public virtual void InitializePlacement()
    {
        m_activeModel.SetActive(true);
        m_burntModel.SetActive(false);
        m_fireParticle.gameObject.SetActive(false);
    }

    private bool m_togglePrompt = false;
    public bool AttemptPlacement(Vector3 p_placement)
    {
        transform.position = p_placement;
        Vector3 hitNormal;
        if (m_buildingPlacer.CanPlace(out hitNormal))
        {
            if (m_rotateWithLand)
            {
                transform.rotation *= Quaternion.FromToRotation(transform.up, hitNormal);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, PlayerInputToggle.Instance.m_physicalCamera.transform.eulerAngles.y, 0);
            }

            return true;
        }
        else
        {
            if (m_rotateWithLand)
            {
                transform.rotation = Quaternion.identity;
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, PlayerInputToggle.Instance.m_physicalCamera.transform.eulerAngles.y, 0);
            }

            return false;
        }
    }



    public void TogglePrompt(bool p_newState)
    {
        if (p_newState)
        {
            if (!m_togglePrompt)
            {
                m_togglePrompt = true;
                Durability_UI.Instance.ShowUI(false);
            }
        }
        else
        {
            if (m_togglePrompt)
            {
                m_togglePrompt = false;
                Durability_UI.Instance.HideUI();
            }
        }
    }


    public virtual void PlaceBuilding()
    {
        Durability_UI.Instance.HideUI();

        m_fireParticle.gameObject.SetActive(true);
        m_interactable.m_canBeInteractedWith = true;
        m_objectPlacedEvent.Invoke();
        //Interactable_Manager.Instance.SearchForInteractable();

        Map_LoadingManager.Instance.GetCurrentOccupiedMapArea().m_allCampfires.Add(gameObject);
        RespawnResourceManager.Instance.AddCampfire(this);

    }

    public void PlaceBuildingUnlit()
    {
        m_objectDiedEvent.Invoke();
        m_fireParticle.Stop();
        m_activeModel.SetActive(false);
        m_burntModel.SetActive(true);
        m_interactable.m_canBeInteractedWith = false;
        m_interactable.GetComponent<Interactable_Campfire>().FireDied();
        //m_fireParticle.gameObject.SetActive(false);
        StartCoroutine(StopParticle());
    }

    private IEnumerator StopParticle()
    {
        while (m_fireParticle.isPlaying)
        {
            yield return null;
        }
        m_fireParticle.gameObject.SetActive(false);
    }
}
