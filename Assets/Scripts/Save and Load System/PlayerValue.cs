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
    public List<CardValue> EquipmentCards = new List<CardValue>();
    public List<CardValue> battleCardsList = new List<CardValue>();
    public List<CardValue> HadCardsLibrary = new List<CardValue>();

    public WeaponValue EquipmentWeapon;
    public List<WeaponValue> HadWeaponsLibrary = new List<WeaponValue>();


    int Health = 100;

    public PlayerValue() {
        Init();
    }
    public void Init()
    {
        InitPlayerEquipmentWeapons();
        InitPlayerEquipmentCards();
    }
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

        string[] starterEquipment = { "Bandage", "Syringe", "Bandage", "Syringe" };
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


    void ClearWeapons()
    {
        EquipmentWeapon = null;
        HadWeaponsLibrary.Clear();
    }

    void ClearCard()
    {
        EquipmentCards.Clear();
        HadCardsLibrary.Clear();
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


        foreach (string card in data.HadWeaponsSaveLibrary)
        {
            CardValue foundCard = GameValue.Instance.GetInitCardValue(card);
            if (foundCard != null)
            {
                HadCardsLibrary.Add(foundCard);
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
