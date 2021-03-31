using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_PlayerPlacement : MonoBehaviour
{
    public static Building_PlayerPlacement Instance;
    //public GameObject m_buildingPrefab;

    public bool m_isPlacing;

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
    public void StartPlacement(GameObject p_buildingPrefab)
    {
        m_currentPrefab = ObjectPooler.Instance.NewObject(p_buildingPrefab, transform.position, Quaternion.identity).transform;
        m_currentPrefab.GetComponent<Building_PlacementManager>().InitializePlacement();
        //m_currentPrefab = ObjectPooler.Instance.NewObject(m_buildingPrefab, transform.position, Quaternion.identity).transform;
        m_placementCoroutine = StartCoroutine(PlaceObject(m_currentPrefab.GetComponent<Building_PlacementManager>()));
    }

    private IEnumerator PlaceObject(Building_PlacementManager p_buildingObject)
    {
        m_isPlacing = true;
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
                p_buildingObject.transform.rotation = Quaternion.Euler(0,PlayerInputToggle.Instance.transform.eulerAngles.y,0);
                p_buildingObject.transform.position = m_detectionOrigin.position + m_detectionOrigin.forward * m_maxDis;
            }

            if (canPlace)
            {
                p_buildingObject.TogglePrompt(true);
                if (Input.GetMouseButtonDown(0) && !PlayerUIManager.Instance.m_isPaused)
                {
                    
                    m_isPlacing = false;
                    p_buildingObject.PlaceBuilding();
                    placed = true;
                }
            }
            else
            {
                p_buildingObject.TogglePrompt(false);
            }
            /*if (!placed)
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
            }*/

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
