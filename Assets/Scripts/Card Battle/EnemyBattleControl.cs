using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    public void Init(EnemyValue enemyValue)
    {
        this.enemyValue = enemyValue;
        enemyNameText.text = enemyValue.EnemyName;
        enemySprite.sprite = enemyValue.GetSprite();

        InitAnimator();

        SetHealth();
        Listener(true);
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
        Debug.Log($"Enemy took {amount} damage! has {enemyValue.Health} health left");
        TriggerTakeDamageAnimation();
        if (enemyValue.Health <= 0)
        {
            Debug.Log("Enemy died!");
            TriggerDieAnimation();
            BattleEnemyManager.Instance.currentEnemys.Remove(this);
            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DeathSequence()
    {
        float animationLength = GetAnimationLength("dieWest");
        yield return new WaitForSeconds(animationLength);

        Destroy(gameObject);
    }

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
        animator.SetTrigger("dieWest");
    }

    public void TriggerTakeDamageAnimation()
    {
        if (animator == null) return;

        animator.SetBool("isTakeDamage", true);

        animator.SetBool("TakeDamageWest", true);

        StartCoroutine(ResetTakeDamageParameters());
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
}