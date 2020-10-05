using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Settings/Player Jumping Settings")]
public class PlayerJumpingSettings : ScriptableObject
{
	public PlayerController.JumpingProperties m_jumpingSettings;
}
