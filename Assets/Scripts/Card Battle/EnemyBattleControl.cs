// Authors: Vincent Luong and Shawn Meng
// Created by: Shawn Meng
// Modified by: Vincent Luong
// Some code generated with assistance from ChatGPT.

using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBattleControl : MonoBehaviour
{
    public TextMeshProUGUI enemyNameText;
    public Image enemyImage;
    public Slider healthBar;
    public TextMeshProUGUI healthText;
    private EnemyValue enemyValue;
    public EnemyValue EnemyValueReference => enemyValue;
    int enemyID;
    public List<RuntimeAnimatorController> enemyAnimators = new List<RuntimeAnimatorController>();


    public void Init(EnemyValue enemyValue)
    {
        this.enemyValue = enemyValue;

        enemyNameText.text = enemyValue.EnemyName;
        enemyImage.sprite = enemyValue.GetSprite();
        SetHealth();
        Listener(true);
    }


    void SetHealth()
    {
        UpdateMaxHealthUI(enemyValue.MaxHealth);
        UpdateHealthUI(enemyValue.Health);

    }


    void  Listener(bool isAdd)
    {
        if (enemyValue != null)
        {
            enemyValue.HealthListener(UpdateHealthUI, isAdd);
            enemyValue.MaxHealthListener(UpdateMaxHealthUI, isAdd);
        }
    }

    private void OnDestroy()
    {
        Listener(false);
    }

    private void UpdateHealthUI(int currentHealth)
    {
        if (healthBar != null)
            healthBar.value = currentHealth;

        if (healthText != null)
            healthText.text = $"{currentHealth}/{enemyValue.MaxHealth}";
    }

    private void UpdateMaxHealthUI(int maxHealth)
    {
        if (healthBar != null)
            healthBar.maxValue = maxHealth;

        if (healthText != null)
            healthText.text = $"{enemyValue.Health}/{maxHealth}";
    }


    public void DealDamage(int amount)
    {
        enemyValue.Health -= amount;

        Debug.Log($"Enemy took {amount} damage! has {enemyValue.Health} health left");

        if (enemyValue.Health <= 0)
        {
            Debug.Log("Enemy died!");
            BattleEnemyManager.Instance.currentEnemys.Remove(this);
            Destroy(gameObject);
        }
    }
}

