
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_LevelMap : MonoBehaviour
{
    public static Map_LevelMap Instance;

    public GameObject m_mapObject;
    public RectTransform m_mapRect;
    [Header("Map Bounding Size")]
    public Vector2 m_mapSize;
    public bool m_isDebugging;
    public Color m_mapDebugColor;
    public bool m_drawWired;

    [Header("Players")]
    public List<Map_PlayerIcon> m_playerIconOwners;
    public List<RectTransform> m_playerIconCanvas;
    public GameObject m_playerIconPrefab;

    private bool m_isOpen;
    public Vector2 m_mapMultiplier;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        m_mapMultiplier = new Vector2(m_mapRect.sizeDelta.x / m_mapSize.x, m_mapRect.sizeDelta.y / m_mapSize.y);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap(!m_isOpen);
        }
        if (m_isOpen)
        {
            for (int i = 0; i < m_playerIconOwners.Count; i++)
            {
                UpdateIconPosition(i);
            }
        }
    }

    public void ToggleMap(bool p_newState)
    {
        m_isOpen = p_newState;
        m_mapObject.SetActive(p_newState);
    }

    public void AddNewPlayerIcon(Map_PlayerIcon p_newIcon, Color p_newColor)
    {
        m_playerIconOwners.Add(p_newIcon);
        GameObject newIcon = Instantiate(m_playerIconPrefab);

        m_playerIconCanvas.Add(newIcon.GetComponent<RectTransform>());
        newIcon.transform.parent = m_mapRect.transform;
        newIcon.transform.localScale = Vector3.one;
        newIcon.GetComponent<UnityEngine.UI.Image>().color = p_newColor;
    }

    public void RemoveIcon(Map_PlayerIcon p_deleteIcon)
    {
        if (m_playerIconOwners.Contains(p_deleteIcon))
        {
            int newIndex = m_playerIconOwners.IndexOf(p_deleteIcon);
            Destroy(m_playerIconCanvas[newIndex].gameObject);
            m_playerIconCanvas.RemoveAt(newIndex);
            m_playerIconOwners.RemoveAt(newIndex);
        }
    }

    private void UpdateIconPosition(int p_index)
    {

        if (m_playerIconOwners.Count > 0)
        {
            if (m_playerIconOwners[p_index] != null)
            {
                Vector2 newPoint = new Vector2(m_playerIconOwners[p_index].transform.position.x, m_playerIconOwners[p_index].transform.position.z);

                newPoint *= m_mapMultiplier;

                m_playerIconCanvas[p_index].localPosition = newPoint;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!m_isDebugging) return;
        Gizmos.color = m_mapDebugColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        if (m_drawWired)
        {
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(m_mapSize.x, 1, m_mapSize.y));
        }
        else
        {
            Gizmos.DrawCube(Vector3.zero, new Vector3(m_mapSize.x, 1, m_mapSize.y));
        }
    }
}
