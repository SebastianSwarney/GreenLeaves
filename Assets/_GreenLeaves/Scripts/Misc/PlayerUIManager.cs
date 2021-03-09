using System.Collections;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public GameObject m_pauseMenu, m_mainPause, m_craftingRecipeMenu, m_audioMenu, m_controlsMenu, m_cameraMenu;

    public static PlayerUIManager Instance;
    public bool m_isPaused;

    [Header("Screen Fade Variables")]
    public CanvasGroup m_screenFadeGroup;
    public GenericWorldEvent m_mainMenuButtonPressed;
    public float m_fadeTime = 0.5f;
    private bool m_transitionToMainMenu;

    public bool m_fadeInOnEnable = false;

    FMOD.Studio.Bus m_ambience;
    FMOD.Studio.Bus m_soundEffects;

    private void Awake()
    {
        Instance = this;
        if (m_fadeInOnEnable)
        {
            StartCoroutine(FadeScreen(false));
        }
        m_ambience = FMODUnity.RuntimeManager.GetBus("bus:/Master/Ambience");
        m_soundEffects = FMODUnity.RuntimeManager.GetBus("bus:/Master/SoundEffects");
    }

    private void Update()
    {
        if (Daytime_WaitMenu.Instance.m_isWaiting) return;
        if (m_transitionToMainMenu) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Interactable_Readable_Menu.Instance.m_isOpen) Interactable_Readable_Menu.Instance.CloseReadableMenu();

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

        PlayerUIManager.Instance.ToggleCameraMode(false);


        m_pauseMenu.SetActive(m_isPaused);
        m_mainPause.SetActive(true);
        m_craftingRecipeMenu.SetActive(false);
        m_audioMenu.SetActive(false);
        m_controlsMenu.SetActive(false);

        Player_Inventory.Instance.ToggleOpenability(!p_newState);
        //Inventory_2DMenu.Instance.CloseInventoryMenu(true);

        if (m_isPaused)
        {
            Interactable_Manager.Instance.ForceCloseMenu();
        }
        else if (!Inventory_2DMenu.Instance.m_isOpen)
        {
            Interactable_Manager.Instance.SearchForInteractable();

        }
        if (!Inventory_2DMenu.Instance.m_isOpen)
        {
            PlayerInputToggle.Instance.ToggleInput(!m_isPaused);
        }

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

        m_ambience.setVolume((p_newFadeState) ? 1 : 0);
        m_soundEffects.setVolume((p_newFadeState) ? 1 : 0);

        float timer = 0;
        while (timer < m_fadeTime)
        {
            yield return null;
            timer += Time.deltaTime;
            m_screenFadeGroup.alpha = (p_newFadeState ? (timer / m_fadeTime) : 1 - (timer / m_fadeTime));

        }

        m_ambience.setVolume((p_newFadeState) ? 0 : 1);
        m_soundEffects.setVolume((p_newFadeState) ? 0 : 1);
        m_screenFadeGroup.alpha = (p_newFadeState ? 1 : 0);

    }


    public void ToggleCameraMode(bool p_toggle)
    {
        m_cameraMenu.SetActive(p_toggle);

        if (p_toggle)
        {
            Inventory_2DMenu.Instance.CloseInventoryMenu(true);
            ScreenshotManager.Instance.EnableCamera();
        }
        else
        {
            ScreenshotManager.Instance.DisableCamera();
        }
    }
}
