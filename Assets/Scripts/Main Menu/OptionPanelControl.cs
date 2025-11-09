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

    private List<Button> optionButtons;

    private Dictionary<PanelType, GameObject> panels = new Dictionary<PanelType, GameObject>();
    void Start()
    {
        optionButtons = new List<Button>() { VolumeButton, DisplayButton, LanguageButton };

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
        //OnMainMenuButtonClick(VolumeButton, () => OpenPanel(PanelType.Volume), false);
        //OnMainMenuButtonClick(DisplayButton, () => OpenPanel(PanelType.Display), false);
        //OnMainMenuButtonClick(LanguageButton, () => OpenPanel(PanelType.Language), false);
        OnMainMenuButtonClick(VolumeButton, () => OpenSubPanel(PanelType.Volume), false);
        OnMainMenuButtonClick(DisplayButton, () => OpenSubPanel(PanelType.Display), false);
        OnMainMenuButtonClick(LanguageButton, () => OpenSubPanel(PanelType.Language), false);
        //OnMainMenuButtonClick(BackButton, () => SetOptionPanelActive(false), false);
        OnMainMenuButtonClick(BackButton, () => SetOptionsPanel(false), false);
    }

    public void SetOptionPanelActive(bool isActive)
    {
        CloseAllPanels();
        gameObject.SetActive(isActive);
        SetOptionButtonsActive(true);

    }
    void SetOptionButtonsActive(bool isActive)
    {
        foreach (var btn in optionButtons)
            btn.gameObject.SetActive(isActive);
    }
    void OpenSubPanel(PanelType key)
    {
        CloseAllPanels();
        SetOptionButtonsActive(false); // hide option buttons when a sub-panel opens
        SetOptionsBackButtonActive(false); // show back button
        if (panels.ContainsKey(key))
            panels[key].SetActive(true);
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
        //OnMainMenuButtonClick(volumePanelControl.BackButton, () => ClosePanel(PanelType.Volume), false);
        OnMainMenuButtonClick(volumePanelControl.BackButton, () => CloseSubPanel(PanelType.Volume), false);
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
        //OnMainMenuButtonClick(displayPanelControl.BackButton, () => ClosePanel(PanelType.Display), false);
        OnMainMenuButtonClick(displayPanelControl.BackButton, () => CloseSubPanel(PanelType.Display), false);
        SettingValue.Instance.OnScreenTypeChanged += UpdateScreenText;

    }

    void OnDisplayScreenButtonClick(int offset)
    {
        ScreenType screenType = GetEnumByOffset(SettingValue.Instance.GetSettingData().screenType, offset);
        SettingValue.Instance.SetScreenType(screenType);
    }

    void UpdateScreenText(ScreenType type)
    {
        if (displayPanelControl.ScreenTypeText == null) return;
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
        //OnMainMenuButtonClick(languagePanelControl.BackButton, () => ClosePanel(PanelType.Language), false);
        OnMainMenuButtonClick(languagePanelControl.BackButton, () => CloseSubPanel(PanelType.Language), false);

        SettingValue.Instance.OnLanguageChanged += UpdateLanguageText;
    }

    void OnLanguageButtonClick(int offset)
    {
        LanguageType languageType = GetEnumByOffset(SettingValue.Instance.GetSettingData().language, offset);
        SettingValue.Instance.SetLanguage(languageType);
    }

    void UpdateLanguageText(LanguageType languageType)
    {
        if (languagePanelControl.LanguageText == null) return;
       languagePanelControl.LanguageText.text = SettingValue.Instance.GetSettingData().language.ToString();
    }


    #endregion
    #region Helper Function

    void OpenPanel(PanelType key)
    {
        CloseAllPanels();
        if (panels.ContainsKey(key))
            panels[key].SetActive(true);
    }
    void CloseSubPanel(PanelType key)
    {
        if (panels.ContainsKey(key))
            panels[key].SetActive(false);

        SetOptionsPanel(true); // show option buttons again
        SetOptionButtonsActive(true);         // show Volume/Display/Language buttons
        SetOptionsBackButtonActive(true);     // show Options Back button again
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

    void SetOptionsPanel(bool isActive)
    {
        //CloseAllPanels();
        gameObject.SetActive(isActive);

        // Show main menu buttons if options panel is closed
        if (!isActive && MainMenuManage.Instance != null)
        {
            MainMenuManage.Instance.SetMainMenuButtons(true);
        }

        if(isActive){
            SetOptionButtonsActive(true);
        }
    }
    void SetOptionsBackButtonActive(bool isActive)
    {
        if (BackButton != null)
            BackButton.gameObject.SetActive(isActive);
    }
}
