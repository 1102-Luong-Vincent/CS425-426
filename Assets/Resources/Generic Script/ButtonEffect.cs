using UnityEngine;
using UnityEngine.UI;
using System;

public static class ButtonEffect
{
    public static void AddButtonEffect(Button button, AudioClip clip, Action onClickAction, bool loop = false)
    {
        if (button == null) return;

        button.onClick.AddListener(() =>
        {
            if (clip != null)
            {
                SoundManage.Instance.PlaySoundEffect(clip, loop);
            }

            onClickAction?.Invoke();
        });
    }

    public static void AddButtonEffect(Button button, string clipFileName, Action onClickAction, bool loop = false)
    {
        if (button == null) return;

        button.onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(clipFileName))
            {
                string path = SoundPath.SoundEffectPath + clipFileName;
                AudioClip clip = Resources.Load<AudioClip>(path);
                SoundManage.Instance.PlaySoundEffect(clip, loop);
            }

            onClickAction?.Invoke();
        });
    }
}
