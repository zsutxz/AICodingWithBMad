using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDuration = 0.5f;

    private static TransitionManager instance;

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

    public static void StartTransition(string sceneName)
    {
        if (instance != null)
        {
            instance.StartCoroutine(instance.FadeAndLoad(sceneName));
        }
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        // Fade in
        fadePanel.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(1f));


        // Load scene
        SceneManager.LoadScene(sceneName);

        // Wait for scene to load
        yield return new WaitForEndOfFrame();

        yield return StartCoroutine(Fade(0f));


        // Fade out
        yield return new WaitForSeconds(0.1f);
        fadePanel.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadePanel.color.a;
        float currentTime = 0f;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, currentTime / fadeDuration);
            fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, alpha);
            yield return null;
        }
    }
}