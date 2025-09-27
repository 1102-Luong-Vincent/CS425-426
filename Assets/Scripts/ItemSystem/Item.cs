using UnityEngine;
using System.Collections.Generic;
public class Item
{
    public string itemType;
    public string cardRarity;
    public float healAmount, boostAmount;
    public float attackBoost, defenseBoost;

    public void Healing()
    {
        var healTable = new Dictionary<string, Dictionary<string, float>>()
        {
            { "Bandage", new Dictionary<string, float>()
                {
                    { "Common", 0.1f },
                    { "Rare", 0.15f },
                    { "Very Rare", 0.2f },
                    { "Epic", 0.3f }
                }
            },
            { "Medkit", new Dictionary<string, float>()
                {
                    { "Common", 0.6f },
                    { "Rare", 0.7f },
                    { "Very Rare", 0.85f },
                    { "Epic", 1.0f }
                }
            }
        };

        // Look up heal value
        if (healTable.ContainsKey(itemType) && healTable[itemType].ContainsKey(cardRarity))
        {
            healAmount = healTable[itemType][cardRarity];
        }
        else
        {
            healAmount = 0f;
        }
    }

    public void ItemBoost()
    {

        var ItemBoostTable = new Dictionary<string, Dictionary<string, float>>()
        {
            { "AttackSeed", new Dictionary<string, float>()
                {
                    { "Common", 0.2f },
                    { "Rare", 0.25f },
                    { "Very Rare", 0.3f },
                    { "Epic", 0.35f }
                }
            },
            { "DefenseSeed", new Dictionary<string, float>()
                {
                    { "Common", 0.2f },
                    { "Rare", 0.25f },
                    { "Very Rare", 0.3f },
                    { "Epic", 0.35f }
                }
            }
        };

        if (ItemBoostTable.ContainsKey(itemType) && ItemBoostTable[itemType].ContainsKey(cardRarity))
        {
            boostAmount = ItemBoostTable[itemType][cardRarity];
        }
        else
        {
            boostAmount = 0f;
        }
    }


    //public void AttackItem()
    //{
    //    //attackBoost = playerDamage * 0.2f; //20% attack up
    //} 

    //public void DefenseItem()
    //{
    //    //defenseBoost = playerDefense * 0.2f; //20% defense up
    //}

    //public void Healing()
    //{
    //    if(itemType == "Bandage" && cardRarity == "Common")
    //    {
    //        healAmount = 0.1f;

    //        //int playerHealth = currentHealth * healAmount; //heals player by 10% with their current health
    //    }
    //    else if(itemType == "Bandage" && cardRarity == "Rare")
    //    {
    //        healAmount = 0.15f;

    //        //int playerHealth = currentHealth * healAmount; //heals player by 15%
    //    }
    //    else if(itemType == "Bandage" && cardRarity == "Very Rare")
    //    {
    //        healAmount = 0.2f;

    //        //int playerHealth = currentHealth * healAmount; //heals player by 20%
    //    }
    //    else if(itemType == "Bandage" && cardRarity == "Epic")
    //    {
    //        healAmount = 0.3f;

    //        //int playerHealth = currentHealth * healthAmount; //heals player by 30%
    //    }

    //    if(itemType == "Medkit" && cardRarity == "Common")
    //    {
    //        healAmount = 0.6f;

    //        //int playerHealth = currentHealth * healAmount; //heals player by 60% with their current health
    //    }
    //    if (itemType == "Medkit" && cardRarity == "Rare")
    //    {
    //        healAmount = 0.7f;

    //        //int playerHealth = currentHealth * healAmount; //heals player by 70% with their current health
    //    }

    //    if (itemType == "Medkit" && cardRarity == "Very Rare")
    //    {
    //        healAmount = 0.85f;

    //        //int playerHealth = currentHealth * healAmount; //heals player by 85% with their current health
    //    }

    //    if (itemType == "Medkit" && cardRarity == "Epic")
    //    {
    //        healAmount = 1.0f;

    //        //int playerHealth = currentHealth * healAmount; //heals player by 100% with their current health
    //    }
    //    else
    //    {
    //        //int playerHealth = currentHealth; //otherwise player keeps current health
    //    }
    //}
}