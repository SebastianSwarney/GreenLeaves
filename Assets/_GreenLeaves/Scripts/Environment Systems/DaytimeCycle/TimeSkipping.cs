using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSkipping : MonoBehaviour
{
    public float m_skipTimeTo;
    public bool m_performAnimation = true;

    public UnityEngine.UI.Text m_text;
    public bool m_update;
    /*
    private void OnValidate()
    {
        if (m_update)
        {
            m_update = false;
            m_text.text = gameObject.name;
            m_skipTimeTo = float.Parse(m_text.text);
        }
    }*/
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            DaytimeCycle_Update.Instance.SetTime(m_skipTimeTo, m_performAnimation);
        }
    }
}
