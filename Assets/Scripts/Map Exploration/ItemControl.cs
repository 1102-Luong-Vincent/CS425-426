// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng
// some code generated with assistance from ChatGPT.

using SmallScaleInc.TopDownPixelCharactersPack1;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

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

    // Whether player is in interaction range
    private bool playerInRange = false;
    private PlayerController currentPlayer = null;


    private void Start()
    {
        HideUI();
    }

    private void Update()
    {
        // Player in range and pressed interaction key
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            TriggerInteraction();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            playerInRange = true;
            currentPlayer = player;
            ShowUI();
        }
    }

    private void OnTriggerExit(Collider other)
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
    }

    /// <summary>
    /// Pickup item effect - Can be overridden in derived classes
    /// </summary>
    protected virtual void PickupItem()
    {
        Debug.Log($"Picked up item: {gameObject.name}");

        // Example: Add to player inventory
        // InventoryManager.Instance.AddItem(itemData);

        // Play pickup sound
        // AudioManager.Instance.PlaySound("PickupSound");

        // Destroy item
        Destroy(gameObject);
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