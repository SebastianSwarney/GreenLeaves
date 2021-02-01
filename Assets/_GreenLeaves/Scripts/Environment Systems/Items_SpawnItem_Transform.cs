using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class will spawn an object at a specific given transform. <br/>
/// Multiple transforms can be used, as long as they are assigned an object as well.
/// </summary>
public class Items_SpawnItem_Transform : Items_SpawnItem
{


    [Header("Transform Class Only")]
    public List<ItemSpawnContainer_Transform> m_spawnedItems;

    public override void SpawnItem()
    {
        foreach (ItemSpawnContainer_Transform newItem in m_spawnedItems)
        {
            newItem.SpawnItems(transform);
        }
    }

    /// <summary>
    /// This data container holds the varaibles used to spawn at different transforms.
    /// 1 data container will result in 1 spawned object.
    /// </summary>
    [System.Serializable]
    public class ItemSpawnContainer_Transform
    {
        public GameObject m_spawnedItem;
        public List<Vector3> m_spawnPoint;
        public Color m_debugColor;
        public void SpawnItems(Transform p_pos)
        {
            foreach (Vector3 spawn in m_spawnPoint)
            {
                GameObject newItem = ObjectPooler.Instance.NewObject(m_spawnedItem, p_pos.position + p_pos.rotation * (spawn), Quaternion.identity);
                if(newItem.GetComponent<Resource_Pickup>() != null)
                {
                    newItem.GetComponent<Resource_Pickup>().ResetResourceAmount();
                    Map_LoadingManager.Instance.GetCurrentOccupiedMapArea().m_allResources.Add(newItem);
                }
            }
        }
    }

    /*private void Update()
    {
        foreach (ItemSpawnContainer_Transform newItem in m_spawnedItems)
        {
            Gizmos.color = newItem.m_debugColor;
            foreach (Vector3 point in newItem.m_spawnPoint)
            {
                Debug.DrawLine(transform.position, transform.position + transform.rotation * (point), newItem.m_debugColor);
            }

        }
    }*/

#if UNITY_EDITOR
    [Header("Debug")]
    public bool m_debug;


    private void OnDrawGizmos()
    {
        if (!m_debug) return;
        foreach (ItemSpawnContainer_Transform newItem in m_spawnedItems)
        {
            Gizmos.color = newItem.m_debugColor;
            foreach (Vector3 point in newItem.m_spawnPoint)
            {
                Gizmos.DrawSphere(transform.position + transform.rotation * (point), 1f);
                Debug.DrawLine(transform.position, transform.position + transform.rotation * (point), newItem.m_debugColor);
            }

        }

    }
#endif
}


