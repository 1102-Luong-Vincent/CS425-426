// Authors: Vincent Luong and Shawn Meng
// Created by: Shawn Meng
// Modified by: Vincent Luong
// Some code generated with assistance from ChatGPT.

using UnityEngine;
using System;
using System.Collections.Generic;

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
            if (!AllEnemiesDead())
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


    public bool AllEnemiesDead()
    {
        foreach (var enemy in BattleEnemyManager.Instance.GetEnemyValues())
        {
            if (enemy.Health > 0) return false;
        }

        return true;
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
