// Author: Vincent Luong
// Created by: Vincent Luong
// Modified by: Vincent Luong
// No external source was used

using UnityEngine;

public class KeyInteractable : Interactable
{
    public string keyID;

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

            // Optional: trigger story
            // StoryManage.Instance.SetStory("FoundKey");
        }
        else
        {
            Debug.Log("Player already has key: " + keyID);
        }
    }
}