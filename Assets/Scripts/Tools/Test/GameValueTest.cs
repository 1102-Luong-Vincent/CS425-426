using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GameValueTest : MonoBehaviour
{
    public bool isTest;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Character Battle Test")]
    public BattleDataTest battleDataTest;

    public List<int> enemyIDs = new List<int>();

    [System.Serializable]
    public class BattleDataTest
    {
        public List<EnemyValue> battleEnemys = new List<EnemyValue>();
        public BattleDataTest(GameValue gameValue, List<int> enemyIDs)
        {
            foreach (var id in enemyIDs)
            {
                EnemyValue e = gameValue.GetInitEnemyValue(id);
                battleEnemys.Add(e);
            }
        }
    }


    public void SetTestValue(GameValue gameValue)
    {

        battleDataTest = new BattleDataTest(gameValue, enemyIDs);
        if (isTest)
        {
            BattleData battleData = new BattleData(battleDataTest.battleEnemys);
            gameValue.SetBattleData(battleData);
        }
        /*  GameValue.Instance.GetPlayerState().CountryENName = playerCountry;
          GameValue.Instance.GetPlayerState().CountryENName = playerCountry;*/
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
