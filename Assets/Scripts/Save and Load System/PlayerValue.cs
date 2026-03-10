//Authors: Vincent Luong, Shawn Meng, Yuhan Tang, and Sean Masterson
//Created by: Team 4
//Modified by: Shawn Meng, Yuhan Tang
//Some code generated with assistance from ChatGPT.

using NUnit.Framework;
using SmallScaleInc.TopDownPixelCharactersPack1;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static ExcelReader;

public class PlayerValue
{
    public List<CardValue> EquipmentCards = new List<CardValue>(); // Starting hand for battle -- first 5 cards in your deck
    public List<CardValue> battleCardsList = new List<CardValue>(); // remaining 15 cards in your deck, not including starting hand
    public List<CardValue> HadCardsLibrary = new List<CardValue>();

    public List<CardValue[]> Decks = new List<CardValue[]>(); // New list to hold multiple decks
    int activeDeckIndex = 0; // Index to track the active deck

    public WeaponValue EquipmentWeapon;
    public List<WeaponValue> HadWeaponsLibrary = new List<WeaponValue>();

    public Dictionary<string, int> Materials = new Dictionary<string, int>();


    int Health = 100;
    int energy = 10;

    const int MAX_CARDS = 20;
    const int STARTING_HAND_SIZE = 5;
    const int MAX_DECKS = 3;

    public PlayerValue() {
        Init();
    }

    // just for test
    public PlayerValue(bool skipInit)
    {
        HadWeaponsLibrary = new List<WeaponValue>();
        EquipmentWeapon = null;
    }

    public void Init()
    {
        InitPlayerEquipmentWeapons();
        InitPlayerEquipmentCards();
        InitPlayerDecks();

        //≤‚ ‘≤ƒ¡œ
        AddMaterial("Whetstone", 999);
        //AddMaterial("Metal", 999);
    }

    // New add Player Material Backpack
    public int GetMaterialCount(string materialName)
    {
        if (string.IsNullOrEmpty(materialName)) return 0;
        return Materials.TryGetValue(materialName, out int count) ? count : 0;
    }

    public void AddMaterial(string materialName, int amount)
    {
        if (string.IsNullOrEmpty(materialName)) return;
        if (amount <= 0) return;

        if (!Materials.ContainsKey(materialName))
            Materials[materialName] = 0;

        Materials[materialName] += amount;
    }

    public bool TrySpendMaterial(string materialName, int amount)
    {
        if (string.IsNullOrEmpty(materialName)) return false;
        if (amount <= 0) return true;

        int have = GetMaterialCount(materialName);
        if (have < amount) return false;

        Materials[materialName] = have - amount;
        return true;
    }
    // Unitl here

    void InitPlayerEquipmentWeapons()
    {
        ClearWeapons();
        string starterEquipment = "Knife";
        EquipmentWeapon = GameValue.Instance.GetInitWeaponValue(starterEquipment);// mad be change by id

        // Need to add player init Had Weapon
        HadWeaponsLibrary.Add(EquipmentWeapon);
    }


    void InitPlayerEquipmentCards()
    {
        ClearCard();

        string[] starterEquipment = { "Bandage", "Syringe" };
        foreach (string equipName in starterEquipment)
        {
            CardValue foundCard = GameValue.Instance.GetInitCardValue(equipName);
            if (foundCard != null)
            {
                EquipmentCards.Add(foundCard);
            }
        }

        string[] allCards = {
        "Bandage", "Syringe", "Medkit", "Revival Serum", "Pills", "Rage Pill",
        "Drugs", "Beer", "Health Potion", "Energy Potion", "Antidote Potion",
        "Field Surgery Kit", "Adrenal Medkit", "Combat Patch", "Berserker Wrap",
        "Stimulant Wrap", "Liquid Courage Kit", "Rapid Recovery Injector",
        "Phoenix Shot", "Boosted Buzz"
        };

        foreach (string cardName in allCards)
        {
            CardValue foundCard = GameValue.Instance.GetInitCardValue(cardName);
            if (foundCard != null)
            {
                battleCardsList.Add(foundCard);
            }
            else
            {
                Debug.LogWarning($"Card {cardName} not found in GameValue library!");
            }
        }

        HadCardsLibrary.AddRange(EquipmentCards);
        HadCardsLibrary.AddRange(battleCardsList);
    }

    public void InitPlayerDecks()
    {
        Decks.Clear();
        for (int i = 0; i < MAX_DECKS; i++)
        {
            CardValue[] newDeck = new CardValue[MAX_CARDS];
            for (int j = 0; j < MAX_CARDS; j++)
            {
                newDeck[j] = null;  //instantiate empty deck with null values, will be populated when player adds cards to deck
            }
            Decks.Add(newDeck);
        }
    }

