// Authors: Vincent Luong and Shawn Meng
// Created by: Shawn Meng
// Modified by: Vincent Luong
// Some code generated with assistance from ChatGPT.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class FuncName
{
    public const string Heal = "Heal";
    public const string RestoreEnergy = "RestoreEnergy";
    public const string StopBleeding = "StopBleeding";
    public const string IncreaseAttack = "IncreaseAttack";
    public const string IncreaseDefense = "IncreaseDefense";
    public const string IncreaseCritDamage = "IncreaseCritDamage";
    public const string IncreaseCritChance = "IncreaseCritChance";
    public const string Revive = "Revive";
    public const string CurePoison = "CurePoison";
    public const string LowerDefense = "LowerDefense";
    public const string DecreaseDefense = "DecreaseDefense";
    public const string LowerHealth = "LowerHealth";

    //AOE attacks
    public const string DamageAll = "DamageAll";
    public const string ApplyBurn = "ApplyBurn";
    public const string ApplyPoison = "ApplyPoison";
    public const string ApplyStun = "ApplyStun";
    public const string ApplyConfusion = "ApplyConfusion";
    public const string ReduceArmor = "ReduceArmor";
    public const string AttachC4Bomb = "AttachC4Bomb";
    public const string DeployMine = "DeployMine";

}

public static class FuncParameter
{
    public const string percent = "percent";
    public const string turns = "turns";
    public const string amount = "amount";

}

public static class CardEffectParser
{
    public static List<Action<BattlePlayerValue,List<EnemyValue>>> ParseEffectString(string effectString)
    {
        var actions = new List<Action<BattlePlayerValue, List<EnemyValue>>>();
        if (string.IsNullOrEmpty(effectString)) return null;
        var commands = effectString.Split(';');

        foreach (var cmdRaw in commands)
        {
            var cmd = cmdRaw.Trim();
            if (string.IsNullOrEmpty(cmd)) continue;

            Match match = Regex.Match(cmd, @"(\w+)\s*\((.*)\)");
            string funcName = match.Success ? match.Groups[1].Value : cmd.Replace("()", "");
            var args = new Dictionary<string, string>();

            if (match.Success && !string.IsNullOrEmpty(match.Groups[2].Value))
            {
                foreach (var pair in match.Groups[2].Value.Split(','))
                {
                    var kv = pair.Split('=');
                    if (kv.Length == 2) args[kv[0].Trim()] = kv[1].Trim();
                }
            }

            var action = GetEffectFunction(funcName, args);
            if (action != null) actions.Add(action);
        }

        return actions;
    }

    static int ParseInt(string s)
    {
        if (string.IsNullOrEmpty(s)) return 0;
        s = s.Trim();
        if (s.EndsWith("%")) s = s.TrimEnd('%');
        if (int.TryParse(s, out int val)) return val;
        if (float.TryParse(s, out float fval)) return Mathf.RoundToInt(fval);
        return 0;
    }

    static float ParsePercent(string s)
    {
        if (string.IsNullOrEmpty(s)) return 0f;
        s = s.Trim();
        if (s.EndsWith("%")) s = s.TrimEnd('%');
        if (float.TryParse(s, out float val)) return val / 100f;
        return 0f;
    }



