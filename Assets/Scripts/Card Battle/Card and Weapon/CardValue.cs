// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng
// No external source was used 

using UnityEngine;
using System;
using System.Collections.Generic;
using static CardEffectParser;

public enum CardType
{
    None, Weapons, AoE, SupportItems
}

public enum CardRarity
{
    Common, Rare, VeryRare, Epic
}

public enum CardAbility
{
    None, Attack, Buffs, Potions, Items, Healing
}

public class CardValue
{
    public string CardName;
    public Sprite CardSprite;
    public CardType CardType = CardType.SupportItems;
    public string CardDescribe;
    public CardRarity rarity;
    public CardAbility ability;
    public int ID;

    //private List<Action<BattlePlayerValue,List<EnemyValue>>> parsedEffects = new List<Action<BattlePlayerValue, List<EnemyValue>>>();

    public CardValue(ExcelCardData excelCardData)
    {
        ID = excelCardData.ID;

        CardName = string.IsNullOrEmpty(excelCardData.cardName) ? "DefaultCard" : excelCardData.cardName;
        if (string.IsNullOrEmpty(excelCardData.cardName))
            Debug.LogWarning($"[CardValue] ID {ID} CardName was null or empty, using DefaultCard");

        CardType = excelCardData.cardType == CardType.None ? CardType.SupportItems : excelCardData.cardType;
        if (excelCardData.cardType == CardType.None)
            Debug.LogWarning($"[CardValue] ID {ID} CardType was None, using SupportItems");

        rarity = excelCardData.rarity;
        ability = excelCardData.ability;
        CardDescribe = excelCardData.cardDescribe;

        //string effectString = excelCardData.cardFunction;
        //parsedEffects = CardEffectParser.ParseEffectString(effectString);

        SetCardSprite();
    }

    public CardValue(string cardname, Sprite sprite, CardType type, string discription, CardRarity rare, CardAbility abil, int id)
    {
        CardName = cardname;
        CardSprite = sprite;
        CardType = type;
        CardDescribe= discription;
        rarity = rare;
        ability = abil;
        ID = id;
    }

    void SetCardSprite()
    {
        if (string.IsNullOrEmpty(CardName))
        {
            Debug.LogError($"[SetCardSprite] CardName is null or empty for ID {ID}");
            return;
        }

        string path = $"Sprite/Card/{CardType}/{CardName}/{CardName.ToLower()}";
        CardSprite = Resources.Load<Sprite>(path);

        if (CardSprite == null)
            Debug.LogWarning($"[SetCardSprite] Failed to load CardSprite! Check path: {path}");
    }

    public void UseEffect(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        CardEffect.UseEffect(CardName,player, enemys);
    }

    public int GetID()
    {
        return ID;
    }

    public int GetCombineCost()
    {
        return (int)(rarity + 1)* 5;
    }
}
