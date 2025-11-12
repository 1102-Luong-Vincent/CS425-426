using TCG_CardMaker;
using UnityEngine;
using UnityEngine.UI;
using static ButtonEffect;

public class GameMenuControl : MonoBehaviour
{
    public static GameMenuControl Instance;

    public GameObject MainPanel;

    [Header("Buttons")]
    public Buttons buttons;

    [System.Serializable]
    public class Buttons
    {
        public Button DeckButton;
        public Button CombineButton;
        public Button UpgradeButton;
        public Button OptionButton;
    }

    [Header("Panels")]
    public Panels panels;

    [System.Serializable]
    public class Panels
    {
        public GameObject DeckPanel;
        public GameObject CombinePanel;
        public GameObject UpgradePanel;
        public GameObject OptionsPanel;
    }

    public enum MenuState
    {
        Deck, Combine, Upgrade, Options 
    }

    MenuState state = MenuState.Deck;
    MenuState previousState = MenuState.Deck;
    private bool menuActive = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (state != previousState && menuActive)
        {
            switch (state)
            {
                case MenuState.Deck:
                    panels.DeckPanel.SetActive(true);
                    panels.CombinePanel.SetActive(false);
                    panels.UpgradePanel.SetActive(false);
                    panels.OptionsPanel.SetActive(false);
                    previousState = MenuState.Deck;
                    break;
                case MenuState.Combine:
                    panels.DeckPanel.SetActive(false);
                    panels.CombinePanel.SetActive(true);
                    panels.UpgradePanel.SetActive(false);
                    panels.OptionsPanel.SetActive(false);
                    previousState = MenuState.Combine;
                    break;
                case MenuState.Upgrade:
                    panels.DeckPanel.SetActive(false);
                    panels.CombinePanel.SetActive(false);
                    panels.UpgradePanel.SetActive(true);
                    panels.OptionsPanel.SetActive(false);
                    previousState = MenuState.Upgrade;
                    break;
                case MenuState.Options:
                    panels.DeckPanel.SetActive(false);
                    panels.CombinePanel.SetActive(false);
                    panels.UpgradePanel.SetActive(false);
                    panels.OptionsPanel.SetActive(true);
                    previousState = MenuState.Options;
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleMenu();
        }
    }
    void InitButtons()
    {

        OnGameMenuButtonClick(buttons.DeckButton, OnDeckButtonClick);
        OnGameMenuButtonClick(buttons.CombineButton, OnCombineButtonClick);
        OnGameMenuButtonClick(buttons.UpgradeButton, OnUpgradeButtonClick);
        OnGameMenuButtonClick(buttons.OptionButton, OnOptionButtonClick);

    }

    void OnDeckButtonClick()
    {
        state = MenuState.Deck;
    }

    void OnCombineButtonClick()
    {
        state = MenuState.Combine;
    }

    void OnUpgradeButtonClick()
    {
        state = MenuState.Upgrade;
    }
    void OnOptionButtonClick()
    {
        state = MenuState.Options;
    }

    void ToggleMenu()
    {
        if(menuActive)
        {
            MainPanel.SetActive(false);
            menuActive = false;
        }
        else
        {
            MainPanel.SetActive(true);
            state = MenuState.Deck;
            menuActive = true;
        }
    }
}
