using UnityEngine;
using UnityEngine.UI;

public class ChangeButtonColor : MonoBehaviour
{
    public Image m_secondaryImage;
    public ColorStruct m_secondaryImageColor;
    public Text m_text;
    public ColorStruct m_textColor;


    [System.Serializable]
    public struct ColorStruct
    {
        public Color m_defaultColor, m_hoverColor, m_selectedColor, m_pressedColor;
    }


    public void HoverOn()
    {
        if(m_secondaryImage != null)
        {
            m_secondaryImage.color = m_secondaryImageColor.m_hoverColor;
        }
        if(m_text!= null)
        {
            m_text.color = m_textColor.m_hoverColor;
        }
    }

    public void Default()
    {
        if (m_secondaryImage != null)
        {
            m_secondaryImage.color = m_secondaryImageColor.m_defaultColor;
        }
        if (m_text != null)
        {
            m_text.color = m_textColor.m_defaultColor;
        }
    }

    public void Selected()
    {
        if (m_secondaryImage != null)
        {
            m_secondaryImage.color = m_secondaryImageColor.m_selectedColor;
        }
        if (m_text != null)
        {
            m_text.color = m_textColor.m_selectedColor;
        }
    }

    public void Pressed()
    {
        if (m_secondaryImage != null)
        {
            m_secondaryImage.color = m_secondaryImageColor.m_pressedColor;
        }
        if (m_text != null)
        {
            m_text.color = m_textColor.m_pressedColor;
        }
    }
}
