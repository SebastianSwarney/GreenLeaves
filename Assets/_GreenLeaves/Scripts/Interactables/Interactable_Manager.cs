using UnityEngine;
using System.Collections;

/// <summary>
/// The manager for the interactable button menu. <br/>
/// Can be called to show or hide the menu.
/// </summary>
public class Interactable_Manager : MonoBehaviour
{
    public static Interactable_Manager Instance;

    public GameObject m_buttonUiParent;

    public Interactable m_currentInteractable;

    /// <summary>
    /// The variables for searching for an interactable, if exiting one
    /// </summary>
    [Header("Raycasting")]
    public LayerMask m_interactableMask;
    public LayerMask m_groundMask;

    public Transform m_camera;
    public float m_highCamRaycastDis, m_lowCamRaycastDis;
    public Cinemachine.CinemachineFreeLook m_cinemachineFreeLook;

    public ButtonMenu m_topMenu, m_rightMenu, m_bottomMenu, m_leftMenu;
    public bool m_topButtonEnabled, m_rightButtonEnabled, m_bottomMenuEnabled, m_leftMenuEnabled;


    public UnityEngine.UI.Text m_interactableName;
    /// <summary>
    /// The actual button's ui in the menu
    /// </summary>
    [System.Serializable]
    public class ButtonMenu
    {
        public GameObject m_buttonParent;
        public GameObject m_buttonEnabledImage;
        public UnityEngine.UI.Text m_interactionText;
        public void SetupButton(bool p_enabled, string p_text, bool p_toggleState)
        {
            m_buttonParent.SetActive(p_toggleState);
            m_buttonEnabledImage.SetActive(p_enabled);
            m_interactionText.text = p_text;
        }
    }

    /// <summary>
    /// The keycodes represented in the buttons
    /// </summary>
    public Keycodes m_keyInputs;

    [System.Serializable]
    public class Keycodes
    {
        public KeyCode m_topKey;
        public KeyCode m_rightKey;
        public KeyCode m_bottomKey;
        public KeyCode m_leftKey;
    }



    private bool m_menuOpen;
    private bool m_canBeOverridden;
    private bool m_menuWasOpen;
    private bool m_canOpen = true;

    [Header("Debugging")]
    public bool m_isDebugging;
    public Color m_debugColor;

    private void Awake()
    {
        Instance = this;
        if (m_camera == null)
        {
            m_camera = Camera.main.transform;
        }

    }
    private void Start()
    {
        if (m_cinemachineFreeLook == null)
        {
            m_cinemachineFreeLook = PlayerInputToggle.Instance.transform.GetComponentInChildren<Cinemachine.CinemachineFreeLook>();
        }
    }


    private void LateUpdate()
    {
        if (Credits.Instance.m_isPlaying) return;
        if (!Inventory_2DMenu.Instance.m_isOpen && !Interactable_Readable_Menu.Instance.m_isOpen && !PlayerUIManager.Instance.m_map.activeSelf)
        {
            SearchForInteractable();
        }
        if(m_currentInteractable != null)
        {
            transform.position = m_currentInteractable.transform.position;
            if(!m_currentInteractable.gameObject.activeSelf || !m_currentInteractable.gameObject.activeInHierarchy)
            {
                HideButtonMenu(m_currentInteractable, true);
            }
        }else if (m_currentInteractable == null && m_menuOpen)
        {
            HideButtonMenu(null, true);
        }

        
    }

