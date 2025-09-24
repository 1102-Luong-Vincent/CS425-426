using UnityEngine;
using UnityEngine.UI;
using System.IO;

public static class SaveLoadPath
{
    public const string AutoPath = "Save/Auto/";
    public const string NormalPath = "Save/Normal/";
}

public class SaveLoadPanelControl : MonoBehaviour
{
    public static SaveLoadPanelControl Instance;
    public GameObject SaveLoadPanel;
    public Button CheckButton;
    public Button CancelButton;

    public SaveLoadButtonControl saveLoadButtonPrefab;
    public Transform NormalSaveTransform;
    public Transform AutoSaveTransform;

    private SaveLoadButtonControl selSaveLoadButton;

    private string normalSavePath;
    private string autoSavePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        normalSavePath = Path.Combine(Application.persistentDataPath, SaveLoadPath.NormalPath);
        autoSavePath = Path.Combine(Application.persistentDataPath, SaveLoadPath.AutoPath);
        if (!Directory.Exists(normalSavePath)) Directory.CreateDirectory(normalSavePath);
        if (!Directory.Exists(autoSavePath)) Directory.CreateDirectory(autoSavePath);
        InitButtons();

        ClosePanel();
    }

    void InitButtons()
    {
        CheckButton.onClick.AddListener(OnCheckButtonClick);
        CancelButton.onClick.AddListener(ClosePanel);
    }


    public void ShowPanel()
    {
        LoadSaveButtons();
        SetSelSaveLoadButton(null);
        SaveLoadPanel.SetActive(true);

    }

    public void ClosePanel()
    {
        SetSelSaveLoadButton(null);
        SaveLoadPanel.SetActive(false);

    }


    public void NormalSaveGame()
    {
        SaveGame(normalSavePath);
    }

    public void AutoSaveGame()
    {
        SaveGame(autoSavePath);
    }

    private void SaveGame(string folderPath)
    {
        SaveData saveData = new SaveData(folderPath, GameValue.Instance);
        string json = JsonUtility.ToJson(saveData, true);
        string fileName = $"save_{System.DateTime.Now:yyyyMMdd_HHmmss}.json";
        string fullPath = Path.Combine(folderPath, fileName);
        File.WriteAllText(fullPath, json);
        Debug.Log($"Save to: {fullPath}");
    }

    void OnCheckButtonClick()
    {
        if (selSaveLoadButton == null) return;
        GameValue.Instance.SetSaveData(selSaveLoadButton.GetSaveData());
    }

    public void SetSelSaveLoadButton(SaveLoadButtonControl saveLoadButtonControl)
    {
        if (selSaveLoadButton != null)
        {
            selSaveLoadButton.CancelSelSaveLoadButton();
        }
        selSaveLoadButton = saveLoadButtonControl;
    }

    void LoadSaveButtons()
    {
        foreach (Transform child in NormalSaveTransform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in AutoSaveTransform)
        {
            Destroy(child.gameObject);
        }

        CreateButtonsFromPath(normalSavePath, NormalSaveTransform);
        CreateButtonsFromPath(autoSavePath, AutoSaveTransform);
    }

    void CreateButtonsFromPath(string folderPath, Transform parent)
    {
        string[] files = Directory.GetFiles(folderPath, "*.json");
        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            SaveLoadButtonControl btn = Instantiate(saveLoadButtonPrefab, parent);
            btn.SetSaveData(saveData);
        }
    }
}
