using UnityEngine;

public enum CardType
{
    None,Weapons, AoE, SupportItems
}

public enum CardRarity
{
    Common, Rare, VeryRare,Epic
}

public enum CardAbility
{
    None,Attack,Buffs, Potions, Items, Healing
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
        Debug.Log($"[SetCardSprite] Loading sprite at: {path}");
        CardSprite = Resources.Load<Sprite>(path);

        if (CardSprite == null)
            Debug.LogWarning($"[SetCardSprite] Failed to load CardSprite! Check path and filename: {path}");
    }
}
