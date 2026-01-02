// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng
// no external source was used

using System;
using System.Collections.Generic;
using UnityEngine;

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

    public void UseWeaponEffect(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        switch (WeaponName)
        {
            case "Knife": UseKnife(player, enemys); break;
            case "Pistol": UsePistol(player, enemys); break;
            case "Shotgun": UseShotgun(player, enemys); break;
        }
    }

    void UseKnife(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.Health -= 10;
        enemys[0].Health -= 15;
    }

    void UsePistol(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        enemys[0].Health -= 10;
    }

    void UseShotgun(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        foreach (var enemy in enemys)
        {
            enemy.Health -= 5;   
        }
    }


}