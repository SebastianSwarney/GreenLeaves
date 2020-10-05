using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Cinemachine;

public class PlayerInput : MonoBehaviour
{
    public int m_playerId;

    public float m_sensitivity;
    private float m_currentSensitivity;

    private PlayerController m_playerController;
    private Player m_playerInputController;

    private bool m_lockLooking;

    public static PlayerInput Instance;

    public CinemachineFreeLook freeLookCam;

    private void Awake()
    {
        m_currentSensitivity = m_sensitivity;
    }

    private void Start()
    {
        m_playerController = GetComponent<PlayerController>();
        m_playerInputController = ReInput.players.GetPlayer(m_playerId);

        ChangeCursorState(false);

        ReadSettings();
    }

    public void ChangeCursorState(bool p_activeState)
    {
        Cursor.visible = !false;

        if (!p_activeState)
        {

            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void Update()
    {
        GetInput();
    }

    private void ReadSettings()
    {
        //m_sensitivity = PlayerPrefs.GetFloat("sensitivity");
    }

    public void GetInput()
    {
        Vector2 movementInput = new Vector2(m_playerInputController.GetAxisRaw("MoveHorizontal"), m_playerInputController.GetAxisRaw("MoveVertical"));
        m_playerController.SetMovementInput(movementInput);

        if (Input.GetKeyDown(KeyCode.P))
        {
            m_lockLooking = !m_lockLooking;
        }

        if (!m_lockLooking)
        {
            Vector2 lookInput = new Vector2(m_playerInputController.GetAxis("LookHorizontal"), m_playerInputController.GetAxis("LookVertical"));

            //freeLookCam.m_XAxis.m_InputAxisValue = lookInput.x;
            //freeLookCam.m_YAxis.m_InputAxisValue = lookInput.y;
        }

        if (m_playerInputController.GetButtonDown("Jump"))
        {
            m_playerController.OnJumpInputDown();
        }
        if (m_playerInputController.GetButtonUp("Jump"))
        {
            m_playerController.OnJumpInputUp();
        }

		if (m_playerInputController.GetButtonDown("Run"))
		{
            m_playerController.OnRunButtonDown();
        }
        if (m_playerInputController.GetButtonUp("Run"))
        {
            m_playerController.OnRunButtonUp();
        }

        if (m_playerInputController.GetButtonDown("Walk"))
        {
            m_playerController.OnWalkButtonDown();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
		{
            ChangeCursorState(!Cursor.visible);
        }
    }

    public void ChangeSensitivity(float m_multiplier)
    {
        m_currentSensitivity = m_sensitivity * m_multiplier;
    }
}