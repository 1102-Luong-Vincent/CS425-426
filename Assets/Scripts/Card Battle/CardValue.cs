using UnityEngine;

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
    public string CardDescribe;
    // not yet wait, maybe need excel file
    public CardRarity rarity; 
    public CardAbility ability;


    public CardValue(ExcelCardData excelCardData) // maybe need make excel to store data or something;
    {
        this.CardName = excelCardData.CardName;
        CardSprite = GetCardSprite();
        rarity = excelCardData.rarity;
        ability = excelCardData.ability;
        CardDescribe = excelCardData.CardDescribe;
    }


    Sprite GetCardSprite() { 
        Sprite cardSprite = Resources.Load<Sprite>($"Sprite/BattleScenes/{CardName}");
        return cardSprite;
    }


}