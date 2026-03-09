using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [Header("Detection & Combat")]
    public CircleCollider2D searchRadius;
    [SerializeField] int EnemyID = 1;
    EnemyValue enemyValue;

    [Header("Movement")]
    Rigidbody2D rb;
    float speed = 0.5f;
    Transform target;

    [Header("Animation")]
    public List<RuntimeAnimatorController> enemyAnimators = new List<RuntimeAnimatorController>();
    private Animator animator;
    private Vector3 previousPosition;
    private string currentDirection = "isSouth";
    [SerializeField] private float directionUpdateInterval = 0.2f;
    private float nextDirectionUpdateTime = 0f;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        enemyValue = GameValue.Instance.GetInitEnemyValue(EnemyID);
        previousPosition = transform.position;
        SetAnimator();
    }

    void SetAnimator()
    {
        if (enemyAnimators == null || enemyAnimators.Count == 0)
        {
            Debug.LogWarning($"[EnemyControl] enemyAnimators list is empty!");
            return;
        }

        if (EnemyID < 0 || EnemyID >= enemyAnimators.Count)
        {
            Debug.LogError($"[EnemyControl] EnemyID {EnemyID} is out of range! List has {enemyAnimators.Count} animators.");
            return;
        }

        if (enemyAnimators[EnemyID] == null)
        {
            Debug.LogError($"[EnemyControl] Animator at index {EnemyID} is null!");
            return;
        }

        animator.runtimeAnimatorController = enemyAnimators[EnemyID];
    }



    void Update()
    {
        if (target != null)
        {
            Vector2 moveDir = Vector2.Lerp(rb.position, target.position, Time.fixedDeltaTime * speed);
            rb.MovePosition(moveDir);

            HandleMovementAnimation();
        }
        else
        {
            ResetAllMovementBools();
            animator.SetBool(currentDirection, true);
            animator.SetBool("isWalking", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy touched player -- Entering Battle");
            List<EnemyValue> enemyValues = new List<EnemyValue>() { enemyValue };
            BattleData battleData = new BattleData(enemyValues);
            battleData.SetMapScene(GameValue.Instance.GetCurrentScence());
            Debug.Log($"battleData Scene is {battleData.GetMapScene()}");

            battleData.SetMapPosition(GameValue.Instance.GetPlayerPosition());
            battleData.SetFieldMonster(gameObject);
            Destroy(gameObject);
            GameValue.Instance.SetBattleData(battleData);
            GameValue.Instance.LoadSceneByEnum(SceneType.BattleScene);
        }
    }

    private void HandleMovementAnimation()
    {
        if (animator == null) return;

        Vector3 currentPos = transform.position;
        Vector2 velocity = (currentPos - previousPosition) / Time.deltaTime;
        previousPosition = currentPos;

        float speedMagnitude = velocity.magnitude;
        bool isMoving = (speedMagnitude > 0.01f);

        ResetAllMovementBools();

        if (isMoving)
        {
            if (Time.time >= nextDirectionUpdateTime)
            {
                nextDirectionUpdateTime = Time.time + directionUpdateInterval;

                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                if (angle < 0) angle += 360f;
                string newDirection = DetermineDirectionFromAngle(angle);

                if (newDirection != currentDirection)
                {
                    UpdateDirection(newDirection);
                    currentDirection = newDirection;
                }
            }

            string movementDirection = currentDirection.Substring(2);
            SetMovementAnimation(true, "Move", movementDirection);
        }
    }

    private void UpdateDirection(string newDirection)
    {
        if (animator == null) return;

        string[] directions = {
            "isWest", "isEast", "isSouth", "isSouthWest", "isNorthEast",
            "isSouthEast", "isNorth", "isNorthWest"
        };

        foreach (string d in directions)
        {
            animator.SetBool(d, false);
        }

        animator.SetBool(newDirection, true);
        animator.SetBool("isWalking", true);
    }

    private void SetMovementAnimation(bool isActive, string baseKey, string direction)
    {
        if (animator == null) return;

        if (isActive)
        {
            string animationKey = $"{baseKey}{direction}";
            animator.SetBool(animationKey, true);
        }
    }

    private void ResetAllMovementBools()
    {
        if (animator == null) return;

        string[] directions = {
            "North", "South", "East", "West",
            "NorthEast", "NorthWest", "SouthEast", "SouthWest"
        };

        foreach (string baseKey in new string[] { "Move", "RunBackwards", "StrafeLeft", "StrafeRight" })
        {
            foreach (string direction in directions)
            {
                animator.SetBool($"{baseKey}{direction}", false);
            }
        }

        animator.SetBool("CrouchRunNorth", false);
        animator.SetBool("CrouchRunSouth", false);
        animator.SetBool("CrouchRunEast", false);
        animator.SetBool("CrouchRunWest", false);
        animator.SetBool("CrouchRunNorthEast", false);
        animator.SetBool("CrouchRunNorthWest", false);
        animator.SetBool("CrouchRunSouthEast", false);
        animator.SetBool("CrouchRunSouthWest", false);
    }

    private string DetermineDirectionFromAngle(float angle)
    {
        if (angle >= 330 || angle < 15)
            return "isEast";
        else if (angle >= 15 && angle < 60)
            return "isNorthEast";
        else if (angle >= 60 && angle < 120)
            return "isNorth";
        else if (angle >= 120 && angle < 165)
            return "isNorthWest";
        else if (angle >= 165 && angle < 195)
            return "isWest";
        else if (angle >= 195 && angle < 240)
            return "isSouthWest";
        else if (angle >= 240 && angle < 300)
            return "isSouth";
        else if (angle >= 300 && angle < 345)
            return "isSouthEast";

        return "isEast"; 
    }

    public void TriggerDieAnimation()
    {
        if (animator == null || !gameObject.activeInHierarchy) return;

        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);

        if (currentDirection.Equals("isNorth"))
            animator.SetTrigger("dieNorth");
        else if (currentDirection.Equals("isSouth"))
            animator.SetTrigger("dieSouth");
        else if (currentDirection.Equals("isEast"))
            animator.SetTrigger("dieEast");
        else if (currentDirection.Equals("isWest"))
            animator.SetTrigger("dieWest");
        else if (currentDirection.Equals("isNorthEast"))
            animator.SetTrigger("dieNorthEast");
        else if (currentDirection.Equals("isNorthWest"))
            animator.SetTrigger("dieNorthWest");
        else if (currentDirection.Equals("isSouthEast"))
            animator.SetTrigger("dieSouthEast");
        else if (currentDirection.Equals("isSouthWest"))
            animator.SetTrigger("dieSouthWest");
    }

    public void TriggerTakeDamageAnimation()
    {
        if (animator == null) return;

        animator.SetBool("isTakeDamage", true);

        if (animator.GetBool("isNorth"))
            animator.SetBool("TakeDamageNorth", true);
        else if (animator.GetBool("isSouth"))
            animator.SetBool("TakeDamageSouth", true);
        else if (animator.GetBool("isEast"))
            animator.SetBool("TakeDamageEast", true);
        else if (animator.GetBool("isWest"))
            animator.SetBool("TakeDamageWest", true);
        else if (animator.GetBool("isNorthEast"))
            animator.SetBool("TakeDamageNorthEast", true);
        else if (animator.GetBool("isNorthWest"))
            animator.SetBool("TakeDamageNorthWest", true);
        else if (animator.GetBool("isSouthEast"))
            animator.SetBool("TakeDamageSouthEast", true);
        else if (animator.GetBool("isSouthWest"))
            animator.SetBool("TakeDamageSouthWest", true);

        StartCoroutine(ResetTakeDamageParameters());
    }

    private IEnumerator ResetTakeDamageParameters()
    {
        yield return new WaitForSeconds(0.5f);

        if (animator != null)
        {
            animator.SetBool("isTakeDamage", false);
            animator.SetBool("TakeDamageNorth", false);
            animator.SetBool("TakeDamageSouth", false);
            animator.SetBool("TakeDamageEast", false);
            animator.SetBool("TakeDamageWest", false);
            animator.SetBool("TakeDamageNorthEast", false);
            animator.SetBool("TakeDamageNorthWest", false);
            animator.SetBool("TakeDamageSouthEast", false);
            animator.SetBool("TakeDamageSouthWest", false);
        }
    }

    public EnemyValue GetEnemyValue() {
        return enemyValue;
    }

    public int GetEnemyID() {

        return EnemyID;
    }


}