using System.Collections;
using UnityEngine;

/// <summary>
/// A class used to show the durability on the trees and bushes. <br/>
/// The Update Text function needs to be called in order to update the text<br/>
/// The Hide and Show functions will toggle the visiblity of the ui.
/// </summary>
public class Durability_UI : MonoBehaviour
{
    public CanvasGroup m_cg;
    public float m_fadeTime;

    public UnityEngine.UI.Text m_durabilityText;

    public GameObject m_controlUI;
    private void Awake()
    {
        if (enabled)
        {
            m_cg.alpha = 0;
            m_cg.gameObject.SetActive(false);
        }
    }

    public void UpdateText(int p_durabilityAmount)
    {
        m_durabilityText.text = "+ " + p_durabilityAmount.ToString();
    }

    public void HideUI()
    {
        StopAllCoroutines();
        StartCoroutine(FadeToggle(true));
    }

    public void ShowUI()
    {
        StopAllCoroutines();
        StartCoroutine(FadeToggle(false));
    }

    public void ShowControlUI(bool p_newState)
    {
        m_controlUI.SetActive(p_newState);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    private IEnumerator FadeToggle(bool p_fadeOut)
    {
        m_cg.gameObject.SetActive(true);
        float timer = m_fadeTime * (p_fadeOut ? (1 - m_cg.alpha) : (m_cg.alpha));
        while (timer < m_fadeTime)
        {
            m_cg.alpha = (p_fadeOut ? (1 - (timer / m_fadeTime)) : (timer / m_fadeTime));
            timer += Time.deltaTime;
            yield return null;
        }
        m_cg.alpha = (p_fadeOut ? 0 : 1);
        if (p_fadeOut)
        {
            m_cg.gameObject.SetActive(false);
        }

    }
}
