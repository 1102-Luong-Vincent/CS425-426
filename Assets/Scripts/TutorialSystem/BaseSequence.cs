using SmallScaleInc.TopDownPixelCharactersPack1;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public abstract class BaseSequence : MonoBehaviour
{
    public bool SkipSequence = false;

    public abstract IEnumerator RunSequence();
    public IEnumerator FadePanelIn(string PanelName)
    {
        TutorialPanel panel = GameObject.Find(PanelName).GetComponent<TutorialPanel>();
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * SequenceManager.Instance.fader.fadeSpeed;
            panel.SetColor(new Color(1f, 1f, 1f, alpha));
            yield return null;
        }
        yield return null;
    }

    public IEnumerator FadePanelOut(string PanelName, bool DestroyOnFade)
    {
        TutorialPanel panel = GameObject.Find(PanelName).GetComponent<TutorialPanel>();
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * SequenceManager.Instance.fader.fadeSpeed;
            panel.SetColor(new Color(1f, 1f, 1f, alpha));
            yield return null;
        }
        if(DestroyOnFade)
            Destroy(panel.gameObject);
        yield return null;
    }

    public IEnumerator LerpTransform(Transform T, Vector3 src, Vector3 dest, float eventTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < eventTime && T != null)
        {
            T.position = Vector3.Lerp(src, dest, elapsedTime / eventTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator RefreshScene()
    {
        // This is a workaround to ensure that all objects in the scene are properly initialized before the sequence starts.
        // It forces a frame to pass, allowing all Start() methods to run.
        yield return null;
        GameValue.Instance.LoadSceneByEnum(SceneType.GameStartScene);
    }
}
