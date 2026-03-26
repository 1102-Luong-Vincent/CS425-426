// Author: Vincent Luong
// Created by: Vincent Luong
// Modified by: Vincent Luong
// no external source was used.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDController : MonoBehaviour
{
    public Image playerIcon;        // portrait icon
    public Slider healthBar;        // health bar UI
    public TMP_Text healthText;     // health text

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

        healthBar.value = currentHealth;

        if (healthText != null)
        {
            healthText.text = currentHealth + " / " + maxHealth;
        }
    }
}
