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
    private int ID;

    private List<Action<BattlePlayerValue>> parsedEffects = new List<Action<BattlePlayerValue>>();

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

        string effectString = excelCardData.cardFunction;
        parsedEffects = CardEffectParser.ParseEffectString(effectString);

        SetCardSprite();
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

    public void UseEffect(BattlePlayerValue player)
    {
        if (parsedEffects == null || parsedEffects.Count == 0)
        {
            Debug.LogWarning($"[UseEffect] No effects parsed for {CardName}");
            return;
        }

        foreach (var effect in parsedEffects)
        {
            try
            {
                effect?.Invoke(player);
            }
            catch (Exception e)
            {
                Debug.LogError($"[UseEffect] Error executing {CardName} effect: {e.Message}");
            }
        }

        Debug.Log($"[UseEffect] Executed {parsedEffects.Count} effects for {CardName}");
    }
}
