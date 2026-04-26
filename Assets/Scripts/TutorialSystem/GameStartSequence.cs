using SmallScaleInc.TopDownPixelCharactersPack1;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameStartSequence : MonoBehaviour
{

    public bool SkipSequence = false;
    GameStartSequence Instance { get; set; }

    //outside objects necessary for sequence
    [SerializeField] private FadeTransition fader;
    [SerializeField] private GameObject DoorTrigger;
    [SerializeField] private PlayerController playercontroller;
    [SerializeField] private GameObject tutorialZombie;
    [SerializeField] private GameObject bedroomDoorOpened;
    [SerializeField] private GameObject bedroomDoorClosed;
    [SerializeField] private GameObject frontDoorOpened;
    [SerializeField] private GameObject frontDoorClosed;
    [SerializeField] private Button Button1;
    [SerializeField] private Button Button2;
    [SerializeField] private Button Button3;
    [SerializeField] private Button SortButton1;
    [SerializeField] private Button SortButton2;
    [SerializeField] private Button DeckButton;
    [SerializeField] private Button CombineButton;
    [SerializeField] private Button UpgradeButton;
    [SerializeField] private Button OptionsButton;
    [SerializeField] private GameObject Camera;

    //sequence flags
    private bool playerMoved = false;
    private bool playerSprinted = false;
    private bool gotWeapon = false;
    private bool gotItem = false;
    private bool openedMenu = false;

    private Vector3 zombieSpawnPoint;
    private EnemyControl zombieController;
    private Vector3 zombieWayPoint;

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
        if (SkipSequence || GameValue.Instance.GetCurrentScence() != SceneType.GameStartScene)
        {
            bedroomDoorClosed.SetActive(false);
            bedroomDoorOpened.SetActive(true);
            fader.SetColor(new Color(0f, 0f, 0f, 0f)); 
            Destroy(this);
            return;
        }
        StartCoroutine(PlayGameStartSequence());
    }

    private void Update()
    {

    }

    public IEnumerator PlayGameStartSequence()
    {
        // initilalize the room
        fader.SetColor(new Color(0f, 0f, 0f, 1f)); //start with a black screen
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
        zombieSpawnPoint = new Vector3(8.14f, -0.45f, 0f);
        zombieWayPoint = new Vector3(6.7f, 0.5f, 0f);
        zombieController = tutorialZombie.GetComponent<EnemyControl>();
        zombieController.overRideControl = true;


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

        //start combat tutorial
        yield return StartCoroutine(CombatTutorial());

        yield return null;
        Destroy(this);
    }

    public IEnumerator moveTutoral()
    {
        // walking tutorial -- prompt user to move with WASD
        yield return StartCoroutine(FadePanelIn("MoveTutorialPanel"));
        playercontroller.enabled = true;

        //wait until player moves
        yield return new WaitUntil(() => playercontroller.isMoving == true);
        yield return StartCoroutine(FadePanelOut("MoveTutorialPanel"));
        yield return new WaitForSeconds(1f);


        // sprinting tutorial -- prompt user to run with SHIFT
        yield return StartCoroutine(FadePanelIn("SprintTutorialPanel"));

        //wait until player sprints
        yield return new WaitUntil(() => playercontroller.isRunning == true);
        yield return StartCoroutine(FadePanelOut("SprintTutorialPanel"));
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator interactTutorial()
    {
        // interact tutorial -- prompt user to interact with items using E
        StartCoroutine(FadePanelIn("InteractTutorialPanel"));
        yield return StartCoroutine(FadePanelIn("InteractTooltipPanel"));

        // wait until player picks up 2 cards
        yield return new WaitUntil(() => GameValue.Instance.GetPlayerValue().GetCardCount() >= 2);
        StartCoroutine(FadePanelOut("InteractTutorialPanel"));
        yield return StartCoroutine(FadePanelOut("InteractTooltipPanel"));
        yield return new WaitForSeconds(1f);
    }
    public IEnumerator inventoryTutorial()
    {
        // inventory tutorial -- prompt user to open inventory with TAB and show them how to manage their cards
        yield return StartCoroutine(FadePanelIn("InventoryTutorialPanel"));
        PlayerMenuManager.Instance.menuToggleEnabled = true;

        //wait for player to open inventory
        yield return new WaitUntil(() => PlayerMenuManager.Instance.IsMenuOpen() == true);

        //override menu freezing timescale so tutorial can continue while menu is open
        Time.timeScale = 1f;
        //prevent player from moving while in tutorial
        playercontroller.enabled = false;

        //open bedroom door while player isn't looking
        bedroomDoorClosed.SetActive(false);
        bedroomDoorOpened.SetActive(true);

        //prevent user from closing menu until they use the inventory screen
        PlayerMenuManager.Instance.menuToggleEnabled = false;

        // prompt user to swap cards in inventory and wait until they have 2 cards in their deck to continue with the tutorial
        yield return StartCoroutine(FadePanelOut("InventoryTutorialPanel"));
        StartCoroutine(FadePanelIn("InventoryTooltipPanel"));
        StartCoroutine(FadePanelIn("CardSwapTutorialPanel1"));
        yield return StartCoroutine(FadePanelIn("CardSwapTutorialPanel2"));

        //wait for user to equip 2 cards in their deck
        yield return new WaitUntil(() => GameValue.Instance.GetPlayerValue().GetDeckCardCount(0) >= 2);
        StartCoroutine(FadePanelOut("InventoryTooltipPanel"));
        StartCoroutine(FadePanelOut("CardSwapTutorialPanel1"));
        yield return StartCoroutine(FadePanelOut("CardSwapTutorialPanel2"));

        //prompt user to close inventory
        yield return StartCoroutine(FadePanelIn("InventoryTutorialPanel2"));
        PlayerMenuManager.Instance.menuToggleEnabled = true;

        //wait for player to close inventory
        yield return new WaitUntil(() => PlayerMenuManager.Instance.IsMenuOpen() == false);
        yield return StartCoroutine(FadePanelOut("InventoryTutorialPanel2"));

        //enable all buttons on inventory menu
        Button1.interactable = true;
        Button2.interactable = true;
        Button3.interactable = true;
        SortButton1.interactable = true;
        SortButton2.interactable = true;
        DeckButton.interactable = true;

        //return control to player
        playercontroller.enabled = true;
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator WeaponTutorial()
    {
        //prompt user to find a weapon
        yield return StartCoroutine(FadePanelIn("WeaponTooltipPanel"));

        //wait until player finds a weapon in the room
        yield return new WaitUntil(() => GameValue.Instance.GetPlayerValue().HadWeaponsLibrary.Count > 0);

        //don't let player move from spot until they have equipped their weapon
        playercontroller.enabled = false;
        yield return StartCoroutine(FadePanelOut("WeaponTooltipPanel"));

        //prompt user to open inventory again to show them how to equip their weapon
        PlayerMenuManager.Instance.menuToggleEnabled = false;
        yield return StartCoroutine(FadePanelIn("InventoryTutorialPanel3"));
        PlayerMenuManager.Instance.menuToggleEnabled = true;

        //wait for user to open inventory
        yield return new WaitUntil(() => PlayerMenuManager.Instance.IsMenuOpen() == true);

        //override menu freezing timescale so tutorial can continue while menu is open
        Time.timeScale = 1f;

        //prevent user from closing menu until they equip their weapon
        PlayerMenuManager.Instance.menuToggleEnabled = false;
        yield return StartCoroutine(FadePanelOut("InventoryTutorialPanel3"));

        //promt user to equip weapon
        yield return StartCoroutine(FadePanelIn("WeaponTutorialPanel"));

        //wait until player equips a weapon
        yield return new WaitUntil(() => GameValue.Instance.GetPlayerValue().EquipmentWeapon != null);
        yield return StartCoroutine(FadePanelOut("WeaponTutorialPanel"));

        //prompt user to close inventory
        yield return StartCoroutine(FadePanelIn("InventoryTutorialPanel4"));
        PlayerMenuManager.Instance.menuToggleEnabled = true;

        //wait for player to close inventory
        yield return new WaitUntil(() => PlayerMenuManager.Instance.IsMenuOpen() == false);
        yield return StartCoroutine(FadePanelOut("InventoryTutorialPanel4"));
        PlayerMenuManager.Instance.menuToggleEnabled = false;

        yield return null;
    }

    public IEnumerator CombatTutorial()
    {
        //open front door
        frontDoorClosed.SetActive(false);
        frontDoorOpened.SetActive(true);

        //move zombie into the room
        tutorialZombie.transform.position = zombieSpawnPoint;

        Transform originalParent = Camera.transform.parent;
        //lerp camera to zombie
        Vector3 originalCameraPosition = Camera.transform.position;

        yield return StartCoroutine(LerpTransform(Camera.transform, originalCameraPosition, 
                                                    new Vector3(tutorialZombie.transform.position.x,
                                                        tutorialZombie.transform.position.y,
                                                            originalCameraPosition.z), 0.5f));
        

        //move zombie to way point
        yield return StartCoroutine(LerpTransform(tutorialZombie.transform, zombieSpawnPoint, zombieWayPoint, 2f));


        //move camera back to original position
        StartCoroutine(LerpTransform(Camera.transform, Camera.transform.position, originalCameraPosition, 1));

        //move zombie to player
        yield return StartCoroutine(LerpTransform(tutorialZombie.transform, zombieWayPoint, playercontroller.transform.position, 2f));
        yield return new WaitForSeconds(1f);
        PlayerMenuManager.Instance.menuToggleEnabled = true;
        playercontroller.enabled = true;



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

    public IEnumerator LerpTransform(Transform T, Vector3 src, Vector3 dest, float eventTime)
    {
        float elapsedTime = 0f;
        while(elapsedTime < eventTime && T != null)
        {
            T.position = Vector3.Lerp(src, dest, elapsedTime / eventTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator RefreshScene()
    {
        // This is a workaround to ensure that all objects in the scene are properly initialized before the sequence starts.
        // It forces a frame to pass, allowing all Start() methods to run.
        yield return null;
        GameValue.Instance.LoadSceneByEnum(SceneType.GameStartScene);
    }
}