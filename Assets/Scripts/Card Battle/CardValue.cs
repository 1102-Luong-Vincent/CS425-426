using UnityEngine;

public enum CardType
{
    Weapons
}

public enum CardRarity
{
    Common, Rare, VeryRare,Epic
}

public enum CardAbility
{
    None,Buffs, Potions, Items, Healing
}

public class CardValue 
{
    public string CardName;
    public Sprite CardSprite;
    public CardType CardType;
    public string CardDescribe;
    // not yet wait, maybe need excel file
    public CardRarity rarity; 
    public CardAbility ability;


    public CardValue(ExcelCardData excelCardData) // maybe need make excel to store data or something;
    {
        this.CardName = excelCardData.CardName;
        CardType = excelCardData.cardType;
        rarity = excelCardData.rarity;
        ability = excelCardData.ability;
        CardDescribe = excelCardData.CardDescribe;

        CardSprite = GetCardSprite();

    }
    Sprite GetCardSprite() { 
        Sprite cardSprite = Resources.Load<Sprite>($"Sprite/Card/{CardType.ToString()}/{CardName}/{CardName.ToLower()}");
        return cardSprite;
    }


}