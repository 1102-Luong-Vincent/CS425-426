using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ExcelReader;

public enum SceneType
{
    None,
    TempMap,
    BattleScene,
    TimeGame,
    RhythmGame,
}


public class GameValue : MonoBehaviour
{
    public static GameValue Instance;
    private PlayerValue playerValue;
    private List<CardValue> AllCards = new List<CardValue>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Init();
        DontDestroyOnLoad(gameObject);
    }


    public void Init()
    {
        InitAllCardInExcel();
        playerValue = new PlayerValue();
    }

    void InitAllCardInExcel()
    {
        List<ExcelCardData> allExcelCardDatas = GetCardData();
        Debug.Log($"in init All card READER {allExcelCardDatas.Count}");
        AllCards.Clear();
        foreach (ExcelCardData excelData in allExcelCardDatas)
        {
            CardValue card = new CardValue(excelData);
            AllCards.Add(card);
        }
    }

    public CardValue GetCardValue(string CardName)
    {
        return AllCards.Find(card => card.CardName == CardName);
    }




    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSceneByEnum(SceneType scene)
    {

        switch (scene)
        {
            case SceneType.None: return;
            case SceneType.TempMap: SceneManager.LoadScene("TempMap"); break;
            case SceneType.BattleScene: SceneManager.LoadScene("BattleScene"); break;
            case SceneType.TimeGame: SceneManager.LoadScene("TimeGame"); break;
            case SceneType.RhythmGame: SceneManager.LoadScene("RhythmGame"); break;
            default:
                Debug.LogWarning("Undefined Scene?" + scene);
                break;
        }
    }


    public void SetSaveData(SaveData saveData)
    {
        playerValue.SetPlayerSaveData(saveData.playerSaveData);
    }


    public PlayerValue GetPlayerValue()
    {
        return playerValue;
    }
}
