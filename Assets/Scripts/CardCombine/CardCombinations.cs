using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CardCombinationRule
{
    public string Card1;
    public string Card2;
    public string Result;
    public float FailChance;

    public CardCombinationRule(string a, string b, string result, float failChance)
    {
        Card1 = a;
        Card2 = b;
        Result = result;
        FailChance = failChance;
    }
}

public class CardCombinations : MonoBehaviour
{
    public static CardCombinations Instance;

    public Dictionary<(string, string), CardCombinationRule> combinationTable = new Dictionary<(string, string), CardCombinationRule>();

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

    void AddRule(string a, string b, string result, float failChance)
    {
        var rule = new CardCombinationRule(a, b, result, failChance);

        combinationTable[(a, b)] = rule;
        combinationTable[(b, a)] = rule;
    }

    void LoadDefaultCombinations()
    {
        combinationTable.Clear();

        //bandage combinations
        AddRule("Bandage", "Medkit", "Field Surgery Kit", 0.10f);
        AddRule("Bandage", "Syringe", "Adrenal Medkit", 0.10f);
        AddRule("Bandage", "Pills", "Combat Patch", 0.10f);
        AddRule("Bandage", "Rage Pill", "Berserker Wrap", 0.15f);
        AddRule("Bandage", "Drugs", "Stimulant Wrap", 0.15f);
        AddRule("Bandage", "Beer", "Liquid Courage Kit", 0.10f);

        //syringe combinations
        AddRule("Syringe", "Medkit", "Rapid Recovery Injector", 0.10f);
        AddRule("Syringe", "Revival Serum", "Phoenix Shot", 0.25f);
        AddRule("Syringe", "Beer", "Boosted Buzz", 0.10f);

        //medkit combinations
        AddRule("Medkit", "Beer", "Survivor's Brew", 0.10f);

        //pills combination
        AddRule("Pills", "Rage Pill", "Overdrive Dose", 0.15f);
        AddRule("Pills", "Syringe", "Combat Stimulant", 0.10f);
        AddRule("Pills", "Medkit", "Balanced Formula", 0.10f);
        AddRule("Pills", "Revival Serum", "Rebirth Capsule", 0.25f);
        AddRule("Pills", "Drugs", "Titan Formula", 0.15f);
        AddRule("Pills", "Beer", "Battle Brew", 0.10f);

        //rage combination
        AddRule("Rage Pill", "Syringe", "Adrenal Surge", 0.15f);
        AddRule("Rage Pill", "Medkit", "Berserker Serum", 0.15f);
        AddRule("Rage Pill", "Revival Serum", "Fury Rebirth", 0.30f);
        AddRule("Rage Pill", "Drugs", "Aggression Compound", 0.20f);
        AddRule("Rage Pill", "Beer", "Drunken Rage", 0.15f);

        //drug combination
        AddRule("Drugs", "Medkit", "Adrenal Mix", 0.15f);
        AddRule("Drugs", "Syringe", "Reflex Booster", 0.15f);
        AddRule("Drugs", "Revival Serum", "Bio-Regen Formula", 0.25f);
        AddRule("Drugs", "Beer", "Nerve Enhancer", 0.15f);

        AddRule("Beer", "Revival Serum", "Soul Brew", 0.25f);

        Debug.Log("[CardCombinations] Loaded " + combinationTable.Count + " combinations.");
    }

    //combination of both cards
    public CardValue Combine(CardValue a, CardValue b)
    {
        if (a == null || b == null)
        {
            Debug.LogWarning("[Combine] One or both cards are null.");
            return null;
        }

        // If combination rule exists
        if (!combinationTable.TryGetValue((a.CardName, b.CardName), out CardCombinationRule rule))
        {
            Debug.Log("[Combine] No combination for: " + a.CardName + " + " + b.CardName);
            return null;
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
}

