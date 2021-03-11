using UnityEngine;
using System.Collections;

public class PlayerEquipmentBreak : MonoBehaviour
{
    public static PlayerEquipmentBreak Instance;

    public CanvasGroup m_cg;
    public float m_fadeTime;
    public float m_showTime;

    private void Awake()
    {
        Instance = this;
        m_cg.alpha = 0;
        m_cg.gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        m_cg.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeUI());
    }

    private IEnumerator FadeUI()
    {
        m_cg.alpha = 1;
        float timer = 0;
        while (timer < m_showTime)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        timer = 0;

        while (timer < m_fadeTime)
        {
            yield return null;
            timer += Time.deltaTime;
            m_cg.alpha = 1 - (timer /m_fadeTime);
        }
        m_cg.alpha = 0;
        m_cg.gameObject.SetActive(false);
    }
}
