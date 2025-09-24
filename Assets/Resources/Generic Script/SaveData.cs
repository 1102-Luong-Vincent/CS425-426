using UnityEngine;
using System;

[System.Serializable]   
public class SaveData
{
    public PlayerSaveData playerSaveData;
    public string SavePath;
    public string SaveTime;   

    public SaveData(string savePath, GameValue gameValue)
    {
        SavePath = savePath;
        playerSaveData = new PlayerSaveData(gameValue.GetPlayerValue());
        SaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
