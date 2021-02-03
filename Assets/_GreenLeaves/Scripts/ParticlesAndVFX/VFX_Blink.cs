using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_Blink : MonoBehaviour
{
    public List<Renderer> m_renderers;
    public float m_blinkSpeed;
    public float m_minBlinkTime, m_maxBlinkTime;
    private float m_currentBlinkTimer;

    private float m_timer;
    private bool m_blinking;
    private int m_blinkState;
    public List<Vector2> m_blinkOffsets;
    private void Awake()
    {
        m_currentBlinkTimer = Random.Range(m_minBlinkTime, m_maxBlinkTime);
    }
    private void Update()
    {
        m_timer += Time.deltaTime;
        if (m_blinking)
        {
            if (m_timer > m_blinkSpeed)
            {
                m_timer = 0;
                m_blinkState += 1;
                if (m_blinkState == 6)
                {
                    m_blinkState = 0;
                    m_blinking = false;
                    m_currentBlinkTimer = Random.Range(m_minBlinkTime, m_maxBlinkTime);
                }
                SetMaterial(m_blinkOffsets[m_blinkState]);
            }
        }
        else
        {
            if (m_timer > m_currentBlinkTimer)
            {
                m_blinking = true;
                m_timer = 0;
                m_blinkState = 0;
            }
        }

    }

    private void SetMaterial(Vector2 p_offset)
    {
        foreach(Renderer rend in m_renderers)
        {
            rend.material.SetTextureOffset("_MainTex", p_offset);
        }
    }
}
