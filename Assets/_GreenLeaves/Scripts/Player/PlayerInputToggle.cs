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

    private bool m_frozenFromGameplay = false;
    private bool m_frozenFromPause = false;

    private void Awake()
    {
        Instance = this;

    }
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleInput(bool p_newState)
    {
        m_frozenFromPause = !p_newState;
        Cursor.visible = !p_newState;
        Cursor.lockState = (p_newState) ? CursorLockMode.Confined : CursorLockMode.None;
        if (m_frozenFromGameplay) return;

        m_playerInput.enabled = p_newState;
        m_cameraRotation.enabled = p_newState;

        if (!p_newState)
        {
            m_playerController.SetMovementInput(Vector2.zero);
        }
    }

    public void ToggleInputFromGameplay(bool p_newState, bool p_toggleCamera = true)
    {
        m_frozenFromGameplay = !p_newState;
        if (m_frozenFromPause) return;
        m_playerInput.enabled = p_newState;
        m_cameraRotation.enabled = p_toggleCamera ? p_newState : true;

        if (!p_newState)
        {
            m_playerController.SetMovementInput(Vector2.zero);
        }
    }

    public Transform GetSplinePos()
    {
        m_splineTracker.transform.position = transform.position + (m_playerCamera.position - transform.position) / 2;
        return m_splineTracker;
    }
}
