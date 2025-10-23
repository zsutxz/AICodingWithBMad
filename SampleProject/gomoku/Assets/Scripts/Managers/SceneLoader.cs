using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void LoadScene(string sceneName)
    {
        if (instance != null)
        {
            instance.LoadSceneInternal(sceneName);
        }
    }

    private bool isTransitioning = false;
    private CanvasGroup fadePanel;

    void Start()
    {
        // Get or create fade panel for transitions
        fadePanel = FindObjectOfType<CanvasGroup>();
        if (fadePanel == null)
        {
            GameObject panel = new GameObject("FadePanel");
            fadePanel = panel.AddComponent<CanvasGroup>();
            panel.AddComponent<RectTransform>();
            panel.AddComponent<Image>().color = Color.black;
        }
        fadePanel.alpha = 1f;
        StartCoroutine(FadeOut());
    }

    private void LoadSceneInternal(string sceneName)
    {
        if (isTransitioning) return;
        StartCoroutine(LoadSceneWithTransition(sceneName));
    }

    private IEnumerator LoadSceneWithTransition(string sceneName)
    {
        isTransitioning = true;

        // Fade out
        yield return StartCoroutine(FadeOut());

        // Load scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade in
        yield return StartCoroutine(FadeIn());

        isTransitioning = false;
    }

    private IEnumerator FadeOut(float duration = 0.5f)
    {
        while (fadePanel.alpha < 1f)
        {
            fadePanel.alpha += Time.deltaTime / duration;
            yield return null;
        }
        fadePanel.alpha = 1f;
    }

    private IEnumerator FadeIn(float duration = 0.5f)
    {
        while (fadePanel.alpha > 0f)
        {
            fadePanel.alpha -= Time.deltaTime / duration;
            yield return null;
        }
        fadePanel.alpha = 0f;
    }
}