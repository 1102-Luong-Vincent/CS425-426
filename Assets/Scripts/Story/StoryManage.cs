using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static CommandExecutor;
using static ExcelReader;

public static class StoryName
{
    public const string Prologue = "Prologue";
}

public class StoryCache
{
    public string FileName;
    public List<ExcelStoryData> Lines = new List<ExcelStoryData>();
    public List<StoryCache> SubStories = new List<StoryCache>();
}

public class StoryManage : MonoBehaviour
{
    public static StoryManage Instance;
    public TextMeshProUGUI storyText;
    public float timeBetweenLines = 1.0f;
    public float typingSpeed = 0.05f;

    private List<ExcelStoryData> storyLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool isSkipTypingRequested = false;
    private bool isGlobalSkipMode = false;

    public Button SkipButton;

    private readonly List<Coroutine> storyCoroutines = new List<Coroutine>();
    private Dictionary<string, StoryCache> storyCacheDict = new Dictionary<string, StoryCache>();

    private ExcelStoryData finalStoryLine;
    private string finalStoryFileName;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        SkipButton.onClick.AddListener(OnSkipButtonClick);
        SetStory(GameValue.Instance.GetHappendStoryName());
        GameValue.Instance.SetHappendStoryName(string.Empty);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isTyping)
        {
            if (IsPointerOverStoryUI())
            {
                isSkipTypingRequested = true;
            }
        }
    }

    private bool IsPointerOverStoryUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        if (results.Count > 0)
        {
            var first = results[0].gameObject;
            return first.layer == LayerMask.NameToLayer("Story");
        }

        return false;
    }

    void OnSkipButtonClick()
    {
        isGlobalSkipMode = true;
        StopStoryCoroutines();
        StartCoroutine(SkipAfterRender());
    }

    private IEnumerator SkipAfterRender()
    {
        storyText.text = finalStoryLine.Content;
        storyText.ForceMeshUpdate();
        Canvas.ForceUpdateCanvases();
        yield return null;
        DisplayAllEffects();
    }


    private void DisplayAllEffects()
    {
        if (storyLines == null || storyLines.Count == 0) return;
        for (int i = currentLineIndex; i < storyLines.Count; i++)
        {
            string effect = storyLines[i].Effect;
            if (!string.IsNullOrEmpty(effect))
            {
                string unwrapped = UnwrapWaitRecursive(effect);
                if (!string.IsNullOrEmpty(unwrapped)) ExecuteEffect(unwrapped);
            }
        }
        currentLineIndex = storyLines.Count;
    }




    void SetStory(string fileName)
    {
        StopStoryCoroutines();

        isTyping = false;
        isSkipTypingRequested = false;
        currentLineIndex = 0;
        storyText.text = string.Empty;

        if (!storyCacheDict.TryGetValue(fileName, out StoryCache cache))
        {
            cache = PreloadStoryRecursive(fileName, new HashSet<string>());
            storyCacheDict[fileName] = cache;
            Debug.Log($"? Preloaded story {fileName} with {cache.Lines.Count} lines (including nested)");
        }

        storyLines = cache.Lines;

        StoryCache deepest = GetDeepestSubStory(cache);
        finalStoryLine = deepest.Lines.LastOrDefault();
        finalStoryFileName = deepest.FileName;
        Debug.Log($"?? Cached final story line from {finalStoryFileName}: {finalStoryLine.Content}");

        if (storyLines != null && storyLines.Count > 0)
        {
            if (isGlobalSkipMode)
            {
                DisplayAllEffects();
                return;
            }

            StartCoroutine(DisplayStoryDialogue());
        }
        else
        {
            Debug.LogError($"? Failed to load story data! fileName: {fileName}");
        }
    }

    private StoryCache GetDeepestSubStory(StoryCache cache)
    {
        if (cache.SubStories == null || cache.SubStories.Count == 0)
            return cache;

        return GetDeepestSubStory(cache.SubStories.Last());
    }




    private StoryCache PreloadStoryRecursive(string fileName, HashSet<string> visited)
    {
        if (visited.Contains(fileName))
        {
            Debug.LogWarning($"?? Loop detected in story: {fileName}");
            return new StoryCache { FileName = fileName };
        }
        visited.Add(fileName);

        List<ExcelStoryData> lines = GetStoryData(fileName);
        if (lines == null || lines.Count == 0)
        {
            Debug.LogWarning($"?? Story file empty: {fileName}");
            return new StoryCache { FileName = fileName };
        }

        StoryCache cache = new StoryCache { FileName = fileName };

        foreach (var line in lines)
        {
            cache.Lines.Add(line);

            if (!string.IsNullOrEmpty(line.Effect)
                && CommandExecutor.TryParseEffect(line.Effect, out string func, out string[] args)
                && func == FunctionName.SetStory
                && args.Length > 0)
            {
                string subFile = args[0];
                if (!string.IsNullOrEmpty(subFile))
                {
                    Debug.Log($"?? Preloading nested story: {subFile} (from {fileName})");
                    StoryCache subCache = PreloadStoryRecursive(subFile, visited);
                    cache.SubStories.Add(subCache);
                    cache.Lines.AddRange(subCache.Lines);
                }
            }
        }

        return cache;
    }

    private IEnumerator DisplayStoryDialogue()
    {
        while (currentLineIndex < storyLines.Count)
        {
            yield return StartStoryCoroutine(LineSpeed(storyLines[currentLineIndex]));
            currentLineIndex++;
            yield return new WaitForSeconds(timeBetweenLines);
        }
    }

    private IEnumerator LineSpeed(ExcelStoryData ExcelStoryline)
    {
        storyText.text = "";
        isTyping = true;
        isSkipTypingRequested = false;

        foreach (char letter in ExcelStoryline.Content.ToCharArray())
        {
            storyText.text += letter;
            yield return new WaitForSeconds(typingSpeed);

            if (isSkipTypingRequested)
            {
                storyText.text = ExcelStoryline.Content;
                break;
            }
        }

        isTyping = false;
        ExecuteEffect(ExcelStoryline.Effect);
    }

    void ExecuteEffect(string effect)
    {
        CommandExecutor.Execute(this, effect);
    }

    private Coroutine StartStoryCoroutine(IEnumerator routine)
    {
        Coroutine c = StartCoroutine(routine);
        storyCoroutines.Add(c);
        return c;
    }

    private void StopStoryCoroutines()
    {
        foreach (var c in storyCoroutines)
        {
            if (c != null) StopCoroutine(c);
        }
        storyCoroutines.Clear();
    }

    private void OnDestroy()
    {
        StopStoryCoroutines();
    }
}
