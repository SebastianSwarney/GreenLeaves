using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputToggle : MonoBehaviour
{
    public static PlayerInputToggle Instance;
    public Transform m_playerCamera;
    public Transform m_splineTracker;
    public Cinemachine.CinemachineFreeLook m_cameraRotation;
    public PlayerInput m_playerInput;
    public PlayerController m_playerController;

    private void Awake()
    {
        Instance = this;
    }

    public void ToggleInput(bool p_newState)
    {
        m_playerInput.enabled = p_newState;
        m_cameraRotation.enabled = p_newState;

        if (!p_newState)
        {
            Debug.Log("Disable");
            m_playerController.SetFlyInput(0);
            m_playerController.SetMovementInput(Vector2.zero);
        }
    }

    public Transform GetSplinePos()
    {
        m_splineTracker.transform.position = transform.position + (m_playerCamera.position - transform.position) / 2;
        return m_splineTracker;
    }
}
