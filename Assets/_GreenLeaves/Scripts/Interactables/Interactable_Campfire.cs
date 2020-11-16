using UnityEngine;

/// <summary>
/// The interactable script on the campfire.<br/>
/// The logic for the interactable menu's states for the campfire are handled here.
/// </summary>
public class Interactable_Campfire : Interactable
{
    /// <summary>
    /// Used to determine how deep within the menu system they are
    /// </summary>
    public enum InteractionState { Interact, PickInteraction, SubInteraction }
    public InteractionState m_currentState;

    /// <summary>
    /// Used to determine which option they decided to use.
    /// </summary>
    public enum InteractionType { Initial, Cook, Rest }
    public InteractionType m_currentType;


    /// <summary>
    /// The rest path
    /// </summary>
    #region Top Button Functions

    public override bool TopButtonEnabled()
    {
        switch (m_currentState)
        {
            case InteractionState.Interact:
                return false;

            case InteractionState.PickInteraction:
                return true;

            case InteractionState.SubInteraction:
                return false;
        }
        return false;
    }

    public override void TopButtonPressed()
    {
        switch (m_currentState)
        {
            case InteractionState.Interact:
                DaytimeCycle_Update.Instance.ToggleDaytimePause(true);

                m_currentState = InteractionState.PickInteraction;
                PlayerInputToggle.Instance.ToggleInput(false);
                Interactable_Manager.Instance.DisplayButtonMenu(this, false);
                break;

            case InteractionState.PickInteraction:
                m_currentType = InteractionType.Rest;
                m_currentState = InteractionState.SubInteraction;


                m_currentState = InteractionState.Interact;
                m_currentType = InteractionType.Initial;
                Debug.LogError("Show rest menu Here");
                Daytime_WaitMenu.Instance.OpenMenu();
                Interactable_Manager.Instance.HideButtonMenu(this, false);
                break;

            case InteractionState.SubInteraction:
                break;
        }
    }

    public override string TopButtonString()
    {
        switch (m_currentState)
        {
            case InteractionState.Interact:
                return "null";

            case InteractionState.PickInteraction:
                return "Rest";

            case InteractionState.SubInteraction:
                return "null";
        }
        return "null";
    }

    #endregion

    /// <summary>
    /// The leave option
    /// </summary>
    #region Right Button Functions

    public override bool RightButtonEnabled()
    {
        switch (m_currentState)
        {
            case InteractionState.Interact:
                return false;

            case InteractionState.PickInteraction:
                return true;

            case InteractionState.SubInteraction:
                switch (m_currentType)
                {
                    case InteractionType.Cook:
                        return false;
                    case InteractionType.Rest:
                        return true;
                }
                return false;
        }
        return false;
    }

    public override void RightButtonPressed()
    {
        DaytimeCycle_Update.Instance.ToggleDaytimePause(false);

        m_currentState = InteractionState.Interact;
        m_currentType = InteractionType.Initial;
        Interactable_Manager.Instance.HideButtonMenu(this, true);
        PlayerInputToggle.Instance.ToggleInput(true);
    }

    public override string RightButtonString()
    {
        switch (m_currentState)
        {
            case InteractionState.Interact:
                return "null";

            case InteractionState.PickInteraction:
                return "Leave";

            case InteractionState.SubInteraction:
                return "Leave";
        }
        return "null";
    }

    #endregion


    /// <summary>
    /// Unused
    /// </summary>
    #region Bottom Button Functions

    public override bool BottomButtonEnabled()
    {
        return false;
    }

    public override string BottomButtonString()
    {
        return "null";
    }

    #endregion


    /// <summary>
    /// The cooking path
    /// </summary>
    #region Left Button Functions

    public override bool LeftButtonEnabled()
    {
        switch (m_currentState)
        {
            case InteractionState.Interact:
                return true;

            case InteractionState.PickInteraction:
                return true;

            case InteractionState.SubInteraction:
                return false;
        }
        return false;
    }

    public override void LeftButtonPressed()
    {
        switch (m_currentState)
        {
            case InteractionState.Interact:
                m_currentState = InteractionState.PickInteraction;

                DaytimeCycle_Update.Instance.ToggleDaytimePause(true);

                PlayerInputToggle.Instance.ToggleInput(false);
                Interactable_Manager.Instance.DisplayButtonMenu(this, false);
                break;

            case InteractionState.PickInteraction:
                DaytimeCycle_Update.Instance.ToggleDaytimePause(true);

                m_currentType = InteractionType.Cook;
                m_currentState = InteractionState.SubInteraction;

                m_currentState = InteractionState.Interact;
                m_currentType = InteractionType.Initial;
                Inventory_2DMenu.Instance.ToggleInventory(false);
                Interactable_Manager.Instance.HideButtonMenu(this, false);
                break;

            case InteractionState.SubInteraction:
                break;
        }
    }

    public override string LeftButtonString()
    {
        switch (m_currentState)
        {
            case InteractionState.Interact:
                return "Interact";

            case InteractionState.PickInteraction:
                return "Cook";

            case InteractionState.SubInteraction:
                return "null";
        }
        return "null";
    }


    #endregion
}
