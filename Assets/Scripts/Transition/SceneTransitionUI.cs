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


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            confirmationPanel.SetActive(true);

            Time.timeScale = 0f;
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
}
