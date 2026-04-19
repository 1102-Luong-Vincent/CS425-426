// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng, Yuhan Tang, Vincent Luong
// No external source was used

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ExcelReader;
using SmallScaleInc.TopDownPixelCharactersPack1;

public enum SceneType
{
    None,
    StoryScene,
    TempMap,
    BattleScene,
    TimeGame,
    RhythmGame,
    GameStartScene,


    Level_1,
    Level_1_Hospital,
    Level_2,

}

public static class ObjectiveConstants
{
    public const string CompletePrologue = "Complete the prologue.";
    public const string ExploreStartRoom = "Explore the room and pick up an item.";
    public const string LeaveStartRoom = "Go to the door and enter the next map.";
    public const string Level1FindGovernmentInfo = "Find information about the government and the whereabouts of the cure.";
    public const string Level1HeadToHospital = "Head to the hospital.";
    public const string Level1FindHospitalKey = "Find the key to the hospital.";
}

[Serializable]
public class GameValue : MonoBehaviour
{
    public static GameValue Instance;

    private GameValueLibrary library;
    public PlayerValue playerValue;
    [SerializeField] SceneType CurrentScene = SceneType.None;
    private String happendStoryName = string.Empty;
    private string currentObjective = string.Empty;
    private readonly List<string> completedObjectives = new List<string>();
    [SerializeField] GameProcessManager gameProcessManager;

    [SerializeField] private BattleData battleData;
    public GameValueTest gameValueTest;
    // new add for weapon upgrade
    private List<ExcelWeaponData> weaponExcelCache;

    //record data for defeated enemies 
    private HashSet<int> defeatedEnemies  = new HashSet<int>();
    //private int nextEnemyID = 1;


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
        currentObjective = string.Empty;
        completedObjectives.Clear();
        
        weaponExcelCache = ExcelReader.GetWeaponsData();
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
            ApplySceneObjectiveOverrides(scene);
            CurrentScene = scene;
            //SetSetPlayerPosition(true);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning($"Scene {sceneName} not found in Build Settings!");
        }
        gameProcessManager.PlayMusic(scene);
    }

    private void ApplySceneObjectiveOverrides(SceneType scene)
    {
        if (scene == SceneType.GameStartScene &&
            (string.IsNullOrWhiteSpace(currentObjective) || currentObjective == ObjectiveConstants.CompletePrologue))
        {
            SetCurrentObjective(ObjectiveConstants.ExploreStartRoom, false, true);
            return;
        }

        if (scene == SceneType.Level_1 &&
            (string.IsNullOrWhiteSpace(currentObjective) ||
             currentObjective == ObjectiveConstants.CompletePrologue ||
             currentObjective == ObjectiveConstants.ExploreStartRoom ||
             currentObjective == ObjectiveConstants.LeaveStartRoom))
        {
            SetCurrentObjective(ObjectiveConstants.Level1FindGovernmentInfo, false, true);
            return;
        }

        if (scene == SceneType.Level_2 &&
            currentObjective == ObjectiveConstants.Level1HeadToHospital)
        {
            SetCurrentObjective(string.Empty);
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
        //playerValue.SetPlayerSaveData(saveData.playerSaveData);
        //PlayerController.Instance.SetPlayerPosition(saveData.playerSaveData.GetPlayerPosition());
        //LoadSceneByEnum(saveData.SceneType);
        if (saveData == null)
        {
            Debug.LogWarning("SaveData is null.");
            return;
        }
        Debug.Log($"Loading save. currentScene = {saveData.currentScene}");
        playerValue.SetPlayerSaveData(saveData.player);
        if (saveData.story != null)
        {
            happendStoryName = saveData.story.currentStoryName ?? string.Empty;
            currentObjective = saveData.story.currentObjective ?? string.Empty;
            completedObjectives.Clear();

            if (saveData.story.completedObjectives != null)
            {
                completedObjectives.AddRange(saveData.story.completedObjectives.Where(objective => !string.IsNullOrWhiteSpace(objective)));
            }
        }
        else
        {
            happendStoryName = string.Empty;
            currentObjective = string.Empty;
            completedObjectives.Clear();
        }
        //PlayerController.Instance.SetPlayerPosition(saveData.player.GetPlayerPosition());
        LoadSceneByEnum(saveData.currentScene);
    }

    //new add Weapon upgrade
    public WeaponValue GetWeaponByNameAndLevel(string weaponName, int level)
    {
        if (weaponExcelCache == null || weaponExcelCache.Count == 0)
            weaponExcelCache = ExcelReader.GetWeaponsData();

        foreach (var w in weaponExcelCache)
        {
            if (w.weaponName == weaponName && w.weaponLevel == level)
            {
                return new WeaponValue(w);
            }
        }

        Debug.LogWarning($"[GetWeaponByNameAndLevel] Not found: {weaponName} Lv{level}");
        return null;
    }

    //data to record defeated enemies
    public void DefeatedEnemies(int worldEnemyID)
    {
        defeatedEnemies.Add(worldEnemyID);
    }

    public bool IsEnemyDefeated(int worldEnemyID)
    {
        return defeatedEnemies.Contains(worldEnemyID);
    }

    //public int GetNextEnemyID()
    //{
    //    return nextEnemyID++;
    //}

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
        return PlayerController.Instance.GetPlayerCurrentPosition();  
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

    public string GetCurrentObjective()
    {
        return currentObjective;
    }

    public List<string> GetCompletedObjectives()
    {
        return new List<string>(completedObjectives);
    }

    #endregion

    #region Set

    public void SetPlayerPosition(Vector3 pos)
    {
        PlayerController.Instance.SetPlayerPosition(pos);
    }



    public void SetBattleData(BattleData battleData)
    {
        this.battleData = battleData;
    }

    public void SetHappendStoryName(string happendStoryName)
    {
      this.happendStoryName = happendStoryName;
    }

    public void SetCurrentObjective(string objective)
    {
        SetCurrentObjective(objective, true, false);
    }

    public void SetCurrentObjective(string objective, bool completePrevious, bool resetCompletedObjectives)
    {
        string nextObjective = objective ?? string.Empty;

        if (resetCompletedObjectives)
        {
            completedObjectives.Clear();
        }

        if (completePrevious &&
            !string.IsNullOrWhiteSpace(currentObjective) &&
            currentObjective != nextObjective &&
            (completedObjectives.Count == 0 || completedObjectives[completedObjectives.Count - 1] != currentObjective))
        {
            completedObjectives.Add(currentObjective);
        }

        currentObjective = nextObjective;
    }

    public void ClearObjectiveProgress()
    {
        currentObjective = string.Empty;
        completedObjectives.Clear();
    }
    #endregion
}
