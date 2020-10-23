using System.Collections;
using UnityEngine;

/// <summary>
/// Used to manipulate the Dissolve shader on the object.<br/>
/// Uses the Disolve Generic shader
/// </summary>
public class VFX_Dissolve : MonoBehaviour
{
    public MeshRenderer m_meshRenderer;
    private MaterialPropertyBlock m_propertyBlock;

    public float m_dissolveTime;
    public GenericWorldEvent m_completelyDissolvedEvent;
    private void Awake()
    {
        if(m_meshRenderer == null)
        {
            m_meshRenderer = GetComponent<MeshRenderer>();
        }
        m_propertyBlock = new MaterialPropertyBlock();
        m_meshRenderer.GetPropertyBlock(m_propertyBlock);
    }
    private void OnEnable()
    {
        if(!m_propertyBlock.isEmpty)
        {
            ToggleDissolve(false);
        }
    }
    
    /// <summary>
    /// Toggles the dissolve effect. |
    /// True = will disolve |
    /// false = Reset
    /// </summary>
    public void ToggleDissolve(bool p_newState)
    {
        StopAllCoroutines();
        if (!p_newState)
        {
            SetDissolve(0);
        }
        else
        {
            StartCoroutine(DissolveObject());
        }
    }

    /// <summary>
    /// Used to manipulate the shader to the specified percentage
    /// </summary>
    public void SetDissolve(float p_newPercent)
    {
        m_propertyBlock.SetFloat("_EffectAmount", p_newPercent);
        m_meshRenderer.SetPropertyBlock(m_propertyBlock);
    }


    /// <summary>
    /// Performs an animation of disolving the object.
    /// When complete, calls the CompletelyDissolvedEvent
    /// </summary>
    /// <returns></returns>
    private IEnumerator DissolveObject()
    {
        float timer = 0;
        while(timer < m_dissolveTime)
        {
            timer += Time.deltaTime;
            SetDissolve(timer / m_dissolveTime);
            yield return null;
        }
        SetDissolve(1);
        m_completelyDissolvedEvent.Invoke();
    }
}
