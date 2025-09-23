using UnityEngine;

public static class SoundPath
{
    public const string BackgroundPath = "Sound/Background Music/";
    public const string SoundEffectPath = "Sound/Sound Effect/";
}


public class SoundManage : MonoBehaviour
{
    public static SoundManage Instance;

    public AudioSource backgroundMusic;
    public AudioSource soundEffect;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    #region Background
    public void SetBackgroundVolume(float volume)
    {
        backgroundMusic.volume = Mathf.Clamp01(volume);
    }



    public void PlayBackgroundMusic(string fileName, bool loop = true)
    {
        string path = SoundPath.BackgroundPath + fileName;
        AudioClip clip = Resources.Load<AudioClip>(path);
        PlayBackgroundMusic(clip, loop);
    }

    public void PlayBackgroundMusic(AudioClip clip = null, bool loop = true)
    {
        if (clip != null) backgroundMusic.clip = clip;
        if (backgroundMusic.clip == null) return;

        backgroundMusic.loop = loop;

        if (!backgroundMusic.isPlaying) backgroundMusic.Play();
    }


    public void StopBackgroundMusic()
    {
        if (backgroundMusic.isPlaying) backgroundMusic.Stop();
    }

    #endregion


    #region SoundEffect
    public void SetSoundEffectVolume(float volume)
    {
        soundEffect.volume = Mathf.Clamp01(volume);
    }

    public void PlaySoundEffect(string fileName,bool loop = false)
    {
        string path = SoundPath.SoundEffectPath + fileName;
        AudioClip clip = Resources.Load<AudioClip>(path);
        PlaySoundEffect(clip,loop);
    }


    public void PlaySoundEffect(AudioClip clip = null,bool loop = false)
    {
        if (clip != null) soundEffect.clip = clip;
        if (soundEffect.clip == null) return;
        soundEffect.Play();
    }

    public void StopSoundEffect()
    {
        if (soundEffect.isPlaying) soundEffect.Stop();
    }

    #endregion

}
