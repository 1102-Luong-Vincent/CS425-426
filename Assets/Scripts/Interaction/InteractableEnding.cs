using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SmallScaleInc.TopDownPixelCharactersPack1;
using System.Collections;

public class InteractableEnding : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI endingText;
    [SerializeField] public GameObject endingPanel;
    [SerializeField] public Button continueButton;

    public bool playerInRange = false;
    public bool isTriggered = false;
    public bool isTyping = false;

    public float timeBetweenLines = 1.0f;
    public float typingSpeed = 0.05f;

    private PlayerController player;

    private void Start()
    {
        endingPanel.SetActive(false);
        continueButton.onClick.AddListener(Continue);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void Continue()
    {
        Time.timeScale = 1.0f;
        endingPanel.SetActive(false);
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Update()
    {
        if(playerInRange && Input.GetKeyDown(KeyCode.E) && !isTriggered)
        {
            Debug.Log("Interact pressed and player is in range");
            Ending();
        }
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Ending()
    {
        isTriggered = true;
        endingPanel.SetActive(true);
        player.enabled = false;
        continueButton.gameObject.SetActive(false);
        Time.timeScale = 0f;

        StartCoroutine(PlayEnding());
    }

    private IEnumerator LineSpeed(string line)
    {
        endingText.text = "";
        isTyping = true;

        foreach(char letter in line)
        {
            endingText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
        continueButton.interactable = false;
    }

    public IEnumerator PlayEnding()
    {
        continueButton.interactable = false;

        yield return StartCoroutine(LineSpeed("I found the cure. The world is broken... but hope remains. Now, it’s up to me."));
        yield return new WaitForSecondsRealtime(2.0f);

        yield return StartCoroutine(LineSpeed("I must deliver this cure and rebuild what we’ve lost. The fight isn’t over yet."));
        yield return new WaitForSecondsRealtime(2.0f);

        yield return StartCoroutine(LineSpeed("Peter takes a deep breath, clutching the vial tightly… humanity’s fate rests in his hands."));
        yield return new WaitForSecondsRealtime(2.0f);

        yield return StartCoroutine(LineSpeed("..."));
        yield return new WaitForSecondsRealtime(2.0f);

        yield return StartCoroutine(LineSpeed("The journey will continue, but the city outside remains silent, waiting... for now."));

        continueButton.gameObject.SetActive(true);
        continueButton.interactable = true;
    }
}