using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_PlacementCamera : Building_PlacementManager
{
    public Rigidbody m_rb;
    public GenericWorldEvent m_initialize;

    
    public override void InitializePlacement()
    {
        m_initialize.Invoke();
        m_rb.isKinematic = true;
    }
    public override void PlaceBuilding()
    {
        m_rb.isKinematic = false;
        Durability_UI.Instance.HideUI();

        
        m_interactable.m_canBeInteractedWith = true;
        m_objectPlacedEvent.Invoke();
        Inventory_2DMenu.Instance.m_toolComponents.DisableCameraIcon();
        Interactable_Manager.Instance.SetInteractable(m_interactable);
    }
}
