using System.Collections;
using UnityEngine;

public class GlobalSceneManager : MonoBehaviour
{
    public static GlobalSceneManager Instance;
    public float m_fadeTime;
    public CanvasGroup m_fadingCanvas;
    private bool m_inTransition;

    public bool m_performAnimationOnSceneChange;

    public bool m_fadeInOnStart = true;

    FMOD.Studio.Bus m_ambience;
    FMOD.Studio.Bus m_soundEffects;
    private void Start()
    {
        Instance = this;

        m_ambience = FMODUnity.RuntimeManager.GetBus("bus:/Master/Ambience");
        m_soundEffects = FMODUnity.RuntimeManager.GetBus("bus:/Master/SoundEffects");
        if (m_fadeInOnStart)
        {
            StartCoroutine(FadeAnimation(false));
        }
    }
    public void LoadNewScene(int p_sceneIndex, bool p_performAnim)
    {
        if (m_inTransition) return;
        m_inTransition = true;
        if (!p_performAnim)
        {
            ChangeScenes(p_sceneIndex);
        }
        else
        {

            StartCoroutine(ChangeScene(p_sceneIndex));
        }
    }
    private void ChangeScenes(int p_newScene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(p_newScene);
    }

    public IEnumerator FadeAnimation(bool p_fadeToBlack)
    {
        float timer = 0;
        while (timer < m_fadeTime)
        {
            timer += Time.deltaTime;
            m_fadingCanvas.alpha = p_fadeToBlack? (timer / m_fadeTime) : (1 - (timer/m_fadeTime));
            m_ambience.setVolume((p_fadeToBlack) ? 1 - (timer / m_fadeTime) : (timer / m_fadeTime));
            m_soundEffects.setVolume((p_fadeToBlack) ? 1 - (timer / m_fadeTime) : (timer / m_fadeTime));

            yield return null;
        }
        
    }

    private IEnumerator ChangeScene(int p_newScene)
    {
        yield return StartCoroutine(FadeAnimation(true));
        ChangeScenes(p_newScene);
    }
    public void ReloadScene()
    {
        LoadNewScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex, m_performAnimationOnSceneChange);
    }
}
