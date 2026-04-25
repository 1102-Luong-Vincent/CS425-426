using System.Collections.Generic;
using UnityEngine;

public static class CardEffect
{
    
    public static void UseEffect(string CardName, BattlePlayerValue player, List<EnemyValue> enemys, CardRarity rarity)
    {
        switch (CardName)
        {
            case "Adrenal Medkit":
                DoAdrenalMedkit(player, enemys);
                break;
            case "Antidote Potion":
                DoAntidotePotion(player, enemys);
                break;
            case "Bandage":
                DoBandage(player, enemys);
                break;
            case "Reflex Tonic":
                DoReflexTonic(player, enemys);
                break;
            case "Berserker Wrap":
                DoBerserkerWrap(player, enemys);
                break;
            case "Boosted Buzz":
                DoBoostedBuzz(player, enemys);
                break;
            case "Combat Patch":
                DoCombatPatch(player, enemys);
                break;
            case "Stamina Capsule":
                DoStaminaCapsule(player, enemys);
                break;
            case "Energy Potion":
                DoEnergyPotion(player, enemys);
                break;
            case "Field Surgery Kit":
                DoFieldSurgeryKit(player, enemys);
                break;
            case "Health Potion":
                DoHealthPotion(player, enemys);
                break;
            case "Liquid Courage Kit":
                DoLiquidCourageKit(player, enemys);
                break;
            case "Medkit":
                DoMedkit(player, enemys);
                break;
            case "Phoenix Shot":
                DoPhoenixShot(player, enemys);
                break;
            case "Emergency Capsule":
                DoEmergencyCapsule(player, enemys);
                break;
            case "Fury Catalyst":
                DoFuryCatalyst(player, enemys);
                break;
            case "Rapid Recovery Injector":
                DoRapidRecoveryInjector(player, enemys);
                break;
            case "Revival Serum":
                DoRevivalSerum(player, enemys);
                break;
            case "Stimulant Wrap":
                DoStimulantWrap(player, enemys);
                break;
            case "Syringe":
                DoSyringe(player, enemys);
                break;
            default:
                Debug.LogWarning($"Unknown card name: {CardName}");
                break;
        }
    }

    static void DoAdrenalMedkit(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.AddHealth(0.15f);
    }

    static void DoAntidotePotion(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.IncreasesCriticalDamage(0.2f);
    }

    static void DoBandage(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.AddHealth(0.50f);
    }

    static void DoReflexTonic(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.AddAttack(10);
    }

    static void DoBerserkerWrap(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.AddAttack(0.2f);
    }

    static void DoBoostedBuzz(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.AddAttack(0.25f);
        player.ReduceDefense(0.5f);
        player.ReduceHealth(0.5f);

    }

    static void DoCombatPatch(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.AddDefense(0.25f);
        player.AddHealth(0.25f);
    }

    static void DoStaminaCapsule(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.IncreasesCriticalDamageChance(0.20f);
    }

    static void DoEnergyPotion(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.AddHealth(0.50f);
        player.AddAttack(0.50f);
        player.ReduceDefense(0.50f);
    }

    static void DoFieldSurgeryKit(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.AddHealth(1.00f);
    }

    static void DoHealthPotion(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.AddHealth(100);
    }

    static void DoLiquidCourageKit(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.AddAttack(5);
        player.AddAttack(0.05f);
    }

    static void DoMedkit(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.AddHealth(0.1f);
        player.AddHealth(10);
    }

    static void DoPhoenixShot(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        foreach (var enemy in enemys)
        {
            enemy.Health -= 10;
        }
    }

    static void DoEmergencyCapsule(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.AddAttack(5);
        player.ReduceHealth(5);
    }

    static void DoFuryCatalyst(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.AddAttack(20);
        player.ReduceHealth(0.2f);
    }

    static void DoRapidRecoveryInjector(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.AddHealth(0.5f);
        player.ReduceDefense(0.2f);

    }

    static void DoRevivalSerum(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        enemys[0].Health -= 100;
    }

    static void DoStimulantWrap(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.ReduceHealth(0.2f);
        player.AddAttack(0.2f);
    }

    static void DoSyringe(BattlePlayerValue player, List<EnemyValue> enemys)
    {
        player.ReduceHealth(0.2f);
        player.AddDefense(0.2f);
    }
}