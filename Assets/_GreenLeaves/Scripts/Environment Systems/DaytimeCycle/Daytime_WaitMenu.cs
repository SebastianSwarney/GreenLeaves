using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daytime_WaitMenu : MonoBehaviour
{

    public static Daytime_WaitMenu Instance;
    public int m_howManyHoursToWait;



    public bool m_isWaiting = false;

    [Header("UI Elements")]
    public GameObject m_menuUi;
    public UnityEngine.UI.Text m_waitHourText;

    public GameObject m_addButton, m_subtractButton, m_acceptButton, m_cancelButton;
    public UnityEngine.UI.Text m_waitingText;
    private void Awake()
    {
        Instance = this;
        enabled = false;
    }


    public void OpenMenu()
    {
        m_waitingText.text = "How long to rest?";
        m_addButton.SetActive(true);
        m_subtractButton.SetActive(false);
        m_acceptButton.SetActive(true);
        m_cancelButton.SetActive(true);

        m_isWaiting = true;
        PlayerInputToggle.Instance.ToggleInput(false);
        RespawnResourceManager.Instance.m_performTimers = false;
        enabled = true;
        m_howManyHoursToWait = 1;
        m_waitHourText.text = m_howManyHoursToWait.ToString();
        m_menuUi.gameObject.SetActive(true);
    }

    public void ExitMenu()
    {
        m_menuUi.gameObject.SetActive(false);
        enabled = false;
        PlayerInputToggle.Instance.ToggleInput(true);
        PlayerInputToggle.Instance.ToggleInputFromGameplay(true);
        RespawnResourceManager.Instance.m_performTimers = true;
        m_isWaiting = false;
    }

    public void AdjustTime(int p_dir)
    {
        m_howManyHoursToWait += p_dir;
        if (m_howManyHoursToWait == 24)
        {
            m_addButton.SetActive(false);
        }
        else if (m_howManyHoursToWait == 1)
        {
            m_subtractButton.SetActive(false);
        }
        else
        {
            m_subtractButton.SetActive(true);
            m_addButton.SetActive(true);
        }

        m_waitHourText.text = m_howManyHoursToWait.ToString();
    }

    public void Accept()
    {
        m_addButton.SetActive(false);
        m_subtractButton.SetActive(false);
        m_cancelButton.SetActive(false);
        m_acceptButton.SetActive(false);
        m_waitingText.text = "Resting...";
        StartCoroutine(PerformWait(m_howManyHoursToWait));
    }

    private IEnumerator PerformWait(float p_hoursToWait)
    {
        m_isWaiting = true;
        PlayerStatsController.Instance.m_pauseStatDrain = true;

        yield return StartCoroutine(DaytimeCycle_Update.Instance.TimeSkip(p_hoursToWait, m_waitHourText));


        m_isWaiting = false;
        yield return new WaitForSeconds(1);

        PlayerInputToggle.Instance.ToggleInput(true);
        DaytimeCycle_Update.Instance.ToggleDaytimePause(false);
        PlayerStatsController.Instance.m_pauseStatDrain = false;

        PlayerStatsController.Instance.AddStatsFromCampfire(p_hoursToWait);

        m_menuUi.SetActive(false);

        PlayerInputToggle.Instance.ToggleInputFromGameplay(true);
        RespawnResourceManager.Instance.m_performTimers = true;

        enabled = false;

    }

}
