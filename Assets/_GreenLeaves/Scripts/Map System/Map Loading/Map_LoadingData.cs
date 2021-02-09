using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_LoadingData : MonoBehaviour
{
    public string m_mapName;

    public List<GameObject> m_allTrees;
    public List<GameObject> m_allThornBushes;
    public List<GameObject> m_allLoadedLogs;

    public List<GameObject> m_toolComponentUnlocks;

    public List<GameObject> m_allResources;

    [Header("Berry Bushes")]
    public List<GameObject> m_allHungerBushes;
    public List<GameObject> m_allEnergyBushes;
    public List<GameObject> m_allStaminaBushes;

    [Header("Campfires")]
    public GameObject m_campfirePrefab;
    public List<GameObject> m_allCampfires;
    private void OnEnable()
    {
        bool initialLoad = false;
        Map_LoadingManager.MapData data = Map_LoadingManager.Instance.LoadMapData(m_mapName, out initialLoad);
        if (!initialLoad)
        {
            LoadMap(data);
        }

        data = Map_LoadingManager.Instance.SaveMapData(this);
        if (initialLoad)
        {
            foreach (GameObject saved in m_allResources)
            {
                Map_LoadingManager.Instance.SaveSingleItem(saved, m_mapName);
            }
            LoadInitialMap(data);
        }
    }

    private void LoadMap(Map_LoadingManager.MapData p_data)
    {
        #region LoadTrees
        foreach (GameObject tree in m_allTrees)
        {
            if (p_data.m_cutDownTrees.Contains(m_allTrees.IndexOf(tree)))
            {
                tree.SetActive(false);
                Debug.Log("Load Tree Stump here if we want to");
            }
        }
        #endregion

        #region Load Thorn Bushes
        foreach (GameObject thornBush in m_allThornBushes)
        {
            if (p_data.m_cutDownThornBush.Contains(m_allThornBushes.IndexOf(thornBush)))
            {
                thornBush.SetActive(false);
            }
        }
        #endregion

        #region Load Tool Component
        foreach (GameObject tool in m_toolComponentUnlocks)
        {
            if (p_data.m_toolComponents.Contains(m_toolComponentUnlocks.IndexOf(tool)))
            {
                tool.SetActive(false);
            }
        }
        #endregion

        #region Load Resources
        foreach (GameObject resource in m_allResources)
        {
            ObjectPooler.Instance.ReturnToPool(resource);
        }
        m_allResources.Clear();
        GameObject newSpawn;
        foreach (Map_LoadingManager.MapData.ItemResource res in p_data.m_itemResources)
        {
            newSpawn = res.m_resourceType;

            foreach (Map_LoadingManager.MapData.ItemResource.ResourceData newTra in res.m_resourceTransforms)
            {
                GameObject newItem = ObjectPooler.Instance.NewObject(newSpawn, newTra.m_worldPos, newTra.m_rotation);
                newItem.GetComponent<Resource_Pickup>().m_resourceAmount = newTra.m_resourceAmount;
                m_allResources.Add(newItem);
            }
        }

        #endregion

        #region Load Berry Bushes

        if (m_allHungerBushes.Count > 0)
        {
            SetBerryRate(m_allHungerBushes, p_data.m_allHungerBushes, 0);
        }
        if (m_allEnergyBushes.Count > 0)
        {
            SetBerryRate(m_allEnergyBushes, p_data.m_allEnergyBushes, 1);
        }
        if (m_allStaminaBushes.Count > 0)
        {
            SetBerryRate(m_allStaminaBushes, p_data.m_allStaminaBushes, 2);
        }
        #endregion

        #region Load Campfire
        m_allCampfires.Clear();
        foreach(Map_LoadingManager.MapData.CampfireVaris fire in p_data.m_campfires)
        {
            GameObject newFire = ObjectPooler.Instance.NewObject(m_campfirePrefab, fire.m_position, fire.m_rotation);
            newFire.GetComponent<Building_PlacementManager>().PlaceBuildingUnlit();
            m_allCampfires.Add(newFire);
        }
        #endregion
    }

    private void LoadInitialMap(Map_LoadingManager.MapData p_data)
    {
        if (m_allHungerBushes.Count > 0)
        {
            SetBerryRate(m_allHungerBushes, p_data.m_allHungerBushes, 0);
        }
        if (m_allEnergyBushes.Count > 0)
        {
            SetBerryRate(m_allEnergyBushes, p_data.m_allEnergyBushes, 1);
        }
        if (m_allStaminaBushes.Count > 0)
        {
            SetBerryRate(m_allStaminaBushes, p_data.m_allStaminaBushes, 2);
        }
    }

    private void SetBerryRate(List<GameObject> p_berryList, List<Map_LoadingManager.MapData.BerryBushes> p_berryData, int p_berryType)
    {
        List<Resource_Pickup_Renewable> activeBerryBushes = new List<Resource_Pickup_Renewable>();
        int currentBerryCount = 0;
        float chance = 0;



        for (int i = 0; i < p_berryList.Count; i++)
        {
            if (p_berryData[i].m_cutDown)
            {
                p_berryList[i].SetActive(false);
                continue;
            }
            else
            {
                activeBerryBushes.Add(p_berryList[i].GetComponentInChildren<Resource_Pickup_Renewable>());
            }
            p_berryList[i].GetComponentInChildren<Resource_Pickup_Renewable>().SetBerryAmount(p_berryData[i].m_berriesLeft);
            currentBerryCount += p_berryData[i].m_berriesLeft;
        }

        if (currentBerryCount < Map_LoadingManager.Instance.m_maxBerryCount)
        {
            if (p_berryType == 0)
            {
                chance = Map_LoadingManager.Instance.GetHungerBerryChance(currentBerryCount);
            }
            else if (p_berryType == 1)
            {
                chance = Map_LoadingManager.Instance.GetEnergyBerryChance(currentBerryCount);
            }
            else if (p_berryType == 2)
            {
                chance = Map_LoadingManager.Instance.GetEnergyBerryChance(currentBerryCount);
            }

            RandomizeBerryList(ref activeBerryBushes);
            foreach (Resource_Pickup_Renewable bush in activeBerryBushes)
            {
                if (Random.Range(0f, 1f) < chance)
                {

                    
                    int amountToAdd = Random.Range(0, Map_LoadingManager.Instance.m_maxBerryCount - currentBerryCount);
                    if (amountToAdd <= 0) continue;

                    int amountNeeded = bush.m_amountOfHarvestable - bush.m_currentAmount;
                    if(amountToAdd > amountNeeded)
                    {
                        amountToAdd = amountNeeded;
                    }

                    currentBerryCount += amountNeeded;
                    chance -= amountToAdd * Map_LoadingManager.Instance.m_percentDecreasePerBerry;
                    bush.SetBerryAmount(bush.m_currentAmount + amountToAdd);

                    if (bush.m_currentAmount < 0)
                    {
                        Debug.Log("Index Error ");
                    }
                }
            }

        }
    }

    private void RandomizeBerryList(ref List<Resource_Pickup_Renewable> p_list)
    {

        Resource_Pickup_Renewable temp = p_list[0];
        int newIndex = Random.Range(0, p_list.Count);
        for (int i = 0; i < p_list.Count; i++)
        {
            p_list[i] = p_list[newIndex];
            p_list[newIndex] = temp;
            newIndex = Random.Range(0, p_list.Count);
            temp = p_list[i];
        }

    }

    public float GetBerrySpawnRate()
    {
        //Base on player health & berries in inventory
        //Adjust using animation curve
        //Less berries and less health = more sawn chance

        //More chance of spawning berries when hungry

        //Note down if a berry spawned with berries, and still has some
        //Redo formula if no bushes have berries, and dont do formula if bushes have berries

        return 0;
    }
}
