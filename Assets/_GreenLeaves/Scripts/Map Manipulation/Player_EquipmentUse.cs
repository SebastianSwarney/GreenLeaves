using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class used to interact with world items. This class will be called during animations, with the equipped Item
/// </summary>
public class Player_EquipmentUse : MonoBehaviour
{
    public virtual void UseEquipment()
    {
        Debug.Log("Place equipment usage code here");
    }
}
