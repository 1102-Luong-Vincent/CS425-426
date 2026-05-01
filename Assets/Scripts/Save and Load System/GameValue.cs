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
    public const string Level1HeadToHospital = "Head west to the hospital.";
    public const string Level1FindHospitalKey = "Head to the library in the north to look for the key to the hospital.";
    public const string HospitalFindTrevorLastNote = "Find Trevor's Last Note on the table.";
    public const string HospitalFindPassageToQuarantineZone = "Go to the left of the hospital to find the passage leading to the Military Quarantine Zone.";
    public const string HospitalOptionalFindTrevor = "Optional: Find out what happened to Trevor";
    public const string TrevorMinibossEnemyName = "Trevor";
    public const string TrevorMinibossSceneName = "Trevor (Miniboss)";
    public const int TrevorMinibossWorldEnemyID = 4;
    public const string Level2FindCure = "Find the cure.";
    public const string Level2OptionalFindIsaac = "Optional: Find Isaac";
    public const string IsaacFinalBossEnemyName = "Isaac";
    public const string IsaacFinalBossSceneName = "Isaac (Final Boss)";
    public const int IsaacFinalBossWorldEnemyID = 9;
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
    private string currentOptionalObjective = string.Empty;
    private readonly List<string> completedOptionalObjectives = new List<string>();
    [SerializeField] GameProcessManager gameProcessManager;

    [SerializeField] private BattleData battleData;
    public GameValueTest gameValueTest;
    // new add for weapon upgrade
    private List<ExcelWeaponData> weaponExcelCache;

    //record data for defeated enemies 
    private HashSet<string> defeatedEnemyKeys  = new HashSet<string>();
    private readonly HashSet<string> collectedInteractableIds = new HashSet<string>();
    private readonly Dictionary<string, Vector3> enemyPositions = new Dictionary<string, Vector3>();
    private bool hasPendingPlayerPosition = false;
    private Vector3 pendingPlayerPosition = Vector3.zero;
    private bool suppressAutoSaveForNextSceneLoad = false;
    private const KeyCode DebugRewardKey = KeyCode.O;
    private const float DebugRewardKeyWindow = 1.5f;
    private const int DebugRewardRequiredPresses = 3;
    private int debugRewardKeyPressCount = 0;
    private float lastDebugRewardKeyPressTime = -999f;
    //private int nextEnemyID = 1;
    public bool FirstUpgradeFlag = false;
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
        currentOptionalObjective = string.Empty;
        completedOptionalObjectives.Clear();
        battleData = null;
        defeatedEnemyKeys.Clear();
        collectedInteractableIds.Clear();
        enemyPositions.Clear();
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
        CheckDebugRewardCheat();
    }

    private void CheckDebugRewardCheat()
    {
        if (!Input.GetKeyDown(DebugRewardKey))
        {
            return;
        }

        if (Time.unscaledTime - lastDebugRewardKeyPressTime > DebugRewardKeyWindow)
        {
            debugRewardKeyPressCount = 0;
        }

        lastDebugRewardKeyPressTime = Time.unscaledTime;
        debugRewardKeyPressCount++;

        if (debugRewardKeyPressCount < DebugRewardRequiredPresses)
        {
            return;
        }

        debugRewardKeyPressCount = 0;
        GiveDebugReward();
    }

    private void GiveDebugReward()
    {
        if (playerValue == null)
        {
            return;
        }

        playerValue.AddMaterial("Whetstone", 100);

        WeaponValue shotgun = GetWeaponByNameAndLevel("Shotgun", 1);
        if (shotgun != null)
        {
            playerValue.HadWeaponsLibrary.Add(shotgun);
        }

        Debug.Log("[Cheat] Added Whetstone x100 and Shotgun Lv1.");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!TryGetSceneType(scene.name, out SceneType loadedScene))
        {
            if (string.Equals(scene.name, "MainMenuScene", StringComparison.OrdinalIgnoreCase))
            {
                CurrentScene = SceneType.None;
                hasPendingPlayerPosition = false;
            }

            suppressAutoSaveForNextSceneLoad = false;
            return;
        }

        CurrentScene = loadedScene;
        ApplyPendingPlayerPositionIfPossible();

        if (suppressAutoSaveForNextSceneLoad)
        {
            suppressAutoSaveForNextSceneLoad = false;
            return;
        }

        if (ShouldAutoSaveScene(loadedScene))
        {
            StartCoroutine(AutoSaveAfterSceneReady());
        }
    }

    private bool TryGetSceneType(string sceneName, out SceneType sceneType)
    {
        return Enum.TryParse(sceneName, true, out sceneType) && sceneType != SceneType.None;
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

        if (scene == SceneType.Level_1_Hospital &&
            ShouldStartHospitalObjectives())
        {
            ClearOptionalObjective();
            SetCurrentObjective(ObjectiveConstants.HospitalFindTrevorLastNote, false, true);
            return;
        }

        if (scene == SceneType.Level_2 &&
            (currentObjective == ObjectiveConstants.Level1HeadToHospital ||
             currentObjective == ObjectiveConstants.HospitalFindPassageToQuarantineZone))
        {
            ClearOptionalObjective();
            SetCurrentObjective(ObjectiveConstants.Level2FindCure, false, true);
            return;
        }

        if (scene == SceneType.Level_2 &&
            string.IsNullOrWhiteSpace(currentObjective))
        {
            ClearOptionalObjective();
            SetCurrentObjective(ObjectiveConstants.Level2FindCure, false, true);
            return;
        }

        if (scene == SceneType.Level_2 &&
            currentObjective == ObjectiveConstants.Level2FindCure &&
            string.IsNullOrWhiteSpace(currentOptionalObjective) &&
            !completedOptionalObjectives.Contains(ObjectiveConstants.Level2OptionalFindIsaac))
        {
            SetOptionalObjective(ObjectiveConstants.Level2OptionalFindIsaac);
        }
    }

    private bool ShouldStartHospitalObjectives()
    {
        return string.IsNullOrWhiteSpace(currentObjective) ||
               currentObjective == ObjectiveConstants.Level1HeadToHospital ||
               currentObjective == ObjectiveConstants.Level1FindHospitalKey ||
               currentObjective == ObjectiveConstants.Level1FindGovernmentInfo;
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
            SetDefeatedEnemySaveData(saveData.world.defeatedEnemyKeys, saveData.world.defeatedEnemyIds, saveData.currentScene);
            SetCollectedInteractableIds(saveData.world.collectedInteractableIds);
            SetEnemyPositionSaveData(saveData.world.enemyPositions, saveData.currentScene);
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
            SetDefeatedEnemySaveData(null, null, SceneType.None);
            SetCollectedInteractableIds(null);
            SetEnemyPositionSaveData(null, SceneType.None);
            playerValue.keyInteractable.Clear();
        }

        if (saveData.story != null)
        {
            happendStoryName = saveData.story.currentStoryName ?? string.Empty;
            currentObjective = saveData.story.currentObjective ?? string.Empty;
            currentOptionalObjective = saveData.story.currentOptionalObjective ?? string.Empty;
            completedObjectives.Clear();
            completedOptionalObjectives.Clear();

            if (saveData.story.completedObjectives != null)
            {
                completedObjectives.AddRange(saveData.story.completedObjectives.Where(objective => !string.IsNullOrWhiteSpace(objective)));
            }

            if (saveData.story.completedOptionalObjectives != null)
            {
                completedOptionalObjectives.AddRange(saveData.story.completedOptionalObjectives.Where(objective => !string.IsNullOrWhiteSpace(objective)));
            }
        }
        else
        {
            happendStoryName = string.Empty;
            currentObjective = string.Empty;
            completedObjectives.Clear();
            currentOptionalObjective = string.Empty;
            completedOptionalObjectives.Clear();
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
        DefeatedEnemies(CurrentScene, worldEnemyID);
    }

    public void DefeatedEnemies(SceneType scene, int worldEnemyID)
    {
        if (!TryGetEnemyKey(scene, worldEnemyID, out string enemyKey))
        {
            return;
        }

        defeatedEnemyKeys.Add(enemyKey);
        enemyPositions.Remove(enemyKey);
    }

    public bool IsEnemyDefeated(int worldEnemyID)
    {
        return IsEnemyDefeated(CurrentScene, worldEnemyID);
    }

    public bool IsEnemyDefeated(SceneType scene, int worldEnemyID)
    {
        return TryGetEnemyKey(scene, worldEnemyID, out string enemyKey) &&
               defeatedEnemyKeys.Contains(enemyKey);
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
        return new List<int>();
    }

    public List<string> GetDefeatedEnemyKeys()
    {
        return defeatedEnemyKeys.OrderBy(key => key).ToList();
    }

    public List<string> GetCollectedInteractableIds()
    {
        return collectedInteractableIds.OrderBy(id => id).ToList();
    }

    public List<EnemyPositionSaveData> GetEnemyPositionSaveData()
    {
        return enemyPositions
            .Where(pair => !defeatedEnemyKeys.Contains(pair.Key))
            .OrderBy(pair => pair.Key)
            .Select(pair => new EnemyPositionSaveData(pair.Key, pair.Value))
            .ToList();
    }

    public void SetEnemyPosition(int worldEnemyID, Vector3 position)
    {
        SetEnemyPosition(CurrentScene, worldEnemyID, position);
    }

    public void SetEnemyPosition(SceneType scene, int worldEnemyID, Vector3 position)
    {
        if (!TryGetEnemyKey(scene, worldEnemyID, out string enemyKey) ||
            defeatedEnemyKeys.Contains(enemyKey))
        {
            return;
        }

        enemyPositions[enemyKey] = position;
    }

    public bool TryGetEnemyPosition(int worldEnemyID, out Vector3 position)
    {
        return TryGetEnemyPosition(CurrentScene, worldEnemyID, out position);
    }

    public bool TryGetEnemyPosition(SceneType scene, int worldEnemyID, out Vector3 position)
    {
        if (!TryGetEnemyKey(scene, worldEnemyID, out string enemyKey) ||
            defeatedEnemyKeys.Contains(enemyKey))
        {
            position = Vector3.zero;
            return false;
        }

        return enemyPositions.TryGetValue(enemyKey, out position);
    }

    public void SetDefeatedEnemyIds(IEnumerable<int> enemyIds)
    {
        SetDefeatedEnemySaveData(null, enemyIds, CurrentScene);
    }

    public void SetDefeatedEnemySaveData(IEnumerable<string> enemyKeys, IEnumerable<int> legacyEnemyIds, SceneType legacyScene)
    {
        defeatedEnemyKeys.Clear();

        if (enemyKeys != null)
        {
            foreach (string enemyKey in enemyKeys)
            {
                if (IsValidEnemyKey(enemyKey))
                {
                    defeatedEnemyKeys.Add(enemyKey);
                    enemyPositions.Remove(enemyKey);
                }
            }
        }

        if (defeatedEnemyKeys.Count > 0 || legacyEnemyIds == null)
        {
            return;
        }

        foreach (int enemyId in legacyEnemyIds)
        {
            if (TryGetEnemyKey(legacyScene, enemyId, out string enemyKey))
            {
                defeatedEnemyKeys.Add(enemyKey);
                enemyPositions.Remove(enemyKey);
            }
        }
    }

    public void SetEnemyPositionSaveData(IEnumerable<EnemyPositionSaveData> positions)
    {
        SetEnemyPositionSaveData(positions, CurrentScene);
    }

    public void SetEnemyPositionSaveData(IEnumerable<EnemyPositionSaveData> positions, SceneType legacyScene)
    {
        enemyPositions.Clear();

        if (positions == null)
        {
            return;
        }

        foreach (EnemyPositionSaveData positionData in positions)
        {
            if (positionData == null)
            {
                continue;
            }

            string enemyKey = positionData.enemyKey;
            if (!IsValidEnemyKey(enemyKey) &&
                !TryGetEnemyKey(legacyScene, positionData.worldEnemyID, out enemyKey))
            {
                continue;
            }

            if (!defeatedEnemyKeys.Contains(enemyKey))
            {
                enemyPositions[enemyKey] = positionData.GetPosition();
            }
        }
    }

    private bool TryGetEnemyKey(SceneType scene, int worldEnemyID, out string enemyKey)
    {
        enemyKey = string.Empty;

        if (scene == SceneType.None ||
            scene == SceneType.BattleScene ||
            worldEnemyID <= 0)
        {
            return false;
        }

        enemyKey = $"{scene}:{worldEnemyID}";
        return true;
    }

    private bool IsValidEnemyKey(string enemyKey)
    {
        if (string.IsNullOrWhiteSpace(enemyKey))
        {
            return false;
        }

        string[] parts = enemyKey.Split(':');
        return parts.Length == 2 &&
               Enum.TryParse(parts[0], true, out SceneType scene) &&
               scene != SceneType.None &&
               scene != SceneType.BattleScene &&
               int.TryParse(parts[1], out int worldEnemyID) &&
               worldEnemyID > 0;
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

    public string GetCurrentOptionalObjective()
    {
        return currentOptionalObjective;
    }

    public List<string> GetCompletedOptionalObjectives()
    {
        return new List<string>(completedOptionalObjectives);
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
        ApplyObjectiveSideEffects(nextObjective);
    }

    private void ApplyObjectiveSideEffects(string objective)
    {
        if (objective == ObjectiveConstants.HospitalFindPassageToQuarantineZone &&
            string.IsNullOrWhiteSpace(currentOptionalObjective) &&
            !completedOptionalObjectives.Contains(ObjectiveConstants.HospitalOptionalFindTrevor))
        {
            SetOptionalObjective(ObjectiveConstants.HospitalOptionalFindTrevor);
        }

        if (objective == ObjectiveConstants.Level2FindCure &&
            string.IsNullOrWhiteSpace(currentOptionalObjective) &&
            !completedOptionalObjectives.Contains(ObjectiveConstants.Level2OptionalFindIsaac))
        {
            SetOptionalObjective(ObjectiveConstants.Level2OptionalFindIsaac);
        }
    }

    public void SetOptionalObjective(string objective)
    {
        currentOptionalObjective = objective ?? string.Empty;
    }

    public void CompleteOptionalObjective(string objective)
    {
        if (string.IsNullOrWhiteSpace(objective))
        {
            return;
        }

        if (currentOptionalObjective == objective)
        {
            currentOptionalObjective = string.Empty;
        }

        if (!completedOptionalObjectives.Contains(objective))
        {
            completedOptionalObjectives.Add(objective);
        }
    }

    public void ClearOptionalObjective()
    {
        currentOptionalObjective = string.Empty;
        completedOptionalObjectives.Clear();
    }

    public void ClearObjectiveProgress()
    {
        currentObjective = string.Empty;
        completedObjectives.Clear();
        ClearOptionalObjective();
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
