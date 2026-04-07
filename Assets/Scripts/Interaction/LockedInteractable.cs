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

                    if (keyUsed)
                    {
                        player.HadCardsLibrary.RemoveAt(i);
                    }
                    Debug.Log("Door unlocked with key");

                    if (InteractableNotification.Instance != null)
                    {
                        InteractableNotification.Instance.ShowNotification("Used Key");
                    }
                    SceneTransitionUI.Instance.ShowConfirmation();
                    return;
                }
            }
        Debug.Log("Door Locked. Missing key: " + keyID);
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
}