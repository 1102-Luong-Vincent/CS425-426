using UnityEngine;
using UnityEngine.UI;

public class TutorialInteract : MonoBehaviour
{
    public GameObject tutorialMessage;
    private static bool isMessage = false; //don't show as default

    public void TriggerEnter(Collider other)
    {
        if(!isMessage && other.CompareTag("Player"))
        {
            tutorialMessage.SetActive(true);
            isMessage = true;

            Debug.Log("Player entered trigger: tutorial message shown");
        }
    }

    public void Update()
    {
        if (tutorialMessage.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            tutorialMessage.SetActive(false);
            Debug.Log("E pressed: tutorial message hidden");
        }
    }
}
