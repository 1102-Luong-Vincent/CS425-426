using UnityEngine;
using UnityEngine.UI;
using static ButtonEffect;

public class MainMenuManage : MonoBehaviour
{
    public static MainMenuManage Instance;

    [Header("Buttons")]
    public Buttons buttons;

    [System.Serializable]
    public class Buttons
    {
        public Button ContinueButton;
        public Button StartButton;
        public Button LoadButton;
        public Button OptionButton;
        public Button ExitButton;
    }
    public OptionPanelControl OptionPanelControl;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PlayerBackgroundMusic();
        InitButtons();
    }


    void PlayerBackgroundMusic()
    {
        SoundManage.Instance.PlayBackgroundMusic(SoundManagerConstants.MainMenuMusicName);
    }


    void InitButtons()
    {
           
        OnMainMenuButtonClick(buttons.ContinueButton, OnContinueButtonClick);
        buttons.ContinueButton.gameObject.SetActive(!SettingValue.Instance.GetSettingData().saveData.IsEmpty());

        OnMainMenuButtonClick(buttons.StartButton, OnStarButtonClick);
        OnMainMenuButtonClick(buttons.LoadButton, OnLoadButtonClick);
        OnMainMenuButtonClick(buttons.OptionButton, OnOptionButtonClick);
        OnMainMenuButtonClick(buttons.ExitButton, OnExitButtonClick);
    }

    public void SetMainMenuButtons(bool isActive)
    {
        buttons.ContinueButton.gameObject.SetActive(isActive && !SettingValue.Instance.GetSettingData().saveData.IsEmpty());
        buttons.StartButton.gameObject.SetActive(isActive);
        buttons.LoadButton.gameObject.SetActive(isActive);
        buttons.OptionButton.gameObject.SetActive(isActive);
        buttons.ExitButton.gameObject.SetActive(isActive);
    }
    void OnContinueButtonClick()
    {
        GameValue.Instance.SetSaveData(SettingValue.Instance.GetSettingData().saveData);
    }

    void OnStarButtonClick()
    {
        Debug.Log("Remember to initialize GameValue");
        GameValue.Instance.SetHappendStoryName(StoryName.Prologue);
       GameValue.Instance.LoadSceneByEnum(SceneType.StoryScene);
    //    GameValue.Instance.LoadSceneByEnum(SceneType.BattleScene);
    }

    void OnLoadButtonClick()
    {
        SaveLoadPanelControl.Instance.ShowPanel();
    }

    void OnOptionButtonClick()
    {
        SetMainMenuButtons(false);
        OptionPanelControl.SetOptionPanelActive(true);
    }
    void OnExitButtonClick()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; 
        #else
            Application.Quit(); // ????????
        #endif
    }
}
