// Author: Shawn Meng
// Created by: Shawn Meng and Vincent Luong
// No external sources were used

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
    Animator anim;
    int weaponEqupped = 0;

    private List<CardValue> startingHeldCards;
    private List<CardValue> startingBattleCards;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem healEffectPrefab;
    //[SerializeField] private Transform playerVisual;

    public List<ResourceValue> InventoryResources = new List<ResourceValue>();
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

    private void Start()
    {
        Transform child = transform.Find("DummyPlayer");

        if (child != null)
        {
            anim = child.GetComponent<Animator>();
        }
        SetWeaponAnimation();

     }

    public void SetBattlePlayerValue(PlayerValue playerValue)
    {
        state = new State();

        HeldCards = new List<CardValue>(playerValue.EquipmentCards);
        BattleCards = new List<CardValue>(playerValue.battleCardsList);

        weapon = playerValue.EquipmentWeapon;

        MaxHealth = playerValue.GetMaxHealth();
        Health = playerValue.GetHealth();
        CaptureStartingState();
        BattlePlayerUIManager.SetPlayer(this);
    }

    public void StartTurn(int turnNumber)
    {
        if(turnNumber > 1)
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

    #region Get

    //public List<CardValue> GetBattleCards() => HeldCards;
    //public WeaponValue GetWeapon() => weapon;

    //#endregion

    //#region Listener with bool control

    //public void HealthListener(Action<int> listener, bool isAdd)
    //{
    //    if (isAdd) OnHealthChanged += listener;
    //    else OnHealthChanged -= listener;
    //}

    //public void MaxHealthListener(Action<int> listener, bool isAdd)
    //{
    //    if (isAdd) OnMaxHealthChanged += listener;
    //    else OnMaxHealthChanged -= listener;
    //}

    #endregion

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
        SpawnHealEffect();
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
        if (healthPercentage <= 0)return;

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
            Debug.Log("Critical Hit!");
            baseDamage *= state.CriticalDamageBuff;
        }

        result.Damage = Mathf.RoundToInt(baseDamage);
        return result;
    }

    private void SpawnHealEffect()
    {
        if (healEffectPrefab == null) return;

        ParticleSystem effect = Instantiate(healEffectPrefab, transform.position, Quaternion.identity);

        effect.Play();
        Destroy(effect.gameObject, effect.main.duration + 0.5f);
    }

    public void CaptureStartingState()
    {
        startingHeldCards = new List<CardValue>(HeldCards);
        startingBattleCards = new List<CardValue>(BattleCards);

        startingBattleState = new BattlePlayerSnapshot()
        {
            Weapon = weapon,
            Health = Health,
            MaxHealth = MaxHealth
        };
    }

    public void RestoreStartingState()
    {
        if (startingBattleState == null) return;

        HeldCards = new List<CardValue>(startingHeldCards);
        BattleCards = new List<CardValue>(startingBattleCards);
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

    public void SaveBattleResultToPlayerValue()
    {
        PlayerValue playerValue = GameValue.Instance.GetPlayerValue();
        playerValue.SetHealth(Mathf.Max(1, Health));
    }

    // Utility to add a resource
    public void AddResource(ResourceValue resource)
    {
        if (resource == null) return;
        GameValue.Instance.GetPlayerValue().AddResource(resource);
        // Check if player already has this resource type
        ResourceValue existing = InventoryResources.Find(r => r.resourceName == resource.resourceName);
        if (existing != null)
        {
            existing.amount += resource.amount;
        }
        else
        {
            InventoryResources.Add(new ResourceValue(resource.resourceName, resource.amount, resource.Type, resource.resourceIcon));
        }

        Debug.Log($"Added {resource.amount}x {resource.resourceName} to inventory!");
    }

    public Animator getAnimator()
    {
        return anim;
    }

    public int GetPlayerHealth()
    {
        return Health;  
    }
    public void SetWeaponAnimation()
    {
        WeaponValue currentWeapon = GameValue.Instance.GetPlayerValue().EquipmentWeapon;
        string weaponName;
        if (currentWeapon != null)
        {
            weaponName = currentWeapon.WeaponName;
            if (weaponName == "Knife")
            {

                anim.SetInteger("WeaponEquipped", 1); // Set to knife animation
                weaponEqupped = 1;
            }
            else if (weaponName == "Pistol")
            {
                anim.SetInteger("WeaponEquipped", 2); // Set to pistol animation
                weaponEqupped = 2;
            }
            else
            {
                anim.SetInteger("WeaponEquipped", 3); // Set to shotgun animation
                weaponEqupped = 3;
            }
        }
    }

    public void AddCard(CardValue card)
    {
        if(card == null)
        {
            return;
        }
        
        GameValue.Instance.GetPlayerValue().HadCardsLibrary.Add(card);
        Debug.Log($"Added card to inventory: {card.CardName} ");
    }
}
