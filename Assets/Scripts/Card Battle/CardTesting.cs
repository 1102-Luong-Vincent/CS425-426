// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng
// Some code generated with assistance from ChatGPT.

using UnityEngine;
using System.Collections.Generic;


public class CardTesting : MonoBehaviour
{
    public string cardName = "Field Surgery Kit";   // set this in Inspector

    private void Start()
    {
        // 1. Get the card by name from GameValue
        CardValue card = GameValue.Instance.GetInitCardValue(cardName);
        if (card == null)
        {
            Debug.LogError("[TEST] Card not found: " + cardName);
            return;
        }

        Debug.Log("[TEST] Loaded Card: " + card.CardName);

        // 2. Get or create a BattlePlayerValue to apply the effect on
        var player = BattlePlayerValue.Instance;
        List<EnemyValue> enemys = BattleEnemyManager.Instance.GetEnemyValues();
        if (player == null)
        {
            Debug.LogError("[TEST] No BattlePlayerValue.Instance in scene! " +
                           "Add a GameObject with BattlePlayerValue component.");
            return;
        }

        // Make sure state exists
        if (player.state == null)
        {
            player.state = new BattlePlayerValue.State();
        }

        // 3. Setup dummy stats for testing
        player.MaxHealth = 100;
        player.Health = 50;

        Debug.Log($"[BEFORE] HP: {player.Health}, " +
                  $"ATK buff: {player.state.AttackBuff}, " +
                  $"DEF buff: {player.state.DefenseBuff}, " +
                  $"CRIT% buff: {player.state.CriticalChanceBuff}");

        // 4. Apply the card's effect (this uses the parsedEffects inside CardValue)
        card.UseEffect(player,enemys);

        // 5. Log the result
        Debug.Log($"[AFTER]  HP: {player.Health}, " +
                  $"ATK buff: {player.state.AttackBuff}, " +
                  $"DEF buff: {player.state.DefenseBuff}, " +
                  $"CRIT% buff: {player.state.CriticalChanceBuff}");
    }
}
