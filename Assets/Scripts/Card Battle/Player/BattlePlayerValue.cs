// Author: Shawn Meng
// Created by: Shawn Meng and Vincent Luong
// No external sources were used

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BattlePlayerValue : MonoBehaviour
{
    public static BattlePlayerValue Instance { get; private set; }

    private WeaponValue weapon;
    private List<CardValue> HeldCards = new List<CardValue>();
    private List<CardValue> BattleCards = new List<CardValue>();

    #region MaxHealth and Health
    private int maxHealth;
    private int health;

    public int MaxHealth
    {
        get => maxHealth;
        set
        {
            if (maxHealth != value)
            {
                maxHealth = value;
                OnMaxHealthChanged?.Invoke(maxHealth);

                if (health > maxHealth) Health = maxHealth;
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

    private event Action<int> OnMaxHealthChanged;
    private event Action<int> OnHealthChanged;

    #endregion


    public State state;
    public class State
    {
        public int Attack = 10;
        public int Defense = 10;

        public bool isBleeding = false;
        public bool isPoisoned = false;

        public float AttackBuff = 1f;
        public float DefenseBuff = 1f;
        public float CriticalDamageBuff = 1.5f;
        public float CriticalChanceBuff = 0.5f;

    }

    public BattlePlayerUIManager BattlePlayerUIManager;

    private BattlePlayerSnapshot startingBattleState;

    [Serializable]
    private class BattlePlayerSnapshot
    {
        public List<CardValue> HeldCards;
        public List<CardValue> BattleCards;
        public WeaponValue Weapon;
        public int Health;
        public int MaxHealth;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetBattlePlayerValue(PlayerValue playerValue)
    {
        state = new State();

        HeldCards = playerValue.EquipmentCards;
        BattleCards = playerValue.battleCardsList;
        weapon = playerValue.EquipmentWeapon;

        MaxHealth = playerValue.GetHealth();
        Health = playerValue.GetHealth();

        BattlePlayerUIManager.SetPlayer(this);
    }

    public void StartTurn()
    {
        DrawCard();    
    }

    public void DrawCard()
    {
        if (BattleCards.Count == 0)
        {
            Debug.LogWarning("No cards left to draw!");
            return;
        }

        int index = UnityEngine.Random.Range(0, BattleCards.Count);
        CardValue drawnCard = BattleCards[index];
        HeldCards.Add(drawnCard);
        BattleCards.RemoveAt(index);
        BattlePlayerUIManager.AddNewCard(drawnCard);
    }

    public void RemoveCard(CardValue usedCard)
    {
        if (usedCard == null) return;
        bool removed = HeldCards.Remove(usedCard);
        BattlePlayerUIManager.Instance.RemoveCard(usedCard);
        if (!removed)
        {
            Debug.LogWarning($"Card {usedCard.CardName} not found in HeldCards!");
        }
    }

    public void AddHealth(int healthToAdd)
    {
        if (health <= 0) return;
        AddHealthAmount(healthToAdd);
    }

    public void AddHealth(float healthPercentage)
    {
        if (healthPercentage <= 0) return;
        int healthToAdd = Mathf.RoundToInt(MaxHealth * healthPercentage);
        AddHealthAmount(healthToAdd);
    }

    void AddHealthAmount(int healthToAdd)
    {
        int HealthAmount = Mathf.Min(healthToAdd, MaxHealth - Health);
        Health += HealthAmount;
        Health = Mathf.Clamp(Health, 0, MaxHealth);
    }

    public void IncreasesCriticalDamage(float IncreasesPercentage)
    {
       state.CriticalChanceBuff += IncreasesPercentage;
    }

    public void IncreasesCriticalDamageChance(float IncreasesPercentage)
    {
        state.CriticalChanceBuff += IncreasesPercentage;
    }

    public void AddAttack(int attackAmount)
    {
        state.Attack += attackAmount;
    }

    public void AddAttack(float increasesPercentage)
    {
        int attackToAdd = Mathf.RoundToInt(state.Attack * increasesPercentage);
        state.Attack += attackToAdd;
    }

    // Defense 相关
    public void AddDefense(int defenseAmount)
    {
        state.Defense += defenseAmount;
        state.Defense = Mathf.Max(0, state.Defense);
    }

    public void AddDefense(float increasesPercentage)
    {
        int defenseToAdd = Mathf.RoundToInt(state.Defense * increasesPercentage);
        AddDefense(defenseToAdd);
    }

    public void ReduceDefense(int defenseAmount)
    {
        AddDefense(-defenseAmount); 
    }

    public void ReduceDefense(float decreasePercentage)
    {
        int defenseToReduce = Mathf.RoundToInt(state.Defense * decreasePercentage);
        AddDefense(-defenseToReduce);
    }

    public void ReduceHealth(float healthPercentage)
    {
        if (healthPercentage <= 0) return;
        int healthToReduce = Mathf.RoundToInt(MaxHealth * healthPercentage);
        ReduceHealth(healthToReduce);
    }

    public void ReduceHealth(int healthAmount)
    {
        if (healthAmount <= 0) return;
        int newHealth = Mathf.Max(1, Health - healthAmount);
        Health = newHealth;
    }

    #region Get

    public DamageResult GetDamageDetailed(float multiplier, float hitChance)
    {
        DamageResult result = new DamageResult();

        result.IsHit = UnityEngine.Random.value < hitChance;
        if (!result.IsHit)
        {
            result.Damage = 0;
            result.IsCritical = false;
            return result;
        }

        //float baseDamage = state.Attack * state.AttackBuff * multiplier;
        float baseDamage = state.Attack + multiplier;
        baseDamage *= state.AttackBuff;

        result.IsCritical = UnityEngine.Random.value < state.CriticalChanceBuff;
        if (result.IsCritical)
        {
            baseDamage *= state.CriticalDamageBuff;
        }

        result.Damage = Mathf.RoundToInt(baseDamage);
        return result;
    }

    public void CaptureStartingState()
    {
        startingBattleState = new BattlePlayerSnapshot()
        {
            HeldCards = new List<CardValue>(HeldCards),
            BattleCards = new List<CardValue>(BattleCards),
            Weapon = weapon,
            Health = Health,
            MaxHealth = MaxHealth
        };
    }

    public void RestoreStartingState()
    {
        if (startingBattleState == null) return;

        HeldCards = new List<CardValue>(startingBattleState.HeldCards);
        BattleCards = new List<CardValue>(startingBattleState.BattleCards);
        weapon = startingBattleState.Weapon;
        Health = startingBattleState.Health;
        MaxHealth = startingBattleState.MaxHealth;

        // Clear and update UI
        BattlePlayerUIManager.ClearAllCardUI();
        foreach (var card in HeldCards)
        {
            BattlePlayerUIManager.AddNewCard(card);
        }

        // Update UI
        BattlePlayerUIManager.SetPlayer(this);
    }
    public List<CardValue> GetBattleCards() => HeldCards;
    public WeaponValue GetWeapon() => weapon;

    #endregion
    #region Listener with bool control
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
}
