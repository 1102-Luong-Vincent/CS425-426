// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng, Yuhan Tang, Vincent Luong
// No external source was used

using System;
using System.Collections;
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
    LV2_Dormitory,
    LV2_Restaurant,
    Level1_Library_1,
    Level1_Library_2,
    Level1_Store
}

public static class ObjectiveConstants
{
    public const string CompletePrologue = "Complete the prologue.";
    public const string CompleteTutorial = "Follow the tutorial guide and complete the task.";
    public const string ExploreStartRoom = "Explore the room and pick up an item.";
    public const string LeaveStartRoom = "Go to the door and enter the next map.";
    public const string Level1FindGovernmentInfo = "Head east and search for information about the government and the whereabouts of the antidote in the abandoned cars.";
    public const string Level1HeadToHospital = "Head to the hospital.";
    public const string Level1FindHospitalKey = "Find the key to the hospital.";
}

[Serializable]
public class GameValue : MonoBehaviour
{
    public static GameValue Instance;

    private GameValueLibrary library;
    public PlayerValue playerValue;
    [SerializeField] SceneType CurrentScene;
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
    private readonly HashSet<string> collectedInteractableIds = new HashSet<string>();
    private bool hasPendingPlayerPosition = false;
    private Vector3 pendingPlayerPosition = Vector3.zero;
    private bool suppressAutoSaveForNextSceneLoad = false;
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    public void Init()
    {
        library = new GameValueLibrary();   
        playerValue = new PlayerValue();
        //CurrentScene = SceneType.None;
        CurrentScene = GetCurrentScence();
        happendStoryName = string.Empty;
        currentObjective = string.Empty;
        completedObjectives.Clear();
        battleData = null;
        defeatedEnemies.Clear();
        collectedInteractableIds.Clear();
        hasPendingPlayerPosition = false;
        pendingPlayerPosition = Vector3.zero;
        suppressAutoSaveForNextSceneLoad = false;
        
        weaponExcelCache = ExcelReader.GetWeaponsData();
    }

    public void ResetGameState()
    {
        Init();
    }



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyPendingPlayerPositionIfPossible();

        if (suppressAutoSaveForNextSceneLoad)
        {
            suppressAutoSaveForNextSceneLoad = false;
            return;
        }

        if (ShouldAutoSaveScene(CurrentScene))
        {
            StartCoroutine(AutoSaveAfterSceneReady());
        }
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
            SetCurrentObjective(ObjectiveConstants.CompleteTutorial, false, true);
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
        QueuePlayerPositionForNextScene(saveData.player != null ? saveData.player.GetPlayerPosition() : Vector3.zero);

        if (saveData.world != null)
        {
            SetDefeatedEnemyIds(saveData.world.defeatedEnemyIds);
            SetCollectedInteractableIds(saveData.world.collectedInteractableIds);
            playerValue.keyInteractable.Clear();

            if (saveData.world.keyInteractableIds != null)
            {
                foreach (string keyId in saveData.world.keyInteractableIds)
                {
                    if (!string.IsNullOrWhiteSpace(keyId))
                    {
                        playerValue.keyInteractable.Add(keyId);
                    }
                }
            }
        }
        else
        {
            SetDefeatedEnemyIds(null);
            SetCollectedInteractableIds(null);
            playerValue.keyInteractable.Clear();
        }

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
        battleData = null;
        //PlayerController.Instance.SetPlayerPosition(saveData.player.GetPlayerPosition());
        suppressAutoSaveForNextSceneLoad = true;
        LoadSceneByEnum(saveData.currentScene);
    }

    private IEnumerator AutoSaveAfterSceneReady()
    {
        yield return null;
        yield return null;

        ApplyPendingPlayerPositionIfPossible();

        if (SaveLoadPanelControl.Instance != null)
        {
            SaveLoadPanelControl.Instance.AutoSaveGame();
        }
    }

    private bool ShouldAutoSaveScene(SceneType scene)
    {
        return scene == SceneType.GameStartScene ||
               scene == SceneType.Level_1 ||
               scene == SceneType.Level_1_Hospital ||
               scene == SceneType.Level_2 ||
               scene == SceneType.LV2_Dormitory ||
               scene == SceneType.LV2_Restaurant ||
               scene == SceneType.Level1_Library_1 ||
               scene == SceneType.Level1_Library_2 ||
               scene == SceneType.Level1_Store;
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

    public void MarkCollectedInteractable(string persistentId)
    {
        if (!string.IsNullOrWhiteSpace(persistentId))
        {
            collectedInteractableIds.Add(persistentId);
        }
    }

    public bool IsCollectedInteractable(string persistentId)
    {
        return !string.IsNullOrWhiteSpace(persistentId) && collectedInteractableIds.Contains(persistentId);
    }

    public List<int> GetDefeatedEnemyIds()
    {
        return defeatedEnemies.OrderBy(id => id).ToList();
    }

    public List<string> GetCollectedInteractableIds()
    {
        return collectedInteractableIds.OrderBy(id => id).ToList();
    }

    public void SetDefeatedEnemyIds(IEnumerable<int> enemyIds)
    {
        defeatedEnemies.Clear();

        if (enemyIds == null)
        {
            return;
        }

        foreach (int enemyId in enemyIds)
        {
            defeatedEnemies.Add(enemyId);
        }
    }

    public void SetCollectedInteractableIds(IEnumerable<string> interactableIds)
    {
        collectedInteractableIds.Clear();

        if (interactableIds == null)
        {
            return;
        }

        foreach (string interactableId in interactableIds)
        {
            if (!string.IsNullOrWhiteSpace(interactableId))
            {
                collectedInteractableIds.Add(interactableId);
            }
        }
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
        if (PlayerController.Instance != null)
        {
            return PlayerController.Instance.GetPlayerCurrentPosition();
        }

        if (hasPendingPlayerPosition)
        {
            return pendingPlayerPosition;
        }

        return Vector3.zero;
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
        pendingPlayerPosition = pos;
        hasPendingPlayerPosition = true;
        ApplyPendingPlayerPositionIfPossible();
    }

    public void QueuePlayerPositionForNextScene(Vector3 pos)
    {
        pendingPlayerPosition = pos;
        hasPendingPlayerPosition = true;
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

    private void ApplyPendingPlayerPositionIfPossible()
    {
        if (!hasPendingPlayerPosition || PlayerController.Instance == null)
        {
            return;
        }

        PlayerController.Instance.SetPlayerPosition(pendingPlayerPosition);
        hasPendingPlayerPosition = false;
    }
    #endregion
}
