using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_PerformShine : MonoBehaviour
{
    public List<Renderer> m_affectedRenderers;
    private List<MaterialPropertyBlock> m_materialBlocks;
    public float m_startingPoint, m_endingPoint;
    public float m_effectLoopTime;
    private float m_timer = 0;

    private void Awake()
    {
        m_materialBlocks = new List<MaterialPropertyBlock>();
        for (int i = 0; i < m_affectedRenderers.Count; i++)
        {
            m_materialBlocks.Add(new MaterialPropertyBlock());
            m_affectedRenderers[i].GetPropertyBlock(m_materialBlocks[i]);
        }

    }

    public void SetMaterialBlocks(float p_newEffectAmount)
    {
        for (int i = 0; i < m_materialBlocks.Count; i++)
        {
            m_materialBlocks[i].SetFloat("_ShinePoint", p_newEffectAmount);
            m_affectedRenderers[i].SetPropertyBlock(m_materialBlocks[i]);
        }
    }
    public void StartEffect()
    {
        StopAllCoroutines();
        if (enabled && gameObject.activeSelf && gameObject.activeInHierarchy)
        {
            StartCoroutine(EffectLoop());
        }
    }
    private IEnumerator EffectLoop()
    {
        while (true)
        {
            m_timer += Time.deltaTime;
            if(m_timer > m_effectLoopTime)
            {
                m_timer = 0;
            }
            SetMaterialBlocks(Mathf.Lerp(m_startingPoint, m_endingPoint, (m_timer / m_effectLoopTime)));
            yield return null;
        }
    }

    public void StopEffect()
    {
        StopAllCoroutines();
        SetMaterialBlocks(m_startingPoint);
    }
}
