using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    [SerializeField] PauseControl pauseControl;
    [SerializeField] SaveLoadPanelControl saveLoadPanelControl;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }





}
