using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Loading_Trigger : MonoBehaviour
{
    public int m_mapSectorIndex;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger : " + other.gameObject.name + " | Tag: " + other.gameObject.tag);
        if (other.gameObject.tag != "Player") return;

        Map_LoadingManager.Instance.LoadMapFromTrigger(m_mapSectorIndex);

    }
}
