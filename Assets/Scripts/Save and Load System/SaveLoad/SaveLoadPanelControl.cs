// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng, Yuhan Tang
// No external source was used


using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Audio;

public static class SaveLoadPath
{
    public const string AutoPath = "Save/Auto/";
    public const string NormalPath = "Save/Normal/";
}

public class SaveLoadPanelControl : MonoBehaviour
{
    public static SaveLoadPanelControl Instance;

    public GameObject SaveLoadRoot;
    public GameObject SavePanel;
    public GameObject LoadPanel;

    public Button SavePanelSaveButton;
    public Button SavePanelCancelButton;

    public Button LoadPanelLoadButton;
    public Button LoadPanelCancelButton;

    public SaveLoadButtonControl saveLoadButtonPrefab;

    public Transform SavePanelNormalSaveTransform;
    public Transform SavePanelAutoSaveTransform;

    public Transform LoadPanelNormalSaveTransform;
    public Transform LoadPanelAutoSaveTransform;

    private SaveLoadButtonControl selSaveLoadButton;

    private string normalSavePath;
    private string autoSavePath;

    private PauseControl pauseControlToCloseAfterLoad;

    [Header("Sound Effects")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip buttonClickSound;

    private void Awake()
    {
        Debug.Log($"SaveLoadPanelControl Awake: {name}, instanceID={GetInstanceID()}");
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
        //SaveButton.onClick.AddListener(OnSaveButtonClick);
        //CheckButton.onClick.AddListener(OnCheckButtonClick);
        //CancelButton.onClick.AddListener(ClosePanel);
        SavePanelSaveButton.onClick.AddListener(OnSaveButtonClick);
        SavePanelCancelButton.onClick.AddListener(ClosePanel);

        LoadPanelLoadButton.onClick.AddListener(OnCheckButtonClick);
        LoadPanelCancelButton.onClick.AddListener(ClosePanel);
    }


    //public void ShowPanel()
    //{
    //    LoadSaveButtons();
    //    SetSelSaveLoadButton(null);
    //    SaveLoadPanel.SetActive(true);

    //}

    public void ClosePanel()
    {
        SetSelSaveLoadButton(null);
        SavePanel.SetActive(false);
        LoadPanel.SetActive(false);
        SaveLoadRoot.SetActive(false);
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

    void OnSaveButtonClick()
    {
        NormalSaveGame();
        //LoadSaveButtons();
        LoadSaveButtonsForSavePanel();
    }

    void OnCheckButtonClick()
    {
        if (selSaveLoadButton == null) return;
        GameValue.Instance.SetSaveData(selSaveLoadButton.GetSaveData());
        audioSource.PlayOneShot(buttonClickSound);

        if (pauseControlToCloseAfterLoad != null)
        {
            pauseControlToCloseAfterLoad.ClosePauseAfterLoad();
            pauseControlToCloseAfterLoad = null;
        }
        ClosePanel();
    }

    public void SetSelSaveLoadButton(SaveLoadButtonControl saveLoadButtonControl)
    {
        if (selSaveLoadButton != null)
        {
            selSaveLoadButton.CancelSelSaveLoadButton();
        }
        selSaveLoadButton = saveLoadButtonControl;
    }

    //void LoadSaveButtons()
    //{
    //    foreach (Transform child in NormalSaveTransform)
    //    {
    //        Destroy(child.gameObject);
    //    }
    //    foreach (Transform child in AutoSaveTransform)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    CreateButtonsFromPath(normalSavePath, NormalSaveTransform);
    //    CreateButtonsFromPath(autoSavePath, AutoSaveTransform);
    //}

    void LoadSaveButtonsForSavePanel()
    {
        ClearChildren(SavePanelNormalSaveTransform);
        ClearChildren(SavePanelAutoSaveTransform);

        CreateButtonsFromPath(normalSavePath, SavePanelNormalSaveTransform);
        CreateButtonsFromPath(autoSavePath, SavePanelAutoSaveTransform);
    }

    void LoadSaveButtonsForLoadPanel()
    {
        ClearChildren(LoadPanelNormalSaveTransform);
        ClearChildren(LoadPanelAutoSaveTransform);

        CreateButtonsFromPath(normalSavePath, LoadPanelNormalSaveTransform);
        CreateButtonsFromPath(autoSavePath, LoadPanelAutoSaveTransform);
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

    public void SetPauseControlToCloseAfterLoad(PauseControl pauseControl)
    {
        pauseControlToCloseAfterLoad = pauseControl;
    }

    public void ShowSavePanel()
    {
        SetSelSaveLoadButton(null);
        LoadSaveButtonsForSavePanel();
        SaveLoadRoot.SetActive(true);
        SavePanel.SetActive(true);
        LoadPanel.SetActive(false);
    }

    public void ShowLoadPanel()
    {
        SetSelSaveLoadButton(null);
        LoadSaveButtonsForLoadPanel();

        SaveLoadRoot.SetActive(true);
        SavePanel.SetActive(false);
        LoadPanel.SetActive(true);
        SaveLoadRoot.transform.SetAsLastSibling();
        LoadPanel.transform.SetAsLastSibling();
    }

    void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
    public bool IsPanelOpen()
    {
        return SaveLoadRoot != null && SaveLoadRoot.activeSelf;
    }
}