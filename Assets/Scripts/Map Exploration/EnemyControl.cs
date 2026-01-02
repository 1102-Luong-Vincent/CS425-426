// Author: Sean Masterson
// Created by: Sean Masterson
// Modified by: Vincent Luong
// No external source was used

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public CircleCollider2D searchRadius;
    [SerializeField] int EnemyID = -1;
    EnemyValue enemyValue;
    Rigidbody2D rb;
    float speed = 0.05f;
    Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        enemyValue = GameValue.Instance.GetInitEnemyValue(EnemyID);
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            //Vector3 moveDir = Vector3.Lerp(rb.position, target.position, Time.fixedDeltaTime * speed);
            Vector2 moveDir = Vector2.Lerp(rb.position, target.position, Time.fixedDeltaTime * speed);
            rb.MovePosition(moveDir);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log(gameObject.name + " detected " + other.gameObject.name);
            target = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(gameObject.name + " lost " + other.gameObject.name);
            target = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy touched player -- Entering Battle");
            List<EnemyValue> enemyValues = new List<EnemyValue>() {enemyValue};
            BattleData battleData = new BattleData(enemyValues);
            battleData.SetMapScene(GameValue.Instance.GetCurrentScence());
            battleData.SetMapPosition(GameValue.Instance.GetPlayerPosition()); // remember where we were on the map
            battleData.SetFieldMonster(gameObject);
            Destroy(gameObject);
            GameValue.Instance.SetBattleData(battleData);
            GameValue.Instance.LoadSceneByEnum(SceneType.BattleScene);

        }
    }
}
