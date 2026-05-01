// Author: Vincent Luong
// Created by: Vincent Luong
// Modified by: Vincent Luong
// No external source was used

using SmallScaleInc.TopDownPixelCharactersPack1;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionUI : MonoBehaviour
{
    public GameObject confirmationPanel;
    public SceneType sceneToLoad;
    [SerializeField] Vector3 PlayerTransitionPosition;
    bool playerInRange = false;
    public static SceneTransitionUI Instance;
    private LockedInteractable lockedInteractable;


    private void Awake()
    {
        Instance = this;
        lockedInteractable = GetComponent<LockedInteractable>();
        lockedInteractable ??= GetComponentInParent<LockedInteractable>();
        lockedInteractable ??= GetComponentInChildren<LockedInteractable>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            playerInRange = true;

            if (lockedInteractable != null)
            {
                return;
            }

            ShowConfirmation();
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            playerInRange = false;
        }
    }

    public void OnConfirmButton()
    {
        HideConfirmation();

        UIManager.Instance.FadeToScene(sceneToLoad, PlayerTransitionPosition);
        
    }

    public void OnCancelButton()
    {
        HideConfirmation();
        playerInRange = false;
    }

    public void ShowConfirmation()
    {
        if (confirmationPanel == null)
        {
            Debug.LogWarning($"No confirmation panel assigned for {gameObject.name}.");
            return;
        }

        confirmationPanel.SetActive(true);
        SoundManage.Instance.StopFootSteps();
        Time.timeScale = 0f;
    }

    public void HideConfirmation()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }
}
