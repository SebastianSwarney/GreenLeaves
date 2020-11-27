using System.Collections;
using UnityEngine;

public class DaytimeCycle_Update : MonoBehaviour
{

    public static DaytimeCycle_Update Instance;
    [Header("Daytime Settings")]
    [SerializeField, Range(0, 24)] private float TimeOfDay;
    public float m_fullDayDuration = 10;

    public Transform m_directionalLightObject;
    public float m_realtime;

    public bool m_isPaused;
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

    }

#endif
    // Start is called before the first frame update
    private void Update()
    {
        if (m_isPaused) return;
        m_realtime += Time.deltaTime;
        TimeOfDay += (24 / m_fullDayDuration) * Time.deltaTime;
        if (TimeOfDay > 24)
        {
            TimeOfDay -= 24;
        }

        UpdateLightRotation();
    }

    public void UpdateTimeOfDayThroughPass(float p_increaseAmount)
    {
        TimeOfDay += p_increaseAmount;
        if (TimeOfDay > 24)
        {
            TimeOfDay -= 24;
        }
        UpdateLightRotation();
    }
    private void UpdateLightRotation()
    {
        m_directionalLightObject.transform.eulerAngles = new Vector3(Mathf.Lerp(-90, 270, TimeOfDay / 24f), 0, 0);
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


    #region Waiting Functionality
    /// <summary>
    /// How much time passes while waiting to pass an hour.<br/>
    /// IE. Real time waiting = this X how many hours you wait
    /// </summary>
    public float m_secondsToWait;
    public float m_increaseAmount;

    public IEnumerator TimeSkip(float p_hoursToWait)
    {
        float waitingTime = m_secondsToWait;
        float increase = m_secondsToWait / p_hoursToWait;
        /*if (increase < 1f)
        {
            m_increaseAmount = (Time.deltaTime / increase);
        }
        else
        {
            m_increaseAmount = (increase * Time.deltaTime);
        }*/
        m_increaseAmount = (0.0041888f / increase);
        Debug.Log("TimeDelta: " + Time.deltaTime.ToString() + " | Increase Amount: " + m_increaseAmount);
        while (waitingTime > 0)
        {
            waitingTime -= 0.0041888f;
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
            if((int)p_setTime > (int)TimeOfDay)
            {
                newTime = ((int)p_setTime - (int)TimeOfDay);
            }
            else
            {
                newTime = (int)(12 - (int)TimeOfDay) + (12 + (int)p_setTime);
            }
            
            StartCoroutine(AnimatedTimeSkip(newTime));
        }
        else
        {
            TimeOfDay = p_setTime;
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
