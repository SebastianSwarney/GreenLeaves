using System.Collections;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{
    public static ScreenshotManager Instance;
    public Camera m_screenshotCamera;
    private bool m_takeScreenshot;

    public bool m_canTakeImage = true;

    public float m_delayTime;
    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!m_canTakeImage) return;
        if (Input.GetKeyDown(KeyCode.O))
        {
            OpenPath();
        }else if (Input.GetMouseButtonDown(1))
        {
            TakeScreenshot();
        }

    }
    public void OpenPath()
    {
        string itemPath = Application.persistentDataPath;
        itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
        System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
    }
    public void TakeScreenshot()
    {

        m_canTakeImage = false;

        m_screenshotCamera.targetTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);
        m_takeScreenshot = true;
        gameObject.SetActive(true);
        m_screenshotCamera.gameObject.SetActive(true);
        StartCoroutine(ScreenshotBuffer());
    }

    public void EnableCamera()
    {
        m_canTakeImage = false;
        gameObject.SetActive(true);
        StartCoroutine(ScreenshotBuffer());
    }

    public void DisableCamera()
    {
        StopAllCoroutines();
        m_canTakeImage = false;
        m_takeScreenshot = false;
        gameObject.SetActive(false);
    }


    private void OnPostRender()
    {
        if (!m_takeScreenshot) return;
        m_takeScreenshot = false;
        RenderTexture tempText = m_screenshotCamera.targetTexture;
        Texture2D renderResult = new Texture2D(tempText.width, tempText.height);
        Rect rect = new Rect(0, 0, tempText.width, tempText.height);
        renderResult.ReadPixels(rect, 0, 0);

        byte[] array = renderResult.EncodeToPNG();

        System.DateTime currentTime = System.DateTime.Now;
        //+ "/" + (currentTime.Month + "_" + currentTime.Day +"_" + currentTime.Year+"_" + currentTime.TimeOfDay) + ".png"
        System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + (currentTime.Month + "-" + currentTime.Day + "-" + currentTime.Year + " " + currentTime.TimeOfDay.Hours + "." + currentTime.TimeOfDay.Minutes + "." + currentTime.TimeOfDay.Seconds) + ".png", array);


        Debug.Log("Screenshot Taken");
        RenderTexture.ReleaseTemporary(tempText);
        m_screenshotCamera.targetTexture = null;
    }

    private IEnumerator ScreenshotBuffer()
    {
        yield return new WaitForSeconds(m_delayTime);
        m_canTakeImage = true;
    }
}
