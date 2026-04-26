using SmallScaleInc.TopDownPixelCharactersPack1;
using System.Collections;
using UnityEngine;

public class GameStartSequence : MonoBehaviour
{
    GameStartSequence Instance { get; set; }

    //outside objects necessary for sequence
    [SerializeField] private FadeTransition fader;
    [SerializeField] private GameObject DoorTrigger;
    [SerializeField] private PlayerController playercontroller;


    //sequence flags
    private bool playerMoved = false;
    private bool playerSprinted = false;
    private bool gotWeapon = false;
    private bool gotItem = false;
    private bool openedMenu = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        StartCoroutine(PlayGameStartSequence());
    }

    private void Update()
    {

    }

    public IEnumerator PlayGameStartSequence()
    {
        // initilalize the room
        DoorTrigger.SetActive(false);
        playercontroller.enabled = false;


        // begin sequence
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(fader.FadeIn());
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(moveTutoral());
        yield return null;
        Destroy(this);
    }

    public IEnumerator moveTutoral()
    {
        // walking tutorial
        yield return StartCoroutine(FadePanelIn("MoveTutorialPanel"));
        playercontroller.enabled = true;
        yield return new WaitUntil(() => playercontroller.isMoving == true);
        yield return StartCoroutine(FadePanelOut("MoveTutorialPanel"));
        yield return new WaitForSeconds(1f);

        // sprinting tutorial
        yield return StartCoroutine(FadePanelIn("SprintTutorialPanel"));
        yield return new WaitUntil(() => playercontroller.isRunning == true);
        yield return StartCoroutine(FadePanelOut("SprintTutorialPanel"));
    }

    public IEnumerator FadePanelIn(string PanelName)
    {
        TutorialPanel panel = GameObject.Find(PanelName).GetComponent<TutorialPanel>();
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fader.fadeSpeed;
            panel.SetColor(new Color(1f, 1f, 1f, alpha));
            yield return null;
        }
        yield return null;
    }

    public IEnumerator FadePanelOut(string PanelName)
    {
        TutorialPanel panel = GameObject.Find(PanelName).GetComponent<TutorialPanel>();
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fader.fadeSpeed;
            panel.SetColor(new Color(1f, 1f, 1f, alpha));
            yield return null;
        }
        Destroy(panel.gameObject);
        yield return null;
    }
}