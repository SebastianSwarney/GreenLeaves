using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VisionEvent : UnityEngine.Events.UnityEvent { }
public class AIVisionCone : MonoBehaviour
{
    public VisionEvent m_playerEnteredCone;

    public string m_playerTag;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == m_playerTag)
        {
            m_playerEnteredCone.Invoke();
        }
    }
}
