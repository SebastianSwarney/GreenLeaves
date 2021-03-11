using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Cinemachine;

public class PlayerInput : MonoBehaviour
{
    public int m_playerId;

    private PlayerController m_playerController;
    private Player m_playerInputController;

    private void Start()
    {
        m_playerController = GetComponent<PlayerController>();
        m_playerInputController = ReInput.players.GetPlayer(m_playerId);
    }



    private void Update()
    {
        GetInput();
    }

    public void GetInput()
    {
        Vector2 movementInput = new Vector2(m_playerInputController.GetAxisRaw("MoveHorizontal"), m_playerInputController.GetAxisRaw("MoveVertical"));
        m_playerController.SetMovementInput(movementInput);

        m_playerController.SetFlyInput(m_playerInputController.GetAxisRaw("MoveFly"));


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
            m_playerController.OnSprintButtonDown();
        }
        if (m_playerInputController.GetButtonUp("Run"))
        {
            //m_playerController.OnSprintButtonUp();
        }

        if (m_playerInputController.GetButtonDown("Walk"))
        {
           m_playerController.OnWalkButtonDown();
        }
    }
}