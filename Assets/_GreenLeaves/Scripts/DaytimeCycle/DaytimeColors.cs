using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DaytimeColors_", menuName = "ScriptableObjects/DaytimeColors", order = 0)]
public class DaytimeColors : ScriptableObject
{
    public List<DayColors> m_dayColors;
    [System.Serializable]
    public struct DayColors
    {
        public float m_timeOfDay;
        [ColorUsage(true, true)]
        public Color m_skyColor, m_equatorColor, m_groundColor;
    }

    public void ChangeColors(float p_currentTime)
    {
        DayColors pastColor = m_dayColors[0], currentColor = m_dayColors[0];

        float percent = 0;
        if (p_currentTime < m_dayColors[0].m_timeOfDay)
        {
            pastColor = m_dayColors[m_dayColors.Count-1];
            currentColor = m_dayColors[0];

            percent = (pastColor.m_timeOfDay + p_currentTime) / (pastColor.m_timeOfDay + currentColor.m_timeOfDay);
        }
        else if (p_currentTime > m_dayColors[m_dayColors.Count-1].m_timeOfDay)
        {
            pastColor = m_dayColors[0];
            currentColor = m_dayColors[m_dayColors.Count-1];

            percent = (pastColor.m_timeOfDay + p_currentTime) / (pastColor.m_timeOfDay + currentColor.m_timeOfDay);
        }
        else
        {

            for (int i = 0; i < m_dayColors.Count; i++)
            {
                if (p_currentTime > m_dayColors[i].m_timeOfDay)
                {
                    pastColor = m_dayColors[i];
                }
                else
                {
                    currentColor = m_dayColors[i];
                    break;
                }
            }

            percent = (p_currentTime - pastColor.m_timeOfDay) / (currentColor.m_timeOfDay - pastColor.m_timeOfDay);
        }
        RenderSettings.ambientEquatorColor = Color.Lerp(pastColor.m_equatorColor, currentColor.m_equatorColor, percent);
        RenderSettings.ambientGroundColor = Color.Lerp(pastColor.m_groundColor, currentColor.m_groundColor, percent);
        RenderSettings.ambientSkyColor = Color.Lerp(pastColor.m_groundColor, currentColor.m_groundColor, percent);
    }
}
