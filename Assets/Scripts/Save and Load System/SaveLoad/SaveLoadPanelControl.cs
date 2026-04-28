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
    private const string AutoSaveFileName = "autosave.json";

    public GameObject SaveLoadRoot;
    public GameObject SavePanel;
    public GameObject LoadPanel;

    public Button SavePanelSaveButton;
    public Button SavePanelCancelButton;

    public Button LoadPanelLoadButton;
    public Button LoadPanelDeleteButton;
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
    private bool isShowingLoadPanel;

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
        EnsurePanelContentParents();
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
        if (LoadPanelDeleteButton != null)
        {
            LoadPanelDeleteButton.onClick.AddListener(OnDeleteButtonClick);
        }
        LoadPanelCancelButton.onClick.AddListener(ClosePanel);
        UpdateLoadPanelButtonStates();
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
        ClearOldAutoSaveFiles();
        SaveGame(autoSavePath, AutoSaveFileName);
    }

    private void SaveGame(string folderPath, string fileName = null)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            fileName = $"save_{System.DateTime.Now:yyyyMMdd_HHmmss}.json";
        }

        string fullPath = Path.Combine(folderPath, fileName);
        SaveData saveData = new SaveData(fullPath, GameValue.Instance);
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(fullPath, json);
        Debug.Log($"Save to: {fullPath}");
    }

    private void ClearOldAutoSaveFiles()
    {
        foreach (string file in Directory.GetFiles(autoSavePath, "*.json"))
        {
            if (!Path.GetFileName(file).Equals(AutoSaveFileName, System.StringComparison.OrdinalIgnoreCase))
            {
                File.Delete(file);
            }
        }
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
        SequenceManager.CancelActiveTutorial();
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
        UpdateLoadPanelButtonStates();
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
        System.Array.Sort(files, (left, right) => File.GetLastWriteTime(right).CompareTo(File.GetLastWriteTime(left)));

        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            if (saveData == null || saveData.IsEmpty())
            {
                continue;
            }
            SaveLoadButtonControl btn = Instantiate(saveLoadButtonPrefab, parent);
            btn.SetSaveData(saveData, file);
        }
    }

    void OnDeleteButtonClick()
    {
        DeleteSelectedSave();
    }

    public void DeleteSelectedSave()
    {
        if (selSaveLoadButton == null) return;

        string saveFilePath = selSaveLoadButton.GetSaveFilePath();
        if (!IsDeletableSaveFile(saveFilePath))
        {
            Debug.LogWarning($"Cannot delete invalid save file: {saveFilePath}");
            return;
        }

        SetSelSaveLoadButton(null);
        File.Delete(saveFilePath);

        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }

        if (isShowingLoadPanel)
        {
            LoadSaveButtonsForLoadPanel();
        }
        else
        {
            LoadSaveButtonsForSavePanel();
        }

        UpdateLoadPanelButtonStates();
    }

    private bool IsDeletableSaveFile(string saveFilePath)
    {
        if (string.IsNullOrWhiteSpace(saveFilePath) || !File.Exists(saveFilePath))
        {
            return false;
        }

        string fullPath = Path.GetFullPath(saveFilePath);
        string normalPath = Path.GetFullPath(normalSavePath);
        string autoPath = Path.GetFullPath(autoSavePath);

        return Path.GetExtension(fullPath).Equals(".json", System.StringComparison.OrdinalIgnoreCase) &&
               (IsPathInsideFolder(fullPath, normalPath) || IsPathInsideFolder(fullPath, autoPath));
    }

    private bool IsPathInsideFolder(string filePath, string folderPath)
    {
        string normalizedFolder = folderPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        return filePath.StartsWith(normalizedFolder, System.StringComparison.OrdinalIgnoreCase);
    }

    private void UpdateLoadPanelButtonStates()
    {
        bool hasSelection = selSaveLoadButton != null;

        if (LoadPanelLoadButton != null)
        {
            LoadPanelLoadButton.interactable = hasSelection;
        }

        if (LoadPanelDeleteButton != null)
        {
            LoadPanelDeleteButton.interactable = hasSelection;
        }
    }

    public void SetPauseControlToCloseAfterLoad(PauseControl pauseControl)
    {
        pauseControlToCloseAfterLoad = pauseControl;
    }

    public void ShowSavePanel()
    {
        EnsurePanelContentParents();
        SetSelSaveLoadButton(null);
        isShowingLoadPanel = false;
        LoadSaveButtonsForSavePanel();
        SaveLoadRoot.SetActive(true);
        SavePanel.SetActive(true);
        LoadPanel.SetActive(false);
    }

    public void ShowLoadPanel()
    {
        EnsurePanelContentParents();
        SetSelSaveLoadButton(null);
        isShowingLoadPanel = true;
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

    private void EnsurePanelContentParents()
    {
        ReparentIfNeeded(SavePanelNormalSaveTransform, SavePanel != null ? SavePanel.transform : null);
        ReparentIfNeeded(SavePanelAutoSaveTransform, SavePanel != null ? SavePanel.transform : null);
        ReparentIfNeeded(LoadPanelNormalSaveTransform, LoadPanel != null ? LoadPanel.transform : null);
        ReparentIfNeeded(LoadPanelAutoSaveTransform, LoadPanel != null ? LoadPanel.transform : null);
    }

    private void ReparentIfNeeded(Transform contentTransform, Transform expectedParent)
    {
        if (contentTransform == null || expectedParent == null || contentTransform.parent == expectedParent)
        {
            return;
        }

        contentTransform.SetParent(expectedParent, false);
    }
}
