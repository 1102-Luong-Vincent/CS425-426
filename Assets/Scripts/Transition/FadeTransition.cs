using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using SmallScaleInc.ZombieRural;

public class FadeTransition : MonoBehaviour
{
    public Image fadeImage;
    public float fadeSpeed = 1f;
    private bool isFading = false;


    public void FadeToScene(SceneType scene, Vector3 pos)
    {
        if (isFading) return;
        StartCoroutine(FadeOut(scene, pos));
    }

    IEnumerator FadeIn()
    {
        isFading = true;

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

        isFading = false;
    }


    IEnumerator FadeOut(SceneType scene,Vector3 pos)
    {
        isFading = true;

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

        //SceneManager.LoadScene(sceneName);
        GameValue.Instance.LoadSceneByEnum(scene);
        GameValue.Instance.SetPlayerPosition(pos);


        yield return StartCoroutine(FadeIn());
    }
}
