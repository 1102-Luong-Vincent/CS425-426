using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class BattleData
{
    public List<EnemyValue> battleEnemys = new List<EnemyValue>();
    public List<CardValue> totalEnemyCardAtBattle = new List<CardValue>();

    public BattleData(List<EnemyValue> battleEnemys)
    {
        this.battleEnemys = battleEnemys;
        SetTotalEnemyCardAtBattle();
    }


    void SetTotalEnemyCardAtBattle()
    {
        totalEnemyCardAtBattle.Clear();

        int totalIndex = GameValue.Instance.GetPlayerValue().HadCardsLibrary.Count;
        List<int> allEnemyCardIDs = new List<int>();
        foreach (var enemy in battleEnemys)
        {
            if (enemy.enemyDeckID.Count > 0)
            {
                allEnemyCardIDs.AddRange(enemy.enemyDeckID);
            }
        }

        if (allEnemyCardIDs.Count == 0)
        {
            Debug.LogWarning("No enemy cards available to build battle deck.");
            return;
        }
        System.Random rand = new System.Random();
        for (int i = allEnemyCardIDs.Count - 1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1);
            (allEnemyCardIDs[i], allEnemyCardIDs[j]) = (allEnemyCardIDs[j], allEnemyCardIDs[i]);
        }

        int count = Mathf.Min(totalIndex, allEnemyCardIDs.Count);
        for (int i = 0; i < count; i++)
        {
            int cardID = allEnemyCardIDs[i];
            CardValue card = GameValue.Instance.GetInitCardValue(cardID);
            if (card != null)
            {
                totalEnemyCardAtBattle.Add(card);
            }
            else
            {
                Debug.LogWarning($"Enemy card ID {cardID} not found in GameValue.");
            }
        }
    }
}
