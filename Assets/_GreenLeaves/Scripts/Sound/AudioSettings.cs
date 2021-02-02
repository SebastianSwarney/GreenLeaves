using UnityEngine;

public class AudioSettings : MonoBehaviour
{

    public UnityEngine.UI.Slider m_masterSlider;
    public UnityEngine.UI.Slider m_musicSlider;
    public UnityEngine.UI.Slider m_ambienceSlider;
    public UnityEngine.UI.Slider m_soundEffectsSlider;

    FMOD.Studio.Bus m_masterBus;
    FMOD.Studio.Bus m_music;
    FMOD.Studio.Bus m_ambience;
    FMOD.Studio.Bus m_soundEffects;

    // Start is called before the first frame update
    void Start()
    {
        m_masterBus = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        m_music = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
        m_ambience = FMODUnity.RuntimeManager.GetBus("bus:/Master/Ambience");
        m_soundEffects = FMODUnity.RuntimeManager.GetBus("bus:/Master/SoundEffects");

        float vol;
        m_masterBus.getVolume(out vol);
        m_masterSlider.value = vol;

        m_music.getVolume(out vol);
        m_musicSlider.value = vol;

        m_ambience.getVolume(out vol);
        m_ambienceSlider.value = vol;

        m_soundEffects.getVolume(out vol);
        m_soundEffectsSlider.value = vol;
    }

    public void UpdateMasterVolume(float p_newAmount)
    {
        m_masterBus.setVolume(p_newAmount);
    }
    public void UpdateMusicVolume(float p_volume)
    {
        m_music.setVolume(p_volume);
    }
    public void UpdateAmbienceVolume(float p_newAmount)
    {
        m_ambience.setVolume(p_newAmount);
    }
    public void UpdateSoundEffectsVolume(float p_newAmount)
    {
        m_soundEffects.setVolume(p_newAmount);
    }

}
