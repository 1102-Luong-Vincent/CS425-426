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
    public TextMeshProUGUI storyText;
    public float timeBetweenLines = 1.0f; //time before next story line appears 
    public float typingSpeed = 0.05f; // Speed of typing effect 

    private List<ExcelStoryData> storyLines;  
    private int currentLineIndex = 0;

    public Button SkipButton; 

    void Start()
    {
        SkipButton.onClick.AddListener(OnSkipButtonClick);
        SetStory(GameValue.Instance.GetHappendStoryName());
        GameValue.Instance.SetHappendStoryName(string.Empty);
    }

    void OnSkipButtonClick()
    {
        // Stop any ongoing coroutines (e.g., typing or display coroutine)
        StopAllCoroutines();

        // Execute all remaining effects and instantly show the last story line
        DisplayAllEffectsAndLastLine();
    }

    /// <summary>
    /// Executes all remaining effects from the current line to the last line,
    /// and immediately displays the content of the last story line.
    /// </summary>
    private void DisplayAllEffectsAndLastLine()
    {
        if (storyLines == null || storyLines.Count == 0) return;

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

        // Immediately display the content of the last story line
        storyText.text = storyLines[storyLines.Count - 1].Content;

        // Update the current line index to the end to prevent further display
        currentLineIndex = storyLines.Count;

    }

    public void SetStory(string fileName)
    {
        storyLines = GetStoryData(fileName);
        if (storyLines != null && storyLines.Count > 0)
        {
            Debug.Log($"Story loaded! Lines: {storyLines.Count}");
            StartCoroutine(DisplayStoryDialogue());
        }
        else
        {
            Debug.LogError($"Failed to load story data! {storyLines != null} && storyLines.Count{storyLines.Count}");
        }

    }

    System.Collections.IEnumerator DisplayStoryDialogue()
    {
        while (currentLineIndex < storyLines.Count)
        {
            string content = storyLines[currentLineIndex].Content;
            string effect = storyLines[currentLineIndex].Effect;

            yield return StartCoroutine(LineSpeed(content)); // Type out the current line 
            ExecuteEffect(effect); 

            yield return new WaitForSeconds(timeBetweenLines); // Wait for the specified time 
            currentLineIndex++;

        }
    }

    System.Collections.IEnumerator LineSpeed(string speed)
    {
        storyText.text = ""; // Clear existing text
        foreach (char letter in speed.ToCharArray())
        {
            storyText.text += letter; // Add one letter at a time
            yield return new WaitForSeconds(typingSpeed); // Wait for the typing speed duration
        }
    }

    void ExecuteEffect(string effect)
    {
         CommandExecutor.Execute(this,effect); 
    }


}