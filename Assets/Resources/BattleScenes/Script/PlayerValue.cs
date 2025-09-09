using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerValue : MonoBehaviour
{
    public List<CardValue> EquipmentCards = new List<CardValue>();
    public List<CardValue> AllLibraryCards = new List<CardValue>();


    private void Awake()
    {
        Init();
    }


    public void Init()
    {
        CardValue Bandage = new CardValue("Bandage");
        EquipmentCards.Add(Bandage);
        CardValue Knife = new CardValue("Knife");
        EquipmentCards.Add(Knife);
        CardValue Pistol = new CardValue("Pistol");
        EquipmentCards.Add(Pistol);
        CardValue Shotgun = new CardValue("Shotgun");
        EquipmentCards.Add(Shotgun);

    }
}



