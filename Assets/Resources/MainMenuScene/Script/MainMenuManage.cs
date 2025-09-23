using UnityEngine;
using UnityEngine.UI;

public class MainMenuManage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static MainMenuManage Instance;

    [Header("Buttons")]
    public Buttons buttons;

    [System.Serializable]
    public class Buttons
    {
        public Button startButton;
        public Button OptionButton;
        public Button exitButton;
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
        buttons.startButton.onClick.AddListener(OnStarButtonClick);
        buttons.OptionButton.onClick.AddListener(OnOptionButtonClick);
        buttons.exitButton.onClick.AddListener(OnExitButtonClick);

    }

    void OnStarButtonClick()
    {
        PlayerValue.Instance.LoadSceneByEnum(SceneType.TempMap);
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
