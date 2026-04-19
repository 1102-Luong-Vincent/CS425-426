// Author: Vincent Luong
// Created by: Vincent Luong
// Modified by: Vincent Luong
// No external source was used

using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyInteractable : Interactable
{
    public string keyID;

    [Header("Save Persistence")]
    [SerializeField] private bool persistCollectedState = true;
    [SerializeField] private string persistentSaveId = string.Empty;

    [Header("Objective Update")]
    [SerializeField] private bool updateObjectiveOnPickup = false;
    [SerializeField, TextArea(2, 3)] private string objectiveAfterPickup = string.Empty;

    private void Awake()
    {
        if (persistCollectedState &&
            GameValue.Instance != null &&
            GameValue.Instance.IsCollectedInteractable(GetPersistentSaveId()))
        {
            Destroy(gameObject);
        }
    }

    protected override void Interact()
    {
        PlayerValue player = GameValue.Instance.GetPlayerValue();

        if (!player.HasKey(keyID))
        {
            player.AddKeyInteractable(keyID);

            Debug.Log("Picked up key: " + keyID);

            // Show pickup notification
            if (InteractableNotification.Instance != null)
            {
                InteractableNotification.Instance.ShowNotification("Key: " + keyID);
            }

            if (updateObjectiveOnPickup && !string.IsNullOrWhiteSpace(objectiveAfterPickup))
            {
                GameValue.Instance.SetCurrentObjective(objectiveAfterPickup);
            }

            if (persistCollectedState)
            {
                GameValue.Instance.MarkCollectedInteractable(GetPersistentSaveId());
            }

            // Optional: trigger story
            // StoryManage.Instance.SetStory("FoundKey");
        }
        else
        {
            Debug.Log("Player already has key: " + keyID);
        }
    }

    private string GetPersistentSaveId()
    {
        if (!string.IsNullOrWhiteSpace(persistentSaveId))
        {
            return persistentSaveId;
        }

        Vector3 position = transform.position;
        return $"{SceneManager.GetActiveScene().name}:{gameObject.name}:KeyInteractable:{keyID}:{Mathf.RoundToInt(position.x * 100f)}:{Mathf.RoundToInt(position.y * 100f)}:{Mathf.RoundToInt(position.z * 100f)}";
    }
}
