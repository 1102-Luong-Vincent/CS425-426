// Authors: Vincent Luong and Shawn Meng
// Created by: Shawn Meng
// Modified by: Vincent Luong
// no external source was used

using UnityEngine;
using UnityEngine.UI;
using static ButtonEffect;
using System.IO;

public class MainMenuManage : MonoBehaviour
{
    public static MainMenuManage Instance;

    [Header("Buttons")]
    public Buttons buttons;

    [Header("Sound Effects")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip buttonClickSound;

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
        UpdateContinueButtonState();

        OnMainMenuButtonClick(buttons.StartButton, OnStarButtonClick);
        OnMainMenuButtonClick(buttons.LoadButton, OnLoadButtonClick);
        OnMainMenuButtonClick(buttons.OptionButton, OnOptionButtonClick);
        OnMainMenuButtonClick(buttons.ExitButton, OnExitButtonClick);
    }

    public void SetMainMenuButtons(bool isActive)
    {
        buttons.ContinueButton.gameObject.SetActive(isActive);
        if (isActive)
        {
            UpdateContinueButtonState();
        }

        buttons.StartButton.gameObject.SetActive(isActive);
        buttons.LoadButton.gameObject.SetActive(isActive);
        buttons.OptionButton.gameObject.SetActive(isActive);
        buttons.ExitButton.gameObject.SetActive(isActive);
    }
    void OnContinueButtonClick()
    {
        SaveData continueSave = GetLatestContinueSave();
        if (continueSave == null)
        {
            UpdateContinueButtonState();
            return;
        }

        audioSource.PlayOneShot(buttonClickSound);
        SequenceManager.CancelActiveTutorial();
        GameValue.Instance.SetSaveData(continueSave);
    }

    void OnStarButtonClick()
    {
        Debug.Log("Remember to initialize GameValue");
        audioSource.PlayOneShot(buttonClickSound);
        SequenceManager.CancelActiveTutorial();
        GameValue.Instance.ResetGameState();
        GameValue.Instance.ClearObjectiveProgress();
        GameValue.Instance.SetHappendStoryName(StoryName.Prologue);
        GameValue.Instance.SetCurrentObjective(ObjectiveConstants.CompletePrologue, false, false);
       GameValue.Instance.LoadSceneByEnum(SceneType.StoryScene);
    //    GameValue.Instance.LoadSceneByEnum(SceneType.BattleScene);
    }

    void OnLoadButtonClick()
    {
        audioSource.PlayOneShot(buttonClickSound);
        SaveLoadPanelControl.Instance.ShowLoadPanel();
    }

    void OnOptionButtonClick()
    {
        SetMainMenuButtons(false);
        OptionPanelControl.SetOptionPanelActive(true);
        audioSource.PlayOneShot(buttonClickSound);
    }
    void OnExitButtonClick()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); // ????????
#endif
        audioSource.PlayOneShot(buttonClickSound);
    }

    private bool HasContinueSave()
    {
        return GetLatestContinueSave() != null;
    }

    private void UpdateContinueButtonState()
    {
        if (buttons.ContinueButton == null)
        {
            return;
        }

        buttons.ContinueButton.gameObject.SetActive(true);
        buttons.ContinueButton.interactable = HasContinueSave();
    }

    private SaveData GetLatestContinueSave()
    {
        SaveData latestSave = null;
        System.DateTime latestWriteTime = System.DateTime.MinValue;

        string normalSavePath = Path.Combine(Application.persistentDataPath, SaveLoadPath.NormalPath);
        string autoSavePath = Path.Combine(Application.persistentDataPath, SaveLoadPath.AutoPath);
        string[] saveFolders = { normalSavePath, autoSavePath };

        foreach (string folder in saveFolders)
        {
            if (!Directory.Exists(folder))
            {
                continue;
            }

            foreach (string file in Directory.GetFiles(folder, "*.json"))
            {
                string json = File.ReadAllText(file);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                if (saveData == null || saveData.IsEmpty())
                {
                    continue;
                }

                System.DateTime writeTime = File.GetLastWriteTime(file);
                if (writeTime > latestWriteTime)
                {
                    latestWriteTime = writeTime;
                    latestSave = saveData;
                }
            }
        }

        return latestSave;
    }
}
