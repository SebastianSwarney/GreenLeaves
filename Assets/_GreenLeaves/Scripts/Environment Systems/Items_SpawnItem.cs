using UnityEngine;

/// <summary>
/// A base class for spawning resources at a position.
/// This script will spawn a single object directly on the object.
/// </summary>
public class Items_SpawnItem : MonoBehaviour
{
    [Header("Base Class Only")]
    public GameObject m_spawnedObject;

    /// <summary>
    /// Called to spawn an object
    /// The spawn location will differ based on the inherited class' overrides
    /// </summary>
    public virtual void SpawnItem()
    {
        ObjectPooler.Instance.NewObject(m_spawnedObject, transform.position, Quaternion.identity);
    }
}
