using System.Collections;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public GameObject m_pauseMenu, m_mainPause, m_audioMenu, m_controlsMenu, m_cameraMenu;

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


    [Header("Compass")]
    public GameObject m_compassRoot;
    public Transform m_compassTracker;
    public float m_compassAngleOffset;
    public float m_compassLerp;

    [Header("Map")]
    public GameObject m_map;
    public bool m_mapUnlocked;
    private float m_prevLerp, m_currentLerp;
    public GenericWorldEvent m_mapEvent;

    private float m_timer;
    
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

    private void LateUpdate()
    {
        UpdateCompass();
    }
    private void Update()
    {
        if (Credits.Instance.m_isPlaying) return;
        if (Daytime_WaitMenu.Instance.m_isWaiting) return;
        if (m_transitionToMainMenu) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Inventory_2DMenu.Instance.m_isOpen)
            {
                Inventory_2DMenu.Instance.ToggleInventory(false);
                Interactable_Manager.Instance.SearchForInteractable();
                return;
            }
            if (m_map.activeSelf)
            {
                m_map.SetActive(false);
                PlayerInputToggle.Instance.ToggleInput(true);
            }
            if (Interactable_Readable_Menu.Instance.m_isOpen) Interactable_Readable_Menu.Instance.CloseReadableMenu();

            TogglePauseMenu(!m_isPaused);
        }
        else if (Input.GetKeyDown(KeyCode.M) && m_mapUnlocked)
        {
            if (m_isPaused) return;
            if (Inventory_2DMenu.Instance.m_isOpen || Interactable_Readable_Menu.Instance.m_isOpen || Credits.Instance.m_isPlaying) return;
            m_mapEvent.Invoke();
            m_map.gameObject.SetActive(!m_map.activeSelf);
            if (m_map.gameObject.activeSelf)
            {
                Interactable_Manager.Instance.ForceCloseMenu();
                PlayerInputToggle.Instance.ToggleInput(false);

            }
            else
            {
                Interactable_Manager.Instance.SearchForInteractable();
                PlayerInputToggle.Instance.ToggleInput(true);
            }
        }


        if (Inventory_2DMenu.Instance.m_isOpen || m_isPaused || Interactable_Readable_Menu.Instance.m_isOpen || m_map.activeSelf) return;
        if (!PlayerInputToggle.Instance.m_playerInput.enabled) return;

        if (m_timer < .05f)
        {
            m_timer += Time.deltaTime;
            return;
        }
        
        if (Input.GetMouseButtonDown(2) || Input.mouseScrollDelta.magnitude > .3f ||Input.GetKeyDown(KeyCode.R))
        {
            m_timer = 0;
            Inventory_2DMenu.Instance.QuickSwapEquipment();
        }
    }

    public void UpdateCompass()
    {
        m_currentLerp = -(PlayerInputToggle.Instance.m_physicalCamera.transform.eulerAngles.y + m_compassAngleOffset);
        m_compassTracker.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(m_prevLerp,m_currentLerp, m_compassLerp));
        m_prevLerp = -(PlayerInputToggle.Instance.m_physicalCamera.transform.eulerAngles.y + m_compassAngleOffset);
    }

    public void ToggleCompass(bool p_newState)
    {
        m_compassRoot.SetActive(p_newState);
    }

    public void TogglePauseMenu(bool p_newState)
    {
        m_isPaused = p_newState;

        PlayerUIManager.Instance.ToggleCameraMode(false);


        m_pauseMenu.SetActive(m_isPaused);
        m_mainPause.SetActive(true);
        m_audioMenu.SetActive(false);
        m_controlsMenu.SetActive(false);

        Player_Inventory.Instance.ToggleOpenability(!p_newState);
        //Inventory_2DMenu.Instance.CloseInventoryMenu(true);

        if (m_isPaused)
        {
            Interactable_Manager.Instance.ForceCloseMenu();
            PlayerInputToggle.Instance.ToggleInputFromGameplay(true);
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

    public void ExitGame() { Application.Quit(); }


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
