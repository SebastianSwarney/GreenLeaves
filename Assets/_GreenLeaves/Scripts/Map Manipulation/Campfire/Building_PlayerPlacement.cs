﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_PlayerPlacement : MonoBehaviour
{
    public static Building_PlayerPlacement Instance;
    //public GameObject m_buildingPrefab;
    public ResourceContainer m_buildingResourceData;

    [Header("Detection")]
    public Transform m_detectionOrigin;
    public float m_maxDis;
    public LayerMask m_detectionLayer;

    private Transform m_currentPrefab;
    private Coroutine m_placementCoroutine;
    private void Awake()
    {
        Instance = this;
    }
    public void StartPlacement(GameObject p_buildingPrefab, ResourceContainer p_resourceContainer)
    {
        m_buildingResourceData = p_resourceContainer;
        m_currentPrefab = ObjectPooler.Instance.NewObject(p_buildingPrefab, transform.position, Quaternion.identity).transform;
        //m_currentPrefab = ObjectPooler.Instance.NewObject(m_buildingPrefab, transform.position, Quaternion.identity).transform;
        m_placementCoroutine = StartCoroutine(PlaceObject(m_currentPrefab.GetComponent<Building_PlacementManager>()));
    }

    private IEnumerator PlaceObject(Building_PlacementManager p_buildingObject)
    {
        bool placed = false;
        bool canPlace = false;
        RaycastHit hit;
        while (!placed)
        {
            canPlace = false;
            if (Physics.Raycast(m_detectionOrigin.position, m_detectionOrigin.forward, out hit, m_maxDis, m_detectionLayer))
            {
                if (p_buildingObject.AttemptPlacement(hit.point))
                {
                    canPlace = true;
                }
            }
            else
            {
                p_buildingObject.ToggleRendererEffects(false);
                p_buildingObject.transform.rotation = Quaternion.identity;
                p_buildingObject.transform.position = m_detectionOrigin.position + m_detectionOrigin.forward * m_maxDis;
            }

            if (canPlace)
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    Debug.Log("Building Placement Input Here", this);
                    p_buildingObject.PlaceBuilding();
                    placed = true;
                }
            }
            if (!placed)
            {
                ///If the player decides to cancel the placement, re-add the item to the inventory
                if (Input.GetKeyDown(KeyCode.X))
                {
                    Debug.Log("Building cancel Input Here", this);
                    Inventory_2DMenu.Instance.m_currentBuldingIcon = null;
                    Inventory_2DMenu.Instance.AddToInventory(m_buildingResourceData, 1);
                    ObjectPooler.Instance.ReturnToPool(m_currentPrefab.gameObject);
                    m_currentPrefab = null;
                    placed = true;
                }
            }

            yield return null;
        }

        m_placementCoroutine = null;
    }

    [Header("DEbugging")]
    public bool m_debug;
    public Color m_debugColor = Color.white;
    private void OnDrawGizmos()
    {
        if (!m_debug || m_detectionOrigin == null) return;
        Gizmos.color = m_debugColor;
        Gizmos.DrawLine(m_detectionOrigin.position, m_detectionOrigin.position + m_detectionOrigin.forward * m_maxDis);
        Gizmos.DrawWireSphere(m_detectionOrigin.position + m_detectionOrigin.forward * m_maxDis, .25f);
    }
}
