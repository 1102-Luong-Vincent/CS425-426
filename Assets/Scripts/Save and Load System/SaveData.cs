// Author: Shawn Meng, Yuhan Tang
// Created by: Shawn Meng, Yuhan Tang
// Modified by: Shawn Meng, Yuhan Tang
// No external source was used

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]   
public class SaveData
{
    public int saveVersion;
    public string savePath;
    public string saveTime;
    public SceneType currentScene;
    // public PlayerSaveData playerSaveData;

    public PlayerSaveData player;
    public WorldSaveData world;
    public StorySaveData story;
    public BattleSaveData battle;

    public SaveData()
    {
        saveVersion = 1;
        savePath = string.Empty;
        saveTime = string.Empty;
        currentScene = SceneType.None;
        player = null;
        world = new WorldSaveData();
        story = new StorySaveData();
        battle = new BattleSaveData();
    }

    public SaveData(string savePath ,GameValue gameValue)
    {
        //this.SceneType = gameValue.GetCurrentScence();
        //SavePath = savePath;
        //playerSaveData = new PlayerSaveData(gameValue.GetPlayerValue());
        //SaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.saveVersion = 1;
        this.saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.savePath = savePath;
        this.currentScene = gameValue.GetCurrentScence();

        this.player = new PlayerSaveData(gameValue.GetPlayerValue());
        this.world = new WorldSaveData(gameValue);
        this.story = new StorySaveData(gameValue);
        this.battle = new BattleSaveData(gameValue);
    }

    public bool IsEmpty()
    {
        return currentScene == SceneType.None || string.IsNullOrEmpty(savePath);
    }
}

[System.Serializable]
public class WorldSaveData
{
    public List<int> defeatedEnemyIds = new();
    public List<string> collectedInteractableIds = new();
    public List<string> keyInteractableIds = new();

    public WorldSaveData() { }

    public WorldSaveData(GameValue gameValue)
    {
        defeatedEnemyIds = gameValue.GetDefeatedEnemyIds();
        collectedInteractableIds = gameValue.GetCollectedInteractableIds();
        keyInteractableIds = gameValue.GetPlayerValue().keyInteractable.ToList();
    }
}

[System.Serializable]
public class StorySaveData
{
    public string currentStoryName;
    public string currentObjective;
    public string currentOptionalObjective;
    public List<string> completedObjectives = new();
    public List<string> completedOptionalObjectives = new();
    public List<string> finishedStoryIds = new();

    public StorySaveData() { }

    public StorySaveData(GameValue gameValue)
    {
        currentStoryName = gameValue.GetHappendStoryName();
        currentObjective = gameValue.GetCurrentObjective();
        currentOptionalObjective = gameValue.GetCurrentOptionalObjective();
        completedObjectives = gameValue.GetCompletedObjectives();
        completedOptionalObjectives = gameValue.GetCompletedOptionalObjectives();
    }
}

[System.Serializable]
public class BattleSaveData
{
    public bool isInBattle;

    public BattleSaveData() { }

    public BattleSaveData(GameValue gameValue)
    {
        isInBattle = gameValue.GetCurrentScence() == SceneType.BattleScene;
    }
}

[System.Serializable]
public class DeckSaveData
{
    public List<string> cardNames = new();
}

[System.Serializable]
public class MaterialSaveData
{
    public string materialName;
    public int amount;
}
