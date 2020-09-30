using UnityEngine;

[CreateAssetMenu (fileName = "Lighting Preset", menuName = "Scriptables/Lighting Preset", order = 1)]
public class LightingPreset : ScriptableObject
{
    public Gradient m_ambientDayColor;
    public Gradient m_ambientNightColor;
    public Gradient DirectionalDayColor;
    public Gradient DirectionalNightColor;
    public Gradient FogColor;
    public AnimationCurve m_directionalDayIntensity;
    public AnimationCurve m_directionalNightIntensity;

    public AnimationCurve m_atmosphereDayThickness;
    public AnimationCurve m_atmosphereNightThickness;
}
