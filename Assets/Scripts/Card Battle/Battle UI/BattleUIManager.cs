// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng
// no external source was used.


using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TurnText;
    [SerializeField] GameObject GameOverPanel;

    [Header("Buttons")]
    [SerializeField] Button RetryButton;
    [SerializeField] Button MainMenuButton;

    void Start()
    {
        SetTurnText(BattleManage.Instance.Turn);
        Listener(true);
        GameOverPanel.SetActive(false);

        RetryButton.onClick.AddListener(Retry);
        MainMenuButton.onClick.AddListener(MainMenu);
    }

    private void OnDestroy()
    {
        Listener(false);
    }


    public void SetTurnText(int turn)
    {
        TurnText.text = $"Turn {turn}";
    }



    void Listener(bool isAdd)
    {
        BattleManage.Instance.TurnListener(SetTurnText, isAdd);
    }


   
    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayGameOver()
    {
        GameOverPanel.SetActive(true);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //reloads the current scene so battle can restart
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}
