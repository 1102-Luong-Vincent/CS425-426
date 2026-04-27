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
    private bool showObjective = true;
    PlayerValue player;

    void Start()
    {
        player = GameValue.Instance.playerValue;

        healthBar.maxValue = player.GetMaxHealth();
        UpdateHUD();
    }

    void Update()
    {
        UpdateHUD();
    }

    void UpdateHUD()
    {
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
            string objective = GameValue.Instance != null ? GameValue.Instance.GetCurrentObjective() : string.Empty;
            string optionalObjective = GameValue.Instance != null ? GameValue.Instance.GetCurrentOptionalObjective() : string.Empty;
            bool hasObjective = !string.IsNullOrWhiteSpace(objective);
            bool hasCompletedObjectives = GameValue.Instance != null && GameValue.Instance.GetCompletedObjectives().Count > 0;
            bool hasOptionalObjective = !string.IsNullOrWhiteSpace(optionalObjective);
            bool hasCompletedOptionalObjectives = GameValue.Instance != null && GameValue.Instance.GetCompletedOptionalObjectives().Count > 0;

            objectiveText.gameObject.SetActive(showObjective && (hasObjective || hasCompletedObjectives || hasOptionalObjective || hasCompletedOptionalObjectives));
            objectiveText.text = BuildObjectiveText();
        }
    }

    public void SetObjectiveVisible(bool visible)
    {
        showObjective = visible;
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
