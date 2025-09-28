using UnityEngine;

public enum CardType
{
    Weapons, AoE, SupportItems
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
    public CardType CardType;
    public string CardDescribe;
    // not yet wait, maybe need excel file
    public CardRarity rarity; 
    public CardAbility ability;
    public int weaponLevel;
    public int maxLevel = 3;
    public float damage; //pertains to everything, healing, etc.
  //  public float attackBoost; //gives a certain amount of extra attack to the player, like 20% extra attack

    

    public CardValue(ExcelCardData excelCardData) // maybe need make excel to store data or something;
    {
        this.CardName = excelCardData.CardName;
        CardType = excelCardData.cardType;
        rarity = excelCardData.rarity;
        ability = excelCardData.ability;
        CardDescribe = excelCardData.CardDescribe;
        if (CardType == CardType.Weapons)
        {
            weaponLevel = excelCardData.weaponLevel;
            maxLevel    = excelCardData.maxLevel;
        }
        damage = excelCardData.damage;

        CardSprite = GetCardSprite();

    }
    Sprite GetCardSprite()
    {
        string path = $"Sprite/Card/{CardType}/{CardName}/{CardName.ToLower()}";
        Debug.Log($"[LoadCardSprite] Try load: {path}");
        Sprite cardSprite = Resources.Load<Sprite>(path);
        if (cardSprite == null) Debug.LogWarning($"[LoadCardSprite] cardSprite == null, check path or filename!");
        return cardSprite;
    }

}