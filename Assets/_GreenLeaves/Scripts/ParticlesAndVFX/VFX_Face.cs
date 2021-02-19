using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_Face : MonoBehaviour
{
    public Transform m_rightEyebrow, m_leftEyebrow;
    public enum FaceExpression { Default, Questioning, Mad, Sad, MadSad}
    public FaceExpression m_currentExpression;
    private FaceExpression m_prevExpress;
    public GameObject m_tears;

    public ExpressionData m_defaultExpression, m_questionExpression, m_madExpression, m_sadExpression, m_madSadExpression;

    [System.Serializable]
    public class ExpressionData
    {
        public Vector3 m_leftPos, m_rightPos;
        public float m_leftRot, m_rightRot;
        public bool m_tearsEnabled;
        public void SetExpression(Transform p_leftBrow, Transform p_rightBrow, GameObject p_tears)
        {
            p_leftBrow.transform.localPosition = m_leftPos;
            p_leftBrow.transform.localEulerAngles = new Vector3(0, 0, m_leftRot);

            p_rightBrow.transform.localPosition = m_rightPos;
            p_rightBrow.transform.localEulerAngles = new Vector3(0, 0, m_rightRot);

            p_tears.SetActive(m_tearsEnabled);
        }
    }
    private void Update()
    {
        if(m_currentExpression != m_prevExpress)
        {
            UpdateExpression();
            m_prevExpress = m_currentExpression;
        }
    }
    public void UpdateExpression()
    {
        switch (m_currentExpression)
        {
            case FaceExpression.Default:
                m_defaultExpression.SetExpression(m_leftEyebrow, m_rightEyebrow, m_tears);
                break;
            case FaceExpression.Questioning:
                m_questionExpression.SetExpression(m_leftEyebrow, m_rightEyebrow, m_tears);
                break;
            case FaceExpression.Mad:
                m_madExpression.SetExpression(m_leftEyebrow, m_rightEyebrow, m_tears);
                break;
            case FaceExpression.Sad:
                m_sadExpression.SetExpression(m_leftEyebrow, m_rightEyebrow, m_tears);
                break;
            case FaceExpression.MadSad:
                m_madSadExpression.SetExpression(m_leftEyebrow, m_rightEyebrow, m_tears);
                break;
        }
    }
}