    /// <summary>
    /// Displays the menu, with the current interactable as a parameter<br/>
    /// If the menu is currently open, and can be overridden, the menu will override with the new item<br/>
    /// 
    /// Enables, Disables, and sets the button's texts as directed by the interactable's scripts
    /// </summary>
    public void DisplayButtonMenu(Interactable p_selectedSystem, bool p_canBeOverridden)
    {
        if (Building_PlayerPlacement.Instance.m_isPlacing) return;
        if (!m_canOpen) return;
        if (!m_canBeOverridden)
        {
            if (m_currentInteractable != null && m_currentInteractable != p_selectedSystem)
            {
                return;
            }
        }
        m_canBeOverridden = p_canBeOverridden;

        transform.position = p_selectedSystem.transform.position;

        if (m_currentInteractable != null && m_currentInteractable != p_selectedSystem)
        {
            m_currentInteractable.ItemDeselect();
        }

        m_currentInteractable = p_selectedSystem;

        m_currentInteractable.ItemSelected();

        m_menuOpen = true;


        m_interactableName.transform.parent.gameObject.SetActive(true);
        m_interactableName.text = p_selectedSystem.GetInteractableName();

        m_topButtonEnabled = p_selectedSystem.TopButtonEnabled();
        m_topMenu.SetupButton(m_topButtonEnabled, p_selectedSystem.TopButtonString(), true);

        m_rightButtonEnabled = p_selectedSystem.RightButtonEnabled();
        m_rightMenu.SetupButton(m_rightButtonEnabled, p_selectedSystem.RightButtonString(), true);

        m_bottomMenuEnabled = p_selectedSystem.BottomButtonEnabled();
        m_bottomMenu.SetupButton(m_bottomMenuEnabled, p_selectedSystem.BottomButtonString(), true);

        m_leftMenuEnabled = p_selectedSystem.LeftButtonEnabled();
        m_leftMenu.SetupButton(m_leftMenuEnabled, p_selectedSystem.LeftButtonString(), true);
        StartCoroutine(DelayAppearance());
    }
    private IEnumerator DelayAppearance()
    {
        yield return null;
        if (m_buttonUiParent == null) yield break;
        m_buttonUiParent.SetActive(true);
    }
    

    /// <summary>
    /// Disables the menu, and hides the buttons.<br/>
    /// If an interactable, that is not the current selected, tries to close the menu, it fails<br/>
    /// 
    /// The bool parameter is used to determine whether the system should search for another interactable,<br/>
    /// on closing of the menu.
    /// </summar>
    public void HideButtonMenu(Interactable p_selectedSystem, bool p_searchForNextInteractable)
    {

        if (p_selectedSystem == m_currentInteractable || m_currentInteractable == null)
        {
            if (m_currentInteractable != null)
            {
                m_currentInteractable.ItemDeselect();
            }
            m_canBeOverridden = false;
            m_menuOpen = false;
            m_buttonUiParent.SetActive(false);


            if (p_searchForNextInteractable && !Inventory_2DMenu.Instance.m_isOpen && !Interactable_Readable_Menu.Instance.m_isOpen)
            {
                SearchForInteractable();
            }
        }
    }




    /// <summary>
    /// Used to for the closing of the menu. <br/>
    /// This is called in the inventory_2dMenu script.<br/>
    /// If the menu is open, and is forced to close, will track the opened state in a bool.<br/>
    /// </summary>
    public void ForceCloseMenu()
    {
        m_canOpen = false;
        m_menuWasOpen = false;
        if (!m_menuOpen) return;

        m_menuWasOpen = true;
        m_canBeOverridden = false;
        m_menuOpen = false;
        m_buttonUiParent.SetActive(false);

        if (m_currentInteractable != null)
        {
            m_currentInteractable.ItemDeselect();
        }
        m_currentInteractable = null;

    }

    /// <summary>
    /// Used to reopen the menu, if it was forced to close.<br/>
    /// The item that it reopens as focus on is the previous interactable it was highlighting.
    /// </summary>
    public void CheckReopen()
    {

        m_canOpen = true;
        if (m_menuWasOpen && m_currentInteractable != null)
        {
            DisplayButtonMenu(m_currentInteractable, m_currentInteractable.m_canBeOverridden);
            m_currentInteractable.ItemSelected();
        }
    }