    public void setActiveDeck(int index)
    {
            if (index< 0 || index >= Decks.Count)
            {
                Debug.LogWarning($"Invalid deck index: {index}");
                return;
            }
        activeDeckIndex = index;
        // set starting hand
        List<CardValue> temp = new List<CardValue>();
        for (int i = 0; i < STARTING_HAND_SIZE; i++)
        {
            temp.Add(Decks[activeDeckIndex][i]);
        }
        EquipmentCards = temp;

        // set rest of deck
        temp.Clear();
        for (int i = STARTING_HAND_SIZE; i < MAX_CARDS; i++)
        {
            temp.Add(Decks[activeDeckIndex][i]);
        }
        battleCardsList = temp;
    }
    // function to add a card to the currently active deck.
    public void AddCardToDeck(CardValue card, int index)
    {
        if(card != null)
        {
            if (index < 0 || index >= MAX_CARDS)
            {
                Debug.LogWarning($"Invalid card index: {index}");
                return;
            }
            Decks[activeDeckIndex][index] = card;
        }
        else
        {
            Debug.LogWarning("Cannot add null card to deck!");
            Decks[activeDeckIndex][index] = null;
        }
    }
    public void AddCard(string cardName)
    {

        CardValue foundCard = GameValue.Instance.GetInitCardValue(cardName);
        if (foundCard != null)
        {
            HadCardsLibrary.Add(foundCard);
        }

    }


    void ClearWeapons()
    {
        EquipmentWeapon = null;
        HadWeaponsLibrary.Clear();
    }

    void ClearCard()
    {
        EquipmentCards.Clear();
        HadCardsLibrary.Clear();
        battleCardsList.Clear();
    }



    public Vector3 GetPlayerPosition()
    {
        if (PlayerController.Instance != null) {
            return PlayerController.Instance.GetPlayerCurrentPosition();
        }else
        {
            return Vector3.zero;
        }
    }

    public void SetPlayerSaveData(PlayerSaveData data)
    {
        ClearWeapons();
        ClearCard();
        foreach (string equipName in data.EquipmentSaveCards)
        {
            CardValue foundCard = GameValue.Instance.GetInitCardValue(equipName);
            if (foundCard != null)
            {
                EquipmentCards.Add(foundCard);
            }
        }
        foreach (string card in data.HadCardsSaveLibrary)
        {
            CardValue foundCard = GameValue.Instance.GetInitCardValue(card);
            if (foundCard != null)
            {
                HadCardsLibrary.Add(foundCard);
            }
        }

        EquipmentWeapon = GameValue.Instance.GetInitWeaponValue(data.EquipmentWeapon);


        //foreach (string card in data.HadWeaponsSaveLibrary)
        //{
        //    CardValue foundCard = GameValue.Instance.GetInitCardValue(card);
        //    if (foundCard != null)
        //    {
        //        HadCardsLibrary.Add(foundCard);
        //    }
        //}
        foreach (string weaponName in data.HadWeaponsSaveLibrary)
        {
            WeaponValue foundWeapon = GameValue.Instance.GetInitWeaponValue(weaponName);
            if (foundWeapon != null)
            {
                HadWeaponsLibrary.Add(foundWeapon);
            }
        }


        SetPlayerPosition(data);
    }

    void SetPlayerPosition(PlayerSaveData data)
    {
        if (PlayerController.Instance == null) return;
        PlayerController.Instance.SetPlayerPosition(data.GetPlayerPosition());

    }


    public int GetHealth()
    {
        return Health;
    }

    public int GetEnergy()
    {
        return energy;
    }

    public int GetMaxCards()
    {
        return MAX_CARDS;
    }

    public CardValue[] GetActiveDeck()
    {
        return Decks[activeDeckIndex];
    }

    public int GetActiveDeckIndex()
    {
        return activeDeckIndex;
    }

}

[System.Serializable]
public class PlayerSaveData
{
    public List<string> EquipmentSaveCards = new List<string>();
    public List<string> HadCardsSaveLibrary = new List<string>();

    public string EquipmentWeapon;
    public List<string> HadWeaponsSaveLibrary = new List<string>();


    public float PlayerPositionX, PlayerPositionY,PlayerPositionZ;

    public PlayerSaveData(PlayerValue playerValue)
    {
        foreach (var equipmentCard in playerValue.EquipmentCards)
        {
            EquipmentSaveCards.Add(equipmentCard.CardName);
        }
        foreach (var card in playerValue.HadCardsLibrary)
        {
            HadCardsSaveLibrary.Add(card.CardName);
        }

        EquipmentWeapon = playerValue.EquipmentWeapon.WeaponName;

        foreach (var weapon in playerValue.HadWeaponsLibrary)
        {
            HadWeaponsSaveLibrary.Add(weapon.WeaponName);
        }

        PlayerPositionX = playerValue.GetPlayerPosition().x;
        PlayerPositionY = playerValue.GetPlayerPosition().y;
        PlayerPositionZ = playerValue.GetPlayerPosition().z;

    }

    public Vector3 GetPlayerPosition()
    {
        Vector3 playerPosition = new Vector3(PlayerPositionX, PlayerPositionY, PlayerPositionZ);
        return playerPosition;
    }


}

