using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public GameObject m_pauseMenu, m_runtimeMenu, m_controlsMenu, m_craftingRecipeMenu;

    private bool m_currentState;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu(!m_currentState);
        }else if (Input.GetKey(KeyCode.P))
        {
            GlobalSceneManager.Instance.ReloadScene();
        }
    }

    public void TogglePauseMenu(bool p_newState)
    {
        m_currentState = p_newState;


        m_pauseMenu.SetActive(m_currentState);
        m_runtimeMenu.SetActive(!m_currentState);
        m_controlsMenu.SetActive(true);
        m_craftingRecipeMenu.SetActive(false);

        Player_Inventory.Instance.ToggleOpenability(!p_newState);
        Inventory_2DMenu.Instance.CloseInventoryMenu(true);
        
        if (m_currentState)
        {
            Interactable_Manager.Instance.ForceCloseMenu();
        }
        else
        {
            Interactable_Manager.Instance.SearchForInteractable();
        }

        PlayerInputToggle.Instance.ToggleInput(!m_currentState);
    }
}
