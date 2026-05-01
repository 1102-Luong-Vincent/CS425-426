// Authors: Vincent Luong and Shawn Meng
// Created by: Shawn Meng
// Modified by: Vincent Luong
// No external sources were used

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BattleEnemyManager : MonoBehaviour
{
    public static BattleEnemyManager Instance { get; private set; }
    public EnemyBattleControl EnemyControlPrefab;
    public List<EnemyBattleControl> currentEnemys = new List<EnemyBattleControl>();
    Animator anim;
    public EnemyValue enemyValue;

    private Vector2 rangeSize = new Vector2(2f, 2f); 
    public float minDistance = 1.5f;

    [Header("Visual Effects")]
    [SerializeField] public ParticleSystem bloodEffect;

    [Header("Sound Effects")]
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip playerHitSound;
    [SerializeField] public AudioClip enemyAttackSound;
    [SerializeField] public AudioClip machineGunSound;
    [SerializeField] public AudioClip swordAttackSound;
    [SerializeField] public AudioClip BatSwingSound;
    [SerializeField] public AudioClip knifeSwingSound;
    [SerializeField] public AudioClip unarmedAttackSound;
    [SerializeField] public AudioClip chainsawAttackSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
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

    // Process enemy statuses at the start of their turn (references BattleManage.cs, CardEffectParser.cs and EnemyValue.cs)
    public void ProcessEnemyStatuses()
    {
        foreach (var enemyCtrl in currentEnemys)
        {
            if (enemyCtrl == null) continue;

            EnemyValue e = enemyCtrl.EnemyValueReference;

        }
    }

    public IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1.2f);

        // pick an enemy card, apply effect, damage player etc.
        Debug.Log("Enemy action!");
        foreach (var enemy in currentEnemys)
        {
            //enemy.PlayAttackSound();
            if(enemy.EnemyValueReference.GetID() == 16 || enemy.EnemyValueReference.GetID() == 18 || enemy.EnemyValueReference.GetID() == 11)
            {
                audioSource.PlayOneShot(enemyAttackSound);
            }
            if(enemy.EnemyValueReference.GetID() == 12)
            {
                audioSource.PlayOneShot(machineGunSound);
            }
            if(enemy.EnemyValueReference.GetID() == 7 || enemy.EnemyValueReference.GetID() == 9 || enemy.EnemyValueReference.GetID() == 15 || enemy.EnemyValueReference.GetID() == 10)
            {
                audioSource.PlayOneShot(BatSwingSound);
            }
            if(enemy.EnemyValueReference.GetID() == 6 || enemy.EnemyValueReference.GetID() == 13)
            {
                audioSource.PlayOneShot(swordAttackSound);
            }
            if(enemy.EnemyValueReference.GetID() == 5)
            {
                audioSource.PlayOneShot(knifeSwingSound);
            }
            if(enemy.EnemyValueReference.GetID() == 17)
            {
                audioSource.PlayOneShot(unarmedAttackSound);
            }
            if(enemy.EnemyValueReference.GetID() == 8)
            {
                audioSource.PlayOneShot(chainsawAttackSound);
            }
            
            enemy.EnemyValueReference.UseEffect(BattlePlayerValue.Instance, GetEnemyValues());
            BattleManage.Instance.GetBattlePlayerController().SpawnBlood();
            anim = enemy.gameObject.GetComponent<Animator>();
            anim.SetBool("Attack2West", true);
            anim.SetBool("isAttackAttacking", true);
            if (BattlePlayerValue.Instance.GetPlayerHealth() > 0)
            {
                BattleManage.Instance.GetBattlePlayerController().GetComponent<Animator>().SetTrigger("Damage");
            }
            else
            {
                BattleManage.Instance.GetBattlePlayerController().GetComponent<Animator>().SetTrigger("Death");
            }
            if (audioSource != null && playerHitSound != null)
            {
                audioSource.PlayOneShot(playerHitSound);
            }

            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2West") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
            anim.SetBool("isAttackAttacking", false);
            anim.SetBool("Attack2West", false);
            
        }

        //yield return new WaitForSeconds(0.5f);

        BattleManage.Instance.StartNextTurn();
    }

    public List<EnemyValue> GetEnemyValues()
    {
        List < EnemyValue > enemyValues = new List <EnemyValue >();
        foreach (var enemy in currentEnemys)
        {
            enemyValues.Add(enemy.EnemyValueReference);
        }

        return enemyValues;
    }

    public List<EnemyBattleControl> GetEnemyBattleControls()
    {
        return currentEnemys;
    }
}