﻿using UnityEngine;

/// <summary>
/// <para>Used on the objects that can be hit by a tool.<br/>
/// After a certain amount of hits, the object will invoke the object died event, and disable the m_canHit boolean. </para>
/// Note: This script does not disable the object, nor does it spawn the object's resources
/// </summary>
public class Manipulation_HitObject : MonoBehaviour
{

    public int m_hitAmount;
    private int m_currentHit;
    public bool m_canHit= true;

    public GenericWorldEvent m_objectHit, m_objectDied;

    /// <summary>
    /// Called to perform the hit on the object. <br/>
    /// The Object Hit event is invoked when the object is hit, but not on the hit that destroys it<br/>
    /// The Object Died event is invoked when the object has been destroyed
    /// </summary>
    public void HitObject()
    {
        if (!m_canHit) return;
        m_currentHit++;
        if(m_currentHit >= m_hitAmount)
        {
            m_objectDied.Invoke();
            gameObject.SetActive(false);
        }
        else
        {
            m_objectHit.Invoke();
        }
    }

    public void ObjectRespawn()
    {
        m_canHit = true;
        m_currentHit = m_hitAmount;
    }
    public void SetHitAmount(int p_hitAmount)
    {
        m_currentHit = p_hitAmount;
    }
}