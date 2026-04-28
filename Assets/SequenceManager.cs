using SmallScaleInc.TopDownPixelCharactersPack1;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class SequenceManager : MonoBehaviour
{
    public static SequenceManager Instance;


    //outside objects used across multiple scenes
    public FadeTransition fader;
    public PlayerController playercontroller;
    public Button Button1;
    public Button Button2;
    public Button Button3;
    public Button SortButton1;
    public Button SortButton2;
    public Button DeckButton;
    public Button CombineButton;
    public Button CombineButton2;
    public Button UpgradeButton;
    public Button OptionsButton;
    public GameObject camera;
    public GameObject WeaponBlocker;
    public GameObject ItemBlocker;
    
    // sequences we are managing:
    GameStartSequence gamestartsequence;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
            return;
        }
        gamestartsequence = GetComponent<GameStartSequence>();
    }
    void Start()
    {
        //initialize multi-scene objects to be disabled at the start of the game, they will be enabled at the end of the tutorial
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

        //start running all game sequences in order
        StartCoroutine(RunSequences());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator RunSequences()
    {
        //tutorial sequence part 1: movement, item management, navigation, weapons. Ends when player encounters first monster.
        if (!gamestartsequence.SkipSequence)
        {
            yield return StartCoroutine(gamestartsequence.RunSequence());
        }
        else
        {
            // if we're skipping this sequence, we're setting the flags this sequence would have set to true
            gamestartsequence.Cleanup();
        }
    }

    private void RefreshSceneReferences()
    {
        playercontroller = FindFirstObjectByType<PlayerController>();
        camera = Camera.main != null ? Camera.main.gameObject : null;
    }
}
