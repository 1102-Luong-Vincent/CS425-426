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


    public BattlePlayerUIManager BattlePlayerUIManager;

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

    public void UsedCard(CardValue usedCard)
    {
        if (usedCard == null) return;
        bool removed = HeldCards.Remove(usedCard);
        BattlePlayerUIManager.Instance.RemoveCard(usedCard);
        if (!removed)
        {
            Debug.LogWarning($"Card {usedCard.CardName} not found in HeldCards!");
        }
    }


    #region Get

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
