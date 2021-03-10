using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used for 
/// </summary>
public class Player_EquipmentUse_MovementTool : Player_EquipmentUse
{
    public enum MovementToolType { Climbing, Moving }
    public MovementToolType m_typeOfMovement;

    public override void ObjectBroke()
    {
        //base.ObjectBroke();
        PlayerEquipmentBreak.Instance.ShowUI();
    }
}
