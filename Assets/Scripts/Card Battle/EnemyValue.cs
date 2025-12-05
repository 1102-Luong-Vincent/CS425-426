using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class EnemyBurnStatus
{
    public int turnsLeft;
    public float percentDmg;

    public EnemyBurnStatus(int turns, float p)
    {
        turnsLeft = turns;
        percentDmg = p;
    }
}

public class EnemyPoisonStatus
{
    public int turnsLeft;
    public float percentDmg;

    public EnemyPoisonStatus(int turns, float p)
    {
        turnsLeft = turns;
        percentDmg = p;
    }
}

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

    public int StunTurns;
    public bool isStunned => StunTurns > 0;

    public float tempArmorReduction = 0.2f;
    public int armorReductionTurns = 2;
    public bool HasArmorDebuff => armorReductionTurns > 0;

    //public float tempArmorReductionPercent
    //public int armorDebuffTurns = 0;

    public int stunTurns = 0;
    public bool IsStunned => stunTurns > 0;

    public bool isConfused = false;
    public int confusedTurns = 0;

    public EnemyBurnStatus burnStatus;
    public EnemyPoisonStatus poisonStatus;

    public int c4TurnsLeft = 0;
    public float c4DamagePercent = 0f;

    public bool hasMine = false;
    public float mineDamagePercent = 0f;

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

    #region StatusEffects

    public void ApplyBurn(EnemyBurnStatus status)
    {
        burnStatus = status;
    }

    public void TickBurn()
    {
        if (burnStatus != null)
        {
            int dmg = Mathf.RoundToInt(MaxHealth * burnStatus.percentDmg);
            Health -= dmg;
            burnStatus.turnsLeft--;
            if (burnStatus.turnsLeft <= 0)
                burnStatus = null;
        }
    }

    public void ApplyPoison(EnemyPoisonStatus status)
    {
        poisonStatus = status;
    }

    public void TickPoison()
    {
        if (poisonStatus != null)
        {
            int dmg = Mathf.RoundToInt(MaxHealth * poisonStatus.percentDmg);
            Health -= dmg;
            poisonStatus.turnsLeft--;
            if (poisonStatus.turnsLeft <= 0)
                poisonStatus = null;
        }
    }

    public void SetStunned(int turns)
    {
        stunTurns = turns;
    }

    public void TickStun()
    {
        if (stunTurns > 0)
            stunTurns--;
    }

    public void SetConfused(int turns)
    {
        isConfused = true;
        confusedTurns = turns;
    }

    public void TickConfusion()
    {
        if (confusedTurns > 0)
        {
            confusedTurns--;
            if (confusedTurns <= 0)
                isConfused = false;
        }
    }

    public void AttachC4(int turns, float percent)
    {
        c4TurnsLeft = turns;
        c4DamagePercent = percent;
    }

    public void TickC4()
    {
        if (c4TurnsLeft > 0)
        {
            c4TurnsLeft--;
            if (c4TurnsLeft <= 0)
            {
                int dmg = Mathf.RoundToInt(MaxHealth * c4DamagePercent);
                Health -= dmg;
            }
        }
    }

    public void DeployMine(float percent)
    {
        hasMine = true;
        mineDamagePercent = percent;
    }

    public void TriggerMine()
    {
        if (hasMine)
        {
            int dmg = Mathf.RoundToInt(MaxHealth * mineDamagePercent);
            Health -= dmg;
            hasMine = false;
        }
    }

    public void TickArmorDebuff()
    {
        if (armorReductionTurns > 0)
        {
            armorReductionTurns--;
            if (armorReductionTurns <= 0)
                tempArmorReduction = 0f;
        }
    }
}
#endregion