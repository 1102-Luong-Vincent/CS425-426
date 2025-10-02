using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ExcelReader;

public class GameValueLibrary 
{
    private List<ExcelWeaponData> AllWeapons = new List<ExcelWeaponData>();
    private List<ExcelCardData> AllCards = new List<ExcelCardData>();
    private List<ExcelEnemyData> AllEnemys = new List<ExcelEnemyData>();


    public GameValueLibrary() {
        AllWeapons = GetWeaponsData();
        AllCards = GetCardData();
        AllEnemys = GetExcelEnemyDatas();
    }

    public WeaponValue GetInitWeapon(string weaponName)
    {
        WeaponValue weaponValue = new WeaponValue(AllWeapons.Find(data => data.weaponName == weaponName));
        return weaponValue;
    }

    public WeaponValue GetInitWeapon(int weaponID)
    {
        WeaponValue weaponValue = new WeaponValue(AllWeapons.Find(data => data.ID == weaponID));
        return weaponValue;
    }



    public CardValue GetInitCard(string cardName)
    {
        CardValue cardValue = new CardValue(AllCards.Find(data => data.cardName == cardName));
        return cardValue;
    }
    public CardValue GetInitCard(int cardId)
    {
        CardValue cardValue = new CardValue(AllCards.Find(data => data.ID == cardId));
        return cardValue;   
    }

    public EnemyValue GetInitEnemyValue(int enemyID)
    {
        EnemyValue enemyValue = new EnemyValue(AllEnemys.Find(data => data.ID == enemyID));
        return enemyValue;
    }

}
