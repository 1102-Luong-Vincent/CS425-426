using SmallScaleInc.TopDownPixelCharactersPack1;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameStartSequence : BaseSequence
{

    //public bool SkipSequence = false;
    GameStartSequence Instance { get; set; }

    //outside objects necessary for sequence
    [SerializeField] private GameObject DoorTrigger;
    [SerializeField] private GameObject tutorialZombie;
    [SerializeField] private GameObject bedroomDoorOpened;
    [SerializeField] private GameObject bedroomDoorClosed;
    [SerializeField] private GameObject frontDoorOpened;
    [SerializeField] private GameObject frontDoorClosed;


    //sequence flags


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
            SequenceManager.Instance.fader.SetColor(new Color(0f, 0f, 0f, 0f));
            Cleanup();
            Destroy(this);
            return;
        }
        //StartCoroutine(PlayGameStartSequence());
    }

    private void Update()
    {

    }

    public override IEnumerator RunSequence()
    {
        // initilalize the room
        SequenceManager.Instance.fader.SetColor(new Color(0f, 0f, 0f, 1f)); //start with a black screen
        DoorTrigger.SetActive(false);
        zombieSpawnPoint = new Vector3(8.14f, -0.45f, 0f);
        zombieWayPoint = new Vector3(6.7f, 0.5f, 0f);
        zombieController = tutorialZombie.GetComponent<EnemyControl>();
        zombieController.overRideControl = true;


        // begin sequence
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(SequenceManager.Instance.fader.FadeIn());
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

        //start combine tutorial
        yield return StartCoroutine(CardCombineTutorial());

        Cleanup();

        yield return null;
        Destroy(this);
    }

    public IEnumerator moveTutoral()
    {
        // walking tutorial -- prompt user to move with WASD
        yield return StartCoroutine(FadePanelIn("MoveTutorialPanel"));
        SequenceManager.Instance.playercontroller.enabled = true;

        //wait until player moves
        yield return new WaitUntil(() => SequenceManager.Instance.playercontroller.isMoving == true);
        yield return StartCoroutine(FadePanelOut("MoveTutorialPanel", true));
        yield return new WaitForSeconds(1f);


        // sprinting tutorial -- prompt user to run with SHIFT
        yield return StartCoroutine(FadePanelIn("SprintTutorialPanel"));

        //wait until player sprints
        yield return new WaitUntil(() => SequenceManager.Instance.playercontroller.isRunning == true);
        yield return StartCoroutine(FadePanelOut("SprintTutorialPanel", true));
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator interactTutorial()
    {
        // interact tutorial -- prompt user to interact with items using E
        StartCoroutine(FadePanelIn("InteractTutorialPanel"));
        yield return StartCoroutine(FadePanelIn("InteractTooltipPanel"));

        // wait until player picks up 2 cards
        yield return new WaitUntil(() => GameValue.Instance.GetPlayerValue().GetCardCount() >= 2);
        StartCoroutine(FadePanelOut("InteractTutorialPanel", true));
        yield return StartCoroutine(FadePanelOut("InteractTooltipPanel", true));

        //set card values to common for crafting tutorial
        GameValue.Instance.GetPlayerValue().HadCardsLibrary[0].rarity = CardRarity.Common;
        GameValue.Instance.GetPlayerValue().HadCardsLibrary[1].rarity = CardRarity.Common;
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
        SequenceManager.Instance.playercontroller.enabled = false;

        //open bedroom door while player isn't looking
        bedroomDoorClosed.SetActive(false);
        bedroomDoorOpened.SetActive(true);

        //prevent user from closing menu until they use the inventory screen
        PlayerMenuManager.Instance.menuToggleEnabled = false;

        // prompt user to swap cards in inventory and wait until they have 2 cards in their deck to continue with the tutorial
        yield return StartCoroutine(FadePanelOut("InventoryTutorialPanel", false));
        StartCoroutine(FadePanelIn("InventoryTooltipPanel"));
        StartCoroutine(FadePanelIn("CardSwapTutorialPanel1"));
        yield return StartCoroutine(FadePanelIn("CardSwapTutorialPanel2"));

        //wait for user to equip 2 cards in their deck
        yield return new WaitUntil(() => GameValue.Instance.GetPlayerValue().GetDeckCardCount(0) >= 2);
        StartCoroutine(FadePanelOut("InventoryTooltipPanel", true));
        StartCoroutine(FadePanelOut("CardSwapTutorialPanel1", true));
        yield return StartCoroutine(FadePanelOut("CardSwapTutorialPanel2", true));

        //prompt user to close inventory
        yield return StartCoroutine(FadePanelIn("InventoryTutorialPanel2"));
        PlayerMenuManager.Instance.menuToggleEnabled = true;

        //wait for player to close inventory
        yield return new WaitUntil(() => PlayerMenuManager.Instance.IsMenuOpen() == false);
        yield return StartCoroutine(FadePanelOut("InventoryTutorialPanel2", false));



        //return control to player
        SequenceManager.Instance.playercontroller.enabled = true;
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator WeaponTutorial()
    {
        //prompt user to find a weapon
        yield return StartCoroutine(FadePanelIn("WeaponTooltipPanel"));

        //wait until player finds a weapon in the room
        yield return new WaitUntil(() => GameValue.Instance.GetPlayerValue().HadWeaponsLibrary.Count > 0);
        PlayerMenuManager.Instance.menuToggleEnabled = false;
        //don't let player move from spot until they have equipped their weapon
        SequenceManager.Instance.playercontroller.enabled = false;
        yield return StartCoroutine(FadePanelOut("WeaponTooltipPanel", true));

        //prompt user to open inventory again to show them how to equip their weapon

        yield return StartCoroutine(FadePanelIn("InventoryTutorialPanel"));
        PlayerMenuManager.Instance.menuToggleEnabled = true;

        //wait for user to open inventory
        yield return new WaitUntil(() => PlayerMenuManager.Instance.IsMenuOpen() == true);

        //override menu freezing timescale so tutorial can continue while menu is open
        Time.timeScale = 1f;

        //prevent user from closing menu until they equip their weapon
        PlayerMenuManager.Instance.menuToggleEnabled = false;
        yield return StartCoroutine(FadePanelOut("InventoryTutorialPanel", false));

        //promt user to equip weapon
        yield return StartCoroutine(FadePanelIn("WeaponTutorialPanel"));

        //wait until player equips a weapon
        yield return new WaitUntil(() => GameValue.Instance.GetPlayerValue().EquipmentWeapon != null);
        yield return StartCoroutine(FadePanelOut("WeaponTutorialPanel", true));

        //prompt user to close inventory
        yield return StartCoroutine(FadePanelIn("InventoryTutorialPanel2"));
        PlayerMenuManager.Instance.menuToggleEnabled = true;

        //wait for player to close inventory
        yield return new WaitUntil(() => PlayerMenuManager.Instance.IsMenuOpen() == false);
        yield return StartCoroutine(FadePanelOut("InventoryTutorialPanel2", false));
        PlayerMenuManager.Instance.menuToggleEnabled = false;

        //update player's objective
        if (GameValue.Instance.GetCurrentObjective() == ObjectiveConstants.CompleteTutorial)
        {
            GameValue.Instance.SetCurrentObjective(ObjectiveConstants.LeaveStartRoom);
        }

        yield return null;
    }

    public IEnumerator CombatTutorial()
    {
        //open front door
        frontDoorClosed.SetActive(false);
        frontDoorOpened.SetActive(true);

        //move zombie into the room
        tutorialZombie.transform.position = zombieSpawnPoint;

        Transform originalParent = SequenceManager.Instance.camera.transform.parent;
        //lerp camera to zombie
        Vector3 originalCameraPosition = SequenceManager.Instance.camera.transform.position;

        yield return StartCoroutine(LerpTransform(SequenceManager.Instance.camera.transform, originalCameraPosition, 
                                                    new Vector3(tutorialZombie.transform.position.x,
                                                        tutorialZombie.transform.position.y,
                                                            originalCameraPosition.z), 0.5f));
        

        //move zombie to way point
        yield return StartCoroutine(LerpTransform(tutorialZombie.transform, zombieSpawnPoint, zombieWayPoint, 2f));


        //move camera back to original position
        StartCoroutine(LerpTransform(SequenceManager.Instance.camera.transform, 
                                    SequenceManager.Instance.camera.transform.position, originalCameraPosition, 1));

        //move zombie to player
        yield return StartCoroutine(LerpTransform(tutorialZombie.transform, zombieWayPoint, 
                                                    SequenceManager.Instance.playercontroller.transform.position, 2f));

        //wait for battle scene to load
        yield return new WaitUntil(() => GameValue.Instance.GetCurrentScence() == SceneType.BattleScene);

        GameObject battleCanvas = GameObject.Find("BattleCanvas");
        SequenceManager.Instance.WeaponBlocker.transform.SetParent(battleCanvas.transform, true);
        SequenceManager.Instance.ItemBlocker.transform.SetParent(battleCanvas.transform, true);
        SequenceManager.Instance.WeaponBlocker.SetActive(true);
        SequenceManager.Instance.ItemBlocker.SetActive(true);
        yield return new WaitForSeconds(1f);
        //wait for player turn to start
        yield return new WaitUntil(() => BattleManage.Instance.Turn == 1);
        StartCoroutine(FadePanelIn("CombatTutorialPanel"));
        yield return new WaitForSeconds(3f);
        StartCoroutine(FadePanelIn("CombatTooltipPanel1"));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(FadePanelIn("CombatTooltipPanel2"));

        //allow player to click on weapon
        SequenceManager.Instance.WeaponBlocker.SetActive(false);

        //wait for player to click on weapon
        yield return new WaitUntil(() => BattleManage.Instance.Turn == 2);
        //block weapon again
        SequenceManager.Instance.WeaponBlocker.SetActive(true);
        StartCoroutine(FadePanelOut("CombatTutorialPanel", false));
        StartCoroutine(FadePanelOut("CombatTooltipPanel1", true));
        yield return StartCoroutine(FadePanelOut("CombatTooltipPanel2", true));

        //wait for player's turn again
        yield return new WaitUntil(() => BattleManage.Instance.Turn == 3);
        yield return new WaitForSeconds(1f);
        // prompt user to click an item
        StartCoroutine(FadePanelIn("CombatTutorialPanel"));
        yield return new WaitForSeconds(1f);
        StartCoroutine(FadePanelIn("CombatTooltipPanel3"));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(FadePanelIn("CombatTooltipPanel4"));
        SequenceManager.Instance.ItemBlocker.SetActive(false);

        //wait for player to click an item
        yield return new WaitUntil(() => BattleManage.Instance.Turn == 4);
        StartCoroutine(FadePanelOut("CombatTutorialPanel", true));
        StartCoroutine(FadePanelOut("CombatTooltipPanel3", true));
        yield return StartCoroutine(FadePanelOut("CombatTooltipPanel4", true));
        //unlock the rest of the combat system
        Destroy(SequenceManager.Instance.WeaponBlocker);
        Destroy(SequenceManager.Instance.ItemBlocker);



        yield return new WaitForSeconds(1f);
    }

    public IEnumerator CardCombineTutorial()
    {
        int originalCardCount = GameValue.Instance.GetPlayerValue().GetCardCount();
        // wait for battle scene to finish
        yield return new WaitUntil(() => GameValue.Instance.GetCurrentScence() == SceneType.GameStartScene);
        yield return new WaitForSeconds(1f);

        //prompt user to open inventory
        StartCoroutine(FadePanelIn("CombineTooltipPanel1"));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(FadePanelIn("InventoryTutorialPanel"));
        PlayerMenuManager.Instance.menuToggleEnabled = true;

        //wait for user to open inventory
        yield return new WaitUntil(() => PlayerMenuManager.Instance.IsMenuOpen() == true);
        PlayerMenuManager.Instance.menuToggleEnabled = false;

        //override timescale freezing so tutorial can continue while inventory is open
        Time.timeScale = 1f;

        StartCoroutine(FadePanelOut("InventoryTutorialPanel", false));
        yield return StartCoroutine(FadePanelOut("CombineTooltipPanel1", true));
        yield return StartCoroutine(FadePanelIn("CombineTutorialPanel1"));



        //enable combine button
        SequenceManager.Instance.CombineButton.interactable = true;

        //wait for user to open combine menu
        yield return new WaitUntil(() => PlayerMenuManager.Instance.GetCurrentState() == PlayerMenuManager.MenuState.Combine);
        SequenceManager.Instance.CombineButton.interactable = false;
        CardCombineManager.Instance.AddChemicals(10);
        yield return StartCoroutine(FadePanelOut("CombineTutorialPanel1", true));

        //prompt user to select 2 cards
        yield return StartCoroutine(FadePanelIn("CombineTutorialPanel2"));
        yield return new WaitUntil(() => CardCombineManager.Instance.BothSlotsOccupied() == true);

        //prompt user to click combine and wait for result
        yield return StartCoroutine(FadePanelOut("CombineTutorialPanel2", true));
        yield return StartCoroutine(FadePanelIn("CombineTutorialPanel3"));

        yield return new WaitUntil(() => GameValue.Instance.GetPlayerValue().GetCardCount() < originalCardCount);


        yield return StartCoroutine(FadePanelOut("CombineTutorialPanel3", true));

        yield return null;
    }


    public void Cleanup()
    {
        SequenceManager.Instance.playercontroller.enabled = true;
        SequenceManager.Instance.Button1.interactable = true;
        SequenceManager.Instance.Button2.interactable = true;
        SequenceManager.Instance.Button3.interactable = true;
        SequenceManager.Instance.SortButton1.interactable = true;
        SequenceManager.Instance.SortButton2.interactable = true;
        SequenceManager.Instance.DeckButton.interactable = true;
        SequenceManager.Instance.CombineButton.interactable = true;
        SequenceManager.Instance.UpgradeButton.interactable = true;
        SequenceManager.Instance.OptionsButton.interactable = true;
        PlayerMenuManager.Instance.menuToggleEnabled = true;
        DestroyPanel("MoveTutorialPanel");
        DestroyPanel("SprintTutorialPanel");
        DestroyPanel("InteractTutorialPanel");
        DestroyPanel("InteractTooltipPanel");
        DestroyPanel("InventoryTutorialPanel");
        DestroyPanel("InventoryTooltipPanel");
        DestroyPanel("CardSwapTutorialPanel1");
        DestroyPanel("CardSwapTutorialPanel2");
        DestroyPanel("WeaponTooltipPanel");
        DestroyPanel("InventoryTutorialPanel2");
        DestroyPanel("CombatTutorialPanel");
        DestroyPanel("CombatTooltipPanel1");
        DestroyPanel("CombatTooltipPanel2");
        DestroyPanel("CombatTooltipPanel3");
        DestroyPanel("CombatTooltipPanel4");
        DestroyPanel("CombineTutorialPanel1");
        DestroyPanel("CombineTutorialPanel2");
        DestroyPanel("CombineTutorialPanel3");
        if (DoorTrigger != null)
            DoorTrigger.SetActive(true);
    }
}
