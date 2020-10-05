using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Settings/Player Base Movement Settings")]
public class PlayerBaseMovementSettings : ScriptableObject
{
	public PlayerController.BaseMovementProperties m_baseMovementSettings;
}
