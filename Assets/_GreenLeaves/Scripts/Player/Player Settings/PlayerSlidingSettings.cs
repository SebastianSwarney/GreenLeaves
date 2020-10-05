using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Settings/Player Sliding Settings")]
public class PlayerSlidingSettings : ScriptableObject
{
	public PlayerController.SlideProperties m_slidingSettings;
}
