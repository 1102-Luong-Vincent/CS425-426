using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static EnumHelper;
using static ButtonEffect;
public enum LanguageType
{
    English,Chinese,Japanese
}

public enum ScreenType
{
    Full, Window
}

public enum PanelType
{
   None,Volume,Display, Language
}

public class OptionPanelControl : MonoBehaviour
{
    public Button VolumeButton;
    public Button DisplayButton;
    public Button LanguageButton;
    public Button BackButton;

    private Dictionary<PanelType, GameObject> panels = new Dictionary<PanelType, GameObject>();

    void Start()
    {
        InitOptionPanel();
        InitOptionPanelButton();
        CloseAllPanels();
        InitVolumePanel();
        InitDisplayPanel();
        InitLanguagePanel();
    }
    #region Option
    void InitOptionPanel()
    {
        panels[PanelType.Volume] = volumePanelControl.VolumePanel;
        panels[PanelType.Display] = displayPanelControl.DisplayPanel;
        panels[PanelType.Language] = languagePanelControl.LanguagePanel;
        SetOptionPanelActive(false);
    }
    void InitOptionPanelButton()
    {
        OnMainMenuButtonClick(VolumeButton, () => OpenPanel(PanelType.Volume), false);
        OnMainMenuButtonClick(DisplayButton, () => OpenPanel(PanelType.Display), false);
        OnMainMenuButtonClick(LanguageButton, () => OpenPanel(PanelType.Language), false);
        OnMainMenuButtonClick(BackButton, () => SetOptionPanelActive(false), false);
    }

    public void SetOptionPanelActive(bool isActive)
    {
        CloseAllPanels();
        gameObject.SetActive(isActive);

    }


    #endregion
    #region Volume

    [Header("VolumePanelControl")]
    public VolumePanelControl volumePanelControl;

    [System.Serializable]
    public class VolumePanelControl
    {
        public GameObject VolumePanel;
        public Slider BackgroundVolumeSlider;
        public Slider SoundEffectsVolumeSlider;
        public Button BackButton;
    }


    void InitVolumePanel()
    {
        volumePanelControl.BackgroundVolumeSlider.onValueChanged.AddListener((value) => SettingValue.Instance.SetBackgroundVolume(value));
        volumePanelControl.SoundEffectsVolumeSlider.onValueChanged.AddListener((value) => SettingValue.Instance.SetSoundEffectVolume(value));
        OnMainMenuButtonClick(volumePanelControl.BackButton, () => ClosePanel(PanelType.Volume), false);
        volumePanelControl.BackgroundVolumeSlider.value = SettingValue.Instance.GetSettingData().backgroundVolume;
        volumePanelControl.SoundEffectsVolumeSlider.value = SettingValue.Instance.GetSettingData().soundEffectsVolume;
    }

    #endregion
    #region Display
    [Header("DisplayPanelControl")]
    public DisplayPanelControl displayPanelControl;

    [System.Serializable]
    public class DisplayPanelControl
    {
        public GameObject DisplayPanel;
        public Button LeftButton;
        public TextMeshProUGUI ScreenTypeText;
        public Button RightButton;
        public Button BackButton;
    }


    void InitDisplayPanel()
    {
        UpdateScreenText(SettingValue.Instance.GetSettingData().screenType);

        OnMainMenuButtonClick(displayPanelControl.LeftButton, () => OnDisplayScreenButtonClick(-1), false);
        OnMainMenuButtonClick(displayPanelControl.RightButton, () => OnDisplayScreenButtonClick(+1), false);
        OnMainMenuButtonClick(displayPanelControl.BackButton, () => ClosePanel(PanelType.Display), false);

        SettingValue.Instance.OnScreenTypeChanged += UpdateScreenText;

    }

    void OnDisplayScreenButtonClick(int offset)
    {
        ScreenType screenType = GetEnumByOffset(SettingValue.Instance.GetSettingData().screenType, offset);
        SettingValue.Instance.SetScreenType(screenType);
    }

    void UpdateScreenText(ScreenType type)
    {
        displayPanelControl.ScreenTypeText.text = SettingValue.Instance.GetSettingData().screenType.ToString();
    }


    #endregion
    #region Language

    [Header("LanguagePanelControl")]
    public LanguagePanelControl languagePanelControl;

    [System.Serializable]
    public class LanguagePanelControl
    {
        public GameObject LanguagePanel;
        public Button LeftButton;
        public TextMeshProUGUI LanguageText;
        public Button RightButton;
        public Button BackButton;

    }
    void InitLanguagePanel()
    {
        UpdateLanguageText(SettingValue.Instance.GetSettingData().language);


        OnMainMenuButtonClick(languagePanelControl.LeftButton, () => OnLanguageButtonClick(-1), false);
        OnMainMenuButtonClick(languagePanelControl.RightButton, () => OnLanguageButtonClick(+1), false);
        OnMainMenuButtonClick(languagePanelControl.BackButton, () => ClosePanel(PanelType.Language), false);
        
        SettingValue.Instance.OnLanguageChanged += UpdateLanguageText;
    }

    void OnLanguageButtonClick(int offset)
    {
        LanguageType languageType = GetEnumByOffset(SettingValue.Instance.GetSettingData().language, offset);
        SettingValue.Instance.SetLanguage(languageType);
    }

    void UpdateLanguageText(LanguageType languageType)
    {
       languagePanelControl.LanguageText.text = SettingValue.Instance.GetSettingData().language.ToString();
    }


    #endregion
    #region Helper Function


    public static void OnMainMenuButtonClick(Button button, Action onClickAction, bool loop = false)
    {
        AddButtonEffect(button, "TestClick", onClickAction, loop);
    }


    void OpenPanel(PanelType key)
    {
        CloseAllPanels();
        if (panels.ContainsKey(key))
            panels[key].SetActive(true);
    }

    void ClosePanel(PanelType key)
    {
        if (panels.ContainsKey(key))
            panels[key].SetActive(false);
    }

    void CloseAllPanels()
    {
        foreach (var panel in panels.Values)
            panel.SetActive(false);
    }

    #endregion

    void OnDestroy()
    {
        if (SettingValue.Instance != null)
        {
            SettingValue.Instance.OnScreenTypeChanged -= UpdateScreenText;
            SettingValue.Instance.OnLanguageChanged -= UpdateLanguageText;
        }
    }

}
