using System;
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
    private bool SetPlayerPosition = false;
    SceneType CurrentScene = SceneType.None;
    private Vector3 playerPosition = Vector3.zero;

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
        if (scene == SceneType.None) return;

        string sceneName = scene.ToString();

        if (IsSceneInBuild(sceneName))
        {
            CurrentScene = scene;
            SetSetPlayerPosition(true);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning($"Scene {sceneName} not found in Build Settings!");
        }
    }
    private bool IsSceneInBuild(string sceneName)
    {
        int count = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < count; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (string.Equals(name, sceneName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }


    public void SetSaveData(SaveData saveData)
    {
        playerValue.SetPlayerSaveData(saveData.playerSaveData);
        playerPosition = saveData.playerSaveData.GetPlayerPosition();
        LoadSceneByEnum(saveData.SceneType);
    }


    public bool GetSetPlayerPosition()
    {
        return SetPlayerPosition;
    }

    public void SetSetPlayerPosition(bool isSetPlayerPosition)
    {
        if (!isSetPlayerPosition) playerPosition = Vector3.zero;
        SetPlayerPosition = isSetPlayerPosition;
    }

    public Vector3 GetPlayerPosition()
    {
        return playerPosition;  
    }

    public SceneType GetCurrentScence()
    {
        return CurrentScene;
    }

    public PlayerValue GetPlayerValue()
    {
        return playerValue;
    }
}
