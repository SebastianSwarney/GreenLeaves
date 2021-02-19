﻿using System.Collections;
using UnityEngine;

/// <summary>
/// A class used to show the durability on the trees and bushes. <br/>
/// The Update Text function needs to be called in order to update the text<br/>
/// The Hide and Show functions will toggle the visiblity of the ui.
/// </summary>
public class Durability_UI : MonoBehaviour
{
    public static Durability_UI Instance;
    public CanvasGroup m_cg;
    public float m_fadeTime;

    public UnityEngine.UI.Text m_durabilityText, m_promptText;

    public RotateAndScaleToPlayer m_rotate;
    public Transform m_playerRoot;

    public float m_heightOffset;
    private void Awake()
    {
        m_cg.alpha = 0;
        m_cg.gameObject.SetActive(false);
        Instance = this;
        
    }
    private void Start()
    {
        m_playerRoot = PlayerInputToggle.Instance.transform;
    }

    private void Update()
    {
        transform.position = m_playerRoot.transform.position + Vector3.up * m_heightOffset;
    }
    public void UpdateText(int p_durabilityAmount)
    {
        if (m_durabilityText == null) return;
        m_durabilityText.text = "+ " + p_durabilityAmount.ToString();
    }
    public void UpdatePromptText(string p_newText)
    {
        if (m_promptText.text == null) return;
        m_promptText.text = p_newText;
    }
    public void HideUI()
    {
        StopAllCoroutines();
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(FadeToggle(true));
        }
    }

    public void ShowUI()
    {
        Debug.Log("Show UI");
        gameObject.SetActive(true);
        m_cg.alpha = 0;
        m_rotate.ForceUpdate();
        StopAllCoroutines();
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(FadeToggle(false));
        }
    }


    private void OnDisable()
    {
        StopAllCoroutines();
        m_cg.alpha = 0;
        m_cg.gameObject.SetActive(false);
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
            StopAllCoroutines();
            gameObject.SetActive(false);
        }

    }
}
