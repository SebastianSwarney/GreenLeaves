﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public float m_fadeTime, m_appearTime;
    private float m_currentFadeTimer;
    private bool m_isFading, m_isAppearing;
    public CanvasGroup m_enterCanvasGroup;

    [Header("Sounds")]
    public bool m_playRandomSounds;
    public List<FMODUnity.StudioEventEmitter> m_randomSounds;
    public float m_minTime = 15, m_maxTime = 20;

    private void Start()
    {
        if (m_playRandomSounds)
        {
            StartCoroutine(RandomSounds());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GlobalSceneManager.Instance.LoadNewScene(1, true);
        }
        else
        {
            if (m_isAppearing)
            {
                m_enterCanvasGroup.alpha = 1;
                if(m_currentFadeTimer > m_appearTime)
                {
                    m_isAppearing = false;
                    m_isFading = true;
                    m_currentFadeTimer = 0;
                }
            }
            else
            {
                if (m_isFading)
                {
                    m_enterCanvasGroup.alpha = 1 - (m_currentFadeTimer / m_fadeTime);
                    if(m_currentFadeTimer >= m_fadeTime)
                    {
                        m_currentFadeTimer = 0;
                        m_isFading = false;
                        m_enterCanvasGroup.alpha = 0;
                    }
                }
                else
                {
                    m_enterCanvasGroup.alpha = m_currentFadeTimer / m_fadeTime;
                    if (m_currentFadeTimer >= m_fadeTime)
                    {
                        m_currentFadeTimer = 0;
                        m_isFading = true;
                        m_isAppearing = true;
                        m_enterCanvasGroup.alpha = 1;
                    }
                }
            }

            m_currentFadeTimer += Time.deltaTime;
        }
    }

    private IEnumerator RandomSounds()
    {
        float timer = 0;
        float currentTime = Random.Range(m_minTime, m_maxTime);
        while (true)
        {
            if (timer > currentTime)
            {
                timer = 0;
                currentTime = Random.Range(m_minTime, m_maxTime);
                if (m_randomSounds.Count > 0)
                {
                    m_randomSounds[Random.Range(0, m_randomSounds.Count)].Play();
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }
}