// Authors: Vincent Luong and Shawn Meng
// Created by: Shawn Meng
// Modified by: Vincent Luong
// No external source was used.

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public static class SoundManagerConstants
{
    public const string BackgroundPath = "Sound/BGM/";
    public const string SoundEffectPath = "Sound/SFX/";
    public const string MainMenuMusicName = "danger and resolution";
    public const string GameplayMusic = "intense-horror-game-ambience";
    public const string GameplayMusic_Hospital = "thevoid";
    public const string GamePlayMusic_Level2 = "trenox8-frozen-nightmare-305204";
    public const string PauseScreenMusic = "enigma horror sound";
    public const string BattleMusic = "silent-escape-survival-thriller";
    public const string Mini_BossMusic = "Mini_Boss";
    public const string Final_BossMusic = "Final_Boss";
    public const string First_BossMusic = "First_Boss";
    public const string FootstepsSound = "Footsteps_Walking";
    public const int FirstBossEnemyId = 4;
    public const int IsaacFinalBossEnemyId = 5;
    public const int TrevorMiniBossEnemyId = 6;
}


public class SoundManage : MonoBehaviour
{
    public static SoundManage Instance;

    public AudioSource backgroundMusic;
    public AudioSource soundEffect;
    public AudioSource playerSoundEffect;

    public string currentMusic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void Start()
    {
        ForceSceneMusic(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ForceSceneMusic(scene.name);
        StopFootSteps();
    }

    #region Background
    public void SetBackgroundVolume(float volume)  // make sure only setting Value use it , other mean only 1 reference form SettingValue;
    {
        if (backgroundMusic == null)
        {
            Debug.LogWarning("[SoundManage] Background AudioSource is not assigned.");
            return;
        }

        backgroundMusic.volume = Mathf.Clamp01(volume);
    }

    public void PlayBackgroundMusic(string fileName, bool loop = true)
    {

        // prevent restarting same music repeatedly
        if (backgroundMusic.clip != null && currentMusic == fileName && backgroundMusic.isPlaying)
        {
            return;
        }

        currentMusic = fileName;

        string path = SoundManagerConstants.BackgroundPath + fileName;
        AudioClip clip = Resources.Load<AudioClip>(path);
        //PlayBackgroundMusic(clip, loop);

        backgroundMusic.clip = clip;
        backgroundMusic.loop = loop;
        backgroundMusic.Play();
    }

    //public void PlayBackgroundMusic(AudioClip clip = null, bool loop = true)
    //{
    //    if (backgroundMusic == null)
    //    {
    //        Debug.LogWarning("[SoundManage] Background AudioSource is not assigned.");
    //        return;
    //    }

    //    if (clip != null) backgroundMusic.clip = clip;
    //    if (backgroundMusic.clip == null) return;

    //    backgroundMusic.loop = loop;

    //    if (!backgroundMusic.isPlaying) backgroundMusic.Play();
    //}


    public void StopBackgroundMusic()
    {
        if (backgroundMusic == null)
        {
            Debug.LogWarning("[SoundManage] Background AudioSource is not assigned.");
            return;
        }

        if (backgroundMusic.isPlaying) backgroundMusic.Stop();
    }

    public void PauseBackgroundMusic()
    {
        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            backgroundMusic.Pause();
        }
    }

    public void ResumeBackgroundMusic()
    {
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.UnPause();
        }
    }

    public void ForceSceneMusic(string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenuScene":
                PlayBackgroundMusic(SoundManagerConstants.MainMenuMusicName);
                break;

            case "StoryScene":
                StopBackgroundMusic();
                break;

            case "GameStartScene":
                StopBackgroundMusic();
                break;

            case "Level_1":
                PlayBackgroundMusic(SoundManagerConstants.GameplayMusic);
                break;

            case "Level1_Store":
                StopBackgroundMusic();
                break;

            case "Level1_Library_1":
                StopBackgroundMusic();
                break;

            case "Level1_Library_2":
                StopBackgroundMusic();
                break;

            case "Level_1_Hospital":
                PlayBackgroundMusic(SoundManagerConstants.GameplayMusic_Hospital);
                break;

            case "Level_2":
                PlayBackgroundMusic(SoundManagerConstants.GamePlayMusic_Level2);
                break;

            case "LV2_Dormitory":
                StopBackgroundMusic();
                break;

            case "LV2_Restaurant":
                StopBackgroundMusic();
                break;

            case "BattleScene":
                PlayBackgroundMusic(GetBattleSceneMusic());
                break;
        }
    }

    public string GetBattleSceneMusic()
    {
        BattleData battleData = GameValue.Instance != null ? GameValue.Instance.GetBattleData() : null;
        if (battleData?.battleEnemys == null)
        {
            return SoundManagerConstants.BattleMusic;
        }

        if (BattleContainsEnemy(battleData, SoundManagerConstants.IsaacFinalBossEnemyId))
        {
            return SoundManagerConstants.Final_BossMusic;
        }

        if (BattleContainsEnemy(battleData, SoundManagerConstants.TrevorMiniBossEnemyId))
        {
            return SoundManagerConstants.Mini_BossMusic;
        }

        if(BattleContainsEnemy(battleData, SoundManagerConstants.FirstBossEnemyId))
        {
            return SoundManagerConstants.First_BossMusic;
        }

        return SoundManagerConstants.BattleMusic;
    }

    public void RefreshCurrentSceneMusic()
    {
        ForceSceneMusic(SceneManager.GetActiveScene().name);
    }

    private bool BattleContainsEnemy(BattleData battleData, int enemyId)
    {
        foreach (EnemyValue enemy in battleData.battleEnemys)
        {
            if (enemy != null && enemy.GetID() == enemyId)
            {
                return true;
            }
        }

        return false;
    }
    #endregion


    #region SoundEffect
    public void SetSoundEffectVolume(float volume) // make sure only setting Value use it , other mean only 1 reference form SettingValue;
    {
        if (soundEffect == null)
        {
            Debug.LogWarning("[SoundManage] Sound effect AudioSource is not assigned.");
            return;
        }

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
        if (soundEffect == null)
        {
            Debug.LogWarning("[SoundManage] Sound effect AudioSource is not assigned.");
            return;
        }

        if (clip != null) soundEffect.clip = clip;
        if (soundEffect.clip == null) return;
        soundEffect.Play();
    }

    public void StopSoundEffect()
    {
        if (soundEffect == null)
        {
            Debug.LogWarning("[SoundManage] Sound effect AudioSource is not assigned.");
            return;
        }

        if (soundEffect.isPlaying) soundEffect.Stop();
    }

    #endregion

    #region Player Footsteps

    public void PlayFootSteps()
    {
        if(playerSoundEffect == null)
        {
            return;
        }

        if (playerSoundEffect.isPlaying)
        {
            return;
        }

        string path = SoundManagerConstants.SoundEffectPath + SoundManagerConstants.FootstepsSound;
        AudioClip clip = Resources.Load<AudioClip>(path);

        if(clip == null)
        {
            return;
        }

        playerSoundEffect.clip = clip;
        playerSoundEffect.loop = true;
        playerSoundEffect.Play();
    }

    public void StopFootSteps()
    {
        if(playerSoundEffect == null)
        {
            return;
        }
        if (playerSoundEffect.isPlaying)
        {
            playerSoundEffect.Stop();
        }
    }
    #endregion

}
