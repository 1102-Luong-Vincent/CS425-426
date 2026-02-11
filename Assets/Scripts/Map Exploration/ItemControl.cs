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
    Custom             // Custom effect
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
    [SerializeField] private ItemInteractionType interactionType = ItemInteractionType.SceneTransition;

    [Header("Custom Effects")]
    [SerializeField] private UnityEvent onInteract; // Configurable event in Unity Inspector

    [Header("Interaction Window Pop Up")]
    [SerializeField] GameObject windowPanel;
    [SerializeField] Button backButton;

    [Header("Item Pick Up ID")]
    [SerializeField] int ItemID = 1;


    [Header("Weapon Pick Up ID")]
    //[SerializeField] private WeaponInteractionType weaponType = WeaponInteractionType.Card;
    [SerializeField] public int weaponID;
    [SerializeField] public bool isWeaponPickup = false;

    [Header("Sound Effects")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip interactionSound;
    [SerializeField] AudioClip pickupSound;

    [Header("Pickup Icon")]
    [SerializeField] private Sprite pickupIcon;
    // Whether player is in interaction range
    private bool playerInRange = false;
    private PlayerController currentPlayer = null;


    private void Start()
    {
        HideUI();
        windowPanel.SetActive(false);
        backButton.onClick.AddListener(() =>
        {
            windowPanel.SetActive(false);
            Time.timeScale = 1f; // Resume the game
        });

        //ItemID = CardValue.Instance.itemID;
    }

    private void Update()
    {
        // Player in range and pressed interaction key
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            TriggerInteraction();

            if (interactionType != ItemInteractionType.Pickup && interactionSound != null)
            {
                audioSource.PlayOneShot(interactionSound);
                windowPanel.SetActive(true);
                Time.timeScale = 0f; //pause the game
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("On trigger Enter");
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            playerInRange = true;
            currentPlayer = player;
            ShowUI();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
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
        }

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

            if (AddCard != null)
            {
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
            InteractableNotification.Instance.ShowNotification(itemName, pickupIcon);
        }
        // Destroy item
        Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);
    }

    /// <summary>
    /// Dialogue effect - Can be overridden in derived classes
    /// </summary>
    protected virtual void StartDialogue()
    {
        Debug.Log($"Starting dialogue: {gameObject.name}");

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

    // ================= Public Methods: For External Calls =================

    /// <summary>
    /// Force trigger interaction (can be called from scripts)
    /// </summary>
    public void ForceInteract()
    {
        TriggerInteraction();
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