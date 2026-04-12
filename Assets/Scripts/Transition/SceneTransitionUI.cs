// Author: Vincent Luong
// Created by: Vincent Luong
// Modified by: Vincent Luong
// No external source was used

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionUI : MonoBehaviour
{
    public GameObject confirmationPanel;
    public SceneType sceneToLoad;
    [SerializeField] Vector3 PlayerTransitionPosition;
    bool playerInRange = false;
    public static SceneTransitionUI Instance;


    private void Awake()
    {
        //if (Instance != null && Instance != this)
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        Instance = this;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowConfirmation();
        }
    }

    public void OnConfirmButton()
    {
        Time.timeScale = 1f;
        confirmationPanel.SetActive(false);
        UIManager.Instance.FadeToScene(sceneToLoad, PlayerTransitionPosition);
    }

    public void OnCancelButton()
    {
        confirmationPanel.SetActive(false);
        Time.timeScale = 1f;
        playerInRange = false;
    }

    public void ShowConfirmation()
    {
        confirmationPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}