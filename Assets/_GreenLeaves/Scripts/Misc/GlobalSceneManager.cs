using System.Collections;
using UnityEngine;

public class GlobalSceneManager : MonoBehaviour
{
    public static GlobalSceneManager Instance;
    public float m_fadeTime;
    public CanvasGroup m_fadingCanvas;
    private bool m_inTransition;

    public bool m_performAnimationOnSceneChange;
    private void Start()
    {
        Instance = this;
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
            
            StartCoroutine(FadeAnimation(p_sceneIndex));
        }
    }
    private void ChangeScenes(int p_newScene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(p_newScene);
    }

    private IEnumerator FadeAnimation(int p_newScene)
    {
        float timer = 0;
        while(timer < m_fadeTime)
        {
            timer += Time.deltaTime;
            m_fadingCanvas.alpha = timer / m_fadeTime;
            yield return null;
        }
        ChangeScenes(p_newScene);
    }

    public void ReloadScene()
    {
        LoadNewScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex, m_performAnimationOnSceneChange);
    }
}
