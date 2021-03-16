using UnityEngine;

/// <summary>
/// <para>Used on the objects that can be hit by a tool.<br/>
/// After a certain amount of hits, the object will invoke the object died event, and disable the m_canHit boolean. </para>
/// Note: This script does not disable the object, nor does it spawn the object's resources
/// </summary>
public class Manipulation_HitObject : MonoBehaviour
{
    [Tooltip("Used to determine what type of bush this is. 0 = Knife, 1 = Axe, 3 = both")]
    public int m_cutType;
    public int m_hitAmount;
    private int m_currentHit;
    public bool m_canHit= true;

    public GenericWorldEvent m_objectHit, m_objectDied, m_objectHitGround;

    public bool m_disableObjectOnDeath = true;

    private void OnDisable()
    {
        if(Interactable_Manager.Instance != null)
        Interactable_Manager.Instance.SearchForInteractable();
    }
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
            if (m_disableObjectOnDeath)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            Durability_UI.Instance.ShowUI(true);
            Durability_UI.Instance.UpdateText(m_hitAmount - m_currentHit);
            m_objectHit.Invoke();
        }
    }

    public void ObjectRespawn()
    {
        
        gameObject.SetActive(true);
        m_canHit = true;
        m_currentHit = 0;
    }
    public void SetHitAmount(int p_hitAmount)
    {
        m_currentHit = p_hitAmount;
    }

    public void ShowUI()
    {
        Durability_UI.Instance.ShowUI(true);
        Durability_UI.Instance.UpdateText(m_hitAmount - m_currentHit);
    }
    public void HideUI()
    {
        Durability_UI.Instance.HideUI();
    }

    public void DisableTree()
    {
        transform.parent.gameObject.SetActive(false);
    }


    public void ObjectHitGround()
    {
        m_objectHitGround.Invoke();
    }
}
