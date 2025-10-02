using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyBattleControl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI enemyNameText;
    public Image enemyImage;
    public Slider healthBar;
    private EnemyValue enemyValue;

    public void Init(EnemyValue enemyValue)
    {
        this.enemyValue = enemyValue;
        enemyNameText.text = enemyValue.EnemyName;
        enemyImage.sprite = enemyValue.GetSprite();
        healthBar.maxValue = enemyValue.HP;
        healthBar.value = enemyValue.HP;

    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