    private static Action<BattlePlayerValue,List<EnemyValue>> GetEffectFunction(string funcName, Dictionary<string, string> args)
    {

        switch (funcName)
        {
            case FuncName.Heal:
                float healPercent = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0f;
                return (player, enemies) => Heal(player, healPercent);

            case FuncName.RestoreEnergy:
                int RestoreEnergyAmount = args.ContainsKey(FuncParameter.amount) ? ParseInt(args[FuncParameter.amount]) : 0;
                return (player, enemies) => RestoreEnergy(player, RestoreEnergyAmount);

            case FuncName.StopBleeding:
                return (player, enemies) => StopBleeding(player);

            case FuncName.IncreaseAttack:
                float atkPercent = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0f;
                int atkTurns = args.ContainsKey(FuncParameter.turns) ? int.Parse(args[FuncParameter.turns]) : 1;
                return (player, enemies) => IncreaseAttack(player, atkPercent, atkTurns);

            case FuncName.IncreaseDefense:
                float defPercent = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0f;
                int defTurns = args.ContainsKey(FuncParameter.turns) ? int.Parse(args[FuncParameter.turns]) : 1;
                return (player, enemies) => IncreaseDefense(player, defPercent, defTurns);

            case FuncName.IncreaseCritDamage:
                float cdPercent = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0f;
                int cdTurns = args.ContainsKey(FuncParameter.turns) ? int.Parse(args[FuncParameter.turns]) : 1;
                return (player, enemies) => IncreaseCritDamage(player, cdPercent, cdTurns);

            case FuncName.IncreaseCritChance:
                float ccPercent = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0f;
                int ccTurns = args.ContainsKey(FuncParameter.turns) ? int.Parse(args[FuncParameter.turns]) : 1;
                return (player, enemies) => IncreaseCritChance(player, ccPercent, ccTurns);

            case FuncName.Revive:
                float revivePercent = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0.2f;
                return (player, enemies) => Revive(player, revivePercent);

            case FuncName.CurePoison:
                return (player, enemies) => CurePoison(player);

            case FuncName.DecreaseDefense:
                float lowDefPercent = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0;
                int lowDefTurns = args.ContainsKey(FuncParameter.turns) ? ParseInt(args[FuncParameter.turns]) : 1;
                return (player, enemies) => LowerDefense(player, lowDefPercent, lowDefTurns);

            case FuncName.LowerHealth:
                float lowHPPercent = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0;
                int lowHPTurns = args.ContainsKey(FuncParameter.turns) ? ParseInt(args[FuncParameter.turns]) : 1;
                return (player, enemies) => LowerHealth(player, lowHPPercent, lowHPTurns);

            // AOE Effects
            case FuncName.DamageAll:
                float percent = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0f;
                return (player, enemies) => DamageAllEnemies(percent);

            case FuncName.ApplyBurn:
                int turns = args.ContainsKey(FuncParameter.turns) ? ParseInt(args[FuncParameter.turns]) : 2;
                float burnPercent = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0.05f;
                return (player, enemies) => ApplyBurnToAllEnemies(turns, burnPercent);

            case FuncName.ApplyPoison:
                int PoisonTurns = args.ContainsKey(FuncParameter.turns) ? ParseInt(args[FuncParameter.turns]) : 2;
                float poisonPercent = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0.05f;
                return (player, enemies) => ApplyPoisonToAllEnemies(PoisonTurns, poisonPercent);

            case FuncName.ApplyStun:
                int StunTurns = args.ContainsKey(FuncParameter.turns) ? ParseInt(args[FuncParameter.turns]) : 1;
                float chanceToStun = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0.4f;
                return (player, enemies) => ApplyStunToAllEnemies(chanceToStun, StunTurns);

            case FuncName.ApplyConfusion:
                int ConfusionTurns = args.ContainsKey(FuncParameter.turns) ? ParseInt(args[FuncParameter.turns]) : 1;
                float chanceToConfuse = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0.3f;
                return (player, enemies) => ApplyConfusionToAllEnemies(chanceToConfuse, ConfusionTurns);

            case FuncName.ReduceArmor:
                int Reductionturns = args.ContainsKey(FuncParameter.turns) ? ParseInt(args[FuncParameter.turns]) : 10;
                float percentReduce = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0.5f;
                return (player, enemies) => ReduceArmorOfAllEnemies(percentReduce, Reductionturns);

            case FuncName.AttachC4Bomb:
                int c4Damage = args.ContainsKey(FuncParameter.amount) ? ParseInt(args[FuncParameter.amount]) : 50;
                int delay = args.ContainsKey(FuncParameter.turns) ? ParseInt(args[FuncParameter.turns]) : 2;
                return (player, enemies) => AttachC4BombToEnemy(delay, c4Damage);

            case FuncName.DeployMine:
                int mineDamage = args.ContainsKey(FuncParameter.amount) ? ParseInt(args[FuncParameter.amount]) : 40;
                return (player, enemies) => DeployMineAtEnemy(mineDamage);

            default:
                Debug.LogWarning($"[CardEffectParser] Unknown function: {funcName}");
                return null;
        }
    }

