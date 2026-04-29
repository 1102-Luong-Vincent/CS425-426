// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng and Vincent Luong
// no external sources were used

using SmallScaleInc.TopDownPixelCharactersPack1;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

// Item interaction type enumeration
public enum ItemInteractionType
{
    SceneTransition,    // Scene transition
    Pickup,            // Pickup item
    Dialogue,          // Start dialogue
    Custom,             // Custom effect
    KeyPickup
}

public class ItemControl : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    public SceneType targetScene = SceneType.None;

    [Header("UI Settings")]
    public Canvas UI;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private string interactionHint = "Press E to interact";

    [Header("Interaction Type")]
    [SerializeField] public ItemInteractionType interactionType = ItemInteractionType.SceneTransition;

    [Header("Custom Effects")]
    [SerializeField] private UnityEvent onInteract; // Configurable event in Unity Inspector

    [Header("Interaction Window Pop Up")]
    [SerializeField] GameObject windowPanel;
    [SerializeField] Button backButton;

    [Header("Item Pick Up ID")]
    [SerializeField] int ItemID = 1;

    [Header("Key Settings")]
    [SerializeField] private bool isKeyPickup = false;


    [Header("Weapon Pick Up ID")]
    //[SerializeField] private WeaponInteractionType weaponType = WeaponInteractionType.Card;
    [SerializeField] public int weaponID;
    [SerializeField] public bool isWeaponPickup = false;

    [Header("Sound Effects")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip interactionSound;
    [SerializeField] AudioClip pickupSound;
    [SerializeField] AudioClip keyPickupSound;
    

    [Header("Pickup Icon")]
    [SerializeField] private Sprite pickupIcon;

    [Header("Save Persistence")]
    [SerializeField] private bool persistCollectedState = true;
    [SerializeField] private string persistentSaveId = string.Empty;

    [Header("Objective Update")]
    [SerializeField] private bool updateObjectiveOnPickup = false;
    [SerializeField, TextArea(2, 3)] private string objectiveAfterPickup = string.Empty;
    [SerializeField] private bool updateObjectiveOnInteract = false;
    [SerializeField, TextArea(2, 3)] private string objectiveAfterInteract = string.Empty;
    [SerializeField, TextArea(2, 3)] private string requiredCurrentObjectiveForObjectiveUpdate = string.Empty;
    [SerializeField] private bool allowReturningToCompletedObjective = false;
    // Whether player is in interaction range
    private bool playerInRange = false;
    private PlayerController currentPlayer = null;

    bool isPickedUp = false;

    private void Start()
    {
        if (ShouldHideAsCollected())
        {
            Destroy(gameObject);
            return;
        }

        HideUI();
        if (windowPanel != null)
        {
            windowPanel.SetActive(false);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(CloseWindow);
        }

        //ItemID = CardValue.Instance.itemID;
    }

    private void Update()
    {
        //disables the interaction key E after initial pickup to prevent spamming
        if (isPickedUp)
        {
            return;
        }
        if(windowPanel != null && windowPanel.activeSelf)
        {
            if(Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(interactionKey))
            {
                CloseWindow();
                return;
            }
        }
        // Player in range and pressed interaction key
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            TriggerInteraction();

            //if (interactionType != ItemInteractionType.Pickup || interactionType != ItemInteractionType.KeyPickup && interactionSound != null)
            //{
            //    audioSource.PlayOneShot(interactionSound);
            //    windowPanel.SetActive(true);
            //    Time.timeScale = 0f; //pause the game
            //}

            if ((interactionType == ItemInteractionType.Dialogue || interactionType == ItemInteractionType.Custom || interactionType == ItemInteractionType.SceneTransition) && interactionSound != null)
            {
                audioSource.PlayOneShot(interactionSound);
                OpenWindow();
                SetObjectiveVisible(false);
                Time.timeScale = 0f;
            }

            if((interactionType == ItemInteractionType.KeyPickup))
            {
                audioSource.PlayOneShot(keyPickupSound);
            }
        }
    }

    public void OpenWindow()
    {
        windowPanel.SetActive(true);

        RectTransform rt = windowPanel.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        SetObjectiveVisible(false);
        SoundManage.Instance.StopFootSteps();
        Time.timeScale = 0f;
    }
    public void CloseWindow()
    {
        if (windowPanel != null)
        {
            windowPanel.SetActive(false);
        }

        SetObjectiveVisible(true);
        Time.timeScale = 1f; // Resume the game
    }

    private void SetObjectiveVisible(bool visible)
    {
        PlayerHUDController hud = FindFirstObjectByType<PlayerHUDController>();
        if (hud != null)
        {
            hud.SetObjectiveVisible(visible);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("On trigger Enter");
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            playerInRange = true;
            currentPlayer = player;
            ShowUI();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            playerInRange = false;
            currentPlayer = null;
            HideUI();
        }
    }

    // Show UI
    private void ShowUI()
    {
        if (UI != null)
        {
            UI.gameObject.SetActive(true);

            // Optional: Update UI text to show interaction hint
            UnityEngine.UI.Text uiText = UI.GetComponentInChildren<UnityEngine.UI.Text>();
            if (uiText != null)
            {
                uiText.text = interactionHint;
            }
        }
    }

    // Hide UI
    private void HideUI()
    {
        if (UI != null)
        {
            UI.gameObject.SetActive(false);
        }
    }

    // Trigger interaction effect
    private void TriggerInteraction()
    {
        switch (interactionType)
        {
            case ItemInteractionType.SceneTransition:
                LoadScene();
                break;

            case ItemInteractionType.Pickup:
                PickupItem();
                break;

            case ItemInteractionType.Dialogue:
                StartDialogue();
                break;

            case ItemInteractionType.Custom:
                ExecuteCustomEffect();
                break;

            case ItemInteractionType.KeyPickup:
                PickupKeyItem();
                break;
        }

        UpdateObjectiveAfterInteract();

        // Invoke custom event (triggered regardless of interaction type)
        onInteract?.Invoke();
    }

    // ================= Interface Methods: Different Effect Implementations =================

    /// <summary>
    /// Scene transition effect
    /// </summary>
    private void LoadScene()
    {
        if (targetScene == SceneType.None)
        {
            Debug.LogWarning("Target scene not set!");
            return;
        }

        Debug.Log($"Transitioning to scene: {targetScene}");
        GameValue.Instance.LoadSceneByEnum(targetScene);
        //UIManager.Instance.FadeToScene(targetScene, currentPlayer.transform.position);
    }

    /// <summary>
    /// Pickup item effect - Can be overridden in derived classes
    /// </summary>
    public void PickupItem()
    {
        Debug.Log($"Picked up item: {gameObject.name}");

        if (pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
        //if(InventoryManager.Instance != null && InventoryManager.Instance.control != null)
        //{
        //    InventoryManager.Instance.control.refreshInventory();
        //}

        //InventoryManager.Instance.AddItem(itemData);

        // Play pickup sound
        // AudioManager.Instance.PlaySound("PickupSound");

        string itemName = "";

        if (isWeaponPickup)
        {
            WeaponValue weapon = GameValue.Instance.GetGameValueLibrary().GetInitWeapon(weaponID);

            if (weapon != null)
            {
                GameValue.Instance.GetPlayerValue().HadWeaponsLibrary.Add(weapon);
                itemName = weapon.WeaponName;
                isPickedUp = true; //prevents spamming when interactable is picked up
                Debug.Log($"Added weapon {weaponID} to inventory.");
            }
            else
            {
                Debug.LogWarning($"Weapon with ID {weaponID} not found in library.");
            }
        }
        else
        {
            CardValue AddCard = GameValue.Instance.GetGameValueLibrary().GetInitCard(ItemID);
            isPickedUp = true; //prevents spamming when interactable is picked up

            if (AddCard != null)
            {
                // roll for rarity
                int rarityRoll = Random.Range(0, 100);
                if(rarityRoll > 0 && rarityRoll < 70)
                {
                    AddCard.rarity = CardRarity.Common;
                }
                else if(rarityRoll >= 70 && rarityRoll < 85)
                {
                    AddCard.rarity = CardRarity.Rare;
                }
                else if (rarityRoll >= 85 && rarityRoll < 95)
                {
                    AddCard.rarity = CardRarity.VeryRare;
                }
                else
                {
                    AddCard.rarity = CardRarity.Epic;
                }
                    GameValue.Instance.GetPlayerValue().HadCardsLibrary.Add(AddCard);
                itemName = AddCard.CardName;
                Debug.Log($"Added card ID {ItemID} to inventory.");
            }
            else
            {
                Debug.LogWarning($"Item with ID {ItemID} not found in library.");
            }
        }

        if (!string.IsNullOrEmpty(itemName))
        {
            if(pickupIcon == null)
            {
                Debug.Log("Where pickupicon at");
            }
            InteractableNotification.Instance.ShowNotification(itemName, pickupIcon);
            Debug.Log("grabbed? " + itemName);
        }

        if (TryTriggerEndingOnPickup())
        {
            return;
        }

        UpdateObjectiveAfterPickup();
        MarkCollectedForSave();

        // Destroy item
        Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);
    }

    private bool TryTriggerEndingOnPickup()
    {
        InteractableEnding ending = GetComponent<InteractableEnding>();
        return ending != null && ending.TryTriggerEnding();
    }

    /// <summary>
    /// Dialogue effect - Can be overridden in derived classes
    /// </summary>
    protected virtual void StartDialogue()
    {
        Debug.Log($"Starting dialogue: {gameObject.name}");

        //Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);

        // Example: Trigger dialogue system
        // DialogueManager.Instance.StartDialogue(dialogueData);
    }

    /// <summary>
    /// Custom effect - Can be overridden in derived classes
    /// </summary>
    protected virtual void ExecuteCustomEffect()
    {
        Debug.Log($"Executing custom effect: {gameObject.name}");

        // Implement your custom logic here
        // Examples: Open chest, activate mechanism, heal player, etc.
    }

    public void PickupKeyItem()
    {
        Debug.Log($"Picked up key: {ItemID}");

        if (keyPickupSound != null)
        {
            audioSource.PlayOneShot(keyPickupSound);
        }

        PlayerValue player = GameValue.Instance.GetPlayerValue();
        CardValue keyItem = GameValue.Instance.GetGameValueLibrary().GetInitCard(ItemID);

        string itemName = "";

        if (keyItem != null)
        {
            player.HadCardsLibrary.Add(keyItem);
            itemName = keyItem.CardName;
            isPickedUp = true; //prevents spamming when interactable is picked up

            InteractableNotification.Instance.ShowNotification(itemName);
            
            Debug.Log($"Key {ItemID} added to inventory");

            UpdateObjectiveAfterPickup();
            MarkCollectedForSave();

            Destroy(gameObject, keyPickupSound != null ? keyPickupSound.length : 0f);
        }
        else
        {
            Debug.LogWarning($"Key ID {ItemID} not found in library!");
        }
    }

    private void UpdateObjectiveAfterPickup()
    {
        if (updateObjectiveOnPickup &&
            !string.IsNullOrWhiteSpace(objectiveAfterPickup) &&
            CanUpdateObjective(objectiveAfterPickup))
        {
            GameValue.Instance.SetCurrentObjective(objectiveAfterPickup);
        }
    }

    private void UpdateObjectiveAfterInteract()
    {
        if (updateObjectiveOnInteract &&
            !string.IsNullOrWhiteSpace(objectiveAfterInteract) &&
            CanUpdateObjective(objectiveAfterInteract))
        {
            GameValue.Instance.SetCurrentObjective(objectiveAfterInteract);
        }
    }

    private bool CanUpdateObjective(string nextObjective)
    {
        if (string.IsNullOrWhiteSpace(requiredCurrentObjectiveForObjectiveUpdate))
        {
            if (!allowReturningToCompletedObjective &&
                GameValue.Instance.GetCompletedObjectives().Contains(nextObjective))
            {
                return false;
            }

            return true;
        }

        if (!string.Equals(
            GameValue.Instance.GetCurrentObjective(),
            requiredCurrentObjectiveForObjectiveUpdate,
            System.StringComparison.Ordinal))
        {
            return false;
        }

        return allowReturningToCompletedObjective ||
               !GameValue.Instance.GetCompletedObjectives().Contains(nextObjective);
    }

    private bool ShouldHideAsCollected()
    {
        return ShouldPersistCollectedState() &&
               GameValue.Instance != null &&
               GameValue.Instance.IsCollectedInteractable(GetPersistentSaveId());
    }

    private void MarkCollectedForSave()
    {
        if (ShouldPersistCollectedState() && GameValue.Instance != null)
        {
            GameValue.Instance.MarkCollectedInteractable(GetPersistentSaveId());
        }
    }

    private bool ShouldPersistCollectedState()
    {
        return persistCollectedState &&
               (interactionType == ItemInteractionType.Pickup || interactionType == ItemInteractionType.KeyPickup);
    }

    private string GetPersistentSaveId()
    {
        if (!string.IsNullOrWhiteSpace(persistentSaveId))
        {
            return persistentSaveId;
        }

        Vector3 position = transform.position;
        return $"{SceneManager.GetActiveScene().name}:{gameObject.name}:{interactionType}:{ItemID}:{weaponID}:{Mathf.RoundToInt(position.x * 100f)}:{Mathf.RoundToInt(position.y * 100f)}:{Mathf.RoundToInt(position.z * 100f)}";
    }
    // ================= Public Methods: For External Calls =================

    /// <summary>
    /// Force trigger interaction (can be called from scripts)
    /// </summary>
    public void ForceInteract()
    {
        TriggerInteraction();
    }

    public ItemInteractionType GetInteractionType()
    {
        return interactionType;
    }
    /// <summary>
    /// Set interaction type
    /// </summary>
    public void SetInteractionType(ItemInteractionType type)
    {
        interactionType = type;
    }

    /// <summary>
    /// Add custom listener
    /// </summary>
    public void AddInteractionListener(UnityAction action)
    {
        onInteract.AddListener(action);
    }

    /// <summary>
    /// Remove custom listener
    /// </summary>
    public void RemoveInteractionListener(UnityAction action)
    {
        onInteract.RemoveListener(action);
    }


    // ================= Gizmos Visualization =================
    private void OnDrawGizmos()
    {
        // Display interaction range in scene view
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            Gizmos.color = playerInRange ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
}
