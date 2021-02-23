using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Tutorial : MonoBehaviour
{
    public static Inventory_Tutorial Instance;

    public bool m_showTutorial = true;
    public int m_currentTutorialIndex;

    public List<GameObject> m_tutorialObjects;

    public GameObject m_pickupAlt, m_rotateAlt, m_letGoAlt;

    public GameObject m_skipButton;
    [Header("Interaction Tutorials")]
    public int m_pickupTutIndex;

    public int m_rotateTutIndex;

    public int m_letGoTutIndex;

    public GameObject m_dropArea, m_equipArea, m_craftArea, m_eatArea;
    public int m_dropIndex, m_equipIndex, m_craftIndex, m_eatIndex;
    private void Awake()
    {
        Instance = this;
    }

    public void StartInventory()
    {
        if (!m_showTutorial) return;
        m_skipButton.SetActive(true);
        NextTutorial();

        m_dropArea.SetActive(false);
        m_equipArea.SetActive(false);
        m_craftArea.SetActive(false);
        m_eatArea.SetActive(false);
        Inventory_2DMenu.Instance.m_canClose = false;
        Inventory_2DMenu.Instance.m_canTap = false;
    }

    public void PickUpTutorial()
    {
        if (!m_showTutorial) return;
        if (m_currentTutorialIndex == m_pickupTutIndex + 1)
        {
            NextTutorial();
        }
    }

    public void RotateTut()
    {
        if (!m_showTutorial) return;
        if (m_currentTutorialIndex == m_rotateTutIndex + 1)
        {
            NextTutorial();
        }
    }

    public void LetGoTutorial()
    {
        if (!m_showTutorial) return;
        if (m_currentTutorialIndex == m_letGoTutIndex + 1)
        {
            NextTutorial();
        }
    }

    public void NextTutorial()
    {
        if (!m_showTutorial) return;

        m_letGoAlt.SetActive(false);
        m_pickupAlt.SetActive(false);
        m_rotateAlt.SetActive(false);

        if (m_currentTutorialIndex == m_tutorialObjects.Count)
        {
            EndTutorial();
            return;
        }

        if (m_currentTutorialIndex - 1 >= 0)
        {
            m_tutorialObjects[m_currentTutorialIndex - 1].SetActive(false);
        }

        Inventory_2DMenu.Instance.m_canTap = false;
        if (m_currentTutorialIndex == m_pickupTutIndex || m_currentTutorialIndex == m_rotateTutIndex || m_currentTutorialIndex == m_letGoTutIndex)
        {
            Inventory_2DMenu.Instance.m_canTap = true;
        }

        if (m_currentTutorialIndex == m_dropIndex)
        {
            m_dropArea.SetActive(true);
        }
        else if (m_currentTutorialIndex == m_equipIndex)
        {
            m_equipArea.SetActive(true);
        }
        else if (m_currentTutorialIndex == m_craftIndex)
        {
            m_craftArea.SetActive(true);
        }
        else if (m_currentTutorialIndex == m_eatIndex)
        {
            m_eatArea.SetActive(true);
        }

        if (Inventory_2DMenu.Instance.m_backpack.m_itemsInBackpack.Count == 0)
        {
            if (m_currentTutorialIndex == m_pickupTutIndex)
            {
                if (Inventory_2DMenu.Instance.m_backpack.m_itemsInBackpack.Count == 0)
                {
                    m_pickupAlt.SetActive(true);
                }
            }
            else if (m_currentTutorialIndex == m_rotateTutIndex)
            {
                if (Inventory_2DMenu.Instance.m_backpack.m_itemsInBackpack.Count == 0)
                {
                    m_rotateAlt.SetActive(true);
                }
            }
            else if (m_currentTutorialIndex == m_letGoTutIndex)
            {
                if (Inventory_2DMenu.Instance.m_backpack.m_itemsInBackpack.Count == 0)
                {
                    m_letGoAlt.SetActive(true);
                }
            }
            else
            {
                m_tutorialObjects[m_currentTutorialIndex].SetActive(true);
            }
        }
        else
        {
            m_tutorialObjects[m_currentTutorialIndex].SetActive(true);
        }

        m_currentTutorialIndex++;

    }

    public void EndTutorial()
    {
        foreach (GameObject tut in m_tutorialObjects)
        {
            tut.SetActive(false);
        }

        m_showTutorial = false;
        m_dropArea.SetActive(true);
        m_equipArea.SetActive(true);
        m_craftArea.SetActive(true);
        m_eatArea.SetActive(true);

        Inventory_2DMenu.Instance.m_canClose = true;
        Inventory_2DMenu.Instance.m_canTap = true;
        m_skipButton.SetActive(false);
        gameObject.SetActive(false);
        enabled = false;
    }
}
