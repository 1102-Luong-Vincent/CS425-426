using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FadeTransition : MonoBehaviour
{
    public static FadeTransition Instance;
    public Image fadeImage;
    public float fadeSpeed = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    IEnumerator FadeIn()
    {
        float alpha = 1f;
        Color color = fadeImage.color;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }
        color.a = 0f;
        fadeImage.color = color;
    }

    IEnumerator FadeOut(string sceneName)
    {
        float alpha = 0f;
        Color color = fadeImage.color;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }
        color.a = 1f;
        fadeImage.color = color;

        SceneManager.LoadScene(sceneName);
        StartCoroutine(FadeIn());
    }
}
