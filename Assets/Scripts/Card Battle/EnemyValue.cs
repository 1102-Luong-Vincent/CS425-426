using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class EnemyValue
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int ID;
    public string EnemyName;
    public Sprite EnemySprire;

    public int HP;
    public int attack;
    public int speed;

    public WeaponValue defaultWeapon;
    public List<int> enemyDeckID = new List<int>();

    public EnemyValue(ExcelEnemyData excelEnemyData)
    {
        ID = excelEnemyData.ID;
        EnemyName = excelEnemyData.enemyName;
        if (string.IsNullOrEmpty(EnemyName))
        {
            Debug.LogError("[SetEnemySprite] EnemyName is null or empty, cannot load sprite.");
            return;
        }


        HP = excelEnemyData.HP;
        attack = excelEnemyData.attack;
        speed = excelEnemyData.speed;  

        SetEnemySprite();
        defaultWeapon = GameValue.Instance.GetInitWeaponValue(excelEnemyData.defaultWeaponID);
        enemyDeckID = excelEnemyData.enemyDeck;
    }


    public Sprite GetSprite()
    {
        return EnemySprire;
    }

    void SetEnemySprite()
    {

        string path = $"Sprite/Enemy/{EnemyName}/{EnemyName.ToLower()}";
        EnemySprire = Resources.Load<Sprite>(path);
        if (EnemySprire == null) Debug.LogWarning($"[LoadCardSprite] cardSprite == null, check path or filename [LoadCardSprite] Try load: {path}!");
    }

}
