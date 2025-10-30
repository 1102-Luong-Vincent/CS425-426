using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class EnemyValue
{
    private int ID;
    public string EnemyName;
    public Sprite EnemySprite;

    #region Health
    private int health;
    private int maxHealth;

    public int MaxHealth
    {
        get => maxHealth;
        set
        {
            if (maxHealth != value)
            {
                maxHealth = Mathf.Max(1, value); 
                if (health > maxHealth) Health = maxHealth;
                OnMaxHealthChanged?.Invoke(maxHealth);
            }
        }
    }

    public int Health
    {
        get => health;
        set
        {
            int newValue = Mathf.Clamp(value, 0, MaxHealth);
            if (health != newValue)
            {
                health = newValue;
                OnHealthChanged?.Invoke(health);
            }
        }
    }

    private event Action<int> OnHealthChanged;
    private event Action<int> OnMaxHealthChanged;

    public void HealthListener(Action<int> listener, bool isAdd)
    {
        if (isAdd) OnHealthChanged += listener;
        else OnHealthChanged -= listener;
    }

    public void MaxHealthListener(Action<int> listener, bool isAdd)
    {
        if (isAdd) OnMaxHealthChanged += listener;
        else OnMaxHealthChanged -= listener;
    }
    #endregion

    public int attack;
    public int speed;

    public WeaponValue defaultWeapon;
    public List<int> enemyDeckID = new List<int>();

    public EnemyValue(ExcelEnemyData excelEnemyData)
    {
        ID = excelEnemyData.ID;
        EnemyName = excelEnemyData.enemyName;
        if (string.IsNullOrEmpty(EnemyName))
        {
            Debug.LogError("[SetEnemySprite] EnemyName is null or empty, cannot load sprite.");
            return;
        }

        MaxHealth = excelEnemyData.Health;
        Health = MaxHealth;
        attack = excelEnemyData.attack;
        speed = excelEnemyData.speed;

        SetEnemySprite();
        defaultWeapon = GameValue.Instance.GetInitWeaponValue(excelEnemyData.defaultWeaponID);
        enemyDeckID = excelEnemyData.enemyDeck;
    }

    public Sprite GetSprite() => EnemySprite;

    void SetEnemySprite()
    {
        string path = $"Sprite/Enemy/{EnemyName}/{EnemyName.ToLower()}";
        EnemySprite = Resources.Load<Sprite>(path);
        if (EnemySprite == null)
            Debug.LogWarning($"[LoadEnemySprite] sprite == null, check path or filename! Try load: {path}");
    }
}
