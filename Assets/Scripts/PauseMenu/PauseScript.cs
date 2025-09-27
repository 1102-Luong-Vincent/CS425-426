using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    public Button ResumeButton;
    public Button SaveButton;
    public Button LoadButton;
    public Button OptionsButton;
    public Button ExitGameButton;

    private bool isPaused = false;
    public OptionPanelControl OptionPanelControl;
    public GameObject pauseScreen;

    private void Start()
    {
        ResumeButton.onClick.AddListener(ResumeGame); //resume game on button click
        SaveButton.onClick.AddListener(SaveGame); //goes to save scene on button click
        LoadButton.onClick.AddListener(LoadGame); //goes to load scene on button click
        OptionsButton.onClick.AddListener(Options); //goes to options scene on button click
        ExitGameButton.onClick.AddListener(ExitGame); //exits the game on button click
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //press escape to pause the game
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        pauseScreen.SetActive(true); //activates the pauseScreen and pauses the screen
        Time.timeScale = 0f; //game stops running.
        isPaused = true;
    }
    void ResumeGame()
    {
        pauseScreen.SetActive(false); //deactivates the pauseScreen and unpauses the screen
        Time.timeScale = 1f; //game starts running again
        isPaused = false;
    }

    void SaveGame()
    {
        SaveLoadPanelControl.Instance.ShowPanel(); //shows the panel to save your game
    }

    void LoadGame()
    {
        SaveLoadPanelControl.Instance.ShowPanel(); //shows the panel that allows you to load your game
    }

    void Options()
    {
        OptionPanelControl.SetOptionPanelActive(true); //shows the panel to adjust settings
    }

    void ExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false; //exits the editor
        Application.Quit(); //quit the application. //doesn't apply to unity. only applications.
    }
}