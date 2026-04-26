using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

public class WeaponUpgradeTests
{
    [Test]
    public void FindPlayerWeapon()
    {
        var player = new PlayerValue(skipInit: true);

        var knife = new WeaponValue{ WeaponName = "Knife", weaponLevel = 1 };
        player.HadWeaponsLibrary.Add(knife);

        bool found = player.HadWeaponsLibrary.Exists(w => w.WeaponName == "Knife");
        Assert.IsTrue(found);
        Debug.Log("Player have a Knife.");
    }

    [Test]
    public void NoMaterial()
    {
        var player = new PlayerValue(skipInit: true);

        // Knife LV1升级到LV2需要5个Whetstone
        var knifeLv2 = new WeaponValue{WeaponName = "Knife", weaponLevel = 2, upgradeMaterialName = "Whetstone", upgradeMaterialNeed = 5};

        // 玩家没有任何材料
        bool canUpgrade = player.GetMaterialCount(knifeLv2.upgradeMaterialName) >= knifeLv2.upgradeMaterialNeed;

        Assert.IsFalse(canUpgrade);
        Debug.Log("No material, cannot upgrade.");
    }

    [Test]
    public void NotEnoughMaterial()
    {
        var player = new PlayerValue(skipInit: true);

        // 升级材料不足够把武器升级到下一级
        var knifeLv2 = new WeaponValue{ WeaponName = "Knife", weaponLevel = 2, upgradeMaterialName = "Whetstone", upgradeMaterialNeed = 5 };

        player.AddMaterial("Whetstone", 3);

        bool canUpgrade = player.GetMaterialCount(knifeLv2.upgradeMaterialName) >= knifeLv2.upgradeMaterialNeed;

        Assert.IsFalse(canUpgrade);
        Debug.Log("No enough material, cannot upgrade.");
    }

    [Test]
    public void Level1ToLevel2Success()
    {
        var player = new PlayerValue(skipInit: true);

        var knifeLv1 = new WeaponValue{ WeaponName = "Knife", weaponLevel = 1, maxLevel = 3 };
        var knifeLv2 = new WeaponValue{WeaponName = "Knife", weaponLevel = 2, maxLevel = 3, damage = 15, upgradeMaterialName = "Whetstone", upgradeMaterialNeed = 5};

        player.HadWeaponsLibrary.Add(knifeLv1);
        player.AddMaterial("Whetstone", 5);

        // 确定玩家有足够数量的材料
        bool canUpgrade = player.GetMaterialCount(knifeLv2.upgradeMaterialName) >= knifeLv2.upgradeMaterialNeed;
        Assert.IsTrue(canUpgrade);

        // 进行升级
        bool spent = player.TrySpendMaterial(knifeLv2.upgradeMaterialName, knifeLv2.upgradeMaterialNeed);
        int index = player.HadWeaponsLibrary.IndexOf(knifeLv1);
        player.HadWeaponsLibrary[index] = knifeLv2;

        Assert.IsTrue(spent);
        Assert.AreEqual(0, player.GetMaterialCount("Whetstone"));
        Assert.AreEqual(2, player.HadWeaponsLibrary.Find(w => w.WeaponName == "Knife").weaponLevel);
        Assert.AreEqual(15f, player.HadWeaponsLibrary.Find(w => w.WeaponName == "Knife").damage);
        Debug.Log("Knife LV1 upgrade to LV2 success.");
    }

    [Test]
    public void Level2ToLevel3Success()
    {
        var player = new PlayerValue(skipInit: true);

        var knifeLv2 = new WeaponValue{WeaponName = "Knife", weaponLevel = 2, maxLevel = 3, damage = 15, upgradeMaterialName = "Whetstone", upgradeMaterialNeed = 10};
        var knifeLv3 = new WeaponValue{WeaponName = "Knife", weaponLevel = 3, maxLevel = 3, damage = 20, upgradeMaterialName = "", upgradeMaterialNeed = 0};

        player.HadWeaponsLibrary.Add(knifeLv2);
        player.AddMaterial("Whetstone", 10);

        bool spent = player.TrySpendMaterial("Whetstone", 10);
        int index = player.HadWeaponsLibrary.IndexOf(knifeLv2);
        player.HadWeaponsLibrary[index] = knifeLv3;

        Assert.IsTrue(spent);
        Assert.AreEqual(0, player.GetMaterialCount("Whetstone"));
        Assert.AreEqual(3, player.HadWeaponsLibrary.Find(w => w.WeaponName == "Knife").weaponLevel);
        Assert.AreEqual(20f, player.HadWeaponsLibrary.Find(w => w.WeaponName == "Knife").damage);
        Debug.Log("Knife LV2 upgrade to LV3 success.");
    }

    [Test]
    public void CannotUpgradeWhenMaxLevel()
    {
        var player = new PlayerValue(skipInit: true);

        // 武器升到LV3以后不能再升级
        var knifeLv3 = new WeaponValue{WeaponName = "Knife", weaponLevel = 3, maxLevel = 3};
        player.HadWeaponsLibrary.Add(knifeLv3);

        bool isMaxLevel = knifeLv3.weaponLevel >= knifeLv3.maxLevel;

        Assert.IsTrue(isMaxLevel);
        Debug.Log("Weapon cannot upgrade when it is max level.");
    }
}