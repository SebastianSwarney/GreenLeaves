using System.Collections;
using UnityEngine;

/// <summary>
/// Utilizes the Shake shader & shake by height shader currently
/// Must pass the position to the shader
/// </summary>
public class VFX_Shake_Shader : MonoBehaviour
{
    public MeshRenderer m_renderer;
    private MaterialPropertyBlock m_block;

    [Header("Shake Properties")]
    public float m_objectHeight;
    public float m_shakeTime;
    public float m_maxTreeDistance;
    public AnimationCurve m_shakeCurve;

    public bool m_shakeByHeight;

    private void Start()
    {
        if (m_renderer == null)
        {
            Debug.LogError("Missing Renderer: " + gameObject.name + " | Retriving at runtime.", gameObject);
            m_renderer = GetComponent<MeshRenderer>();
        }

        m_renderer.GetPropertyBlock(m_block);
        m_block.SetFloat("_XPos", transform.position.x);
        m_block.SetFloat("_ZPos", transform.position.z);
        if (m_shakeByHeight)
        {
            m_block.SetFloat("_YPos", transform.position.y);
            m_block.SetFloat("_Height", m_objectHeight);
        }
        m_renderer.SetPropertyBlock(m_block);
    }
    private void OnEnable()
    {
        if (m_block == null)
        {
            m_block = new MaterialPropertyBlock();
        }
        else
        {
            SetShakeAmount(0);
        }
    }


    /// <summary>
    /// Call this function to start a new shake on the object. <br/>
    /// Note: Will stop the current shake if there is one, and start a new one
    /// </summary>
    public void StartShake()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float timer = 0;
        while (timer < m_shakeTime)
        {
            timer += Time.deltaTime;

            SetShakeAmount(1 - (timer / m_shakeTime));

            yield return null;
        }
        SetShakeAmount(0);
    }

    public void SetShakeAmount(float p_percent)
    {
        float amount = m_shakeCurve.Evaluate(p_percent);
        m_block.SetFloat("_Distance", Mathf.Lerp(0, m_maxTreeDistance, amount));
        m_renderer.SetPropertyBlock(m_block);
    }


    /// <summary>
    /// The wind dir must be inbetween (-0.5, 0.5);
    /// </summary>
    public void SetWindAmount(float p_xWindDir, float p_zWindDir)
    {
        m_block.SetFloat("_XWindDir", p_xWindDir);
        m_block.SetFloat("_ZWindDir", p_zWindDir);
        m_renderer.SetPropertyBlock(m_block);
    }
}
