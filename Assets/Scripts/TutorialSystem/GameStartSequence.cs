using SmallScaleInc.TopDownPixelCharactersPack1;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameStartSequence : MonoBehaviour
{
    GameStartSequence Instance { get; set; }

    //outside objects necessary for sequence
    [SerializeField] private FadeTransition fader;
    [SerializeField] private GameObject DoorTrigger;
    [SerializeField] private PlayerController playercontroller;
    [SerializeField] private GameObject bedroomDoorOpened;
    [SerializeField] private GameObject bedroomDoorClosed;
    [SerializeField] private Button Button1;
    [SerializeField] private Button Button2;
    [SerializeField] private Button Button3;
    [SerializeField] private Button SortButton1;
    [SerializeField] private Button SortButton2;
    [SerializeField] private Button DeckButton;
    [SerializeField] private Button CombineButton;
    [SerializeField] private Button UpgradeButton;
    [SerializeField] private Button OptionsButton;

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
        PlayerMenuManager.Instance.menuToggleEnabled = false;
        Button1.interactable = false;
        Button2.interactable = false;
        Button3.interactable = false;
        SortButton1.interactable = false;
        SortButton2.interactable = false;
        DeckButton.interactable = false;
        CombineButton.interactable = false;
        UpgradeButton.interactable = false;
        OptionsButton.interactable = false;

        // begin sequence
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(fader.FadeIn());
        yield return new WaitForSeconds(1f);

        //start movement tutorial
        yield return StartCoroutine(moveTutoral());

        //start interact tutorial
        yield return StartCoroutine(interactTutorial());

        //start inventory tutorial
        yield return StartCoroutine(inventoryTutorial());

        //start weapon tutorial
        yield return StartCoroutine(WeaponTutorial());
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
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator interactTutorial()
    {
        // interact tutorial
        StartCoroutine(FadePanelIn("InteractTutorialPanel"));
        yield return StartCoroutine(FadePanelIn("InteractTooltipPanel"));
        yield return new WaitUntil(() => GameValue.Instance.GetPlayerValue().GetCardCount() >= 2);
        StartCoroutine(FadePanelOut("InteractTutorialPanel"));
        yield return StartCoroutine(FadePanelOut("InteractTooltipPanel"));
        yield return new WaitForSeconds(1f);
    }
    public IEnumerator inventoryTutorial()
    {
        // inventory tutorial
        yield return StartCoroutine(FadePanelIn("InventoryTutorialPanel"));
        PlayerMenuManager.Instance.menuToggleEnabled = true;
        yield return new WaitUntil(() => PlayerMenuManager.Instance.IsMenuOpen() == true);
        Time.timeScale = 1f;
        playercontroller.enabled = false;
        bedroomDoorClosed.SetActive(false);
        bedroomDoorOpened.SetActive(true);
        PlayerMenuManager.Instance.menuToggleEnabled = false;


        yield return StartCoroutine(FadePanelOut("InventoryTutorialPanel"));
        StartCoroutine(FadePanelIn("InventoryTooltipPanel"));
        StartCoroutine(FadePanelIn("CardSwapTutorialPanel1"));
        yield return StartCoroutine(FadePanelIn("CardSwapTutorialPanel2"));
        yield return new WaitUntil(() => GameValue.Instance.GetPlayerValue().GetDeckCardCount(0) >= 2);
        StartCoroutine(FadePanelOut("InventoryTooltipPanel"));
        StartCoroutine(FadePanelOut("CardSwapTutorialPanel1"));
        yield return StartCoroutine(FadePanelOut("CardSwapTutorialPanel2"));


        yield return StartCoroutine(FadePanelIn("InventoryTutorialPanel2"));
        PlayerMenuManager.Instance.menuToggleEnabled = true;
        yield return new WaitUntil(() => PlayerMenuManager.Instance.IsMenuOpen() == false);
        yield return StartCoroutine(FadePanelOut("InventoryTutorialPanel2"));


        Button1.interactable = true;
        Button2.interactable = true;
        Button3.interactable = true;
        SortButton1.interactable = true;
        SortButton2.interactable = true;
        DeckButton.interactable = true;
        playercontroller.enabled = true;
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator WeaponTutorial()
    {
        yield return StartCoroutine(FadePanelIn("WeaponTooltipPanel"));
        yield return new WaitUntil(() => GameValue.Instance.GetPlayerValue().HadWeaponsLibrary.Count > 0);
        yield return StartCoroutine(FadePanelOut("WeaponTooltipPanel"));

        yield return StartCoroutine(FadePanelIn("InventoryTutorialPanel3"));
        yield return new WaitUntil(() => PlayerMenuManager.Instance.IsMenuOpen() == true);
        playercontroller.enabled = false;
        Time.timeScale = 1f;
        PlayerMenuManager.Instance.menuToggleEnabled = false;
        yield return StartCoroutine(FadePanelOut("InventoryTutorialPanel3"));

        yield return StartCoroutine(FadePanelIn("WeaponTutorialPanel"));
        yield return new WaitUntil(() => GameValue.Instance.GetPlayerValue().EquipmentWeapon != null);
        yield return StartCoroutine(FadePanelOut("WeaponTutorialPanel"));

        yield return StartCoroutine(FadePanelIn("InventoryTutorialPanel4"));
        PlayerMenuManager.Instance.menuToggleEnabled = true;
        yield return new WaitUntil(() => PlayerMenuManager.Instance.IsMenuOpen() == false);
        playercontroller.enabled = true;
        yield return StartCoroutine(FadePanelOut("InventoryTutorialPanel4"));


        yield return null;
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