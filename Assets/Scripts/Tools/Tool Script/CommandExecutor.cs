using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class FunctionName
{
    public const string LoadSceneByEnum = "LoadSceneByEnum";
    public const string SetStory = "SetStory";
    public const string Wait = "Wait";
}

public static class CommandExecutor
{
    public static void Execute(MonoBehaviour runner, string effect)
    {
        if (string.IsNullOrEmpty(effect)) return;

        Debug.Log($"[Effect] {effect}");
        if (TryParseEffect(effect, out string functionName, out string[] args))
        {
            object[] parsedArgs = ParseArgs(args);
            ExecuteEffect(runner, functionName, parsedArgs);
        }
    }

    public static string UnwrapWaitRecursive(string effect)
    {
        if (string.IsNullOrEmpty(effect)) return effect;

        if (TryParseEffect(effect, out string functionName, out string[] args))
        {
            if (functionName == FunctionName.Wait && args.Length >= 2)
            {
                return UnwrapWaitRecursive(args[1].Trim());
            }
        }
        return effect;
    }

    // ? ?? public?? StoryManage ??
    public static bool TryParseEffect(string effect, out string functionName, out string[] args)
    {
        functionName = null;
        args = null;
        if (string.IsNullOrEmpty(effect)) return false;

        int parenIndex = effect.IndexOf('(');
        int lastParenIndex = effect.LastIndexOf(')');
        if (parenIndex < 0 || lastParenIndex < 0 || lastParenIndex <= parenIndex) return false;

        functionName = effect.Substring(0, parenIndex).Trim();
        string argsContent = effect.Substring(parenIndex + 1, lastParenIndex - parenIndex - 1).Trim();
        args = SplitArgs(argsContent);

        for (int i = 0; i < args.Length; i++)
            args[i] = args[i].Trim().Trim('"');

        return true;
    }

    private static string[] SplitArgs(string content)
    {
        int depth = 0;
        int lastSplit = 0;
        List<string> result = new List<string>();

        for (int i = 0; i < content.Length; i++)
        {
            char c = content[i];
            if (c == '(') depth++;
            else if (c == ')') depth--;
            else if (c == ',' && depth == 0)
            {
                result.Add(content.Substring(lastSplit, i - lastSplit));
                lastSplit = i + 1;
            }
        }

        result.Add(content.Substring(lastSplit));
        return result.ToArray();
    }

    private static object[] ParseArgs(string[] args)
    {
        object[] parsed = new object[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];
            if (bool.TryParse(arg, out bool b)) parsed[i] = b;
            else if (int.TryParse(arg, out int n)) parsed[i] = n;
            else
            {
                string tmp = arg.EndsWith("f") ? arg[..^1] : arg;
                if (float.TryParse(tmp, out float f)) parsed[i] = f;
                else parsed[i] = arg;
            }
        }
        return parsed;
    }

    private static void ExecuteEffect(MonoBehaviour runner, string functionName, object[] args)
    {
        switch (functionName)
        {
            case FunctionName.LoadSceneByEnum:
                ExecuteLoadSceneByEnum(args);
                break;
            case FunctionName.Wait:
                ExecuteWait(runner, args);
                break;
            case FunctionName.SetStory:
                break;
            default:
                Debug.LogWarning($"Unknown effect: {functionName}");
                break;
        }
    }

    private static void ExecuteLoadSceneByEnum(object[] args)
    {
        if (args.Length < 1) return;
        string sceneName = args[0].ToString();

        if (Enum.TryParse(typeof(SceneType), sceneName, true, out object sceneObj))
        {
            GameValue.Instance.LoadSceneByEnum((SceneType)sceneObj);
        }
        else
        {
            Debug.LogWarning($"Failed to parse SceneType: {sceneName}");
        }
    }

    private static void ExecuteWait(MonoBehaviour runner, object[] args)
    {
        if (args.Length < 2)
        {
            Debug.LogWarning("Wait requires 2 arguments: delay and nested effect");
            return;
        }

        float delay = Convert.ToSingle(args[0]);
        string nestedEffect = args[1].ToString();
        runner.StartCoroutine(WaitCoroutine(runner, delay, nestedEffect));
    }

    private static IEnumerator WaitCoroutine(MonoBehaviour runner, float delay, string nestedEffect)
    {
        yield return new WaitForSeconds(delay);
        Execute(runner, nestedEffect);
    }

}
