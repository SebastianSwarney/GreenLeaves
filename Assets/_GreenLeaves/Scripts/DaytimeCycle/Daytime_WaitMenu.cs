using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daytime_WaitMenu : MonoBehaviour
{

    public static Daytime_WaitMenu Instance;
    public KeyCode m_exitKeyCode, m_increaseKeyCode, m_decreaseKeyCode, m_acceptKeyCode;

    public int m_howManyHoursToWait;



    private bool m_isWaiting = false;

    [Header("UI Elements")]
    public GameObject m_menuUi;
    public UnityEngine.UI.Text m_waitHourText;

    private void Awake()
    {
        Instance = this;
        enabled = false;
    }
    private void Update()
    {
        if (m_isWaiting) return;
        if (Input.GetKeyDown(m_exitKeyCode))
        {
            Debug.Log("Exit key press here", this);
            m_menuUi.gameObject.SetActive(false);
            enabled = false;
            PlayerInputToggle.Instance.ToggleInput(true);

        }
        else if (Input.GetKeyDown(m_increaseKeyCode))
        {
            Debug.Log("Increase key press here", this);
            if (m_howManyHoursToWait + 1 < 25)
            {
                m_howManyHoursToWait++;
                m_waitHourText.text = m_howManyHoursToWait.ToString();
            }
        }
        else if (Input.GetKeyDown(m_decreaseKeyCode))
        {
            Debug.Log("Decrease key press here", this);
            if (m_howManyHoursToWait - 1 > 0)
            {
                m_howManyHoursToWait--;
                m_waitHourText.text = m_howManyHoursToWait.ToString();
            }
        }
        else if (Input.GetKeyDown(m_acceptKeyCode))
        {
            Debug.Log("Accept key press here", this);
            m_isWaiting = true;
            StartCoroutine(PerformWait(m_howManyHoursToWait));

        }
    }

    public void OpenMenu()
    {
        PlayerInputToggle.Instance.ToggleInput(false);
        enabled = true;
        m_howManyHoursToWait = 1;
        m_waitHourText.text = m_howManyHoursToWait.ToString();
        m_menuUi.gameObject.SetActive(true);
    }

    
    private IEnumerator PerformWait(float p_hoursToWait)
    {

        Debug.LogError("Likely Want to stop stat drain here",this.gameObject);
        yield return StartCoroutine(DaytimeCycle_Update.Instance.TimeSkip(p_hoursToWait));

        m_isWaiting = false;

        yield return new WaitForSeconds(1);
        PlayerInputToggle.Instance.ToggleInput(true);
        DaytimeCycle_Update.Instance.ToggleDaytimePause(false);
        Debug.LogError("Refresh stats for waiting here", this.gameObject);
        m_menuUi.SetActive(false);
        enabled = false;

    }

}
