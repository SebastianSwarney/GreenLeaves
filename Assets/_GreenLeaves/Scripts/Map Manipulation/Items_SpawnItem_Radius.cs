using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns an object in a specified radius around the object. <br/>
/// The Spawn Offset will offset the origin of the radius
/// </summary>
public class Items_SpawnItem_Radius : Items_SpawnItem
{
    [Header("Radius Class Only")]
    public List<GameObject> m_objectsToSpawn;
    public float m_spawnRadius;
    public Vector3 m_spawnOffset;


    /// <summary>
    /// Called to spawn the objects.
    /// </summary>
    public override void SpawnItem()
    {
        foreach(GameObject obj in m_objectsToSpawn)
        {
            ///May want to make this so that it spawns the items no in each other
            Vector3 newSpawnPoint = new Vector3(Random.Range(-m_spawnRadius, m_spawnRadius), 0, Random.Range(-m_spawnRadius, m_spawnRadius));
            newSpawnPoint += transform.position + m_spawnOffset;
            GameObject newRes = ObjectPooler.Instance.NewObject(obj, newSpawnPoint, Quaternion.identity);
            if(newRes.GetComponent<Manipulation_HitObject>() != null)
            {
                newRes.GetComponent<Manipulation_HitObject>().ObjectRespawn();
            }
        }
    }


#if UNITY_EDITOR

    [Header("Debugging")]
    public bool m_isDebugging;
    public Color m_debuggingColor = Color.white;
    private void OnDrawGizmos()
    {
        if (!m_isDebugging) return;
        Gizmos.color = m_debuggingColor;
        Gizmos.DrawWireSphere(transform.position + m_spawnOffset, m_spawnRadius);
    }

#endif
}

