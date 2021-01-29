using System.Collections;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public GameObject m_pauseMenu, m_runtimeMenu, m_controlsMenu, m_craftingRecipeMenu;

    public static PlayerUIManager Instance;
    public bool m_isPaused;

    [Header("Screen Fade Variables")]
    public CanvasGroup m_screenFadeGroup;
    public GenericWorldEvent m_mainMenuButtonPressed;
    public float m_fadeTime = 0.5f;
    private bool m_transitionToMainMenu;

    public bool m_fadeInOnEnable = false;
    private void Awake()
    {
        Instance = this;
        if (m_fadeInOnEnable)
        {
            StartCoroutine(FadeScreen(false));
        }
    }

    private void Update()
    {
        if (Daytime_WaitMenu.Instance.m_isWaiting) return;
        if (m_transitionToMainMenu) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu(!m_isPaused);
        }
        else if (Input.GetKey(KeyCode.P))
        {
            GlobalSceneManager.Instance.ReloadScene();
        }
    }

    public void TogglePauseMenu(bool p_newState)
    {
        m_isPaused = p_newState;


        m_pauseMenu.SetActive(m_isPaused);
        m_runtimeMenu.SetActive(!m_isPaused);
        m_controlsMenu.SetActive(true);
        m_craftingRecipeMenu.SetActive(false);

        Player_Inventory.Instance.ToggleOpenability(!p_newState);
        Inventory_2DMenu.Instance.CloseInventoryMenu(true);

        if (m_isPaused)
        {
            Interactable_Manager.Instance.ForceCloseMenu();
        }
        else
        {
            Interactable_Manager.Instance.SearchForInteractable();
        }

        PlayerInputToggle.Instance.ToggleInput(!m_isPaused);
    }

    public void GoToMainMenu()
    {
        m_mainMenuButtonPressed.Invoke();
        m_transitionToMainMenu = true;
        m_pauseMenu.SetActive(false);
        StartCoroutine(FadeToMainMenu());
    }


    private IEnumerator FadeToMainMenu()
    {
        yield return StartCoroutine(FadeScreen(true));
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    private IEnumerator FadeScreen(bool p_newFadeState)
    {
        m_screenFadeGroup.alpha = (p_newFadeState ? 0 : 1);

        float timer = 0;
        while (timer < m_fadeTime)
        {
            yield return null;
            timer += Time.deltaTime;
            m_screenFadeGroup.alpha = (p_newFadeState ? (timer / m_fadeTime) : 1 - (timer / m_fadeTime));
        }
        m_screenFadeGroup.alpha = (p_newFadeState ? 1 : 0);

    }
}
