using UnityEngine;

public static class SoundManagerConstants
{
    public const string BackgroundPath = "Sound/BGM/";
    public const string SoundEffectPath = "Sound/SFX/";
    public const string MainMenuMusicName = "Alternative Menu sound";
    public const string GameplayMusic = "background tension music";
    public const string PauseScreenMusic = "enigma horror sound";
    public const string BattleMusic = "enigma horror sound";

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
    public void SetBackgroundVolume(float volume)  // make sure only setting Value use it , other mean only 1 reference form SettingValue;
    {
        backgroundMusic.volume = Mathf.Clamp01(volume);
    }

    public void PlayBackgroundMusic(string fileName, bool loop = true)
    {
        string path = SoundManagerConstants.BackgroundPath + fileName;
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
    public void SetSoundEffectVolume(float volume) // make sure only setting Value use it , other mean only 1 reference form SettingValue;
    {
        soundEffect.volume = Mathf.Clamp01(volume);
    }

    public void PlaySoundEffect(string fileName,bool loop = false)
    {
        string path = SoundManagerConstants.SoundEffectPath + fileName;
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
