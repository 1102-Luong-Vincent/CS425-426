using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ExcelReader;

public class PlayerValue
{
    public List<CardValue> EquipmentCards = new List<CardValue>();
    public List<CardValue> AllLibraryCards = new List<CardValue>();

    public PlayerValue() {
        Init();
    }
    public void Init()
    {
        SetPlayerEquipmentCards();
    }
    void SetPlayerEquipmentCards()
    {
        EquipmentCards.Clear();
        string[] starterEquipment = { "Bandage", "Knife", "Pistol", "Shotgun" };

        foreach (string equipName in starterEquipment)
        {
            CardValue foundCard = GameValue.Instance.GetCardValue(equipName);

            if (foundCard != null)
            {
                EquipmentCards.Add(foundCard);
            }
        }
    }

    public Vector3 GetPlayerPosition()
    {
        if (PlayerMoveControl.Instance != null) {
            return PlayerMoveControl.Instance.GetPlayerCurrentPosition();
        }else
        {
            return Vector3.zero;
        }
    }


    public void SetPlayerSaveData(PlayerSaveData data)
    {
        foreach (string equipName in data.EquipmentSaveCards)
        {
            CardValue foundCard = GameValue.Instance.GetCardValue(equipName);
            if (foundCard != null)
            {
                EquipmentCards.Add(foundCard);
            }
        }

        foreach (string card in data.AllLibrarySaveCards)
        {
            CardValue foundCard = GameValue.Instance.GetCardValue(card);
            if (foundCard != null)
            {
                AllLibraryCards.Add(foundCard);
            }
        }

        SetPlayerPosition(data);
    }

    void SetPlayerPosition(PlayerSaveData data)
    {
        if (PlayerMoveControl.Instance == null) return;

        PlayerMoveControl.Instance.SetPlayerPosition(data.GetPlayerPosition());

    }

}

[System.Serializable]
public class PlayerSaveData
{
    public List<string> EquipmentSaveCards = new List<string>();
    public List<string> AllLibrarySaveCards = new List<string>();
    public float PlayerPositionX, PlayerPositionY,PlayerPositionZ;

    public PlayerSaveData(PlayerValue playerValue)
    {
        foreach (var equipmentCard in playerValue.EquipmentCards)
        {
            EquipmentSaveCards.Add(equipmentCard.CardName);
        }
        foreach (var card in playerValue.AllLibraryCards)
        {
            AllLibrarySaveCards.Add(card.CardName);
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
