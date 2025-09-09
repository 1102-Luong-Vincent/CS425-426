using UnityEngine;

public enum Rarity
{
    Common, Rare, VeryRare,Epic
}

public enum Ability
{
    Buffs, Potions, Items, Healing
}

public class CardValue 
{
    public string CardName;
    public Sprite CardSprite;
    public string CardDescribe;
    // not yet wait, maybe need excel file
    public Rarity rarity; 
    public Ability ability;


    public CardValue(string CardName) // maybe need make excel to store data or something;
    {
        this.CardName = CardName;
        CardSprite = GetCardSprite();
        CardDescribe = GetCardDescribe();
    }


    Sprite GetCardSprite() { 
        Sprite cardSprite = Resources.Load<Sprite>($"Sprite/BattleScenes/{CardName}");
        return cardSprite;
    }

    string GetCardDescribe()// maybe need make excel to store data or something;
    {
        string cardDescribe = "";

        switch (CardName)
        {
            case "Bandage": return "heals player";
            case "Knife": return "has fast speed. The player can attack twice. Damage is low";
            case "Pistol": return "has average speed. Damage is average. *unique ability: bullet can shoot through multiple enemies*";
            case "Shotgun": return "has slow speed. Damage is high. *unique ability: bullets have a chance to dismember enemies, causing them to do less damage to the player*";

        }
        return cardDescribe;

    }

}