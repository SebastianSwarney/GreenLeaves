
using System.Collections.Generic;
using UnityEngine;

public class Building_PlacementManager : MonoBehaviour
{

    public enum PlacementState { Placing, Placed }
    public PlacementState m_currentState;
    public Building_PlacementDetection m_buildingPlacer;

    public ParticleSystem m_fireParticle;
    public Vector3 m_placement;


    public GenericWorldEvent m_objectPlacedEvent;

    public Durability_UI m_instructionPrompt;
    public Interactable_Campfire m_interactable;

    public GameObject m_activeModel, m_burntModel;
    public void InitializePlacement()
    {
        m_activeModel.SetActive(true);
        m_burntModel.SetActive(false);
        m_fireParticle.gameObject.SetActive(false);
    }

    private bool m_togglePrompt = false;
    public bool AttemptPlacement(Vector3 p_placement)
    {
        transform.position =  p_placement;
        Vector3 hitNormal;
        if (m_buildingPlacer.CanPlace(out hitNormal))
        {
            transform.rotation *= Quaternion.FromToRotation(transform.up, hitNormal);

            return true;
        }
        else
        {
            transform.rotation = Quaternion.identity;

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
                m_instructionPrompt.ShowUI();
            }
        }
        else
        {
            if (m_togglePrompt)
            {
                m_togglePrompt = false;
                m_instructionPrompt.HideUI();
            }
        }
    }


    public void PlaceBuilding()
    {
        m_instructionPrompt.HideUI();
        m_currentState = PlacementState.Placed;
        m_fireParticle.gameObject.SetActive(true);
        m_objectPlacedEvent.Invoke();
        m_interactable.m_canBeInteractedWith = true;
        Interactable_Manager.Instance.SearchForInteractable();

        Map_LoadingManager.Instance.GetCurrentOccupiedMapArea().m_allCampfires.Add(gameObject);
    }

    public void PlaceBuildingUnlit()
    {
        m_fireParticle.gameObject.SetActive(false);
        m_activeModel.SetActive(false);
        m_burntModel.SetActive(true);
        m_instructionPrompt.HideUI();
        m_currentState = PlacementState.Placed;
        m_interactable.m_canBeInteractedWith = false;
        m_fireParticle.gameObject.SetActive(false);
    }
}
