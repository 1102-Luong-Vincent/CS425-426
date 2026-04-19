// Author: Vincent Luong
// Created by: Vincent Luong
// Modified by: Vincent Luong
// No external source was used

using SmallScaleInc.TopDownPixelCharactersPack1;
using TMPro;
using UnityEngine;

public class LockedInteractable : MonoBehaviour
{
    [Header("Door Settings")]
    public int keyID;
    public bool keyUsed = false;
    public bool isUnlocked = false;
    public TextMeshProUGUI doorLockedText;
    [SerializeField] private bool keepKeyAfterUnlock = false;
    [Header("Objective Update")]
    [SerializeField] private bool updateObjectiveWhenLocked = false;
    [SerializeField, TextArea(2, 3)] private string objectiveWhenLocked = string.Empty;
    [SerializeField, TextArea(2, 3)] private string requiredCurrentObjectiveForLockedUpdate = string.Empty;

    private bool playerInRange = false;
    private bool triggered = false;

    public PlayerValue playerValue;

    public void Start()
    {
        playerValue = GameValue.Instance.GetPlayerValue();
        doorLockedText.gameObject.SetActive(false);
        //DontDestroyOnLoad(gameObject);
    }

    public void OpenDoor()
    {
        SceneTransitionUI.Instance.ShowConfirmation();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            SceneTransitionUI.Instance.confirmationPanel.SetActive(false);
            Time.timeScale = 1f;
            doorLockedText.gameObject.SetActive(true);

            if (triggered)
            {
                triggered = true;
            }
            return;
        }

        PlayerValue player = GameValue.Instance.GetPlayerValue();

            if (isUnlocked)
            {
                //OpenDoor();
                SceneTransitionUI.Instance.ShowConfirmation();
                return;
            }

            for (int i = 0; i < player.HadCardsLibrary.Count; i++)
            {
                if (player.HadCardsLibrary[i].ID == keyID)
                {
                    isUnlocked = true;
                    bool shouldConsumeKey = keyUsed && !keepKeyAfterUnlock;

                    if (shouldConsumeKey)
                    {
                        player.HadCardsLibrary.RemoveAt(i);
                    }
                    Debug.Log("Door unlocked with key");

                    if (InteractableNotification.Instance != null)
                    {
                        InteractableNotification.Instance.ShowNotification(shouldConsumeKey ? "Used Key" : "Door Unlocked");
                    }
                    SceneTransitionUI.Instance.ShowConfirmation();
                    return;
                }
            }
        Debug.Log("Door Locked. Missing key: " + keyID);
        UpdateObjectiveWhenLocked();
        doorLockedText.text = "Door Locked. Needs a key";

        SceneTransitionUI.Instance.confirmationPanel.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.GetComponent<PlayerController>() != null)
        { 
            playerInRange = false;
            doorLockedText.gameObject.SetActive(false);
        }
    }

    private void UpdateObjectiveWhenLocked()
    {
        if (!updateObjectiveWhenLocked || string.IsNullOrWhiteSpace(objectiveWhenLocked))
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(requiredCurrentObjectiveForLockedUpdate) &&
            !string.Equals(
                GameValue.Instance.GetCurrentObjective(),
                requiredCurrentObjectiveForLockedUpdate,
                System.StringComparison.Ordinal))
        {
            return;
        }

        if (GameValue.Instance.GetCompletedObjectives().Contains(objectiveWhenLocked))
        {
            return;
        }

        GameValue.Instance.SetCurrentObjective(objectiveWhenLocked);
    }
}
