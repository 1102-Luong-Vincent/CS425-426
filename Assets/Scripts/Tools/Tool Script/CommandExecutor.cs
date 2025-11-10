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
    // Entry point for executing effect strings
    public static void Execute(MonoBehaviour runner, string effect)
    {
        Debug.Log(effect);
        if (TryParseEffect(effect, out string functionName, out string[] args))
        {
            object[] parsedArgs = ParseArgs(args); // Auto type detection
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
                // Recursively unwrap the nested effect (2nd argument)
                return UnwrapWaitRecursive(args[1].Trim());
            }
        }

        // Not a Wait function -> return as is
        return effect;
    }


    // Parse function name and argument strings
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
        {
            args[i] = args[i].Trim().Trim('"');
        }

        return true;
    }

    // Split arguments at top-level commas (handles nested parentheses)
    private static string[] SplitArgs(string content)
    {
        int parenDepth = 0;
        int lastSplit = 0;
        List<string> result = new List<string>();

        for (int i = 0; i < content.Length; i++)
        {
            char c = content[i];
            if (c == '(') parenDepth++;
            else if (c == ')') parenDepth--;
            else if (c == ',' && parenDepth == 0)
            {
                result.Add(content.Substring(lastSplit, i - lastSplit));
                lastSplit = i + 1;
            }
        }
        result.Add(content.Substring(lastSplit));
        return result.ToArray();
    }

    // Auto type parsing: int, float, bool, string
    // Auto type parsing: int, float, bool, string
    private static object[] ParseArgs(string[] args)
    {
        object[] parsed = new object[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];

            // Boolean
            if (bool.TryParse(arg, out bool b)) parsed[i] = b;
            // Integer
            else if (int.TryParse(arg, out int n)) parsed[i] = n;
            // Float (support "1f" format)
            else
            {
                string tmp = arg.EndsWith("f") ? arg.Substring(0, arg.Length - 1) : arg;
                if (float.TryParse(tmp, out float f)) parsed[i] = f;
                else parsed[i] = arg; // fallback string
            }
        }
        return parsed;
    }

    private static void ExecuteEffect(MonoBehaviour runner, string functionName, object[] args)
    {
        if (string.IsNullOrEmpty(functionName)) return;

        switch (functionName)
        {
            case FunctionName.LoadSceneByEnum:ExecuteLoadSceneByEnum(args);break;
            case FunctionName.Wait:ExecuteWait(runner, args);break;
            case FunctionName.SetStory:SetStory(runner, args);break;
            default:Debug.LogWarning($"Unknown effect: {functionName}");break;
        }
    }

    private static void ExecuteLoadSceneByEnum(object[] args)
    {

        if (args.Length < 1) return;
        string sceneName = args[0].ToString();

        if (Enum.TryParse(typeof(SceneType), sceneName, true, out object sceneObj))
        {
            SceneType scene = (SceneType)sceneObj;
            GameValue.Instance.LoadSceneByEnum(scene);
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

        if (args[0] is float delayFloat)
        {
            string nestedEffect = args[1].ToString();
            runner.StartCoroutine(WaitCoroutine(runner, delayFloat, nestedEffect));
        }
        else if (args[0] is int delayInt)
        {
            string nestedEffect = args[1].ToString();
            runner.StartCoroutine(WaitCoroutine(runner, delayInt, nestedEffect));
        }
        else
        {
            Debug.LogWarning($"Failed to parse delay time: {args[0]}");
        }
    }

    private static IEnumerator WaitCoroutine(MonoBehaviour runner, float delay, string nestedEffect)
    {
        yield return new WaitForSeconds(delay);
        Execute(runner, nestedEffect);
    }


    private static void SetStory(MonoBehaviour runner, object[] args)
    {
        if (args == null || args.Length < 1)
        {
            Debug.LogWarning("ReadStory requires at least one argument: the story file name.");
            return;
        }

        string fileName = args[0]?.ToString();
        if (string.IsNullOrWhiteSpace(fileName))
        {
            Debug.LogWarning("Invalid story file name provided to ReadStory.");
            return;
        }

        if (StoryManage.Instance == null)
        {
            Debug.LogError("StoryManage.Instance is null. Cannot set story.");
            return;
        }

        StoryManage.Instance.SetStory(fileName);
        Debug.Log($"Story loaded: {fileName}");
    }


}
