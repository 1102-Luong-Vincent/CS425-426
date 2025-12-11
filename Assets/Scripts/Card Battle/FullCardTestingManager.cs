// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng
// Some code generated with assistance from ChatGPT.

using System.Collections.Generic;
using UnityEngine;

public class FullCardTestingManager : MonoBehaviour
{
    [Header("Card names to test (must match CardValue.xlsx CardName)")]
    public string[] cardNames;

    private BattlePlayerValue testPlayer;

    void Start()
    {
        Debug.Log("========== AUTO CARD TEST START ==========");

        PrepareTestPlayer();

        if (cardNames == null || cardNames.Length == 0)
        {
            Debug.LogWarning("[CardTestingFull] No card names set in Inspector.");
            return;
        }

        foreach (var name in cardNames)
        {
            if (string.IsNullOrWhiteSpace(name)) continue;
            TestSingleCard(name.Trim());
        }

        Debug.Log("========== AUTO CARD TEST COMPLETE ==========");
    }

    // ---------------------------------------------
    // 1. Setup Fake Player
    // ---------------------------------------------
    void PrepareTestPlayer()
    {
        // Either use existing BattlePlayerValue.Instance or create a dummy one
        if (BattlePlayerValue.Instance != null)
        {
            testPlayer = BattlePlayerValue.Instance;
            Debug.Log("[INIT] Using existing BattlePlayerValue.Instance");
        }
        else
        {
            GameObject playerObj = new GameObject("TestPlayer");
            testPlayer = playerObj.AddComponent<BattlePlayerValue>();
            Debug.Log("[INIT] Created dummy BattlePlayerValue");
        }

        ResetPlayerState();
    }

    void ResetPlayerState()
    {
        testPlayer.MaxHealth = 100;
        testPlayer.Health = 50;

        if (testPlayer.state == null)
            testPlayer.state = new BattlePlayerValue.State();

        testPlayer.state.AttackBuff = 0f;
        testPlayer.state.DefenseBuff = 0f;
        testPlayer.state.CriticalChanceBuff = 0f;
        testPlayer.state.CriticalDamageBuff = 0f;
        testPlayer.state.isBleeding = false;
        testPlayer.state.isPoisoned = false;
    }

    // ---------------------------------------------
    // 2. Test a single card by name
    // ---------------------------------------------
    void TestSingleCard(string cardName)
    {
        Debug.Log("---------------------------------------------------");
        Debug.Log("[CARD] Testing: " + cardName);

        CardValue card = GameValue.Instance.GetInitCardValue(cardName);
        if (card == null)
        {
            Debug.LogError("[CARD] Card not found in GameValue: " + cardName);
            return;
        }

        // Reset player each time so tests are independent
        ResetPlayerState();

        Debug.Log($"[BEFORE] HP: {testPlayer.Health}/{testPlayer.MaxHealth}, " +
                  $"ATK buff: {testPlayer.state.AttackBuff}, " +
                  $"DEF buff: {testPlayer.state.DefenseBuff}, " +
                  $"CRIT% buff: {testPlayer.state.CriticalChanceBuff}, " +
                  $"CRIT DMG buff: {testPlayer.state.CriticalDamageBuff}");

        // IMPORTANT: CardValue already parsed Excel effects internally,
        // so just call UseEffect instead of re-parsing strings.
        try
        {
            card.UseEffect(testPlayer);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ERROR] Exception while applying {cardName}: {ex.Message}\n{ex.StackTrace}");
        }

        Debug.Log($"[AFTER]  HP: {testPlayer.Health}/{testPlayer.MaxHealth}, " +
                  $"ATK buff: {testPlayer.state.AttackBuff}, " +
                  $"DEF buff: {testPlayer.state.DefenseBuff}, " +
                  $"CRIT% buff: {testPlayer.state.CriticalChanceBuff}, " +
                  $"CRIT DMG buff: {testPlayer.state.CriticalDamageBuff}");
    }
}
