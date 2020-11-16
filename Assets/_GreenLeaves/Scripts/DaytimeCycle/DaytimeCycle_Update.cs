using UnityEngine;

public class DaytimeCycle_Update : MonoBehaviour
{

    public static DaytimeCycle_Update Instance;
    [Header("Daytime Settings")]
    [SerializeField, Range(0, 24)] private float TimeOfDay;
    public float m_fullDayDuration = 10;

    public Transform m_directionalLightObject;
    public float m_realtime;

    private bool m_isPaused;
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
}
