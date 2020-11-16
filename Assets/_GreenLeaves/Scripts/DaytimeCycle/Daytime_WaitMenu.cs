using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daytime_WaitMenu : MonoBehaviour
{

    public static Daytime_WaitMenu Instance;
    public KeyCode m_exitKeyCode, m_increaseKeyCode, m_decreaseKeyCode, m_acceptKeyCode;

    public int m_howManyHoursToWait;

    /// <summary>
    /// How much time passes while waiting to pass an hour.<br/>
    /// IE. Real time waiting = this X how many hours you wait
    /// </summary>
    public float m_secondsToWait;

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
            StartCoroutine(PerformWait());

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

    public float m_increaseCheck, m_increaseAmount;
    public float m_waitTime;

    public bool m_fix;

    private IEnumerator PerformWait()
    {
        float waitingTime = m_secondsToWait;
        m_waitTime = waitingTime;
        float increase = m_secondsToWait/ m_howManyHoursToWait;
        m_increaseAmount = (increase < 1f) ? (Time.deltaTime / increase) : (increase * Time.deltaTime);
        while (waitingTime > 0)
        {
            waitingTime -= Time.deltaTime;
            DaytimeCycle_Update.Instance.UpdateTimeOfDayThroughPass(m_increaseAmount);
            m_increaseCheck += increase;
            yield return null;
        }
        m_isWaiting = false;

        yield return new WaitForSeconds(1);

        m_menuUi.SetActive(false);
        PlayerInputToggle.Instance.ToggleInput(true);

        DaytimeCycle_Update.Instance.ToggleDaytimePause(false);

        enabled = false;

    }

}
