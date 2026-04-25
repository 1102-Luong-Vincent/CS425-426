// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng
// no external source was used.

using UnityEngine;
using System.Collections.Generic;
using static ExcelReader;

[System.Serializable]
public class CardCombinationRule
{
    public int Card1;
    public int Card2;
    public int Result;
    public float FailChance;

    public CardCombinationRule(int a, int b, int result, float failChance)
    {
        Card1 = a;
        Card2 = b;
        Result = result;
        FailChance = failChance;
    }

    public CardCombinationRule(ExcelCardCombinationFunction ExcelCardCombinationFunction)
    {
        Card1 = ExcelCardCombinationFunction.card1ID;
        Card2 = ExcelCardCombinationFunction.card2ID;
        Result = ExcelCardCombinationFunction.resultID;
        FailChance = ExcelCardCombinationFunction.failChance;
    }


}

public class CardCombinations : MonoBehaviour
{
    public static CardCombinations Instance;

    public Dictionary<(int, int), CardCombinationRule> combinationTable = new Dictionary<(int, int), CardCombinationRule>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadDefaultCombinations();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void AddRule(ExcelCardCombinationFunction ExcelCardCombinationFunction)
    {
        var rule = new CardCombinationRule(ExcelCardCombinationFunction);

        combinationTable[(ExcelCardCombinationFunction.card1ID, ExcelCardCombinationFunction.card2ID)] = rule;
        combinationTable[(ExcelCardCombinationFunction.card2ID, ExcelCardCombinationFunction.card1ID)] = rule;
    }



    void LoadDefaultCombinations()
    {
        combinationTable.Clear();

        List<ExcelCardCombinationFunction> cardCombinationFunctions = GetCardCombinationFunction();
        foreach (var function in cardCombinationFunctions)
        {
            AddRule(function);
        }
        Debug.Log("[CardCombinations] Loaded " + combinationTable.Count + " combinations.");
    }

    public CardValue Combine(CardValue a, CardValue b)
    {
        if (a == null || b == null)
        {
            Debug.LogWarning("[Combine] One or both cards are null.");
            return new CardValue(new ExcelCardData());
        }

        int idA = a.GetID();
        int idB = b.GetID();

        int resultID;

        if (idA == idB)
        {
            resultID = (idA + 1) % 20;
            Debug.Log($"[Combine] Same cards: {a.CardName}(ID:{idA}) + {b.CardName}(ID:{idB}) → ID:{resultID}");
        }
        else
        {
            resultID = (idA + idB) % 20;
            Debug.Log($"[Combine] Different cards: {a.CardName}(ID:{idA}) + {b.CardName}(ID:{idB}) → ID:{resultID}");
        }

        CardValue returnCard = GameValue.Instance.GetInitCardValue(resultID);

        // check for the highest level of rarity in the pair of cards. If both cards have the same rarity, upgrade the rarity by 1 level.
        if (a.rarity > b.rarity)
        {
            returnCard.rarity = a.rarity;
        }
        else
        {
            returnCard.rarity = b.rarity;
        }
        if (returnCard.rarity != CardRarity.Epic && a.rarity == b.rarity) // if both cards have the same rarity, upgrade the rarity
        {
            returnCard.rarity += 1;
        }

        return returnCard;
    }


    //combination of both cardsOld
    public CardValue OldCombine(CardValue a, CardValue b)
    {
        if (a == null || b == null)
        {
            Debug.LogWarning("[Combine] One or both cards are null.");
            return new CardValue(new ExcelCardData());
        }

        // If combination rule exists
        if (!combinationTable.TryGetValue((a.GetID(), b.GetID()), out CardCombinationRule rule))
        {
            Debug.Log("[Combine] No combination for: " + a.CardName + " + " + b.CardName);
            return new CardValue(new ExcelCardData());
        }

        Debug.Log($"[Combine] Attempting: {a.CardName} + {b.CardName} → {rule.Result}");

        // Roll success/failure
        bool success = Random.value >= rule.FailChance;

        if (success)
        {
            Debug.Log("[Combine] SUCCESS → " + rule.Result);
            return GameValue.Instance.GetInitCardValue(rule.Result);
        }
        else
        {
            Debug.Log("[Combine] FAIL (Chance: " + rule.FailChance + ")");
            return null;
        }
    }

    public CardValue GetResultCard(CardValue a, CardValue b)
    {
        if (a == null || b == null)
        {
            Debug.LogWarning("[Combine] One or both cards are null.");
            return new CardValue(new ExcelCardData());
        }


        return Combine(a, b);
    }

}