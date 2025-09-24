using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class StoryScript : MonoBehaviour
{
    public TextMeshProUGUI storyText;
    public string[] storyLines;
    public float timeBetweenLines = 1.0f; //time before next story line appears
    public float typingSpeed = 0.05f; // Speed of typing effect

    private int currentLineIndex = 0;

    void Start()
    {
        TextAsset storyFile = Resources.Load<TextAsset>("Story"); // Load the text file from Resources/Story folder

        if(storyFile != null)
        {
            Debug.Log("Story file loaded! Lines: " + storyFile.text.Split('\n').Length);
            storyLines = storyFile.text.Split('\n');
            StartCoroutine(DisplayStoryDialogue());
        }
        else
        {
            Debug.LogError("Failed to load story file!");
        }
    }

    System.Collections.IEnumerator DisplayStoryDialogue()
    {
        while(currentLineIndex < storyLines.Length) 
        {
            yield return StartCoroutine(LineSpeed(storyLines[currentLineIndex])); // Type out the current line
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
}