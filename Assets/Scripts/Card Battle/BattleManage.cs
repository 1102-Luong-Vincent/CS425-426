// Authors: Vincent Luong and Shawn Meng
// Created by: Shawn Meng
// Modified by: Vincent Luong
// Some code generated with assistance from ChatGPT.

using UnityEngine;
using System;

public class BattleManage : MonoBehaviour
{
    public static BattleManage Instance { get; private set; }

    private BattleData battleData;
    public BattleUIManager BattleUIManager;

    private int turn = 1;
    private event Action<int> OnTurnChanged;

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
        if (IsPlayerTurn())
        {
            if (BattlePlayerValue.Instance.Health > 0)
            {
                BattlePlayerValue.Instance.StartTurn();
            }
            else
            {
                BattleUIManager.DisplayGameOver();
            }
        } else
        {
            if (!allEnemiesDead())
            {
                Debug.Log("Enemy Turn");
                BattleEnemyManager.Instance.ProcessEnemyStatuses();
                StartCoroutine(BattleEnemyManager.Instance.EnemyTurn());
            }
            else
            {
                Debug.Log("enemies dead");
                EndBattle();
            }
        }

        DebugTest();
    }

    bool IsPlayerTurn()
    {
        return Turn % 2 == 1;
    }

    void DebugTest()
    {
        BattlePlayerTestUIManager.Instance.CheckPlayerState(BattlePlayerValue.Instance);
    }

    //Attack Card
    public void ApplyPlayerCardEffect(BattleCardControl card, EnemyBattleControl target)
    {
        if (card.GetCardValue() != null)
        {
            Debug.Log($"Player uses {card.GetCardValue().CardName}!");
        }
        else if (card.GetWeaponValue() != null)
        {
            target.DealDamage(20);
        }
            /*switch (card.CardName)
            {
                case "Knife":
                    target.DealDamage(20);
                    break;

                default:
                    Debug.LogWarning($"Card {card.CardName} has no effect implemented yet.");
                    break;
            }*/
    }

    public bool allEnemiesDead()
    {
        return BattleEnemyManager.Instance.currentEnemys.Count == 0;
    }

    void EndBattle()
    {
        Debug.Log($"going back to previous map {battleData.GetMapScene()}");
        GameValue.Instance.LoadSceneByEnum(battleData.GetMapScene());
        GameValue.Instance.SetPlayerPosition(battleData.GetMapPosition());
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