    /// <summary>
    /// Used to get the input while the interactable menu is open.<br/>
    /// Only detects the input if the button in the menu is enabled.
    /// </summary>
    public void DetectInput()
    {
        if (m_topButtonEnabled && Input.GetKeyDown(m_keyInputs.m_topKey))
        {
            m_currentInteractable.TopButtonPressed();
        }
        else if (m_rightButtonEnabled && Input.GetKeyDown(m_keyInputs.m_rightKey))
        {
            m_currentInteractable.RightButtonPressed();
        }
        else if (m_bottomMenuEnabled && Input.GetKeyDown(m_keyInputs.m_bottomKey))
        {
            m_currentInteractable.BottomButtonPressed();
        }
        else if (m_leftMenuEnabled && Input.GetKeyDown(m_keyInputs.m_leftKey))
        {
            m_currentInteractable.LeftButtonPressed();
        }
    }

    public void ClearInteractable()
    {
        m_canBeOverridden = true;
        m_currentInteractable = null;
    }

    /// <summary>
    /// Searches for an interactable within range.<br/>
    /// This function was to check if the player was currently in range of an item, but did not pick it, or previously lost it.<br/>
    /// This is to prevent the interactables from checking their trigger on every frame.<br/>
    /// 
    /// This is called when the interactable menu is hidden, and the "Search" boolean is true;
    /// </summary>
    public void SearchForInteractable()
    {
        if (!enabled) return;
        m_canOpen = true;

        Interactable newInteractable = null;
        if (FoundObject(out newInteractable))
        {
            if (m_currentInteractable != null)
            {
                if (m_currentInteractable != newInteractable)
                {
                    m_currentInteractable.ItemDeselect();
                    m_canBeOverridden = false;
                    m_menuOpen = false;
                    m_buttonUiParent.SetActive(false);
                }
                else
                {
                    return;
                }
            }
            m_currentInteractable = newInteractable;
            m_currentInteractable.DisplayMessage();
        }
        else
        {
            if (m_currentInteractable != null)
            {
                m_currentInteractable.ItemDeselect();
                m_canBeOverridden = false;
                m_menuOpen = false;
                m_buttonUiParent.SetActive(false);
            }
            m_currentInteractable = null;

        }

    }

    public void SetInteractable(Interactable p_selected)
    {
        if(m_currentInteractable != null)
        {
            m_currentInteractable.ItemDeselect();
        }
        m_currentInteractable = p_selected;
        m_currentInteractable.DisplayInteractableMessage();
    }

    public bool FoundObject(out Interactable p_closestInteractable)
    {

        p_closestInteractable = null;
        Vector3 groundPoint = new Vector3(Screen.width / 2, Screen.height / 2);
        float dis = Mathf.Lerp(m_lowCamRaycastDis, m_highCamRaycastDis, m_cinemachineFreeLook.m_YAxis.Value);
        if (m_camera == null) return false;
        RaycastHit[] allHit = Physics.RaycastAll(m_camera.position, m_camera.forward, dis, m_interactableMask);

        float currentDis = 1000;
        float measuredDis = 0;

        Interactable fetchedInteractable;
        foreach (RaycastHit hit in allHit)
        {
            Debug.DrawLine(m_camera.position, hit.point, Color.red);
            measuredDis = Vector3.Distance(groundPoint, Camera.main.WorldToScreenPoint(hit.transform.position));
            if (measuredDis < currentDis)
            {
                fetchedInteractable = hit.transform.GetComponent<Interactable>();
                if (fetchedInteractable.CanInteract())
                {
                    p_closestInteractable = fetchedInteractable;
                    currentDis = measuredDis;
                }
            }
        }

        if (p_closestInteractable != null)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Only ran if the menu is open. Used to detect input.
    /// </summary>
    private void Update()
    {
        if (!m_menuOpen || m_currentInteractable == null) return;

        DetectInput();

    }


    /*private void OnDrawGizmos()
    {
        if (!m_isDebugging) return;
        Gizmos.color = m_debugColor;

        if (m_cinemachineFreeLook == null || m_camera == null) return;
        float dis = Mathf.Lerp(m_lowCamRaycastDis, m_highCamRaycastDis, m_cinemachineFreeLook.m_YAxis.Value);
        Gizmos.DrawLine(m_camera.position, m_camera.position + m_camera.forward * dis);

    }*/
}
