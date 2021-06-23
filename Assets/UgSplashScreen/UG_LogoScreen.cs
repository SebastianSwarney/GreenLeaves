using UnityEngine;

public class UG_LogoScreen : MonoBehaviour
{
    public CanvasGroup m_cgGroup;
    public float m_fadeOutTime;
    public float m_fadeInTime;
    public Animator m_logoAnimator;

    public bool m_loadFirstScene;
    private void Awake()
    {
        m_cgGroup.alpha = 1;
    }
    private void Start()
    {
        m_logoAnimator.enabled = false;
        StartCoroutine(DelayLogo());
    }


    private System.Collections.IEnumerator DelayLogo()
    {
        
        float timer = 0;
        while (timer < m_fadeInTime)
        {
            timer += Time.deltaTime;
            m_cgGroup.alpha = (timer / m_fadeInTime);
            yield return null;
        }
        m_cgGroup.alpha = 1;

        m_logoAnimator.enabled = true;
    }

    public void StopLogo()
    {

        StartCoroutine(FadeOut());

    }
    private System.Collections.IEnumerator FadeOut()
    {
        float timer = 0;
        while (timer < m_fadeOutTime)
        {
            timer += Time.deltaTime;
            m_cgGroup.alpha = 1-(timer / m_fadeInTime);
            yield return null;
        }
        m_cgGroup.alpha = 0;

        yield return null;
        if(m_loadFirstScene)
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
