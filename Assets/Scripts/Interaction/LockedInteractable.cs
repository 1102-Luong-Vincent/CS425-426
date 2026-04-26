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
    [SerializeField] private SceneTransitionUI sceneTransitionUI;
    [Header("Objective Update")]
    [SerializeField] private bool updateObjectiveWhenLocked = false;
    [SerializeField, TextArea(2, 3)] private string objectiveWhenLocked = string.Empty;
    [SerializeField, TextArea(2, 3)] private string requiredCurrentObjectiveForLockedUpdate = string.Empty;

    public PlayerValue playerValue;

    public void Start()
    {
        playerValue = GameValue.Instance.GetPlayerValue();
        ResolveSceneTransitionUI();

        if (doorLockedText != null)
        {
            doorLockedText.gameObject.SetActive(false);
        }
    }

    public void OpenDoor()
    {
        if (sceneTransitionUI != null)
        {
            sceneTransitionUI.ShowConfirmation();
            return;
        }

        Debug.LogWarning($"No {nameof(SceneTransitionUI)} reference found for locked door {gameObject.name}.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() == null)
        {
            return;
        }

        playerValue ??= GameValue.Instance.GetPlayerValue();
        ResolveSceneTransitionUI();

        if (doorLockedText != null)
        {
            doorLockedText.gameObject.SetActive(false);
        }

        sceneTransitionUI?.HideConfirmation();

        if (isUnlocked)
        {
            OpenDoor();
            return;
        }

        for (int i = 0; i < playerValue.HadCardsLibrary.Count; i++)
        {
            if (playerValue.HadCardsLibrary[i].ID == keyID)
            {
                isUnlocked = true;
                bool shouldConsumeKey = keyUsed && !keepKeyAfterUnlock;

                if (shouldConsumeKey)
                {
                    playerValue.HadCardsLibrary.RemoveAt(i);
                }

                Debug.Log("Door unlocked with key");

                if (InteractableNotification.Instance != null)
                {
                    InteractableNotification.Instance.ShowNotification(shouldConsumeKey ? "Used Key" : "Door Unlocked");
                }

                OpenDoor();
                return;
            }
        }

        Debug.Log("Door Locked. Missing key: " + keyID);
        UpdateObjectiveWhenLocked();

        if (doorLockedText != null)
        {
            doorLockedText.text = "Door Locked. Needs a key";
            doorLockedText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() != null && doorLockedText != null)
        {
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

    private void ResolveSceneTransitionUI()
    {
        sceneTransitionUI ??= GetComponent<SceneTransitionUI>();
        sceneTransitionUI ??= GetComponentInChildren<SceneTransitionUI>();
        sceneTransitionUI ??= GetComponentInParent<SceneTransitionUI>();
    }
}
