//Authors: Vincent Luong, Shawn Meng, Yuhan Tang, and Sean Masterson
//Created by: Team 4
//Modified by: Shawn Meng, Yuhan Tang
//No external sources were used

using SmallScaleInc.TopDownPixelCharactersPack1;
using System;
using System.Collections.Generic;
using UnityEngine;
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
    public HashSet<string> keyInteractable = new HashSet<string>();

    public List<ResourceValue> InventoryResources = new List<ResourceValue>();

    int Health = 100;
    int MaxHealth = 100;
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
        setActiveDeck(activeDeckIndex);
        //测试材料
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
        //string starterEquipment = "Knife";
        // EquipmentWeapon = GameValue.Instance.GetInitWeaponValue(starterEquipment);// mad be change by id

        // Need to add player init Had Weapon
        // HadWeaponsLibrary.Add(EquipmentWeapon);
        // test for shotgun
        //WeaponValue shotgun = GameValue.Instance.GetWeaponByNameAndLevel("Shotgun", 1);
        //if (shotgun != null)
        //{
        //    HadWeaponsLibrary.Add(shotgun);

        //    // EquipmentWeapon = shotgun;
        //}
    }


    void InitPlayerEquipmentCards()
    {
        ClearCard();

        string[] starterEquipment = { }; //"Bandage", "Syringe" };
        foreach (string equipName in starterEquipment)
        {
            CardValue foundCard = GameValue.Instance.GetInitCardValue(equipName);
            if (foundCard != null)
            {
                EquipmentCards.Add(foundCard);
            }
        }

        string[] allCards = {
        //"Bandage", "Syringe", "Medkit", "Revival Serum",
        //"Health Potion", "Energy Potion", "Antidote Potion",
        //"Field Surgery Kit", "Adrenal Medkit", "Combat Patch", "Berserker Wrap",
        //"Stimulant Wrap", "Liquid Courage Kit", "Rapid Recovery Injector",
        //"Phoenix Shot", "Boosted Buzz"
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

        CardValue[] activeDeck = Decks[activeDeckIndex];

        EquipmentCards = new List<CardValue>();
        battleCardsList = new List<CardValue>();

        for (int i = 0; i < STARTING_HAND_SIZE; i++)
        {
            if(activeDeck[i] != null)   EquipmentCards.Add(activeDeck[i]);
        }

        for (int i = STARTING_HAND_SIZE; i < MAX_CARDS; i++)
        {
            if (activeDeck[i] != null) battleCardsList.Add(activeDeck[i]);
        }
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
        Materials.Clear();
        Decks.Clear();

        if (data == null)
        {
            Debug.LogWarning("SetPlayerSaveData called with null data.");
            return;
        }

        // 重新加载基本数据，restore base stats
        Health = data.health;
        MaxHealth = data.maxHealth;
        energy = data.energy;
        activeDeckIndex = data.activeDeckIndex;

        //foreach (string equipName in data.EquipmentSaveCards)
        //{
        //    if (string.IsNullOrEmpty(equipName)) continue;

        //    CardValue foundCard = GameValue.Instance.GetInitCardValue(equipName);
        //    if (foundCard != null)
        //    {
        //        EquipmentCards.Add(foundCard);
        //    }
        //}
        foreach (string card in data.HadCardsSaveLibrary)
        {
            if (string.IsNullOrEmpty(card)) continue;

            CardValue foundCard = GameValue.Instance.GetInitCardValue(card);
            if (foundCard != null)
            {
                HadCardsLibrary.Add(foundCard);
            }
        }

        if (!string.IsNullOrEmpty(data.EquipmentWeapon))
        {
            EquipmentWeapon = GameValue.Instance.GetInitWeaponValue(data.EquipmentWeapon);
        }


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
            if (string.IsNullOrEmpty(weaponName)) continue;

            WeaponValue foundWeapon = GameValue.Instance.GetInitWeaponValue(weaponName);
            if (foundWeapon != null)
            {
                HadWeaponsLibrary.Add(foundWeapon);
            }
        }
        //重新加载材料
        if (data.materials != null)
        {
            foreach (var material in data.materials)
            {
                if (material == null) continue;
                if (string.IsNullOrEmpty(material.materialName)) continue;

                Materials[material.materialName] = material.amount;
            }
        }
        //重新加载deck
        if (data.decks != null && data.decks.Count > 0)
        {
            foreach (var savedDeck in data.decks)
            {
                CardValue[] deckArray = new CardValue[MAX_CARDS];

                for (int i = 0; i < MAX_CARDS; i++)
                {
                    if (savedDeck == null || savedDeck.cardNames == null || i >= savedDeck.cardNames.Count)
                    {
                        deckArray[i] = null;
                        continue;
                    }

                    string cardName = savedDeck.cardNames[i];

                    if (string.IsNullOrEmpty(cardName))
                    {
                        deckArray[i] = null;
                    }
                    else
                    {
                        deckArray[i] = GameValue.Instance.GetInitCardValue(cardName);
                    }
                }

                Decks.Add(deckArray);
            }
        }
        while (Decks.Count < MAX_DECKS)
        {
            Decks.Add(new CardValue[MAX_CARDS]);
        }
        if (activeDeckIndex < 0 || activeDeckIndex >= Decks.Count)
        {
            activeDeckIndex = 0;
        }

        setActiveDeck(activeDeckIndex);

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

    public int GetMaxHealth()
    {
        return MaxHealth;
    }

    public int GetEnergy()
    {
        return energy;
    }

    public int GetCardCount()
    {
        return HadCardsLibrary.Count;
    }

    public int GetDeckCardCount(int deckIndex)
    {
        int count = 0;
        for(int i = 0; i < MAX_CARDS; i++)
        {
            if (Decks[deckIndex][i] != null) count++;
        }
        return count;
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

    public void SetHealth(int health)
    {
        Health = health;
    }

    public void AddKeyInteractable(string keyID)
    {
        keyInteractable.Add(keyID);
    }

    public bool HasKey(string keyID)
    {
        return keyInteractable.Contains(keyID);
    }

    public void RemoveKey(string keyID)
    {
        keyInteractable.Remove(keyID);
    }

    public void AddResource(ResourceValue resource)
    {
        if (resource == null) return;

        if (resource.Type == ResourceType.Material)
        {
            AddMaterial(resource.resourceName, resource.amount);
        }

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
}

[System.Serializable]
public class PlayerSaveData
{
    public int health;
    public int maxHealth;
    public int energy;

    public int activeDeckIndex;
    public List<DeckSaveData> decks = new List<DeckSaveData>(); // new add mark

    public List<MaterialSaveData> materials = new List<MaterialSaveData>();

    public List<string> EquipmentSaveCards = new List<string>();
    public List<string> HadCardsSaveLibrary = new List<string>();

    public string EquipmentWeapon;
    public List<string> HadWeaponsSaveLibrary = new List<string>();


    public float PlayerPositionX, PlayerPositionY, PlayerPositionZ;

    public PlayerSaveData(PlayerValue playerValue)
    {
        //new
        health = playerValue.GetHealth();
        maxHealth = playerValue.GetMaxHealth();
        energy = playerValue.GetEnergy();

        activeDeckIndex = playerValue.GetActiveDeckIndex();

        for (int i = 0; i < playerValue.Decks.Count; i++)
        {
            DeckSaveData deckData = new DeckSaveData();

            foreach (var card in playerValue.Decks[i])
            {
                deckData.cardNames.Add(card != null ? card.CardName : "");
            }

            decks.Add(deckData);
        }  // mark

        foreach (var equipmentCard in playerValue.EquipmentCards)
        {
            EquipmentSaveCards.Add(equipmentCard.CardName);
        }
        foreach (var card in playerValue.HadCardsLibrary)
        {
            HadCardsSaveLibrary.Add(card.CardName);
        }

        //EquipmentWeapon = playerValue.EquipmentWeapon.WeaponName;
        EquipmentWeapon = playerValue.EquipmentWeapon != null ? playerValue.EquipmentWeapon.WeaponName : "";

        foreach (var weapon in playerValue.HadWeaponsLibrary)
        {
            HadWeaponsSaveLibrary.Add(weapon.WeaponName);
        }

        // store material
        foreach (var pair in playerValue.Materials)
        {
            materials.Add(new MaterialSaveData
            {
                materialName = pair.Key,
                amount = pair.Value
            });
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