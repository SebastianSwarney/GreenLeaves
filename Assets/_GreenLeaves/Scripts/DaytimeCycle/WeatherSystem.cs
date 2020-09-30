using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    public enum WeatherType { Clear, Rain, Fog, Hail, Snow };
    public WeatherType m_currentWeather;

    public ModularParticleSystem m_rainParticles, m_hailParticles, m_snowParticles, m_fogParticles;

    public float m_transitionTime;

    public float m_stopEmittingRate;
    public float m_fullEmissionRate;
    [Header("Debugging")]
    public bool m_changeWeatherType;
    public WeatherType m_debugWeatherType;
    private void Update()
    {
        if (m_changeWeatherType)
        {
            m_changeWeatherType = false;
            ChangeWeather(m_debugWeatherType);
        }
    }
    public void ChangeWeather(WeatherType p_newWeatherType)
    {
        if (p_newWeatherType == m_currentWeather) return;
        ModularParticleSystem old = GetWeatherSystem(m_currentWeather);
        m_currentWeather = p_newWeatherType;
        if(m_currentWeather != WeatherType.Clear)
        {
            LightingManager.Instance.ToggleStormLighting(true);
        }
        else
        {
            LightingManager.Instance.ToggleStormLighting(false);
        }

        if(old!= null)
        {
            old.StopParticleSystem();
            if (GetWeatherSystem(m_currentWeather) != null)
            {
                GetWeatherSystem(m_currentWeather).StartParticlesImmediately();
            }
        }
        else
        {
            if (GetWeatherSystem(m_currentWeather) != null)
            {
                GetWeatherSystem(m_currentWeather).StartParticleSystem();
            }
        }

    }

    private ModularParticleSystem GetWeatherSystem(WeatherType p_currentWeatherType)
    {
        switch (p_currentWeatherType)
        {
            case WeatherType.Clear:
                return null;
            case WeatherType.Fog:
                return m_fogParticles;
            case WeatherType.Hail:
                return m_hailParticles;
            case WeatherType.Rain:
                return m_rainParticles;
            case WeatherType.Snow:
                return m_snowParticles;
        }
        return null;
    }

}
