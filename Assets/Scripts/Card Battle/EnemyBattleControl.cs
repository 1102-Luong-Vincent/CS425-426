using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBattleControl : MonoBehaviour
{
    public TextMeshProUGUI enemyNameText;
    public SpriteRenderer enemySprite;
    public Slider healthBar;
    public TextMeshProUGUI healthText;
    private EnemyValue enemyValue;
    public EnemyValue EnemyValueReference => enemyValue;
    int enemyID;
    string currentDirection;

    [Header("Animation")]
    public List<RuntimeAnimatorController> enemyAnimators = new List<RuntimeAnimatorController>();
    private Animator animator;

    [Header("Sound Effects")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip zombieDeathSound;
    [SerializeField] AudioClip explosionSound;

    [SerializeField] private DamageText damageText;

    [Header("Visual Effects")]
    [SerializeField] ParticleSystem bloodEffect;

    private EnemySnapshot startingBattleState;

    [System.Serializable]
    public class EnemySnapshot
    {
        public int Health;
        public int MaxHealth;
        public EnemyValue EnemyValue;
    }

    public void Init(EnemyValue enemyValue)
    {
        this.enemyValue = enemyValue;
        enemyNameText.text = enemyValue.EnemyName;
        enemySprite.sprite = enemyValue.GetSprite();

        if(enemyValue.GetID() == 4)
        {
            enemyValue.explodeOnDeath = true;
            enemyValue.explosionDamage = 15;
            //audioSource.PlayOneShot(explosionSound);
        }

        InitAnimator();

        SetHealth();
        Listener(true);

        CaptureStartingState();
    }

    void InitAnimator()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning($"[EnemyBattleControl] No Animator found on {gameObject.name}");
            return;
        }

        if (enemyAnimators != null && enemyAnimators.Count > 0 && enemyValue != null)
        {
            int animatorIndex = enemyValue.GetID(); 

            if (animatorIndex >= 0 && animatorIndex < enemyAnimators.Count)
            {
                if (enemyAnimators[animatorIndex] != null)
                {
                    animator.runtimeAnimatorController = enemyAnimators[animatorIndex];
                    Debug.Log($"[EnemyBattleControl] Set animator to: {enemyAnimators[animatorIndex].name}");
                }
            }
        }

        SetDefaultDirection();
    }
    void SetDefaultDirection()
    {
        if (animator == null) return;

        ResetDirectionBools();

        currentDirection = "isWest";
        animator.SetBool(currentDirection, true);
        TriggerTakeDamageAnimation();
    }

    void ResetDirectionBools()
    {
        string[] directions =
        {
        "isWest","isEast","isSouth","isSouthWest",
        "isNorthEast","isSouthEast","isNorth","isNorthWest"};

        foreach (var d in directions)
            animator.SetBool(d, false);
    }


    void SetHealth()
    {
        UpdateMaxHealthUI(enemyValue.MaxHealth);
        UpdateHealthUI(enemyValue.Health);
    }

    void Listener(bool isAdd)
    {
        if (enemyValue != null)
        {
            enemyValue.HealthListener(UpdateHealthUI, isAdd);
            enemyValue.MaxHealthListener(UpdateMaxHealthUI, isAdd);
        }
    }

    private void OnDestroy()
    {
        Listener(false);
    }

    private void UpdateHealthUI(int currentHealth)
    {
        if (healthBar != null)
            healthBar.value = currentHealth;
        if (healthText != null)
            healthText.text = $"{currentHealth}/{enemyValue.MaxHealth}";
    }

    private void UpdateMaxHealthUI(int maxHealth)
    {
        if (healthBar != null)
            healthBar.maxValue = maxHealth;
        if (healthText != null)
            healthText.text = $"{enemyValue.Health}/{maxHealth}";
    }

    public void DealDamage(int amount)
    {
        enemyValue.Health -= amount;

        SpawnBlood();

        Debug.Log($"Enemy took {amount} damage! has {enemyValue.Health} health left");
        //damageText.ShowDamage(amount, transform);

        TriggerTakeDamageAnimation();

        if (enemyValue.Health <= 0)
        {
            Debug.Log("Enemy died!");
            TriggerDieAnimation();
            
            StartCoroutine(DeathSequence());

            if (enemyValue.explodeOnDeath)
            {
                //StartCoroutine(DeathSequenceWithExplosion());
                Explode();
            }
            BattleEnemyManager.Instance.currentEnemys.Remove(this);
        }
    }

    public void Explode()
    {
        Debug.Log("Enemy exploded!");

        if (BattlePlayerValue.Instance == null)
        {
            Debug.LogError("Player instance is NULL!");
            return;
        }

        int damage = enemyValue.explosionDamage;
        BattlePlayerValue.Instance.Health -= damage;

        Debug.Log($"Player took {damage} from explosion");
    }


    //void ShowDamage(int damage, Transform targetTransform)
    //{
    //    Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

    //    GameObject textObj = Instantiate(damageTextPrefab, canvas.transform);
    //    DamageText dmgText = textObj.GetComponent<DamageText>();
    //    dmgText.ShowDamage(damage, targetTransform);
    //}

    private void SpawnBlood() //function for playing the particle system
    {

        if (bloodEffect == null) return;

        ParticleSystem effect = Instantiate(bloodEffect, transform.position, Quaternion.identity);

        effect.Play();

        Destroy(effect.gameObject, effect.main.duration + 1.0f);
    }
    private IEnumerator DeathSequence()
    {
        float animationLength = GetAnimationLength("dieWest");
        yield return new WaitForSeconds(animationLength);

        yield return new WaitForSeconds(2.0f);

        Explode(); //upon death, enemy explodes dealing additional damage to player

        //yield return new WaitForSeconds(1.3f);

        //DropResources();

        Destroy(gameObject);
    }

    //private IEnumerator DeathSequenceWithExplosion(float delay)
    //{
    //    float animationLength = GetAnimationLength("dieWest");
    //    yield return new WaitForSeconds(animationLength);

    //    Explode(); //upon death, enemy explodes dealing additional damage to player

    //    yield return new WaitForSeconds(animationLength);
    //    Destroy(gameObject);
    //}
    private float GetAnimationLength(string animationName)
    {
        if (animator == null) return 2f;

        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        foreach (var clip in clipInfo)
        {
            if (clip.clip.name.Contains(animationName))
            {
                return clip.clip.length;
            }
        }
        return 2f; // 默认值
    }

    public void TriggerDieAnimation()
    {
        if (animator == null || !gameObject.activeInHierarchy) return;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);

        if(audioSource != null && zombieDeathSound != null)
        {
            audioSource.PlayOneShot(zombieDeathSound);
        }
        animator.SetTrigger("dieWest");
    }

    public void TriggerTakeDamageAnimation()
    {
        if (animator == null) return;

        animator.SetBool("isTakeDamage", true);

        animator.SetBool("TakeDamageWest", true);

        //if (audioSource != null && zombieHitSound != null)
        //{
        //    audioSource.PlayOneShot(zombieHitSound);
        //}
        
        StartCoroutine(ResetTakeDamageParameters());
    }

    public Animator GetAnimator()
    {
        return animator;
    }
    private System.Collections.IEnumerator ResetTakeDamageParameters()
    {
        yield return new WaitForSeconds(0.5f);

        if (animator != null)
        {
            animator.SetBool("isTakeDamage", false);
            animator.SetBool("TakeDamageWest", false);
        }
    }

    public void PlayAttackSound()
    {
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    public void DropResources()
    {
        if (enemyValue.ResourceDrops == null || enemyValue.ResourceDrops.Count == 0)
            return;

        string resourceText = "You Win! You have acquired ";

        foreach(var resource in enemyValue.ResourceDrops) //counts each resource and directly adds the resource to the player's inventory
        {
            int dropAmount = UnityEngine.Random.Range(3, 7); 
            resource.amount = dropAmount;
            BattlePlayerValue.Instance.AddResource(resource);

            resourceText += $"{resource.resourceName} x{dropAmount} ";

            //if(InteractableNotification.Instance != null)
            //{
            //    InteractableNotification.Instance.ShowResourceNotification(
            //        resource.resourceName,
            //        resource.resourceIcon,
            //        dropAmount
            //    );
            //}
        }

        if(BattleRewards.Instance != null)
        {
            BattleRewards.Instance.ShowReward(resourceText);
        }

        Debug.Log($"Dropped {enemyValue.ResourceDrops.Count} resources from {enemyValue.EnemyName}");
    }

    #region Saves enemy state upon restarting battle
    public void CaptureStartingState()
    {
        startingBattleState = new EnemySnapshot
        {
            Health = enemyValue.Health,
            MaxHealth = enemyValue.MaxHealth,
            EnemyValue = enemyValue
        };
    }

    public void RestoreStartingState()
    {
        if (startingBattleState == null) return;

        enemyValue.MaxHealth = startingBattleState.MaxHealth;
        enemyValue.Health = startingBattleState.Health;

        UpdateMaxHealthUI(enemyValue.MaxHealth);
        UpdateHealthUI(enemyValue.Health);

        // Reactivate enemy if it was destroyed/dead
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

    }
    #endregion
}