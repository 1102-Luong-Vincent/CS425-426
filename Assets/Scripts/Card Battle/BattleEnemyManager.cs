using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class BattleEnemyManager : MonoBehaviour
{
    public static BattleEnemyManager Instance { get; private set; }
    public EnemyBattleControl EnemyControlPrefab;
    public List<EnemyBattleControl> currentEnemys = new List<EnemyBattleControl>();


    private Vector2 rangeSize = new Vector2(600f, 600f); 
    public float minDistance = 1.5f;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetEnemy(BattleData data)
    {
        foreach (var enemy in currentEnemys)
        {
            if (enemy != null) Destroy(enemy.gameObject);
        }
        currentEnemys.Clear();


        for (int i = 0; i < data.battleEnemys.Count; i++)
        {

            EnemyBattleControl newEnemy = Instantiate(
                EnemyControlPrefab,
                GetRandomPositionInTransform(),
                Quaternion.identity,
                transform
            );

            newEnemy.Init(data.battleEnemys[i]);
            currentEnemys.Add(newEnemy);
        }
    }


    private Vector3 GetRandomPositionInTransform()
    {
        const int maxTries = 100;
        for (int i = 0; i < maxTries; i++)
        {
            float x = transform.position.x + Random.Range(-rangeSize.x / 2f, rangeSize.x / 2f);
            float y = transform.position.y + Random.Range(-rangeSize.y / 2f, rangeSize.y / 2f);
            Vector3 candidate = new Vector3(x, y, 0f);

            bool overlap = false;
            foreach (var enemy in currentEnemys)
            {
                if (enemy == null) continue;
                if (Vector3.Distance(enemy.transform.position, candidate) < minDistance)
                {
                    overlap = true;
                    break;
                }
            }

            if (!overlap) return candidate;
        }

        return transform.position;
    }


}

