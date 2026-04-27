// Author: Shawn Meng, Yuhan Tang
// Created by: Shawn Meng, Yuhan Tang
// Modified by: Shawn Meng, Yuhan Tang
// no external source was used

using System;
using System.Collections.Generic;
using UnityEngine;
using static ExcelReader;

public struct DamageResult
{
    public int Damage;
    public bool IsHit;
    public bool IsCritical;
}


public class WeaponValue
{
    public string WeaponName;
    public Sprite WeaponSprite;
    public string WeaponDescribe;
    public CardRarity rarity;
    public CardAbility ability;
    public int weaponLevel;
    public int maxLevel = 3;
    public float damage;
    public string upgradeMaterialName;
    public int upgradeMaterialNeed;

    public WeaponValue() { }

    public WeaponValue(ExcelWeaponData excelData) 
    {
        this.WeaponName = excelData.weaponName;
        rarity = excelData.rarity;
        ability = excelData.ability;
        WeaponDescribe = excelData.weaponDescribe;
        weaponLevel = excelData.weaponLevel;
        maxLevel = excelData.maxLevel;
        damage = excelData.damage;

        upgradeMaterialName = excelData.materialName;
        upgradeMaterialNeed = excelData.materialNeed;

        WeaponSprite = GetWeaponSprite();

    }
    Sprite GetWeaponSprite()
    {
        string path = $"Sprite/Card/Weapons/{WeaponName}/{WeaponName.ToLower()}";
        Sprite cardSprite = Resources.Load<Sprite>(path);
        if (cardSprite == null) Debug.LogWarning($"[LoadCardSprite] cardSprite == null, check path or filename [LoadCardSprite] Try load: {path}!");
        return cardSprite;
    }

    public void UseWeaponEffect(BattlePlayerValue player, List<EnemyBattleControl> enemys)
    {
        switch (WeaponName)
        {
            case "Knife": UseKnife(player, enemys); break;
            case "Pistol": UsePistol(player, enemys); break;
            case "Shotgun": UseShotgun(player, enemys); break;
        }
    }

    void UseKnife(BattlePlayerValue player, List<EnemyBattleControl> enemys)
    {
        DamageResult damageResult = player.GetDamageDetailed(damage, 1f);
        enemys[0].DealDamage(damageResult.Damage);
    }

    void UsePistol(BattlePlayerValue player, List<EnemyBattleControl> enemys)
    {
        DamageResult damageResult = player.GetDamageDetailed(damage, 0.9f);
        enemys[0].DealDamage(damageResult.Damage);
    }

    void UseShotgun(BattlePlayerValue player, List<EnemyBattleControl> enemys)
    {
        if (enemys == null || enemys.Count == 0 || enemys[0] == null) return;

        EnemyBattleControl target = enemys[0];
        const int pelletCount = 3;
        const float pelletHitChance = 0.7f;

        for (int i = 0; i < pelletCount; i++)
        {
            if (target.EnemyValueReference.Health <= 0) break;

            DamageResult damageResult = GetShotgunDamageDetailed(player, pelletHitChance);
            if (damageResult.IsHit)
            {
                target.DealDamage(damageResult.Damage);
            }
        }
    }

    DamageResult GetShotgunDamageDetailed(BattlePlayerValue player, float hitChance)
    {
        DamageResult result = new DamageResult();

        result.IsHit = UnityEngine.Random.value < hitChance;
        if (!result.IsHit)
        {
            result.Damage = 0;
            result.IsCritical = false;
            return result;
        }

        float baseDamage = (player.state.Attack * 0.5f) + damage;
        baseDamage *= player.state.AttackBuff;

        result.IsCritical = UnityEngine.Random.value < player.state.CriticalChanceBuff;
        if (result.IsCritical)
        {
            Debug.Log("Critical Hit!");
            baseDamage *= player.state.CriticalDamageBuff;
        }

        result.Damage = Mathf.RoundToInt(baseDamage);
        return result;
    }

}
