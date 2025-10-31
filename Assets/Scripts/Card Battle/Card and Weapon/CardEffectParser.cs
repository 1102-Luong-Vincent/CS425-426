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
}

public static class FuncParameter
{
    public const string percent = "percent";
    public const string turns = "turns";
    public const string amount = "amount";

}

public static class CardEffectParser
{
    public static List<Action<BattlePlayerValue>> ParseEffectString(string effectString)
    {
        var actions = new List<Action<BattlePlayerValue>>();
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



    private static Action<BattlePlayerValue> GetEffectFunction(string funcName, Dictionary<string, string> args)
    {

        switch (funcName)
        {
            case FuncName.Heal:
                float healPercent = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0f;
                return player => Heal(player, healPercent);

            case FuncName.RestoreEnergy:
                int RestoreEnergyAmount = args.ContainsKey(FuncParameter.amount) ? ParseInt(args[FuncParameter.amount]) : 0;
                return player => RestoreEnergy(player, RestoreEnergyAmount);


            case FuncName.StopBleeding:
                return player => StopBleeding(player);

            case FuncName.IncreaseAttack:
                float atkPercent = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0f;
                int atkTurns = args.ContainsKey(FuncParameter.turns) ? int.Parse(args[FuncParameter.turns]) : 1;
                return player => IncreaseAttack(player, atkPercent, atkTurns);

            case FuncName.IncreaseDefense:
                float defPercent = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0f;
                int defTurns = args.ContainsKey(FuncParameter.turns) ? int.Parse(args[FuncParameter.turns]) : 1;
                return player => IncreaseDefense(player, defPercent, defTurns);

            case FuncName.IncreaseCritDamage:
                float cdPercent = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0f;
                int cdTurns = args.ContainsKey(FuncParameter.turns) ? int.Parse(args[FuncParameter.turns]) : 1;
                return player => IncreaseCritDamage(player, cdPercent, cdTurns);

            case FuncName.IncreaseCritChance:
                float ccPercent = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0f;
                int ccTurns = args.ContainsKey(FuncParameter.turns) ? int.Parse(args[FuncParameter.turns]) : 1;
                return player => IncreaseCritChance(player, ccPercent, ccTurns);

            case FuncName.Revive:
                float revivePercent = args.ContainsKey(FuncParameter.percent) ? ParsePercent(args[FuncParameter.percent]) : 0.2f;
                return player => Revive(player, revivePercent);

            case FuncName.CurePoison:
                return player => CurePoison(player);


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
}
