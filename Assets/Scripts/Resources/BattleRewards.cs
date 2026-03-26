// Authors: Vincent Luong
// Created by: Vincent Luong
// Modified by: Vincent Luong
// No external sources were used

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleRewards : MonoBehaviour
{
    public static BattleRewards Instance;

    [Header("UI References")]
    [SerializeField] public GameObject panel; // The main panel
    [SerializeField] public TextMeshProUGUI resourceText;
    [SerializeField] public Button continueButton;

    [SerializeField] public GameObject battleCanvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject); //causes the rest of the UI to fail if this were uncommented. 

        panel.SetActive(false);

        continueButton.onClick.AddListener(HidePanel);
    }

    public void ShowReward(string message)
    {
        resourceText.text = message;
        panel.SetActive(true);

        if (battleCanvas != null)
        {
            battleCanvas.SetActive(false);
        }

        Time.timeScale = 0f;
    }

    public void HidePanel()
    {
        Time.timeScale = 1f;
        panel.SetActive(false);

        var battleData = GameValue.Instance.GetBattleData();

        GameValue.Instance.LoadSceneByEnum(battleData.GetMapScene());
        GameValue.Instance.SetPlayerPosition(battleData.GetMapPosition());

        SoundManage.Instance.PlayBackgroundMusic(SoundManagerConstants.GameplayMusic);
    }
}
