using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using static ExcelReader;
using static CommandExecutor;
using UnityEngine.SceneManagement;

public static class StoryName
{
    public const string Prologue = "Prologue";
}

public class StoryManage : MonoBehaviour
{
    public static StoryManage Instance;
    public TextMeshProUGUI storyText;
    public float timeBetweenLines = 1.0f; //time before next story line appears 
    public float typingSpeed = 0.05f; // Speed of typing effect 

    private List<ExcelStoryData> storyLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool isSkipTypingRequested = false;
    private bool isGlobalSkipMode = false;

    public Button SkipButton;

    // Store reference to the current story coroutine
    private Coroutine currentStoryCoroutine;

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
            isSkipTypingRequested = true;
        }
    }

    void OnSkipButtonClick()
    {
        StopAllCoroutines();
        isGlobalSkipMode = true;
        if (currentStoryCoroutine != null)
        {
            StopCoroutine(currentStoryCoroutine);
            currentStoryCoroutine = null;
        }

        DisplayAllEffectsAndLastLine();
    }

    private void DisplayAllEffectsAndLastLine()
    {
        if (storyLines == null || storyLines.Count == 0) return;
        storyText.text = storyLines[storyLines.Count - 1].Content;

        // Loop through all remaining story lines and execute their effects

        for (int i = currentLineIndex; i < storyLines.Count; i++)
            {
                string effect = storyLines[i].Effect;
                if (!string.IsNullOrEmpty(effect))
                {
                    // Remove all nested Wait(...) before execution
                    string unwrapped = UnwrapWaitRecursive(effect);
                    if (!string.IsNullOrEmpty(unwrapped))
                    {
                        ExecuteEffect(unwrapped); // Trigger the final unwrapped effect
                    }
                }
            }

        currentLineIndex = storyLines.Count;
    }

    public void SetStory(string fileName)
    {
        StopAllCoroutines();
        currentStoryCoroutine = null;

        isTyping = false;
        isSkipTypingRequested = false;
        currentLineIndex = 0;
        storyText.text = string.Empty;

        storyLines = GetStoryData(fileName);

        if (storyLines != null && storyLines.Count > 0)
        {
            Debug.Log($"Story loaded! Lines: {storyLines.Count}");

            if (isGlobalSkipMode)
            {
                DisplayAllEffectsAndLastLine();
                return;
            }

            currentStoryCoroutine = StartCoroutine(DisplayStoryDialogue());
        }
        else
        {
            Debug.LogError($"Failed to load story data! fileName: {fileName}");
        }
    }

    System.Collections.IEnumerator DisplayStoryDialogue()
    {
        while (currentLineIndex < storyLines.Count)
        {
            yield return StartCoroutine(LineSpeed(storyLines[currentLineIndex]));
            currentLineIndex++;
        }

        // Clear coroutine reference when finished
        currentStoryCoroutine = null;
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

    // Clean up coroutines when object is destroyed
    private void OnDestroy()
    {
        StopAllCoroutines();
        currentStoryCoroutine = null;
    }
}