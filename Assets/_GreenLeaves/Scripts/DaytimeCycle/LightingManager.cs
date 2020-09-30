using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    public static LightingManager Instance;
    [Header("Lighting Components")]
    [SerializeField] private Light DirectionalDayLight;
    [SerializeField] private Light DirectionalNightLight;
    [SerializeField] private LightingPreset preset;

    [Header("Daytime Settings")]
    [SerializeField, Range(0, 24)] private float TimeOfDay;

    public float m_dayTimeStart, m_daytimeEnd;

    public float m_fullDayDuration = 10;
    public bool m_runInEditor;



    [Header("Storm Settings")]
    public bool m_inStorm;
    public LightingPreset m_stormLighting;
    public float m_timeToMaxPercent;
    public float m_percentOfStorm;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Setup();
        StartCoroutine(IncreaseTime());
    }
    private void Setup()
    {
        if (DirectionalDayLight != null) return;
        if (RenderSettings.sun != null)
        {
            DirectionalDayLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalDayLight = light;
                    return;
                }
            }
        }
    }
    private IEnumerator IncreaseTime()
    {

        while (true)
        {

            TimeOfDay += SecondsToHours(HoursToSeconds(24) / (m_fullDayDuration)) / 60;
            if (TimeOfDay > 24)
            {
                TimeOfDay -= 24;
            }
            yield return null;
        }
    }
    private void Update()
    {
        if (preset == null) return;

        UpdateLighting(TimeOfDay);

    }

    public float p_lightPercent;
    public bool m_isDayTime;
    private void UpdateLighting(float p_time)
    {

        float newPercent = 0;
        bool isDayTime = true;
        if (p_time > m_dayTimeStart && p_time < m_daytimeEnd)
        {
            isDayTime = true;

            newPercent = (p_time - m_dayTimeStart) / (m_daytimeEnd - m_dayTimeStart);

        }
        else
        {
            isDayTime = false;
            if (p_time < m_dayTimeStart)
            {
                newPercent = (p_time + (24 - m_daytimeEnd)) / (m_dayTimeStart+(24 - m_daytimeEnd));
            }
            else
            {
                newPercent = (p_time - m_daytimeEnd) / ((24 - m_daytimeEnd) + m_dayTimeStart);
            }

        }

        m_isDayTime = isDayTime;
        p_lightPercent = newPercent;
        Color newAmbient = Color.white;
        if (isDayTime)
        {
            newAmbient = Color.Lerp(preset.m_ambientDayColor.Evaluate(newPercent), m_stormLighting.m_ambientDayColor.Evaluate(newPercent), m_percentOfStorm);
            RenderSettings.skybox.SetFloat("_Exposure", preset.m_atmosphereDayThickness.Evaluate(newPercent));
        }
        else
        {
            newAmbient = Color.Lerp(preset.m_ambientNightColor.Evaluate(newPercent), m_stormLighting.m_ambientNightColor.Evaluate(newPercent), m_percentOfStorm);
            RenderSettings.skybox.SetFloat("_Exposure", preset.m_atmosphereNightThickness.Evaluate(newPercent));
        }
        RenderSettings.ambientLight = newAmbient;
        RenderSettings.skybox.SetColor("_SkyTint", newAmbient);
        

        Color newFog = Color.Lerp(preset.FogColor.Evaluate(newPercent), m_stormLighting.FogColor.Evaluate(newPercent), m_percentOfStorm);
        RenderSettings.fogColor = newFog;


        if (DirectionalDayLight != null)
        {
            if (isDayTime)
            {
                DirectionalNightLight.gameObject.SetActive(false);
                DirectionalDayLight.gameObject.SetActive(true);
                ///Daytime
                DirectionalDayLight.intensity = Mathf.Lerp(preset.m_directionalDayIntensity.Evaluate(newPercent), m_stormLighting.m_directionalDayIntensity.Evaluate(newPercent), m_percentOfStorm);
                DirectionalDayLight.color = preset.DirectionalDayColor.Evaluate(newPercent);

            }
            else
            {
                DirectionalDayLight.gameObject.SetActive(false);
                DirectionalNightLight.gameObject.SetActive(true);
                DirectionalNightLight.intensity = Mathf.Lerp(preset.m_directionalNightIntensity.Evaluate(newPercent), m_stormLighting.m_directionalNightIntensity.Evaluate(newPercent), m_percentOfStorm);
                DirectionalNightLight.color = preset.DirectionalNightColor.Evaluate(newPercent);
            }
            DirectionalDayLight.transform.localRotation = Quaternion.Euler(new Vector3(Mathf.Lerp(-30, 206, newPercent), 0, 0));
            DirectionalNightLight.transform.localRotation = Quaternion.Euler(new Vector3(Mathf.Lerp(-30, 206, newPercent), 0, 0));
        }
    }

    public bool m_updateTime = false;
    private void OnValidate()
    {
        if (!m_runInEditor) return;
        m_updateTime = false;
        Setup();
        UpdateLighting(TimeOfDay);
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


    public void ToggleStormLighting(bool p_newState)
    {
        m_inStorm = p_newState;
        StartCoroutine(ToggleStorm(p_newState));
    }
    private IEnumerator ToggleStorm(bool p_newState)
    {
        float timer = 0;
        while (timer < m_timeToMaxPercent)
        {
            timer += Time.deltaTime;
            m_percentOfStorm = (p_newState ? timer / m_timeToMaxPercent : 1 - timer / m_timeToMaxPercent);
            yield return null;
        }
        m_percentOfStorm = (p_newState ? 1 : 0);
    }
}
