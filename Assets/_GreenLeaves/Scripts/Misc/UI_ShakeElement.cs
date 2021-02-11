using UnityEngine;

public class UI_ShakeElement : MonoBehaviour
{
    public Vector2 m_shakeMax;
    private Vector3 m_startingPos;
    [Range(0,1)]
    public float m_shakeAmount;

    // Start is called before the first frame update
    void Start()
    {
        m_startingPos = transform.localPosition;
    }

    public void UpdateShakeAmount(float p_shakeAmount)
    {
        m_shakeAmount = p_shakeAmount;
        if (m_shakeAmount == 0)
        {
            transform.localPosition = m_startingPos;
        }
    }
    private void Update()
    {
        if(m_shakeAmount > 0)
        {
            transform.localPosition = m_startingPos + new Vector3(Random.Range(-m_shakeMax.x, m_shakeMax.x), Random.Range(-m_shakeMax.x, m_shakeMax.x),0) * m_shakeAmount;
        }
    }
}
