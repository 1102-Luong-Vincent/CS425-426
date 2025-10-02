using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class SettingData
{
    public SaveData saveData = null;
    public float backgroundVolume = 0.7f;
    public float soundEffectsVolume = 0.7f;
    public ScreenType screenType = ScreenType.Full;
    public LanguageType language = LanguageType.English;
}

public class SettingValue : MonoBehaviour
{
    public static SettingValue Instance;

    private string savePath;
    private SettingData settingData = new SettingData();

    public event Action<float> OnBackgroundVolumeChanged;
    public event Action<float> OnSoundEffectVolumeChanged;
    public event Action<ScreenType> OnScreenTypeChanged;
    public event Action<LanguageType> OnLanguageChanged;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "settings.json");

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
        InitSettings();
    }

    public void InitSettings()
    {
        SetBackgroundVolume(settingData.backgroundVolume);
        SetSoundEffectVolume(settingData.soundEffectsVolume);
        SetScreenType(settingData.screenType);
        SetLanguage(settingData.language);
    }

    public void SaveSettings()
    {
        SaveCurrentGameValue();
        string json = JsonUtility.ToJson(settingData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Settings saved to " + savePath);

    }

    public void LoadSettings()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            settingData = JsonUtility.FromJson<SettingData>(json);
            Debug.Log("Settings loaded from " + savePath);
        }
        else
        {
            SaveSettings();
            Debug.Log("No settings found, created default at " + savePath);
        }
    }

    void SaveCurrentGameValue()
    {
        if (GameValue.Instance.GetCurrentScence() == SceneType.None) return;
        settingData.saveData = new SaveData(savePath, GameValue.Instance);
    }

    public SettingData GetSettingData()
    {
        return settingData;
    }

    #region Sound
    public void SetBackgroundVolume(float value)
    {
        if (Mathf.Approximately(settingData.backgroundVolume, value)) return;
        settingData.backgroundVolume = value;
        OnBackgroundVolumeChanged?.Invoke(value);
        SoundManage.Instance.SetBackgroundVolume(value);
    }

    public void SetSoundEffectVolume(float value)
    {
        if (Mathf.Approximately(settingData.soundEffectsVolume, value)) return;
        settingData.soundEffectsVolume = value;
        OnSoundEffectVolumeChanged?.Invoke(value);
        SoundManage.Instance.SetSoundEffectVolume(value);
    }
    #endregion

    #region Language
    public void SetLanguage(LanguageType lang)
    {
        if (settingData.language == lang) return;
        settingData.language = lang;
        OnLanguageChanged?.Invoke(lang);
        ApplyLanguage(lang);
    }

    private void ApplyLanguage(LanguageType lang)
    {
        switch (lang)
        {
            case LanguageType.English:
                Debug.Log("Switch to English");
                break;
        }
    }
    #endregion

    #region Screen
    public void SetScreenType(ScreenType type)
    {
        if (settingData.screenType == type) return;
        settingData.screenType = type;
        OnScreenTypeChanged?.Invoke(type);
        ApplyScreenType(type);
    }

    private void ApplyScreenType(ScreenType type)
    {
        switch (type)
        {
            case ScreenType.Full:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case ScreenType.Window:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }
    #endregion

    private void OnDestroy()
    {
        SaveSettings();
    }
}
