// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng
// No external source was used

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveLoadButtonControl : MonoBehaviour
{
    public Button SaveLoadButton;
    public TextMeshProUGUI SaveLoadText;
    private SaveData saveData;
    private string saveFilePath;

    private void Awake()
    {
        SaveLoadButton.onClick.AddListener(OnSaveLoadButtonClick);
    }

    public SaveData GetSaveData()
    {
        return saveData;
    }

    public string GetSaveFilePath()
    {
        return saveFilePath;
    }
   
    public void SetSaveData(SaveData saveData, string saveFilePath)
    {
        this.saveData = saveData;
        this.saveFilePath = saveFilePath;
        //SaveLoadText.text = saveData.SaveTime.ToString();
        SaveLoadText.text = string.IsNullOrWhiteSpace(saveData.saveName)
            ? saveData.saveTime
            : saveData.saveName;
    }

    void OnSaveLoadButtonClick()
    {
        SaveLoadPanelControl.Instance.SetSelSaveLoadButton(this);
        SelSavaLoadButton();

    }

    void SelSavaLoadButton()
    {
        SaveLoadButton.image.color = Color.red;
    }


    public void CancelSelSaveLoadButton()
    {
        SaveLoadButton.image.color = Color.white;

    }


}
