// Author: Vincent Luong
// Created by: Vincent Luong
// Modified by: Vincent Luong
// No external source was used.

using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private const string BattleSceneName = "BattleScene";
    private const string MainMenuSceneName = "MainMenuScene";

    public static UIManager Instance;

    [SerializeField] FadeTransition fadeTransition;
    [SerializeField] MiniMapController miniMapController;
    [SerializeField] GameObject playerHealthHUD;


    private void Awake()
    {
        Debug.Log($"UIManager Awake: {name}, instanceID={GetInstanceID()}, scene={gameObject.scene.name}");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitMiniMapController();
        ApplyMiniMapVisibility(SceneManager.GetActiveScene().name);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void FadeToScene(SceneType scene,Vector3 pos)
    {
        fadeTransition.FadeToScene(scene,pos);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (string.Equals(scene.name, MainMenuSceneName, System.StringComparison.OrdinalIgnoreCase))
        {
            Destroy(gameObject);
            return;
        }

        ApplyMiniMapVisibility(scene.name);

        if (playerHealthHUD == null)
        {
            return;
        }

        if (string.Equals(scene.name, BattleSceneName, System.StringComparison.OrdinalIgnoreCase))
        {
            playerHealthHUD.SetActive(false);
        }
        else
        {
            playerHealthHUD.SetActive(true);
        }
    }

    private void InitMiniMapController()
    {
        if (miniMapController == null)
        {
            miniMapController = GetComponentInChildren<MiniMapController>(true);
        }
    }

    private void ApplyMiniMapVisibility(string sceneName)
    {
        InitMiniMapController();

        if (miniMapController == null)
        {
            return;
        }

        miniMapController.gameObject.SetActive(ShouldShowMiniMap(sceneName));
    }

    private bool ShouldShowMiniMap(string sceneName)
    {
        return !string.Equals(sceneName, BattleSceneName, System.StringComparison.OrdinalIgnoreCase)
            && !string.Equals(sceneName, MainMenuSceneName, System.StringComparison.OrdinalIgnoreCase);
    }


}
