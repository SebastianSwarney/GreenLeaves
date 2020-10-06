using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputToggle : MonoBehaviour
{
    public static PlayerInputToggle Instance;
    public Cinemachine.CinemachineFreeLook m_cameraRotation;
    public PlayerInput m_playerInput;

    private void Awake()
    {
        Instance = this;
    }

    public void ToggleInput(bool p_newState)
    {
        m_playerInput.enabled = p_newState;
        m_cameraRotation.enabled = p_newState;
    }
}
