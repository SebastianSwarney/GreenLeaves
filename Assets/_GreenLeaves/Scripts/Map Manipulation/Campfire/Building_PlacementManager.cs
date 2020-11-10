
using System.Collections.Generic;
using UnityEngine;

public class Building_PlacementManager : MonoBehaviour
{

    public enum PlacementState { Placing, Placed }
    public PlacementState m_currentState;
    public Building_PlacementDetection m_buildingPlacer;

    public List<MeshRenderer> m_campfireRenderers;
    public ParticleSystem m_fireParticle;
    public Vector3 m_placement;

    private MaterialPropertyBlock m_propBlock;

    public GenericWorldEvent m_objectPlacedEvent;
    
    private void Awake()
    {
        m_propBlock = new MaterialPropertyBlock();
    }

    public bool AttemptPlacement(Vector3 p_placement)
    {
        transform.position = p_placement;
        Vector3 hitNormal;
        if (m_buildingPlacer.CanPlace(out hitNormal))
        {
            transform.rotation *= Quaternion.FromToRotation(transform.up, hitNormal);
            ToggleRendererEffects(true);
            return true;
        }
        else
        {
            transform.rotation = Quaternion.identity;
            ToggleRendererEffects(false);
            return false;
        }
    }

    public void ToggleRendererEffects(bool p_newState)
    {
        if(m_propBlock == null)
        {
            m_propBlock = new MaterialPropertyBlock();
        }
        foreach(MeshRenderer render in m_campfireRenderers)
        {
            render.GetPropertyBlock(m_propBlock);
            m_propBlock.SetFloat("_EffectAmount", (p_newState ? 0 : .5f));
            render.SetPropertyBlock(m_propBlock);
        }
    }


    public void PlaceBuilding()
    {
        
        m_currentState = PlacementState.Placed;
        ToggleRendererEffects(true);
        m_objectPlacedEvent.Invoke();
    }
}
