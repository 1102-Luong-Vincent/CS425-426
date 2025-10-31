using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class EnemyBattleControl : MonoBehaviour
{
    public TextMeshProUGUI enemyNameText;
    public Image enemyImage;
    public Slider healthBar;
    public TextMeshProUGUI healthText;
    private EnemyValue enemyValue;

    public void Init(EnemyValue enemyValue)
    {
        this.enemyValue = enemyValue;

        enemyNameText.text = enemyValue.EnemyName;
        enemyImage.sprite = enemyValue.GetSprite();

        SetHealth();
        Listener(true);
    }


    void SetHealth()
    {
        UpdateMaxHealthUI(enemyValue.MaxHealth);
        UpdateHealthUI(enemyValue.Health);

    }


    void  Listener(bool isAdd)
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
}

