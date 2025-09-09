using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static ExcelReader;

public class PlayerValue : MonoBehaviour
{
    public static PlayerValue Instance { get; private set; }
    public List<CardValue> EquipmentCards = new List<CardValue>();
    public List<CardValue> AllLibraryCards = new List<CardValue>();
    private List<CardValue> AllCards = new List<CardValue>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Init();
        DontDestroyOnLoad(gameObject);
    }

    public void Init()
    {
        InitAllCardInExcel();
        SetPlayerEquipmentCards();

    }

    void InitAllCardInExcel()
    {
        List<ExcelCardData> allExcelCardDatas = GetCardData();
        Debug.Log($"in init All card READER {allExcelCardDatas.Count}");
        AllCards.Clear();
        foreach (ExcelCardData excelData in allExcelCardDatas)
        {
            CardValue card = new CardValue(excelData);
            AllCards.Add(card);
        }
    }

    void SetPlayerEquipmentCards()
    {
        EquipmentCards.Clear();
        string[] starterEquipment = { "Bandage", "Knife", "Pistol", "Shotgun" };

        foreach (string equipName in starterEquipment)
        {
            CardValue foundCard = AllCards.Find(card => card.CardName == equipName);

            if (foundCard != null)
            {
                EquipmentCards.Add(foundCard);
            }
        }
    }

}



