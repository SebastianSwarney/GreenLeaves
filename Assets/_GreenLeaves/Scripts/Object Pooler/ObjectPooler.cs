using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    #region Variables
    public Dictionary<string, Queue<GameObject>> m_objectPool = new Dictionary<string, Queue<GameObject>>();
    public List<GameObject> m_pooledObjects;
    public List<Transform> m_pooledObjectParents;

    public int m_growthRate = 5;
    public int m_initalPoolSize = 5;

    public static ObjectPooler Instance { get; private set; }
    #endregion


    #region Object pooler
    void Awake()
    {
        Instance = this;
        InitialGrowth();
    }



    ///<Summary>
    //returns an object from the pooled objects
    ///needs the name of the pool to access it
    ///returns an object from the pool
    ///<summary>
    public GameObject NewObject(GameObject p_requestedObject, Transform p_spawner, bool p_moveToTransform = true, bool p_rotateToTransform = true, bool p_isActive = true)
    {
        return GetNewObject(p_requestedObject.name, p_requestedObject, p_spawner, p_moveToTransform, p_rotateToTransform, p_isActive);
    }

    private GameObject GetNewObject(string p_poolName, GameObject p_requestedObject, Transform p_spawner, bool p_moveToTransform = true, bool p_rotateToTransform = true, bool p_isActive = true)
    {
        if (!m_objectPool.ContainsKey(p_poolName))
        {
            CreateNewPool(p_requestedObject);
        }

        GameObject newObject = m_objectPool[p_poolName].Dequeue();
        if (m_objectPool[p_poolName].Count == 0)
        {
            IncreasePool(p_poolName, newObject, newObject.transform.parent.gameObject);
        }

        if (p_spawner != null)
        {

            if (p_moveToTransform)
            {
                newObject.transform.position = p_spawner.position;
            }

            if (p_rotateToTransform)
            {
                newObject.transform.rotation = p_spawner.rotation;
            }
        }
        else
        {
            if (p_moveToTransform)
            {
                newObject.transform.position = transform.position;
            }

            if (p_rotateToTransform)
            {
                newObject.transform.rotation = p_spawner.rotation;
            }
        }
        newObject.SetActive(p_isActive);
        return newObject;
    }


    public GameObject NewObject(GameObject p_requestedObject, Vector3 p_spawnPosition, Quaternion p_angle, bool p_isActive = true)
    {
        return GetNewObject(p_requestedObject.name, p_requestedObject, p_spawnPosition, p_angle, p_isActive);
    }

    private GameObject GetNewObject(string p_poolName, GameObject p_requestedObject, Vector3 p_spawnPostion, Quaternion p_angle, bool p_isActive)
    {
        if (!m_objectPool.ContainsKey(p_poolName))
        {
            CreateNewPool(p_requestedObject);
        }

        GameObject newObject = m_objectPool[p_poolName].Dequeue();
        if (m_objectPool[p_poolName].Count == 0)
        {
            IncreasePool(p_poolName, newObject, newObject.transform.parent.gameObject);
        }

        newObject.transform.position = p_spawnPostion;
        newObject.transform.rotation = p_angle;
        newObject.SetActive(p_isActive);
        return newObject;
    }

    ///<summary>
    ///When the pool is equal to zero, increase the pool
    ///called in the NewObject function
    ///<summary>
    private void IncreasePool(string p_poolName, GameObject p_pooledObject, GameObject p_poolParent)
    {
        for (int i = 0; i < m_growthRate; i++)
        {
            GameObject newObj = Instantiate(p_pooledObject);
            newObj.transform.parent = p_poolParent.transform;
            newObj.SetActive(false);
            newObj.name = p_pooledObject.name;
            m_objectPool[p_poolName].Enqueue(newObj);
        }
    }

    ///<summary>
    ///Returns the object to it's designated pool
    ///Called from the object
    ///<summary>
    public void ReturnToPool(GameObject p_pooledObject)
    {
        if (!m_objectPool.ContainsKey(p_pooledObject.name))
        {
            CreateNewPool(p_pooledObject);
        }
        m_objectPool[p_pooledObject.name].Enqueue(p_pooledObject);
        p_pooledObject.SetActive(false);
    }




    ///<summary>
    ///This function is only called at start
    ///creates all the pools, and puts then under the right transform
    ///<summary>
    private void InitialGrowth()
    {
        int indexNumbers = 0;
        foreach (GameObject newPool in m_pooledObjects)
        {
            Queue<GameObject> currentPool = new Queue<GameObject>();
            m_pooledObjectParents[indexNumbers].name = newPool.name + " Pool";
            for (int i = 0; i < m_initalPoolSize; i++)
            {
                GameObject newObj = Instantiate(newPool);
                newObj.transform.parent = m_pooledObjectParents[indexNumbers];
                currentPool.Enqueue(newObj);
                newObj.name = newPool.gameObject.name;
                newObj.SetActive(false);
            }

            string poolName = newPool.name;
            m_objectPool.Add(poolName, currentPool);
            indexNumbers += 1;
        }
    }

    private void CreateNewPool(GameObject p_newPool)
    {
        GameObject newParent = new GameObject(p_newPool.name + "s");

        m_pooledObjectParents.Add(newParent.transform);
        m_pooledObjects.Add(p_newPool);
        newParent.transform.parent = this.transform;
        newParent.name = p_newPool.name;
        Queue<GameObject> newQueue = new Queue<GameObject>();
        m_objectPool.Add(p_newPool.name, newQueue);
        IncreasePool(p_newPool.name, p_newPool, newParent);

    }


    #endregion
}
