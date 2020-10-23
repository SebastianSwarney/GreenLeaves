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
        foreach(ItemSpawnContainer_Transform newItem in m_spawnedItems)
        {
            newItem.SpawnItems();
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
        public List<Transform> m_spawnPoint;

        public void SpawnItems()
        {
            foreach (Transform spawn in m_spawnPoint)
            {
                ObjectPooler.Instance.NewObject(m_spawnedItem, spawn.position, Quaternion.identity);
            }
        }
    }
}


