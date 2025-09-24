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
        SoundManage.Instance.PlayBackgroundMusic("TestBGM");
    }


    void InitButtons()
    {

        OnMainMenuButtonClick(buttons.StartButton, OnStarButtonClick, false);
        OnMainMenuButtonClick(buttons.LoadButton, OnLoadButtonClick, false);
        OnMainMenuButtonClick(buttons.OptionButton, OnOptionButtonClick, false);
        OnMainMenuButtonClick(buttons.ExitButton, OnExitButtonClick, false);


    }

    void OnStarButtonClick()
    {
        // !!! Remember to initialize GameValue
        Debug.Log("Remember to initialize GameValue"); 
        GameValue.Instance.LoadSceneByEnum(SceneType.TempMap);
    }

    void OnLoadButtonClick()
    {
        SaveLoadPanelControl.Instance.ShowPanel();
    }

    void OnOptionButtonClick()
    {
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
