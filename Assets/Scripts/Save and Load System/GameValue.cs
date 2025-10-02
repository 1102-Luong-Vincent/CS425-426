using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ExcelReader;

public enum SceneType
{
    None,
    StoryScene,
    TempMap,
    BattleScene,
    TimeGame,
    RhythmGame,
    GameStartScene,
}


public class GameValue : MonoBehaviour
{
    public static GameValue Instance;

    private GameValueLibrary library;
    private PlayerValue playerValue;
    SceneType CurrentScene = SceneType.None;
    private Vector3 playerPosition = Vector3.zero;
    private String happendStoryName = string.Empty;

    private BattleData battleData;
    public GameValueTest gameValueTest;


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

        if (gameValueTest != null) gameValueTest.SetTestValue(this);
    }


    public void Init()
    {
        library = new GameValueLibrary();   
        playerValue = new PlayerValue();
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
            //SetSetPlayerPosition(true);
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


    /*   public bool GetSetPlayerPosition()
       {
           return SetPlayerPosition;
       }

       public void SetSetPlayerPosition(bool isSetPlayerPosition)
       {
           if (!isSetPlayerPosition) playerPosition = Vector3.zero;
           SetPlayerPosition = isSetPlayerPosition;
       }*/


    #region Get

    public BattleData GetBattleData()
    {
        return battleData;
    }

    public EnemyValue GetInitEnemyValue(int enemyValue)
    {
        return GetGameValueLibrary().GetInitEnemyValue(enemyValue);
    }

    public WeaponValue GetInitWeaponValue(int WeaponID)
    {
        return GetGameValueLibrary().GetInitWeapon(WeaponID);
    }


    public WeaponValue GetInitWeaponValue(string WeaponName)
    {
        return GetGameValueLibrary().GetInitWeapon(WeaponName);
    }

    public CardValue GetInitCardValue(int CardID)
    {
        return GetGameValueLibrary().GetInitCard(CardID);
    }

    public CardValue GetInitCardValue(string CardName)
    {
        return GetGameValueLibrary().GetInitCard(CardName);
    }


    public GameValueLibrary GetGameValueLibrary()
    {
        return library;
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

    public string GetHappendStoryName()
    {
        return happendStoryName;
    }

    #endregion

    #region Set

    public void SetBattleData(BattleData battleData)
    {
        this.battleData = battleData;
    }

    public void SetHappendStoryName(string happendStoryName)
    {
      this.happendStoryName = happendStoryName;
    }
    #endregion
}
