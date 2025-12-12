// Author: Sean Masterson
// Created by: Sean Masterson
// Modified by: Sean Masterson
// No external source was used

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public SphereCollider searchRadius;
    [SerializeField] int EnemyID = -1;
    EnemyValue enemyValue;
    Rigidbody rb;
    float speed = 0.05f;
    Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        enemyValue = GameValue.Instance.GetInitEnemyValue(EnemyID);
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            Vector3 moveDir = Vector3.Lerp(rb.position, target.position, Time.fixedDeltaTime * speed);
            rb.MovePosition(moveDir);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log(gameObject.name + " detected " + other.gameObject.name);
            target = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log(gameObject.name + " lost " + other.gameObject.name);
            target = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("Enemy touched player -- Entering Battle");
            List<EnemyValue> enemyValues = new List<EnemyValue>() {enemyValue};
            BattleData battleData = new BattleData(enemyValues);
            GameValue.Instance.SetBattleData(battleData);
            GameValue.Instance.LoadSceneByEnum(SceneType.BattleScene);
        }
    }
}
