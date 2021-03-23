﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{

    public static MusicPlayer Instance;
    public float m_minTimeBetweenTracks, m_maxTimeBetweenTracks;

    private float m_currentTimer, m_currentRandomTime;
    public FMODUnity.StudioEventEmitter m_emitter;

    [FMODUnity.EventRef]
    public string m_initialSongEvent;

    public bool m_playRandomInitialSong;
    public bool m_playSongOnStart;

    public float m_delayInitialTime;
    private bool m_delayingInitialTime;
    public List<DaytimeBasedSongs> m_daytimeBasedSongs;
    [System.Serializable]
    public struct DaytimeBasedSongs
    {
        [FMODUnity.EventRef]
        public string m_daytimeBasedSongEvent;
        public float m_minDaytime, m_maxDaytime;
    }


    [Header("Runtime Song States")]
    public MusicPlayer_Trigger.MusicTriggerType m_currentMusicType;
    [Header("Summit Song")]
    [FMODUnity.EventRef]
    public string m_summitSongEvent;
    public bool m_summitSongPlaying;

    [Header("Climbing Song")]
    [FMODUnity.EventRef]
    public string m_climbingSongEvent;
    public bool m_climbingSongPlaying;


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        m_delayingInitialTime = true;
        StartCoroutine(DelayInitialSong());
    }

    private IEnumerator DelayInitialSong()
    {
        yield return new WaitForSeconds(m_delayInitialTime);
        if (m_playRandomInitialSong)
        {
            string current = GetCurrentSongType();
            if (current != "oof")
            {
                m_emitter.Event = current;
            }
            else
            {
                m_emitter.Event = m_initialSongEvent;
            }
        }
        else
        {
            m_emitter.Event = m_initialSongEvent;
        }
        m_emitter.Lookup();
        if (m_playSongOnStart)
        {
            m_emitter.Play();
            m_currentTimer = 0;
            m_currentRandomTime = Random.Range(m_minTimeBetweenTracks, m_maxTimeBetweenTracks);
        }
        m_delayingInitialTime = false;
    }
    private void Update()
    {


        if (m_delayingInitialTime) return;
        if (!m_emitter.IsPlaying() && !m_summitSongPlaying)
        {
            m_currentTimer += Time.deltaTime;
            if (m_currentTimer > m_currentRandomTime)
            {
                string current = GetCurrentSongType();


                if (current != "oof")
                {
                    if (m_emitter.Event != current)
                    {
                        m_emitter.Event = current;
                        m_emitter.Lookup();
                    }
                    m_emitter.Play();
                    m_emitter.AllowFadeout = false;
                }
                else
                {
                    Debug.Log("No song data available for time: " + DaytimeCycle_Update.Instance.m_timeOfDay.ToString(), this.gameObject);
                }
                m_currentRandomTime = Random.Range(m_minTimeBetweenTracks, m_maxTimeBetweenTracks);
            }
        }
    }


    public string GetCurrentSongType()
    {
        float currentTime = DaytimeCycle_Update.Instance.m_timeOfDay;

        foreach (DaytimeBasedSongs data in m_daytimeBasedSongs)
        {
            if (data.m_minDaytime < data.m_maxDaytime)
            {
                if (currentTime >= data.m_minDaytime && currentTime <= data.m_maxDaytime)
                {
                    return data.m_daytimeBasedSongEvent;
                }
            }
            else
            {
                if (currentTime > data.m_minDaytime || currentTime < data.m_maxDaytime)
                {
                    return data.m_daytimeBasedSongEvent;
                }
            }
        }

        return "oof";
    }

    public void ChangeSong(MusicPlayer_Trigger.MusicTriggerType p_newSongType)
    {

        switch (p_newSongType)
        {
            case MusicPlayer_Trigger.MusicTriggerType.Exploration:
                StartCoroutine(FadeOutCurrentMusic(GetCurrentSongType(), p_newSongType));
                break;
            case MusicPlayer_Trigger.MusicTriggerType.Climbing:
                StartCoroutine(FadeOutCurrentMusic(m_climbingSongEvent, p_newSongType));
                break;
            case MusicPlayer_Trigger.MusicTriggerType.Summit:
                StartCoroutine(FadeOutCurrentMusic(m_summitSongEvent, p_newSongType));
                break;
        }

    }


    private IEnumerator FadeOutCurrentMusic(string p_currentEvent, MusicPlayer_Trigger.MusicTriggerType p_newSongType)
    {
        bool playing = false;

        switch (p_newSongType)
        {
            case MusicPlayer_Trigger.MusicTriggerType.Exploration:
                break;
            case MusicPlayer_Trigger.MusicTriggerType.Climbing:
                m_climbingSongPlaying = true;
                break;
            case MusicPlayer_Trigger.MusicTriggerType.Summit:
                m_summitSongPlaying = true;
                break;
        }

        m_currentMusicType = p_newSongType;

        m_summitSongPlaying = true;


        if (m_emitter.IsPlaying())
        {
            playing = true;
            m_emitter.AllowFadeout = true;
            m_emitter.Stop();
        }


        while (playing)
        {
            yield return null;
            if (!m_emitter.IsPlaying())
            {
                playing = false;
            }
        }
        m_emitter.Event = p_currentEvent;
        m_emitter.Lookup();
        m_emitter.Play();



        switch (p_newSongType)
        {
            case MusicPlayer_Trigger.MusicTriggerType.Exploration:
                m_climbingSongPlaying = false;
                m_summitSongPlaying = false;
                m_currentTimer = 0;
                break;
            case MusicPlayer_Trigger.MusicTriggerType.Climbing:
                m_climbingSongPlaying = true;
                m_summitSongPlaying = false;
                break;
            case MusicPlayer_Trigger.MusicTriggerType.Summit:
                m_summitSongPlaying = true;
                m_climbingSongPlaying = false;
                break;
        }
    }
}
