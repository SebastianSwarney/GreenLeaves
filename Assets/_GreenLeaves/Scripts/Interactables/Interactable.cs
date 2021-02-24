using UnityEngine;

/// <summary>
/// The base class for anything that can be interacted with.<br/>
/// The "Interactable_Manager" will be called to display messages <br/>
/// based on the triggered interactable, and when it's buttons are<br/> 
/// pressed, this script will be called to perform it's functions.<br/>
/// </summary>
public class Interactable : MonoBehaviour
{

    public string m_interactableName;
    [System.Serializable]
    public class ButtonMessages
    {
        public bool m_enableButton;
        public string m_buttonMessage;
    }

    public ButtonMenu m_buttonMenu;
    [System.Serializable]
    public class ButtonMenu
    {
        public ButtonMessages m_topButton;
        public ButtonMessages m_rightButton;
        public ButtonMessages m_bottomButton;
        public ButtonMessages m_leftButton;
    }

    public string m_playerTag;
    public bool m_canBeOverridden;
    public bool m_canBeInteractedWith = true;

    public GenericWorldEvent m_itemSelectedEvent, m_itemDeselectedEvent;
    

    /// <summary>
    /// Calls the interactable manager to display this current item<br/>
    /// and it's appropriate button messages
    /// </summary>
    public void DisplayMessage()
    {
        Interactable_Manager.Instance.DisplayButtonMenu(this, m_canBeOverridden);
    }

    /// <summary>
    /// The functions called when their respective button is pressed in the interactable manager.<br/>
    /// These functions are virtual, and their functionality differs in each inheritor.
    /// </summary>
    #region Button Presses
    public virtual void TopButtonPressed()
    {
    }

    public virtual void RightButtonPressed()
    {
    }

    public virtual void BottomButtonPressed()
    {
    }

    public virtual void LeftButtonPressed()
    {
    }
    #endregion


    ///<summary>
    ///Checks on whether th button should be enabled.<br/>
    ///This is it's own function as the button may be disabled/enabled on different branches in the interhitor scripts
    ///</summary>
    #region Check enabled
    public virtual bool TopButtonEnabled()
    {
        return m_buttonMenu.m_topButton.m_enableButton;
    }
    public virtual bool RightButtonEnabled()
    {
        return m_buttonMenu.m_rightButton.m_enableButton;
    }
    public virtual bool BottomButtonEnabled()
    {
        return m_buttonMenu.m_bottomButton.m_enableButton;
    }
    public virtual bool LeftButtonEnabled()
    {
        return m_buttonMenu.m_leftButton.m_enableButton;
    }
    #endregion

    ///<summary>
    ///Checks on what the button's messabe should be.<br/>
    ///This is it's own function as the button may be different on different branches in the interhitor scripts
    ///</summary>
    #region Check Message
    public virtual string TopButtonString()
    {
        return m_buttonMenu.m_topButton.m_buttonMessage;
    }
    public virtual string RightButtonString()
    {
        return m_buttonMenu.m_rightButton.m_buttonMessage;
    }
    public virtual string BottomButtonString()
    {
        return m_buttonMenu.m_bottomButton.m_buttonMessage;
    }
    public virtual string LeftButtonString()
    {
        return m_buttonMenu.m_leftButton.m_buttonMessage;
    }

    #endregion


    /// <summary>
    /// Basically calls the interactable manager when the trigger is setoff by the player
    /// </summary>
    #region Trigger detection

    public bool CanInteract()
    {
        return m_canBeInteractedWith;
    }

    public void DisplayInteractableMessage()
    {
        DisplayMessage();
    }

    public void RemoveInteractableMessage()
    {
        Interactable_Manager.Instance.HideButtonMenu(this, true);
    }

    #endregion

    public virtual string GetInteractableName()
    {
        return m_interactableName;
    }
    public void ItemSelected()
    {
        m_itemSelectedEvent.Invoke();
    }
    public void ItemDeselect()
    {
        m_itemDeselectedEvent.Invoke();
    }

    private void OnEnable()
    {
        ResetInteractivity();
    }
    public void ResetInteractivity()
    {
        m_canBeInteractedWith = true;
    }
}
