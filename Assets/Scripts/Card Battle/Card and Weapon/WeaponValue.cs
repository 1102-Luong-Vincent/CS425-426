using System;
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

}