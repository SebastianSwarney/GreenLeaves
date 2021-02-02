using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_LoadingManager : MonoBehaviour
{

    public static Map_LoadingManager Instance;

    public string m_currentMainArea;

    public List<MapSections> m_mapScenes;

    [System.Serializable]
    public class MapSections
    {
        public string m_areaSceneName;
        public bool m_areaLoaded;
    }


    public List<MapData> m_allMapData;

    [System.Serializable]
    public class MapData
    {
        public string m_mapName;

        public Map_LoadingData m_mapLoadingData;

        public List<int> m_cutDownTrees = new List<int>();
        public List<int> m_cutDownThornBush = new List<int>();
        public List<int> m_cutDownLog = new List<int>();

        public List<int> m_toolComponents = new List<int>();

        public List<BerryBushes> m_allBerryBushes = new List<BerryBushes>();
        public List<ItemResource> m_itemResources = new List<ItemResource>();


        #region Data Containers

        [System.Serializable]
        public class BerryBushes
        {
            public int m_berriesLeft;
            public bool m_cutDown;
        }

        [System.Serializable]
        public class ItemResource
        {
            public GameObject m_resourceType;
            public string m_resourceName;
            public List<ResourceData> m_resourceTransforms = new List<ResourceData>();
            
            [System.Serializable]
            public class ResourceData
            {
                public Vector3 m_worldPos;
                public Quaternion m_rotation;
                public int m_resourceAmount;
            }
            public void AddNewResource(Transform p_resource)
            {
                ResourceData newTransform = new ResourceData();
                newTransform.m_worldPos = p_resource.position;
                newTransform.m_rotation = p_resource.rotation;
                newTransform.m_resourceAmount = p_resource.GetComponent<Resource_Pickup>().m_resourceAmount;
                m_resourceTransforms.Add(newTransform);
            }

            public void ClearResourceList()
            {
                m_resourceTransforms.Clear();
            }
        }

        #endregion

        public void AddItemToItemList(GameObject p_attemptedItem)
        {
            ResourceData data = p_attemptedItem.GetComponent<Resource_Pickup>().m_resourceInfo.m_resourceData;
            foreach (ItemResource ty in m_itemResources)
            {
                if (ty.m_resourceName == data.m_resourceName)
                {
                    ty.AddNewResource(p_attemptedItem.transform);
                    return;
                }
            }
            Debug.LogError("Item: " + p_attemptedItem.name + " | Cannot be added");

        }

        public void SaveMapData(Map_LoadingData p_data, List<ResourceDataTypes> p_allResourceTypes)
        {
            m_mapName = p_data.m_mapName;

            #region Save Trees
            m_cutDownTrees.Clear();
            foreach (GameObject loadedTree in p_data.m_allTrees)
            {
                if (!loadedTree.activeSelf)
                {
                    m_cutDownTrees.Add(p_data.m_allTrees.IndexOf(loadedTree));
                }
            }
            #endregion

            #region Save Berry Bushes
            m_allBerryBushes.Clear();
            foreach (GameObject loadedBush in p_data.m_allBerryBushes)
            {
                BerryBushes newBush = new BerryBushes();
                newBush.m_cutDown = !loadedBush.activeSelf;
                newBush.m_berriesLeft = loadedBush.GetComponentInChildren<Resource_Pickup_Renewable>().m_currentAmount;
                m_allBerryBushes.Add(newBush);
            }
            #endregion

            #region Save Thorn Bushes
            m_cutDownThornBush.Clear();
            foreach (GameObject loadedBush in p_data.m_allThornBushes)
            {
                if (!loadedBush.activeSelf)
                {
                    m_cutDownThornBush.Add(p_data.m_allThornBushes.IndexOf(loadedBush));
                }
            }
            #endregion

            #region Save Log
            m_cutDownLog.Clear();
            foreach (GameObject loadedLog in p_data.m_allLoadedLogs)
            {
                if (!loadedLog.activeSelf)
                {
                    m_cutDownLog.Add(p_data.m_allLoadedLogs.IndexOf(loadedLog));
                }
            }

            #endregion

            #region Save Tool Components
            foreach (GameObject loadedComponent in p_data.m_toolComponentUnlocks)
            {
                if (!loadedComponent.activeSelf)
                {
                    m_cutDownLog.Add(p_data.m_toolComponentUnlocks.IndexOf(loadedComponent));
                }
            }
            #endregion

            #region Save Resource Items

            if (m_itemResources.Count == 0)
            {
                foreach (ResourceDataTypes ty in p_allResourceTypes)
                {
                    ItemResource cont = new ItemResource();
                    cont.m_resourceType = ty.m_resourcePrefab;
                    cont.m_resourceName = ty.m_resourceType.m_resourceData.m_resourceName;
                    m_itemResources.Add(cont);
                }
            }

            foreach (ItemResource ty in m_itemResources)
            {
                ty.m_resourceTransforms.Clear();
            }

            foreach (GameObject loadedItem in p_data.m_allResources)
            {
                if (!loadedItem.activeSelf) continue;
                ResourceData data = loadedItem.GetComponent<Resource_Pickup>().m_resourceInfo.m_resourceData;
                foreach (ItemResource ty in m_itemResources)
                {
                    if (ty.m_resourceName == data.m_resourceName)
                    {
                        ty.AddNewResource(loadedItem.transform);
                        continue;
                    }
                }
            }
            #endregion


        }

    }


    public List<ResourceDataTypes> m_allResourceTypes;
    [System.Serializable]
    public struct ResourceDataTypes
    {
        public ResourceContainer m_resourceType;
        public GameObject m_resourcePrefab;
    }



    public List<LoadingAreas> m_sceneTriggerLoaders;


    [System.Serializable]
    public class LoadingAreas
    {
        public string m_mainLoadingAreaScene;
        public List<int> m_loadAreas;
        public List<int> m_unloadAreas;
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }


    #region Saving & Loading Functionality
    public void SaveMapData(Map_LoadingData p_data)
    {
        int saveIndex = 0;
        bool saved = false;

        foreach (MapData data in m_allMapData)
        {
            if (data.m_mapName == p_data.m_mapName)
            {
                saveIndex = m_allMapData.IndexOf(data);
                saved = true;
                break;
            }
        }

        if (!saved)
        {
            MapData newMap = new MapData();
            newMap.m_mapName = p_data.m_mapName;
            m_allMapData.Add(newMap);
            saveIndex = m_allMapData.Count - 1;
        }
        m_allMapData[saveIndex].m_mapLoadingData = p_data;
        m_allMapData[saveIndex].SaveMapData(p_data, m_allResourceTypes);

    }

    public void SaveSingleItem(GameObject p_newItem, string p_mapAreaName)
    {
        MapData map = GetMapDataByName(p_mapAreaName);
        map.AddItemToItemList(p_newItem);
    }

    public MapData LoadMapData(string p_mapName, out bool p_initialLoad)
    {
        foreach (MapData data in m_allMapData)
        {
            if (data.m_mapName == p_mapName)
            {
                p_initialLoad = false;
                return data;
            }
        }
        p_initialLoad = true;
        return null;
    }

    public void LoadMapFromTrigger(int p_index)
    {

        for (int i = 0; i < m_sceneTriggerLoaders[p_index].m_loadAreas.Count; i++)
        {
            MapSections current = m_mapScenes[m_sceneTriggerLoaders[p_index].m_loadAreas[i]];
            if (!current.m_areaLoaded)
            {
                current.m_areaLoaded = true;
                LoadSceneAdditive(current.m_areaSceneName);
            }

        }

        for (int i = 0; i < m_sceneTriggerLoaders[p_index].m_unloadAreas.Count; i++)
        {
            MapSections current = m_mapScenes[m_sceneTriggerLoaders[p_index].m_unloadAreas[i]];
            if (current.m_areaLoaded)
            {
                current.m_areaLoaded = false;
                UnloadArea(current.m_areaSceneName);
            }
        }


        m_currentMainArea = m_sceneTriggerLoaders[p_index].m_mainLoadingAreaScene;
    }

    private void LoadSceneAdditive(string p_loadedScene)
    {

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(p_loadedScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);

    }

    public void UnloadArea(string p_unloadSpace)
    {
        if (m_currentMainArea == p_unloadSpace)
        {
            List<GameObject> poolResources = GetCurrentOccupiedMapArea().m_allResources;
            SaveMapData(GetCurrentOccupiedMapArea());
            foreach (GameObject pooled in poolResources)
            {
                ObjectPooler.Instance.ReturnToPool(pooled);
            }
        }
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(p_unloadSpace);

    }

    #endregion

    #region Getter Functions

    public Map_LoadingData GetCurrentOccupiedMapArea()
    {
        foreach (MapData data in m_allMapData)
        {
            if (data.m_mapName == m_currentMainArea)
            {
                return data.m_mapLoadingData;
            }
        }
        Debug.LogError("No Map Area Data Exists for: " + m_currentMainArea);
        return null;
    }

    public MapData GetMapDataByName(string p_name)
    {
        foreach (MapData data in m_allMapData)
        {
            if (data.m_mapName == p_name)
            {
                return data;
            }
        }
        Debug.LogError("No Map Data Exists for: " + m_currentMainArea);
        return null;
    }

    #endregion

    /*[Header("Debug")]
    public bool m_createResouceDataContainers;
    public List<ResourceContainer> m_allContainers;

    private void OnValidate()
    {
        if (m_createResouceDataContainers)
        {
            m_createResouceDataContainers = false;
            m_allResourceTypes.Clear();
            foreach (ResourceContainer cont in m_allContainers)
            {
                ResourceDataTypes newCont = new ResourceDataTypes();
                newCont.m_resourceType = cont;
                newCont.m_resourcePrefab = cont.m_resourceData.m_resourcePrefab;
                m_allResourceTypes.Add(newCont);
            }
        }
    }*/
}
