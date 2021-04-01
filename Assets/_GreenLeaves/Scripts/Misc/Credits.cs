using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{

    public static Credits Instance;
    public bool m_isPlaying;

    public List<CreditContainer> m_theImportantPeople;
    public UnityEngine.UI.Text m_nameText, m_roleText;

    [System.Serializable]
    public class CreditContainer
    {
        public string m_name;
        public string m_roles;
    }

    public int m_currentIndex;

    public int m_summitIndex;

    public float m_fadeTime, m_stayTime;
    public AnimationCurve m_fadeCurve;
    public CanvasGroup m_mainCreditsCG;
    public List<CanvasGroup> m_toolMenuCG;

    public GameObject m_barriers;

    public float m_delaySummitTime = 0;

    public float m_summitFadeTime, m_summitStayTime;
    public AnimationCurve m_summitFadeCurve;


    public AnimationCurve m_musicFadeCurve;
    public float m_delayMusicCreds, m_musicFadeTime, m_musicStayTime;

    [Header("UI")]
    public GameObject m_mainCredits;
    public GameObject m_summitLogo;
    public GameObject m_musicStudentsCreds;
    private void Awake()
    {
        Instance = this;
    }
    public void StartCredits()
    {
        m_isPlaying = true;
        if (m_barriers)
        {
            m_barriers.SetActive(true);
        }
        StartCoroutine(CreditsAnim());
    }


    private IEnumerator CreditsAnim()
    {

        PlayerController.Instance.m_isCredits = true;
        m_summitLogo.SetActive(false);
        m_mainCredits.SetActive(true);
        m_mainCreditsCG.alpha = 0;

        float timer = 0;
        while (timer < .75f)
        {
            timer += Time.deltaTime;
            foreach (CanvasGroup gr in m_toolMenuCG)
            {
                gr.alpha = (1 - (timer / .75f));
            }
            yield return null;
        }

        foreach (CanvasGroup gr in m_toolMenuCG)
        {
            gr.alpha = 0;
        }

        #region Main Credits
        timer = 0;
        while (m_currentIndex < m_summitIndex)
        {
            m_nameText.text = m_theImportantPeople[m_currentIndex].m_name;
            m_roleText.text = m_theImportantPeople[m_currentIndex].m_roles;
            timer = 0;
            m_mainCreditsCG.alpha = 0;

            while (timer < m_fadeTime)
            {
                timer += Time.deltaTime;
                m_mainCreditsCG.alpha = m_fadeCurve.Evaluate(timer / m_fadeTime);
                yield return null;
            }
            m_mainCreditsCG.alpha = 1;
            timer = 0;
            while (timer < m_stayTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            timer = 0;
            while (timer < m_fadeTime)
            {
                timer += Time.deltaTime;
                m_mainCreditsCG.alpha = m_fadeCurve.Evaluate(1 - (timer / m_fadeTime));
                yield return null;
            }
            m_mainCreditsCG.alpha = 0;
            m_currentIndex++;
            yield return null;
        }

        #endregion

        #region Summit Logo
        m_mainCredits.SetActive(false);
        m_mainCreditsCG.alpha = 0;
        m_summitLogo.SetActive(true);

        yield return new WaitForSeconds(m_delaySummitTime);
        timer = 0;
        while (timer < m_summitFadeTime)
        {
            m_mainCreditsCG.alpha = m_fadeCurve.Evaluate(timer / m_summitFadeTime);
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        while (timer < m_summitStayTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        while (timer < m_summitFadeTime)
        {
            m_mainCreditsCG.alpha = m_fadeCurve.Evaluate(1 - (timer / m_summitFadeTime));
            timer += Time.deltaTime;
            yield return null;
        }


        #endregion


        #region Music People Creds
        m_mainCreditsCG.alpha = 0;
        m_summitLogo.SetActive(false);
        m_musicStudentsCreds.SetActive(true);

        yield return new WaitForSeconds(m_delayMusicCreds);
        timer = 0;
        while (timer < m_musicFadeTime)
        {
            timer += Time.deltaTime;
            m_mainCreditsCG.alpha = m_musicFadeCurve.Evaluate(timer / m_musicFadeTime);
            yield return null;
        }

        timer = 0;
        while (timer < m_musicStayTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        while (timer < m_musicFadeTime)
        {
            timer += Time.deltaTime;
            m_mainCreditsCG.alpha = m_musicFadeCurve.Evaluate(1 - (timer / m_musicFadeTime));
            yield return null;
        }

        #endregion
        if (m_barriers)
        {
            m_barriers.SetActive(false);
        }
        PlayerController.Instance.m_isCredits = false;
        m_isPlaying = false;



        timer = 0;
        while (timer < .75f)
        {
            timer += Time.deltaTime;
            foreach (CanvasGroup gr in m_toolMenuCG)
            {
                gr.alpha = (timer / .75f);
            }
            yield return null;
        }

        foreach (CanvasGroup gr in m_toolMenuCG)
        {
            gr.alpha = 1;
        }
    }
}