    public static void Heal(BattlePlayerValue player, float percent)
    {
        player.Health += Mathf.RoundToInt(player.MaxHealth * percent);
    }

    public static void RestoreEnergy(BattlePlayerValue player, int amount)
    {
        Debug.LogWarning("Havent do Energy system yet");
    }

    public static void StopBleeding(BattlePlayerValue player)
    {
        player.state.isBleeding = false;
    }

    public static void IncreaseAttack(BattlePlayerValue player, float percent, int turns = 1)
    {
        player.state.AttackBuff += percent;
    }

    public static void IncreaseDefense(BattlePlayerValue player, float percent, int turns = 1)
    {
        player.state.DefenseBuff += percent;
    }

    public static void IncreaseCritDamage(BattlePlayerValue player, float percent, int turns = 1)
    {
        player.state.CriticalDamageBuff += percent;
    }

    public static void IncreaseCritChance(BattlePlayerValue player, float percent, int turns = 1)
    {
        player.state.CriticalChanceBuff += percent;
    }

    public static void Revive(BattlePlayerValue player, float percent)
    {
        if (player.Health <= 0)
            player.Health = Mathf.RoundToInt(player.MaxHealth * percent);
    }

    public static void CurePoison(BattlePlayerValue player)
    {
        player.state.isPoisoned = false;
    }

    public static void LowerDefense(BattlePlayerValue player, float percent, int turns = 1)
    {
        player.state.DefenseBuff -= percent;
    }

    public static void LowerHealth(BattlePlayerValue player, float percent, int turns = 1)
    {
        int dmg = Mathf.RoundToInt(player.MaxHealth * percent);
        player.Health -= dmg;
    }

    //AOE Effects
    public static void DamageAllEnemies(float percent)
    {
        foreach (var enemy in BattleEnemyManager.Instance.currentEnemys)
        {
            if (enemy == null) continue;
            int damage = Mathf.RoundToInt(enemy.EnemyValueReference.MaxHealth * percent);
            enemy.EnemyValueReference.Health -= damage;
        }
    }

    public static void ApplyBurnToAllEnemies(int turns, float percent)
    {
        foreach (var enemy in BattleEnemyManager.Instance.currentEnemys)
        {
            if (enemy == null) continue;
            enemy.EnemyValueReference.ApplyBurn(new EnemyBurnStatus(turns, percent));
        }
    }

    public static void ApplyPoisonToAllEnemies(int turns, float percent)
    {
        foreach (var enemy in BattleEnemyManager.Instance.currentEnemys)
        {
            if (enemy == null) continue;
            enemy.EnemyValueReference.ApplyPoison(new EnemyPoisonStatus(turns, percent));
        }
    }
    public static void ApplyStunToAllEnemies(float chance, int turns)
    {
        foreach (var enemy in BattleEnemyManager.Instance.currentEnemys)
        {
            if (UnityEngine.Random.value <= chance)
            {
                enemy.EnemyValueReference.SetStunned(turns);
            }
        }
    }

    public static void ApplyConfusionToAllEnemies(float chance, int turns)
    {
        foreach (var enemy in BattleEnemyManager.Instance.currentEnemys)
        {
            if (UnityEngine.Random.value <= chance)
            {
                enemy.EnemyValueReference.SetConfused(turns);
            }
        }
    }

    public static void ReduceArmorOfAllEnemies(float percent, int turns)
    {
        foreach (var enemy in BattleEnemyManager.Instance.currentEnemys)
        {
            enemy.EnemyValueReference.tempArmorReduction += percent;
            enemy.EnemyValueReference.armorReductionTurns = turns;
        }
    }

    public static void AttachC4BombToEnemy(int delay, int damage)
    {
        foreach (var enemy in BattleEnemyManager.Instance.currentEnemys)
        {
            if (enemy == null) continue;
            enemy.EnemyValueReference.AttachC4(delay, damage); //explodes after 2 turns (delays) and deals damage
        }
    }

    public static void DeployMineAtEnemy(int damage)
    {
        foreach (var enemy in BattleEnemyManager.Instance.currentEnemys)
        {
            if (enemy == null) continue;
            enemy.EnemyValueReference.DeployMine(damage);
        }
    }
}