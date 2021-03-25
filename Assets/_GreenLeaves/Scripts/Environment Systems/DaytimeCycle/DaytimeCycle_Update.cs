using System.Collections;
using UnityEngine;

public class DaytimeCycle_Update : MonoBehaviour
{

    public static DaytimeCycle_Update Instance;
    [Header("Daytime Settings")]
    [Range(0, 24)]
    public float m_timeOfDay;
    public float m_fullDayDuration = 10;
    public float m_passOutAmount = 3;

    public Transform m_directionalLightObject;
    public Light m_directionalLight;
    public Light m_nightLight;
    private bool m_inCave;

    public float m_realtime;

    public bool m_isPaused;

    [Header("Gradient Colors")]
    public DaytimeColors m_currentGradientData;

    public float m_cavePercent;
    public float m_lightingAdjustmentTime;
    private Coroutine m_caveLightingCoroutine;
    private void Awake()
    {
        Instance = this;
    }

#if UNITY_EDITOR
    public bool m_updateInEditor;

    private void OnValidate()
    {
        if (!m_updateInEditor) return;
        UpdateLightRotation();
        m_currentGradientData.ChangeColors(m_timeOfDay, m_cavePercent, m_directionalLight, m_nightLight, m_inCave);
    }

#endif
    // Start is called before the first frame update
    private void Update()
    {
        if (m_isPaused) return;
        m_realtime += Time.deltaTime;
        m_timeOfDay += (24 / m_fullDayDuration) * Time.deltaTime;
        if (m_timeOfDay > 24)
        {
            m_timeOfDay -= 24;
        }

        UpdateLightRotation();
        m_currentGradientData.ChangeColors(m_timeOfDay, m_cavePercent, m_directionalLight, m_nightLight, m_inCave);
    }



    public void UpdateTimeOfDayThroughPass(float p_increaseAmount)
    {
        m_timeOfDay += p_increaseAmount;
        if (m_timeOfDay > 24)
        {
            m_timeOfDay -= 24;
        }
        UpdateLightRotation();
    }
    private void UpdateLightRotation()
    {
        m_directionalLightObject.transform.eulerAngles = new Vector3(Mathf.Lerp(-90, 270, m_timeOfDay / 24f), 0, 0);
    }


    private float HoursToSeconds(float p_hour)
    {
        return p_hour * 60 * 60;
    }
    private float MinutesToSeconds(float p_mins)
    {
        return p_mins * 60;
    }
    private float SecondsToHours(float p_secs)
    {
        return p_secs / 60 / 60;
    }

    /// <summary>
    /// Toggle the pausing of the increasing time.<br/>
    /// True = Paused <br/>
    /// False = Increase Time
    /// </summary>
    public void ToggleDaytimePause(bool p_newState)
    {
        m_isPaused = p_newState;
    }

    private IEnumerator AdjustCaveLighting(bool p_darken)
    {
        m_inCave = p_darken;
        float timer = m_cavePercent * m_lightingAdjustmentTime;
        while (p_darken ? (timer < m_lightingAdjustmentTime) : (timer > 0))
        {
            if (p_darken)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer -= Time.deltaTime;
            }
            m_cavePercent = timer / m_lightingAdjustmentTime;
            yield return null;
        }
    }

    public void ChangeLightingToCave(bool p_darken)
    {
        if (m_caveLightingCoroutine != null)
        {
            StopCoroutine(m_caveLightingCoroutine);
        }
        StartCoroutine(AdjustCaveLighting(p_darken));
    }

    public void PassOut()
    {
        m_timeOfDay += m_passOutAmount;
    }

    #region Waiting Functionality
    /// <summary>
    /// How much time passes while waiting to pass an hour.<br/>
    /// IE. Real time waiting = this X how many hours you wait
    /// </summary>
    public float m_secondsToWait;
    public float m_increaseAmount;

    public IEnumerator TimeSkip(float p_hoursToWait, UnityEngine.UI.Text p_textEl = null)
    {
        int startingTime = (int)p_hoursToWait;
        float startingWait = m_secondsToWait;
        float waitingTime = m_secondsToWait;
        float increase = m_secondsToWait / p_hoursToWait;
        m_increaseAmount = (0.0041888f / increase);
        while (waitingTime > 0)
        {
            waitingTime -= 0.0041888f;
            if (p_textEl != null)
            {
                p_textEl.text = ((int)(Mathf.Lerp(startingTime, 0, 1 - (waitingTime / m_secondsToWait))) + 1).ToString();
            }
            UpdateTimeOfDayThroughPass(m_increaseAmount);
            yield return null;
        }
    }

    private bool m_canRun = true;
    public void SetTime(float p_setTime, bool p_animate)
    {
        if (!m_canRun) return;
        m_canRun = false;
        StopAllCoroutines();
        if (p_setTime > 24)
        {
            p_setTime = p_setTime % 24;
        }
        if (p_animate)
        {
            float newTime = 0;
            if ((int)p_setTime > (int)m_timeOfDay)
            {
                newTime = ((int)p_setTime - (int)m_timeOfDay);
            }
            else
            {
                newTime = (int)(12 - (int)m_timeOfDay) + (12 + (int)p_setTime);
            }

            StartCoroutine(AnimatedTimeSkip(newTime));
        }
        else
        {
            m_timeOfDay = p_setTime;
            UpdateLightRotation();
            m_canRun = true;
        }
    }

    private IEnumerator AnimatedTimeSkip(float p_newTime)
    {
        bool wasPaused = m_isPaused;
        m_isPaused = true;
        Debug.Log("Hours to wait: " + p_newTime);
        yield return StartCoroutine(TimeSkip(p_newTime));
        m_isPaused = wasPaused;
        m_canRun = true;
    }



    #endregion
}
