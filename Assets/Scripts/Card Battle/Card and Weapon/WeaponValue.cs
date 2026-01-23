// Author: Shawn Meng, Yuhan Tang
// Created by: Shawn Meng, Yuhan Tang
// Modified by: Shawn Meng, Yuhan Tang
// no external source was used

using System;
using System.Collections.Generic;
using UnityEngine;

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


    public WeaponValue(ExcelWeaponData excelData) 
    {
        this.WeaponName = excelData.weaponName;
        rarity = excelData.rarity;
        ability = excelData.ability;
        WeaponDescribe = excelData.weaponDescribe;
        weaponLevel = excelData.weaponLevel;
        maxLevel = excelData.maxLevel;
        damage = excelData.damage;

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

        foreach (var enemy in enemys)
        {
            DamageResult damageResult = player.GetDamageDetailed(damage, 0.8f);
            enemy.DealDamage(damageResult.Damage);
        }
    }


}