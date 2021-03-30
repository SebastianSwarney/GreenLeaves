using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter_FootSteps : MonoBehaviour
{
    public static SoundEmitter_FootSteps Instance;

    public Transform m_playerHeadObject;
    public LayerMask m_nonTerrainMask;

    [FMODUnity.EventRef]
    public string[] m_terrainSounds;
    [FMODUnity.EventRef]
    public string[] m_objectFootsteps;
    public WaterFootstepData[] m_waterFootsteps;

    public FMODUnity.StudioEventEmitter m_eventEmitter;
    public string m_currentSound;
    private string m_currentSoundTrack;

    [Header("JumpSounds")]
    [FMODUnity.EventRef]
    public string[] m_jumpSounds;
    public WaterFootstepData[] m_waterJump;

    [Header("LandSounds")]
    [FMODUnity.EventRef]
    public string[] m_landSounds;
    [FMODUnity.EventRef]
    public string m_waterLand;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Used to keep track of the different terrains the player has walked on
    /// Is kept as a dictionary, to easily cache the terrain should the player walk on it
    /// rather than get all the values at runtime, it only gets it once at runtime.
    /// </summary>
    public Dictionary<string, CachedTerrain> m_cachedTerrain;
    [System.Serializable]
    public class CachedTerrain
    {
        public TerrainData m_tData;
        public int alphamapWidth;
        public int alphamapHeight;
        public float[,,] splatmapData;
        public int numTextures;

        public void AssignData(Terrain p_newTerrain)
        {
            m_tData = p_newTerrain.terrainData;
            alphamapWidth = m_tData.alphamapWidth;
            alphamapHeight = m_tData.alphamapHeight;
            splatmapData = m_tData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
            numTextures = splatmapData.Length / (alphamapWidth * alphamapHeight);
        }
    }

    [System.Serializable]
    public class WaterFootstepData
    {
        [FMODUnity.EventRef]
        public string m_fmodEvent;
        public float m_maxFootstepDepth;
    }

    public void PlaySound()
    {
        int p_ind;
        float p_hitDis;
        m_currentSound = GetFootstepEvent(out p_ind, out p_hitDis);
        if (m_currentSoundTrack != m_currentSound)
        {
            m_eventEmitter.Event = m_currentSound;
            m_eventEmitter.Lookup();
        }

        m_currentSoundTrack = m_currentSound;
        m_eventEmitter.Play();
    }


    public void PlayJumpSound()
    {
        int hitIndex = 0;
        float p_hitDis = 0;
        GetFootstepEvent(out hitIndex, out p_hitDis);

        if (hitIndex < 3)
        {
            m_currentSound = m_jumpSounds[hitIndex + 1];
        }
        else
        {
            bool found = false;
            foreach (WaterFootstepData data in m_waterJump)
            {
                if (data.m_maxFootstepDepth < p_hitDis)
                {
                    found = true;
                    m_currentSound = data.m_fmodEvent;
                    break;
                }
            }
            if (!found)
            {
                m_currentSound = m_waterJump[m_waterJump.Length - 1].m_fmodEvent;
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


    public void PlayLandSound()
    {
        int hitIndex = 0;
        float p_hitDis = 0;
        GetFootstepEvent(out hitIndex, out p_hitDis);

        if (hitIndex < 3)
        {
            m_currentSound = m_landSounds[hitIndex + 1];
        }
        else
        {
            m_currentSound = m_waterLand;
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
    public string GetFootstepEvent(out int p_hitIndex, out float p_waterDis)
    {
        p_waterDis = 0;
        p_hitIndex = 0;
        RaycastHit hit;
        Debug.DrawLine(m_playerHeadObject.position, m_playerHeadObject.position + (transform.position - m_playerHeadObject.position) * 5, Color.red, .5f);
        if (Physics.Raycast(m_playerHeadObject.position, transform.position - m_playerHeadObject.position, out hit, 5, m_nonTerrainMask))
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
                    p_hitIndex = -1;
                    return m_terrainSounds[0];
                }
            }
            else
            {
                p_hitIndex = -1;
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

                return GetSteppedObject(newStepObject, hit, out p_hitIndex, out p_waterDis);
            }

        }

        return m_terrainSounds[0];//m_terrainSounds[GetActiveTerrainTextureIdx(transform.position, Terrain.activeTerrain)];

        return "Oof";
    }

    /// <summary>
    /// Returns the fmod event of the stepped on object. <br/>
    /// The water has a special function to measure how deep the raycast is, to play the <br/>
    /// proper depth for the water<br/>
    /// Called in the GetFootstepEvent function
    /// </summary>
    private string GetSteppedObject(SoundChanger_FootSteps p_newStepObject, RaycastHit hit, out int p_hitIndex, out float p_waterDis)
    {
        p_waterDis = 0;
        switch (p_newStepObject.m_soundType)
        {
            case SoundChanger_FootSteps.SoundType.Rock:
                p_hitIndex = 0;
                return m_objectFootsteps[0];
                break;

            case SoundChanger_FootSteps.SoundType.Leaves:
                p_hitIndex = 1;
                return m_objectFootsteps[1];
                break;
            case SoundChanger_FootSteps.SoundType.Wood:
                p_hitIndex = 2;
                return m_objectFootsteps[2];
                break;

            case SoundChanger_FootSteps.SoundType.Water:
                p_hitIndex = 3;
                float dis = Vector3.Distance(m_playerHeadObject.position, hit.point);
                p_waterDis = dis;
                foreach (WaterFootstepData data in m_waterFootsteps)
                {
                    if (data.m_maxFootstepDepth < dis)
                    {

                        return data.m_fmodEvent;
                    }
                }
                return m_waterFootsteps[m_waterFootsteps.Length - 1].m_fmodEvent;
                break;
        }
        p_hitIndex = 0;
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
            CachedTerrain newTerrain = new CachedTerrain();
            newTerrain.AssignData(p_tData);
            m_cachedTerrain.Add(p_tData.gameObject.name, newTerrain);
        }
        CachedTerrain cachedData = m_cachedTerrain[p_tData.gameObject.name];

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
