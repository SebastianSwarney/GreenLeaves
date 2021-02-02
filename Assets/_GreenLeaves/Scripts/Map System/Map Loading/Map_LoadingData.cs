using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_LoadingData : MonoBehaviour
{
    public string m_mapName;

    public List<GameObject> m_allTrees;
    public List<GameObject> m_allBerryBushes;
    public List<GameObject> m_allThornBushes;
    public List<GameObject> m_allLoadedLogs;

    public List<GameObject> m_toolComponentUnlocks;

    public List<GameObject> m_allResources;

    private void OnEnable()
    {
        bool initialLoad = false;
        Map_LoadingManager.MapData data = Map_LoadingManager.Instance.LoadMapData(m_mapName, out initialLoad);
        if (!initialLoad)
        {
            LoadMap(data);
        }
        Map_LoadingManager.Instance.SaveMapData(this);
        if (initialLoad)
        {
            foreach (GameObject saved in m_allResources)
            {
                Map_LoadingManager.Instance.SaveSingleItem(saved, m_mapName);
            }
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

        #region Load Berry Bushes
        for (int i = 0; i < m_allBerryBushes.Count; i++)
        {
            if (p_data.m_allBerryBushes[i].m_cutDown)
            {
                m_allBerryBushes[i].SetActive(false);
                continue;
            }
            m_allBerryBushes[i].GetComponentInChildren<Resource_Pickup_Renewable>().SetBerryAmount(p_data.m_allBerryBushes[i].m_berriesLeft);
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
    }

}
