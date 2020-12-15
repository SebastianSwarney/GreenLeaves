﻿using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DaytimeColors_", menuName = "ScriptableObjects/DaytimeColors", order = 0)]
public class DaytimeColors : ScriptableObject
{
    public List<DayColors> m_dayColors;

    public DayColors m_caveColor;

    [System.Serializable]
    public struct DayColors
    {
        public float m_timeOfDay;
        [ColorUsage(true, true)]
        public Color m_skyColor, m_equatorColor, m_groundColor;
    }

    public void ChangeColors(float p_currentTime, float p_cavePercent)
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
        RenderSettings.ambientEquatorColor = Color.Lerp(Color.Lerp(pastColor.m_equatorColor, currentColor.m_equatorColor, percent), m_caveColor.m_equatorColor, p_cavePercent);
        RenderSettings.ambientGroundColor = Color.Lerp(Color.Lerp(pastColor.m_groundColor, currentColor.m_groundColor, percent), m_caveColor.m_groundColor, p_cavePercent);
        RenderSettings.ambientSkyColor = Color.Lerp(Color.Lerp(pastColor.m_skyColor, currentColor.m_skyColor, percent), m_caveColor.m_skyColor, p_cavePercent);
    }
}