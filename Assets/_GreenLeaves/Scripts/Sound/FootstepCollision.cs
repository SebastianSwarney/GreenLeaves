using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepCollision : MonoBehaviour
{
    public GenericWorldEvent m_footstepCollisionEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            m_footstepCollisionEvent.Invoke();
        }
    }
}
