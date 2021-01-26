using UnityEngine;

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
    public CapsuleCollider m_capCol;


    public ButtonMenu m_topMenu, m_rightMenu, m_bottomMenu, m_leftMenu;
    public bool m_topButtonEnabled, m_rightButtonEnabled, m_bottomMenuEnabled, m_leftMenuEnabled;


    public UnityEngine.UI.Text m_interactableName;
    /// <summary>
    /// The actual button's ui in the menu
    /// </summary>
    [System.Serializable]
    public class ButtonMenu
    {
        public GameObject m_buttonImage;
        public UnityEngine.UI.Text m_buttonText;
        public void SetupButton(bool p_enabled, string p_text)
        {
            m_buttonImage.SetActive(p_enabled);
            m_buttonText.gameObject.SetActive(p_enabled);
            m_buttonText.text = p_text;
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
    }


    /// <summary>
    /// Displays the menu, with the current interactable as a parameter<br/>
    /// If the menu is currently open, and can be overridden, the menu will override with the new item<br/>
    /// 
    /// Enables, Disables, and sets the button's texts as directed by the interactable's scripts
    /// </summary>
    public void DisplayButtonMenu(Interactable p_selectedSystem, bool p_canBeOverridden)
    {
        if (!m_canOpen) return;
        if (!m_canBeOverridden)
        {
            if (m_currentInteractable != null && m_currentInteractable != p_selectedSystem)
            {
                return;
            }
        }
        m_canBeOverridden = p_canBeOverridden;

        
        if(m_currentInteractable != null && m_currentInteractable != p_selectedSystem)
        {
            m_currentInteractable.ItemDeselect();
        }

        m_currentInteractable = p_selectedSystem;

        m_currentInteractable.ItemSelected();

        m_menuOpen = true;
        if (m_buttonUiParent == null) return;
        m_buttonUiParent.SetActive(true);

        m_interactableName.transform.parent.gameObject.SetActive(true);
        m_interactableName.text = p_selectedSystem.m_interactableName;

        m_topButtonEnabled = p_selectedSystem.TopButtonEnabled();
        m_topMenu.SetupButton(m_topButtonEnabled, p_selectedSystem.TopButtonString());

        m_rightButtonEnabled = p_selectedSystem.RightButtonEnabled();
        m_rightMenu.SetupButton(m_rightButtonEnabled, p_selectedSystem.RightButtonString());

        m_bottomMenuEnabled = p_selectedSystem.BottomButtonEnabled();
        m_bottomMenu.SetupButton(m_bottomMenuEnabled, p_selectedSystem.BottomButtonString());

        m_leftMenuEnabled = p_selectedSystem.LeftButtonEnabled();
        m_leftMenu.SetupButton(m_leftMenuEnabled, p_selectedSystem.LeftButtonString());
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
            if(m_currentInteractable != null)
            {
                m_currentInteractable.ItemDeselect();
            }
            m_canBeOverridden = false;
            m_menuOpen = false;
            m_buttonUiParent.SetActive(false);


            if (p_searchForNextInteractable && !Inventory_2DMenu.Instance.m_isOpen)
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
        Collider[] cols = Physics.OverlapCapsule(transform.position + (m_capCol.height / 2 * Vector3.up), transform.position - (m_capCol.height / 2 * Vector3.up), m_capCol.radius - .05f, m_interactableMask); //Physics.OverlapSphere(transform.position, m_searchRadius, m_interactableMask);
        if (cols.Length > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (m_currentInteractable != cols[i].GetComponent<Interactable>())
                {
                    if(cols[i].GetComponent<Interactable>() == null)
                    {
                        Debug.LogError(cols[i].gameObject + " does not have interactable", cols[i].gameObject);
                        continue;
                    }
                    if (cols[i].GetComponent<Interactable>().m_canBeInteractedWith)
                    {
                        m_currentInteractable = null;
                        cols[i].GetComponent<Interactable>().DisplayMessage();
                        return;
                    }
                }
            }

        }

        m_currentInteractable = null;

    }

    /// <summary>
    /// Only ran if the menu is open. Used to detect input.
    /// </summary>
    private void Update()
    {
        if (!m_menuOpen || m_currentInteractable == null) return;

        DetectInput();

    }

    private void OnDrawGizmos()
    {
        if (!m_isDebugging) return;
        Gizmos.color = m_debugColor;
        Gizmos.DrawSphere(transform.position + (m_capCol.height / 2 * Vector3.up), m_capCol.radius);
        Gizmos.DrawSphere(transform.position - (m_capCol.height / 2 * Vector3.up), m_capCol.radius);
    }
}
