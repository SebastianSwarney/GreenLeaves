using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpMessage : MonoBehaviour
{
    public static PopUpMessage Instance;
    public UnityEngine.UI.Text m_text;
    public CanvasGroup m_cg;
    public float m_fadeTime;
    public float m_stayTime;
    private void Awake()
    {
        Instance = this;
        m_cg.gameObject.SetActive(false);
    }

    public void ShowMessage(string p_message)
    {
        m_cg.gameObject.SetActive(true);
        StopAllCoroutines();
        m_text.text = p_message;
        m_cg.alpha = 1;

        StartCoroutine(ShowMessage());
    }

    private IEnumerator ShowMessage()
    {
        float timer = 0;
        while(timer < m_stayTime)
        {
            yield return null;
            timer += Time.deltaTime;
            
        }
        timer = 0;

        while (timer < m_stayTime)
        {
            yield return null;
            timer += Time.deltaTime;
            m_cg.alpha = 1 - (timer / m_stayTime);
        }
        m_cg.alpha = 0;
        m_cg.gameObject.SetActive(false);
    }
}
