//Author: Vincent Luong
//Created by: Vincent Luong
//Modified by: Shawn Meng, Yuhan Tang
//no external source was used

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseControl : MonoBehaviour
{
    public GameObject pauseScreen;

    public Button ResumeButton;
    public Button SaveButton;
    public Button LoadButton;
    public Button RestartButton;
    public Button OptionsButton;
    public Button ExitGameButton;
    public Button MainMenuButton;

    private bool isPaused = false;
    public OptionPanelControl OptionPanelControl;
    //[SerializeField] private AudioSource PauseMusic;
    //[SerializeField] private AudioSource gameplayMusic;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickSound;

    private void Start()
    {
        Debug.Log($"PauseControl Start: {name}, instanceID={GetInstanceID()}");
        if (ResumeButton != null)  ResumeButton.onClick.AddListener(ResumeGame); //resume game on button click
        if (SaveButton != null) SaveButton.onClick.AddListener(SaveGame); //goes to save scene on button click
        if (LoadButton != null) LoadButton.onClick.AddListener(LoadGame); //goes to load scene on button click
        if (RestartButton != null) RestartButton.onClick.AddListener(RestartGame); //restarts the current level on button click
        if (OptionsButton != null) OptionsButton.onClick.AddListener(Options); //goes to options scene on button click
        if (ExitGameButton != null) ExitGameButton.onClick.AddListener(ExitGame); //exits the game on button click
        if (MainMenuButton != null) MainMenuButton.onClick.AddListener(MainMenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //press escape to pause the game
        {
            if (SaveLoadPanelControl.Instance != null && SaveLoadPanelControl.Instance.IsPanelOpen())
            {
                SaveLoadPanelControl.Instance.ClosePanel();
                return;
            }
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
        Debug.Log($"PauseGame called by: {name}, instanceID={GetInstanceID()}");
        Debug.Log($"pauseScreen before active = {pauseScreen.activeSelf}");
        pauseScreen.SetActive(true);
        Debug.Log($"pauseScreen after active = {pauseScreen.activeSelf}");

        pauseScreen.SetActive(true); //activates the pauseScreen and pauses the screen
        Time.timeScale = 0f; //game stops running.
        isPaused = true;

        SoundManage.Instance.PlayBackgroundMusic(SoundManagerConstants.PauseScreenMusic);
        //if(gameplayMusic != null && gameplayMusic.isPlaying)
        //{
        //    gameplayMusic.Pause();
        //}
        //if(PauseMusic != null && !PauseMusic.isPlaying)
        //{
        //    PauseMusic.Play();
        //}
    }
    void ResumeGame()
    {
        if (isPaused)
        {
            pauseScreen.SetActive(false); //deactivates the pauseScreen and unpauses the screen
            Time.timeScale = 1f; //game starts running again
            isPaused = false;
            audioSource.PlayOneShot(buttonClickSound);
            SoundManage.Instance.StopBackgroundMusic(); // Stop any pause music first

            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "Level_1") //did this for now for testing the music. I know it's not the best way to do it.  
            {
                SoundManage.Instance.PlayBackgroundMusic(SoundManagerConstants.GameplayMusic);
            }
            if (currentScene == "Level_1_Hospital") //did this for now for testing the music. I know it's not the best way to do it.
            {
                SoundManage.Instance.PlayBackgroundMusic(SoundManagerConstants.GameplayMusic_Hospital);
            }
        }
    }

    void SaveGame()
    {
        audioSource.PlayOneShot(buttonClickSound);
        //SaveLoadPanelControl.Instance.ShowPanel(); //shows the panel to save your game
        SaveLoadPanelControl.Instance.ShowSavePanel();

    }

    void LoadGame()
    {
        Debug.Log(SaveLoadPanelControl.Instance == null
            ? "SaveLoadPanelControl.Instance is NULL"
            : $"SaveLoadPanelControl.Instance found: {SaveLoadPanelControl.Instance.name}");
        audioSource.PlayOneShot(buttonClickSound);
        SaveLoadPanelControl.Instance.SetPauseControlToCloseAfterLoad(this); // close the pause panel after load game
        //SaveLoadPanelControl.Instance.ShowPanel(); //shows the panel that allows you to load your game
        SaveLoadPanelControl.Instance.ShowLoadPanel();
    }

    void RestartGame()
    {
        Time.timeScale = 1f; // Resume time scale
        audioSource.PlayOneShot(buttonClickSound);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    void Options()
    {
        OptionPanelControl.SetOptionPanelActive(true); //shows the panel to adjust settings
    }

    void ExitGame()
    {
        //UnityEditor.EditorApplication.isPlaying = false; //exits the editor
        Application.Quit(); //quit the application. //doesn't apply to unity. only applications.
    }
    void MainMenu()
    {
        Time.timeScale = 1f; //make sure time scale is back to normal before going to main menu
        audioSource.PlayOneShot(buttonClickSound);
        SceneManager.LoadScene("MainMenuScene"); //loads the main menu scene
        
    }

    public void ClosePauseAfterLoad()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}