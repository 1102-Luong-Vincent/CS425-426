using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneDataManager : MonoBehaviour
{
    [SerializeField] List<EnemyControl> enemyControls = new List<EnemyControl>();

    private void Start()
    {
        // Automatically collect enemies in scene
        enemyControls = new List<EnemyControl>(GetComponentsInChildren<EnemyControl>(true));

        Debug.Log($"Start(): enemyControls collected = {enemyControls.Count}");

        CheckDestroyEnemys();
    }

    void CheckDestroyEnemys()
    {
        if (GameValue.Instance == null)
        {
            Debug.Log("GameValue.Instance is NULL");
            return;
        }

        if (GameValue.Instance.GetBattleData() == null)
        {
            Debug.Log("GameValue.Instance.GetBattleData() is NULL");
            return;
        }

        BattleData battleData = GameValue.Instance.GetBattleData();

        if (battleData.GetMapScene() != GameValue.Instance.GetCurrentScence())
        {
            Debug.Log("BattleData does not belong to this scene.");
            return;
        }

        DestroyEnemyByWorldID(battleData.GetWorldEnemyID());
    }

#if UNITY_EDITOR
    [ContextMenu("Collect EnemyControls From Children")]
    void CollectEnemies()
    {
        Undo.RecordObject(this, "Collect EnemyControls");
        enemyControls = new List<EnemyControl>(GetComponentsInChildren<EnemyControl>(true));
        EditorUtility.SetDirty(this);

        Debug.Log($"Collected {enemyControls.Count} EnemyControl(s)");
    }
#endif

    public void DestroyEnemys(List<EnemyValue> destroyEnemyValues)
    {
        Debug.Log($"DestroyEnemys(): destroyEnemyValues count = {destroyEnemyValues.Count}");
        Debug.Log($"DestroyEnemys(): enemyControls count = {enemyControls.Count}");

        foreach (EnemyValue e in destroyEnemyValues)
        {
            if (e == null) continue;
            Debug.Log($"Destroy target enemy: {e.EnemyName} ID {e.GetID()}");
        }

        List<EnemyControl> toRemove = new List<EnemyControl>();

        foreach (EnemyControl enemy in enemyControls)
        {
            if (enemy == null)
            {
                Debug.Log("enemyControls contains NULL");
                continue;
            }

            int enemyID = enemy.GetEnemyID();

            foreach (EnemyValue destroyEnemy in destroyEnemyValues)
            {
                if (destroyEnemy == null) continue;

                if (destroyEnemy.GetID() == enemyID)
                {
                    Debug.Log($"Destroyed : {enemy.name}");

                    toRemove.Add(enemy);
                    break;
                }
                else
                {
                    Debug.Log(
                        $"No match -> destroyEnemy: {destroyEnemy.EnemyName} {destroyEnemy.GetID()} " +
                        $"vs sceneEnemy: {enemy.name} {enemyID}"
                    );
                }
            }
        }

        foreach (EnemyControl enemy in toRemove)
        {
            enemyControls.Remove(enemy);
        }

        foreach (EnemyControl enemy in toRemove)
        {
            Destroy(enemy.gameObject);
        }



        Debug.Log($"DestroyEnemys finished. Remaining enemies: {enemyControls.Count}");
    }

    private void DestroyEnemyByWorldID(int worldEnemyID)
    {
        if (worldEnemyID <= 0)
        {
            return;
        }

        EnemyControl targetEnemy = null;
        foreach (EnemyControl enemy in enemyControls)
        {
            if (enemy != null && enemy.GetWorldEnemyID() == worldEnemyID)
            {
                targetEnemy = enemy;
                break;
            }
        }

        if (targetEnemy == null)
        {
            return;
        }

        enemyControls.Remove(targetEnemy);
        Destroy(targetEnemy.gameObject);
    }
}
