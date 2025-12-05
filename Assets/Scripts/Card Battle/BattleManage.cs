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
            BattlePlayerValue.Instance.StartTurn();
        } else
        {
            Debug.Log("Enemy Turn");
            
            BattleEnemyManager.Instance.ProcessEnemyStatuses();
            StartCoroutine(BattleEnemyManager.Instance.EnemyTurn());
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
    public void ApplyPlayerCardEffect(CardValue card, EnemyBattleControl target)
    {
        Debug.Log($"Player uses {card.CardName}!");

        switch (card.CardName)
        {
            case "Knife":
                target.DealDamage(20);
                break;

            default:
                Debug.LogWarning($"Card {card.CardName} has no effect implemented yet.");
                break;
        }
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
