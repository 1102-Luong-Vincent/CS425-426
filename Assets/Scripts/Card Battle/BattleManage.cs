// Authors: Vincent Luong and Shawn Meng
// Created by: Shawn Meng
// Modified by: Vincent Luong, Yuhan Tang
// No external source was used

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BattleManage : MonoBehaviour
{
    public static BattleManage Instance { get; private set; }

    private BattleData battleData;
    public BattleUIManager BattleUIManager;
    [SerializeField] private BattleAnimation battleAnimation;


    [SerializeField] BattlePlayerController player;

    private int turn = 1;
    private event Action<int> OnTurnChanged;
    bool isPlayerAttacking = false;
    private bool playerActionLocked = false;
    public bool isBattleOver = false;

    public int Turn
    {
        get => turn;
        set
        {
            if (turn != value)
            {
                turn = value;
                OnTurnChanged?.Invoke(turn);
            }
        }
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        ResetBattle();
        battleData = GameValue.Instance.GetBattleData();
        BattleEnemyManager.Instance.SetEnemy(battleData);
        //BattleEnemyManager.Instance.SetEnemy();
        SetValue();
    }

    void SetValue()
    {
        Turn = 0;
        Test();
        StartNextTurn();
    }

    void Test()
    {
        BattlePlayerValue.Instance.SetBattlePlayerValue(GameValue.Instance.GetPlayerValue());
    }

    public void StartNextTurn()
    {
        Turn++;

        SetPlayerActionLocked(false);

        if (IsPlayerTurn())
        {
            if (BattlePlayerValue.Instance.Health > 0)
            {
                BattlePlayerValue.Instance.StartTurn(Turn);
            }
            else
            {
                SetBattleOver(true);
                BattlePlayerUIManager.Instance.HideAllCards();
                BattleUIManager.DisplayGameOver();
            }
        } else
        {
            if (!AllEnemiesDead())
            {
                Debug.Log("Enemy Turn");
                BattleEnemyManager.Instance.ProcessEnemyStatuses();
                StartCoroutine(BattleEnemyManager.Instance.EnemyTurn());
            }
            else
            {
                Debug.Log("enemies dead");
                StartCoroutine(BattleVictory());
            }
        }

        DebugTest();
    }

    public bool IsPlayerTurn()
    {
        return Turn % 2 == 1;
    }

    void DebugTest()
    {
        BattlePlayerTestUIManager.Instance.CheckPlayerState(BattlePlayerValue.Instance);
    }


    public bool AllEnemiesDead()
    {
        foreach (var enemy in BattleEnemyManager.Instance.GetEnemyBattleControls())
        {
            if (enemy.EnemyValueReference.Health  > 0) return false;
        }

        return true;
    }

    public IEnumerator BattleVictory()
    {
        yield return new WaitForSeconds(1);
        EndBattle();
    }
    void EndBattle() //modified to display a win panel for the rewards. 
    {

        GameValue.Instance.DefeatedEnemies(battleData.worldEnemyID);
        CompleteTutorialObjectiveIfNeeded();
        string rewardMessage = "";
        string bonusRewardMessage = "";

        foreach (var enemy in battleData.battleEnemys)
        {
            foreach (var resource in enemy.ResourceDrops)
            {
                int dropAmount = UnityEngine.Random.Range(3, 7);

                resource.amount = dropAmount;
                BattlePlayerValue.Instance.AddResource(resource);

                //string color = "#FFFFFF";

                //switch (resource.Type)
                //{
                //    case ResourceType.Chemical:
                //        color = "#2EC4B6"; // green
                //        break;

                //    case ResourceType.Material:
                //        color = "#FFD166"; // yellow
                //        break;
                //}

                rewardMessage += $" {resource.resourceName} x{dropAmount}\n";
            }
        }

        //added additional rewards for defeating enemy

        float itemDropChance = 0.15f; //15% chance enemy will drop card as a bonus reward
        float whetstoneDropChance = 0.5f; //50% chance enemy will drop whetstone as a bonus reward
        bool hasBonusReward = false;
        bool isBoss = battleData.battleEnemys.Exists(boss => boss.GetID() == 4 || boss.GetID() == 5 || boss.GetID() == 6); //if Id = 4, ID = 5 or ID = 6, considered to be boss or miniboss

        //random card bonus reward drop
        if (UnityEngine.Random.value <= itemDropChance && !isBoss) //only targets regular enemies
        {
            CardValue randomItemCard = GetRandomItemCardReward(); //references the first set of random cards (targets common and rare cards)

            if (randomItemCard != null)
            {
                //BattlePlayerValue.Instance.AddCard(randomItemCard);
                GameValue.Instance.GetPlayerValue().AddCard(randomItemCard.CardName); //after obtaining card as bonus reward, adds to inventory

                if (!hasBonusReward)
                {
                    bonusRewardMessage += $"\n<color=#FF0000> Bonus Reward:</color>\n"; //red color
                    hasBonusReward = true;
                }

                bonusRewardMessage += $" Card: {randomItemCard.CardName}\n";
            }
        }

        //whetstone bonus reward drop
        if (UnityEngine.Random.value <= whetstoneDropChance && !isBoss) //only targets regular enemies
        {
            int whetstoneDropAmount = UnityEngine.Random.Range(3, 7);
            ResourceValue whetstoneReward = new ResourceValue("Whetstone", whetstoneDropAmount, ResourceType.Material);
            BattlePlayerValue.Instance.AddResource(whetstoneReward);

            if (!hasBonusReward)
            {
                bonusRewardMessage += $"\n<color=#FF0000> Bonus Reward:</color>\n"; //red color
                hasBonusReward = true;
            }

            bonusRewardMessage += $" Whetstone x{whetstoneDropAmount}\n";
        }

        //boss and miniboss battle

        if (isBoss) //boss gives guaranteed rewards
        {
            CardValue randomItemCard = GetRandomItemCardReward2(); //refernces the second set of random cards (targets rare cards)

            if (randomItemCard != null) { }
            {
                BattlePlayerValue.Instance.AddCard(randomItemCard);
                bonusRewardMessage += $" Card: {randomItemCard.CardName}\n";
            }

            int whetstoneDropAmount = UnityEngine.Random.Range(10, 12);

            ResourceValue whetstoneReward = new ResourceValue("Whetstone", whetstoneDropAmount, ResourceType.Material);
            BattlePlayerValue.Instance.AddResource(whetstoneReward);

            bonusRewardMessage += $" Whetstone x{whetstoneDropAmount}\n";

        }
        if (BattleRewards.Instance != null)
        {
            Debug.Log("SHOWING PANEL");
            BattleRewards.Instance.ShowReward(rewardMessage,  bonusRewardMessage);
        }
        else
        {
            Debug.LogError("BattleRewards INSTANCE NULL");
        }

        //Debug.Log($"going back to previous map {battleData.GetMapScene()}");

        //foreach (var enemy in BattleEnemyManager.Instance.GetEnemyBattleControls())
        //{
        //    enemy.DropResources();
        //    break; // only call once
        //}

        //GameValue.Instance.LoadSceneByEnum(battleData.GetMapScene());
        //GameValue.Instance.SetPlayerPosition(battleData.GetMapPosition());
        //Debug.Log($"End Battle, and battleData enemys conut is {battleData.battleEnemys.Count}");
        //SoundManage.Instance.PlayBackgroundMusic(SoundManagerConstants.GameplayMusic);

    }

    private void CompleteTutorialObjectiveIfNeeded()
    {
        if (battleData.GetMapScene() == SceneType.GameStartScene &&
            battleData.worldEnemyID == 1 &&
            GameValue.Instance.GetCurrentObjective() == ObjectiveConstants.CompleteTutorial)
        {
            GameValue.Instance.SetCurrentObjective(ObjectiveConstants.LeaveStartRoom);
        }
    }

    CardValue GetRandomItemCardReward() //lists all cards
    {
        List<CardValue> possibleItemCards = new List<CardValue>()
        {
            GameValue.Instance.GetInitCardValue("Adrenal Medkit"),
            GameValue.Instance.GetInitCardValue("Antidote Potion"),
            GameValue.Instance.GetInitCardValue("Bandage"),
            GameValue.Instance.GetInitCardValue("Reflex Tonic"),
            GameValue.Instance.GetInitCardValue("Berserker Wrap"),
            GameValue.Instance.GetInitCardValue("Boosted Buzz"),
            GameValue.Instance.GetInitCardValue("Combat Patch"),
            GameValue.Instance.GetInitCardValue("Stamina Capsule"),
            GameValue.Instance.GetInitCardValue("Energy Potion"),
            GameValue.Instance.GetInitCardValue("Field Surgery Kit"),
            GameValue.Instance.GetInitCardValue("Health Potion"),
            GameValue.Instance.GetInitCardValue("Liquid Courage Kit"),
            GameValue.Instance.GetInitCardValue("Medkit"),
            GameValue.Instance.GetInitCardValue("Phoenix Shot"),
            GameValue.Instance.GetInitCardValue("Emergency Capsule"),
            GameValue.Instance.GetInitCardValue("Fury Catalyst"),
            GameValue.Instance.GetInitCardValue("Rapid Recovery Injector"),
            GameValue.Instance.GetInitCardValue("Revival Serum"),
            GameValue.Instance.GetInitCardValue("Stimulant Wrap"),
            GameValue.Instance.GetInitCardValue("Syringe")
        };

        // Remove nulls just in case
        possibleItemCards.RemoveAll(card => card == null);

        if (possibleItemCards.Count == 0) return null;

        int index = UnityEngine.Random.Range(0, possibleItemCards.Count);
        return possibleItemCards[index];
    }

    CardValue GetRandomItemCardReward2() //only lists the rare cards (more than one stat)
    {
        List<CardValue> possibleItemCards = new List<CardValue>()
        {
            GameValue.Instance.GetInitCardValue("Boosted Buzz"),
            GameValue.Instance.GetInitCardValue("Combat Patch"),
            GameValue.Instance.GetInitCardValue("Energy Potion"),
            GameValue.Instance.GetInitCardValue("Health Potion"),
            GameValue.Instance.GetInitCardValue("Liquid Courage Kit"),
            GameValue.Instance.GetInitCardValue("Phoenix Shot"),
            GameValue.Instance.GetInitCardValue("Fury Catalyst"),
            GameValue.Instance.GetInitCardValue("Rapid Recovery Injector"),
            GameValue.Instance.GetInitCardValue("Revival Serum"),
            GameValue.Instance.GetInitCardValue("Stimulant Wrap"),
            GameValue.Instance.GetInitCardValue("Syringe")
        };

        // Remove nulls just in case
        possibleItemCards.RemoveAll(card => card == null);

        if (possibleItemCards.Count == 0) return null;

        int index = UnityEngine.Random.Range(0, possibleItemCards.Count);
        return possibleItemCards[index];
    }

    public void ResetBattle()
    {
        Debug.Log("Resetting battle...");

        // 1. Reset player to starting state
        BattlePlayerValue.Instance.RestoreStartingState();

        // 2. Reset all enemies
        foreach (var enemy in BattleEnemyManager.Instance.GetEnemyBattleControls())
        {
            enemy.RestoreStartingState();
        }

        // 3. Reset turn counter
        Turn = 0;

        // 4. Start player turn
        BattlePlayerValue.Instance.StartTurn(Turn);

        // 5. Update UI
        BattleUIManager.SetTurnText(Turn);
    }

    public BattlePlayerController GetBattlePlayerController()
    {
        return player;
    }

    public void IsPlayerAttackingDone()
    {
        if (!IsPlayerTurn())
        {
            return;
        }

        isPlayerAttacking = false;
        StartNextTurn();
    }


    public bool IsPlayerActionLocked()
    {
        return playerActionLocked;
    }

    public void SetPlayerActionLocked(bool value)
    {
        playerActionLocked = value;
    }

    public bool BattleOver()
    {
        return isBattleOver;
    }

    public void SetBattleOver(bool value)
    {
        isBattleOver = value;
    }

    public BattleAnimation GetBattleAnimation()
    {
        return battleAnimation;
    }
    #region Turn Function Interface
    public void TurnListener(Action<int> listener, bool isAdd)
    {
        if (isAdd)
            OnTurnChanged += listener;
        else
            OnTurnChanged -= listener;
    }
    #endregion
}
