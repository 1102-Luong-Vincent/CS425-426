// Author: Vincent Luong
// Created by: Vincent Luong
// Modified by: Vincent Luong
// no external source was used.

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class PlayerHUDController : MonoBehaviour
{
    public Image playerIcon;        // portrait icon
    public Slider healthBar;        // health bar UI
    public TMP_Text healthText;     // health text
    public TMP_Text objectiveText;  // current objective text

    private const string ObjectiveHeader = "Objectives:";
    private static readonly Color ObjectivePanelColor = new Color(0f, 0f, 0f, 0.72f);
    private static readonly Vector2 ObjectivePanelPadding = new Vector2(18f, 14f);
    private const float ObjectivePanelMinWidth = 520f;
    private const float ObjectivePanelMinHeight = 160f;
    private bool showObjective = true;
    private bool isObjectivePanelOpen = false;
    private GameObject objectivePanel;
    private RectTransform objectivePanelRect;
    private RectTransform objectiveTextRect;
    PlayerValue player;

    void Start()
    {
        RefreshPlayerReference();

        if (player != null)
        {
            healthBar.maxValue = player.GetMaxHealth();
        }

        SetupObjectivePanel();
        UpdateHUD();
    }

    void Update()
    {
        if (showObjective && Time.timeScale > 0f && Input.GetKeyDown(KeyCode.T))
        {
            isObjectivePanelOpen = !isObjectivePanelOpen;
        }

        UpdateHUD();
    }

    void UpdateHUD()
    {
        RefreshPlayerReference();
        if (player == null)
        {
            return;
        }

        int currentHealth = player.GetHealth();
        int maxHealth = player.GetMaxHealth();

        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        if (healthText != null)
        {
            healthText.text = currentHealth + " / " + maxHealth;
        }

        if (objectiveText != null)
        {
            string objectiveDisplayText = BuildObjectiveText();
            bool hasObjectiveContent = !string.IsNullOrWhiteSpace(objectiveDisplayText);

            objectiveText.text = objectiveDisplayText;
            ResizeObjectivePanel();
            SetObjectivePanelActive(showObjective && isObjectivePanelOpen && hasObjectiveContent);
        }
    }

    private void RefreshPlayerReference()
    {
        if (GameValue.Instance != null)
        {
            player = GameValue.Instance.playerValue;
        }
    }

    public void SetObjectiveVisible(bool visible)
    {
        showObjective = visible;
        SetObjectivePanelActive(showObjective && isObjectivePanelOpen && !string.IsNullOrWhiteSpace(BuildObjectiveText()));
    }

    private void SetupObjectivePanel()
    {
        if (objectiveText == null || objectivePanel != null)
        {
            return;
        }

        objectiveTextRect = objectiveText.rectTransform;
        RectTransform originalParent = objectiveTextRect.parent as RectTransform;
        if (originalParent == null)
        {
            return;
        }

        Vector2 originalAnchoredPosition = objectiveTextRect.anchoredPosition;
        Vector2 originalSize = objectiveTextRect.sizeDelta;
        Vector2 originalAnchorMin = objectiveTextRect.anchorMin;
        Vector2 originalAnchorMax = objectiveTextRect.anchorMax;
        Vector2 originalPivot = objectiveTextRect.pivot;
        int originalSiblingIndex = objectiveTextRect.GetSiblingIndex();

        objectivePanel = new GameObject("ObjectivePanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        objectivePanelRect = objectivePanel.GetComponent<RectTransform>();
        objectivePanelRect.SetParent(originalParent, false);
        objectivePanelRect.SetSiblingIndex(originalSiblingIndex);
        objectivePanelRect.anchorMin = originalAnchorMin;
        objectivePanelRect.anchorMax = originalAnchorMax;
        objectivePanelRect.pivot = originalPivot;
        objectivePanelRect.anchoredPosition = originalAnchoredPosition + new Vector2(-ObjectivePanelPadding.x, ObjectivePanelPadding.y);
        objectivePanelRect.sizeDelta = new Vector2(
            Mathf.Max(originalSize.x + ObjectivePanelPadding.x * 2f, ObjectivePanelMinWidth),
            Mathf.Max(originalSize.y + ObjectivePanelPadding.y * 2f, ObjectivePanelMinHeight));

        Image panelImage = objectivePanel.GetComponent<Image>();
        panelImage.color = ObjectivePanelColor;
        panelImage.raycastTarget = false;

        objectiveTextRect.SetParent(objectivePanelRect, false);
        objectiveTextRect.anchorMin = Vector2.zero;
        objectiveTextRect.anchorMax = Vector2.one;
        objectiveTextRect.pivot = new Vector2(0f, 1f);
        objectiveTextRect.anchoredPosition = new Vector2(ObjectivePanelPadding.x, -ObjectivePanelPadding.y);
        objectiveTextRect.sizeDelta = new Vector2(-ObjectivePanelPadding.x * 2f, -ObjectivePanelPadding.y * 2f);
        objectiveText.textWrappingMode = TextWrappingModes.Normal;
        objectiveText.raycastTarget = false;

        SetObjectivePanelActive(false);
    }

    private void ResizeObjectivePanel()
    {
        if (objectivePanelRect == null || objectiveText == null)
        {
            return;
        }

        objectiveText.ForceMeshUpdate(true);
        float preferredHeight = objectiveText.preferredHeight + ObjectivePanelPadding.y * 2f;
        Vector2 size = objectivePanelRect.sizeDelta;
        size.y = Mathf.Max(preferredHeight, ObjectivePanelMinHeight);
        objectivePanelRect.sizeDelta = size;
    }

    private void SetObjectivePanelActive(bool active)
    {
        if (objectivePanel != null)
        {
            objectivePanel.SetActive(active);
        }
        else if (objectiveText != null)
        {
            objectiveText.gameObject.SetActive(active);
        }
    }

    private string BuildObjectiveText()
    {
        if (GameValue.Instance == null)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        var completed = GameValue.Instance.GetCompletedObjectives();
        string current = GameValue.Instance.GetCurrentObjective();
        var completedOptional = GameValue.Instance.GetCompletedOptionalObjectives();
        string currentOptional = GameValue.Instance.GetCurrentOptionalObjective();

        if (completed.Count == 0 &&
            completedOptional.Count == 0 &&
            string.IsNullOrWhiteSpace(current) &&
            string.IsNullOrWhiteSpace(currentOptional))
        {
            return string.Empty;
        }

        builder.Append(ObjectiveHeader);

        foreach (string completedObjective in completed)
        {
            builder.Append("\n");
            builder.Append("<color=#BDBDBD><s>");
            builder.Append(completedObjective);
            builder.Append("</s></color>");
        }

        if (!string.IsNullOrWhiteSpace(current))
        {
            builder.Append("\n");
            builder.Append(current);
        }

        foreach (string optionalObjective in completedOptional)
        {
            builder.Append("\n");
            builder.Append("<color=#BDBDBD><s>");
            builder.Append(optionalObjective);
            builder.Append("</s></color>");
        }

        if (!string.IsNullOrWhiteSpace(currentOptional))
        {
            builder.Append("\n");
            builder.Append("<color=#FFD166>");
            builder.Append(currentOptional);
            builder.Append("</color>");
        }

        return builder.ToString();
    }
}
