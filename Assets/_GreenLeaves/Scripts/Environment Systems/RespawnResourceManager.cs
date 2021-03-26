using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnResourceManager : MonoBehaviour
{


    public static RespawnResourceManager Instance;
    public bool m_performTimers;


    public List<GameObject> m_allResourcesPrefabs;
    [Header("Pre-placed Resources")]
    public List<GameObject> m_allPrePlacedResources;
    [Header("Not this one, the one above")]
    public List<ResourceCounter> m_respawnResources;
    [System.Serializable]
    public class ResourceCounter
    {
        public GameObject m_resourcePrefab;
        public Vector3 m_worldPosition;
        public Quaternion m_worldRotation;
        public Transform m_parent;

        public float m_currentTimer;
        public GameObject TimerCompleted(float p_targetTime)
        {
            if (m_currentTimer >= p_targetTime)
            {
                GameObject spawnedObject = ObjectPooler.Instance.NewObject(m_resourcePrefab, m_worldPosition, m_worldRotation);
                spawnedObject.transform.parent = m_parent;
                spawnedObject.GetComponent<Resource_Pickup>().m_resourceAmount = 1;
                return spawnedObject;
            }
            m_currentTimer += Time.deltaTime;
            return null;
        }

        public GameObject PerformTimeSkip(float p_targetTime, float p_timeSkipped)
        {
            m_currentTimer += p_timeSkipped;

            if (m_currentTimer >= p_targetTime)
            {
                GameObject spawnedObject = ObjectPooler.Instance.NewObject(m_resourcePrefab, m_worldPosition, m_worldRotation);
                spawnedObject.transform.parent = m_parent;
                spawnedObject.GetComponent<Resource_Pickup>().m_resourceAmount = 1;
                return spawnedObject;
            }

            return null;
        }
    }


    public float m_timeToRespawnResources;
    private List<int> m_removeResourceCounter = new List<int>();
    private GameObject m_returnedResource;

    [Header("Berries")]
    public List<BerryCounter> m_berries;
    public float m_berryTimeToRespawnFull;
    [System.Serializable]
    public class BerryCounter
    {
        public Resource_Pickup_Renewable m_currentBerryCollection;
        public float m_currentTimer;
        public bool m_completed;

        public void PerformTimer(float p_timeToRespawn)
        {
            if (m_completed) return;
            if (m_currentTimer >= p_timeToRespawn)
            {
                m_completed = true;
                m_currentBerryCollection.ResetAmount();
            }
            m_currentTimer += Time.deltaTime;
        }

        public void PerformTimeSkip(float p_timeToRespawn, float p_addedTime)
        {
            if (m_completed) return;
            m_currentTimer += p_addedTime;
            if (m_currentTimer > p_timeToRespawn)
            {
                m_completed = true;
                m_currentBerryCollection.ResetAmount();
            }
        }
    }


    [Header("Campfire")]
    public List<CampfireTimer> m_campfires;
    public float m_campfireLifeTime;
    [System.Serializable]
    public class CampfireTimer
    {
        public Building_PlacementManager m_campfire;
        public float m_timer;

        public bool CampfireAlive(float p_lifetime)
        {
            if(m_timer > p_lifetime)
            {
                m_campfire.PlaceBuildingUnlit();
                return false;
            }
            m_timer += Time.deltaTime;
            return true;
        }

        public bool AliveAfterTimeSkip(float p_lifetime, float p_addedTime)
        {
            m_timer += p_addedTime;
            if (m_timer > p_lifetime)
            {
                m_campfire.PlaceBuildingUnlit();
                return false;
            }
            return true;
        }

    }

    private void Awake()
    {
        Instance = this;
    }

    


    private void Update()
    {
        if (!m_performTimers) return;
        if (Inventory_2DMenu.Instance.m_isOpen || PlayerUIManager.Instance.m_isPaused  || Interactable_Readable_Menu.Instance.m_isOpen) return;

        foreach (BerryCounter berr in m_berries)
        {
            if (berr.m_currentBerryCollection.enabled && !berr.m_completed)
            {
                berr.PerformTimer(m_berryTimeToRespawnFull);
            }
            else
            {
                m_removeResourceCounter.Add(m_berries.IndexOf(berr));
            }
        }
        if (m_removeResourceCounter.Count > 0)
        {
            m_removeResourceCounter.Reverse();
            for (int i = 0; i < m_removeResourceCounter.Count; i++)
            {
                m_berries.RemoveAt(m_removeResourceCounter[i]);
            }
            m_removeResourceCounter.Clear();
        }



        foreach (ResourceCounter res in m_respawnResources)
        {
            m_returnedResource = res.TimerCompleted(m_timeToRespawnResources);
            if (m_returnedResource != null)
            {
                m_removeResourceCounter.Add(m_respawnResources.IndexOf(res));
                m_allPrePlacedResources.Add(m_returnedResource);
                m_returnedResource = null;
            }
        }
        if (m_removeResourceCounter.Count > 0)
        {
            m_removeResourceCounter.Reverse();
            for (int i = 0; i < m_removeResourceCounter.Count; i++)
            {
                m_respawnResources.RemoveAt(m_removeResourceCounter[i]);
            }
            m_removeResourceCounter.Clear();
        }


        foreach(CampfireTimer fire in m_campfires)
        {
            if (!fire.CampfireAlive(m_campfireLifeTime))
            {
                m_removeResourceCounter.Add(m_campfires.IndexOf(fire));
            }
        }
        if(m_removeResourceCounter.Count > 0)
        {
            m_removeResourceCounter.Reverse();
            for (int i = 0; i < m_removeResourceCounter.Count; i++)
            {
                m_campfires.RemoveAt(m_removeResourceCounter[i]);
            }
            m_removeResourceCounter.Clear();
        }
    }



    public void AddBerryCollection(Resource_Pickup_Renewable p_addedBerry)
    {

        foreach (BerryCounter berr in m_berries)
        {
            if (berr.m_currentBerryCollection == p_addedBerry)
            {
                if (berr.m_completed)
                {
                    berr.m_completed = false;
                    berr.m_currentTimer = 0;
                }

                return;
            }
        }

        BerryCounter newBerry = new BerryCounter();
        newBerry.m_currentBerryCollection = p_addedBerry;
        newBerry.m_currentTimer = 0;
        newBerry.m_completed = false;
        m_berries.Add(newBerry);

    }

    public void AddNewResourceTimer(Resource_Pickup p_pickup)
    {
        if (!m_allPrePlacedResources.Contains(p_pickup.gameObject)) return;
        ResourceCounter newCounter = new ResourceCounter();
        foreach (GameObject prefab in m_allResourcesPrefabs)
        {
            if (prefab.name == p_pickup.gameObject.name)
            {
                newCounter.m_resourcePrefab = prefab;
                break;
            }
        }
        newCounter.m_worldPosition = p_pickup.transform.position;
        newCounter.m_worldRotation = p_pickup.transform.rotation;
        newCounter.m_parent = p_pickup.transform.parent;
        m_respawnResources.Add(newCounter);
        m_allPrePlacedResources.Remove(p_pickup.gameObject);
    }

    public void AddCampfire(Building_PlacementManager p_campfire)
    {
        CampfireTimer fire = new CampfireTimer();
        fire.m_campfire = p_campfire;
        fire.m_timer = 0;
        m_campfires.Add(fire);
    }

    public void TimeSkipped(float p_amountOfTime)
    {
        foreach (BerryCounter berr in m_berries)
        {

            berr.PerformTimeSkip(m_berryTimeToRespawnFull, p_amountOfTime);
            if (berr.m_completed)
            {
                m_removeResourceCounter.Add(m_berries.IndexOf(berr));
            }
        }
        if (m_removeResourceCounter.Count > 0)
        {
            m_removeResourceCounter.Reverse();
            for (int i = 0; i < m_removeResourceCounter.Count; i++)
            {
                m_berries.RemoveAt(m_removeResourceCounter[i]);
            }
            m_removeResourceCounter.Clear();
        }



        foreach (ResourceCounter res in m_respawnResources)
        {
            m_returnedResource = res.PerformTimeSkip(m_timeToRespawnResources, p_amountOfTime);
            if (m_returnedResource != null)
            {
                m_removeResourceCounter.Add(m_respawnResources.IndexOf(res));
                m_allPrePlacedResources.Add(m_returnedResource);
                m_returnedResource = null;
            }
        }
        if (m_removeResourceCounter.Count > 0)
        {
            m_removeResourceCounter.Reverse();
            for (int i = 0; i < m_removeResourceCounter.Count; i++)
            {
                m_respawnResources.RemoveAt(m_removeResourceCounter[i]);
            }
            m_removeResourceCounter.Clear();
        }


        foreach (CampfireTimer fire in m_campfires)
        {
            if (!fire.AliveAfterTimeSkip(m_campfireLifeTime, p_amountOfTime))
            {
                m_removeResourceCounter.Add(m_campfires.IndexOf(fire));
            }
        }
        if (m_removeResourceCounter.Count > 0)
        {
            m_removeResourceCounter.Reverse();
            for (int i = 0; i < m_removeResourceCounter.Count; i++)
            {
                m_campfires.RemoveAt(m_removeResourceCounter[i]);
            }
            m_removeResourceCounter.Clear();
        }
    }
}
