using System.Collections.Generic;
using UnityEngine;


public class VFX_DropSound : MonoBehaviour
{
    public LayerMask m_nonTerrainMask;
    public LayerMask m_groundLayerMask;
    public LayerMask m_waterMask;
    public string m_groundMask;
    private int m_groundLayerMaskInt;
    private int m_waterLayerMask;

    [FMODUnity.EventRef]
    public string[] m_terrainSounds;
    [FMODUnity.EventRef]
    public string[] m_objectFootsteps;
    [FMODUnity.EventRef]
    public string m_waterSound;

    public FMODUnity.StudioEventEmitter m_eventEmitter;
    public string m_currentSound;
    private string m_currentSoundTrack;


    public VFX_SpawnParticle m_particleSpawner;

    /// <summary>
    /// Used to keep track of the different terrains the player has walked on
    /// Is kept as a dictionary, to easily cache the terrain should the player walk on it
    /// rather than get all the values at runtime, it only gets it once at runtime.
    /// </summary>
    public Dictionary<string, SoundEmitter_FootSteps.CachedTerrain> m_cachedTerrain;



    private void Start()
    {
        m_groundLayerMaskInt = LayerMask.NameToLayer(m_groundMask);
        m_waterLayerMask = LayerMask.NameToLayer("Water");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == m_waterLayerMask)
        {
            RaycastHit hit;
            Debug.DrawLine(transform.position + Vector3.up * 5, transform.position + Vector3.up * 5 + Vector3.down * 100, Color.red, 2);
            if (Physics.Raycast(transform.position + Vector3.up * 5, Vector3.down, out hit, 100, m_waterMask))
            {
                if (Physics.Linecast(transform.position, hit.point, m_groundLayerMask))
                {
                    return;
                }

                m_particleSpawner.SpawnParticleWithAngle(hit.normal);
            }
            else
            {

                m_particleSpawner.SpawnParticlePrefab();
            }

            if (!m_eventEmitter.IsPlaying())
            {
                PlaySound(true);
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.layer == m_groundLayerMaskInt)
        {
            if (!m_eventEmitter.IsPlaying())
            {
                PlaySound(false);
            }
        }
    }
    public void PlaySound(bool p_waterSound)
    {
        if (p_waterSound)
        {
            m_currentSound = m_waterSound;
        }
        else
        {
            if (Physics.Raycast(transform.position, Vector3.up, 7, m_waterMask))
            {
                m_currentSound = m_waterSound;
            }
            else
            {
                m_currentSound = GetFootstepEvent();
            }
        }
        if (m_currentSoundTrack != m_currentSound)
        {
            m_eventEmitter.Event = m_currentSound;
            m_eventEmitter.Lookup();
        }

        m_currentSoundTrack = m_currentSound;
        m_eventEmitter.Play();
    }

    /// <summary>
    /// REturns the FMOD event of the current stepped on object <br/>
    /// </summary>
    public string GetFootstepEvent()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.position - Vector3.up, out hit, 10, m_nonTerrainMask))
        {
            SoundChanger_FootSteps newStepObject;
            if (hit.transform.GetComponent<SoundChanger_FootSteps>())
            {
                newStepObject = hit.transform.GetComponent<SoundChanger_FootSteps>();
            }
            else if (hit.transform.parent != null)
            {
                if (hit.transform.parent.GetComponent<SoundChanger_FootSteps>())
                {
                    newStepObject = hit.transform.parent.GetComponent<SoundChanger_FootSteps>();
                }
                else
                {
                    return m_terrainSounds[0];
                }
            }
            else
            {
                return m_terrainSounds[0];
            }
            if (newStepObject == null)
            {
                if (hit.transform.parent.parent != null)
                {
                    newStepObject = hit.transform.parent.parent.GetComponent<SoundChanger_FootSteps>();
                }
            }
            if (newStepObject != null)
            {
                return GetSteppedObject(newStepObject, hit);
            }

        }

        return m_terrainSounds[0];//m_terrainSounds[GetActiveTerrainTextureIdx(transform.position, Terrain.activeTerrain)];

        return m_waterSound;
    }

    /// <summary>
    /// Returns the fmod event of the stepped on object. <br/>
    /// The water has a special function to measure how deep the raycast is, to play the <br/>
    /// proper depth for the water<br/>
    /// Called in the GetFootstepEvent function
    /// </summary>
    private string GetSteppedObject(SoundChanger_FootSteps p_newStepObject, RaycastHit hit)
    {
        switch (p_newStepObject.m_soundType)
        {
            case SoundChanger_FootSteps.SoundType.Rock:
                return m_objectFootsteps[0];
                break;

            case SoundChanger_FootSteps.SoundType.Leaves:
                return m_objectFootsteps[1];
                break;
            case SoundChanger_FootSteps.SoundType.Wood:
                return m_objectFootsteps[2];
                break;
        }
        return "Oof";
    }

    /// <summary>
    /// These functions will help determine which splat map the player is currently on, and will get the proper sound<br/>
    /// to match the terrain.<br/>
    /// Currently not used, as there are some issues.
    /// </summary>
    #region Terrain Splat Map Detection
    public int GetActiveTerrainTextureIdx(Vector3 p_position, Terrain p_tData)
    {
        if (!m_cachedTerrain.ContainsKey(p_tData.gameObject.name))
        {
            SoundEmitter_FootSteps.CachedTerrain newTerrain = new SoundEmitter_FootSteps.CachedTerrain();
            newTerrain.AssignData(p_tData);
            m_cachedTerrain.Add(p_tData.gameObject.name, newTerrain);
        }
        SoundEmitter_FootSteps.CachedTerrain cachedData = m_cachedTerrain[p_tData.gameObject.name];

        Vector3 terrainCord = ConvertToSplatMapCoordinate(p_position);
        int activeTerrainIndex = 0;
        float largestOpacity = 0f;

        for (int i = 0; i < cachedData.numTextures; i++)
        {
            if (largestOpacity < cachedData.splatmapData[(int)terrainCord.z, (int)terrainCord.x, i])
            {
                activeTerrainIndex = i;
                largestOpacity = cachedData.splatmapData[(int)terrainCord.z, (int)terrainCord.x, i];
            }
        }

        return activeTerrainIndex;
    }

    private Vector3 ConvertToSplatMapCoordinate(Vector3 worldPosition)
    {
        Vector3 splatPosition = new Vector3();
        Terrain ter = Terrain.activeTerrain;
        Vector3 terPosition = ter.transform.position;
        splatPosition.x = ((worldPosition.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth;
        splatPosition.z = ((worldPosition.z - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.alphamapHeight;
        return splatPosition;
    }
    #endregion

}
