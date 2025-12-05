using TCG_CardMaker;
using UnityEngine;
using UnityEngine.UI;
using static ButtonEffect;

public class PlayerMenuManager : MonoBehaviour
{
    public static PlayerMenuManager Instance;

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
        public CardCombineManager CombinePanel;
        public GameObject UpgradePanel;
        public OptionPanelControl OptionsPanel;
    }

    public enum MenuState
    {
        Deck, Combine, Upgrade, Options, Closed 
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenu();
        }
    }

    // Update is called once per frame
    void UpdateMenu()
    {
        if (state != previousState && menuActive)
        {
            closePreviousPanel();
            switch (state)
            {
                case MenuState.Deck:
                    panels.DeckPanel.SetActive(true);
                    if (panels.DeckPanel != null)
                        panels.DeckPanel.GetComponent<InventoryUIControl>().onInventoryOpen();
                    panels.CombinePanel.ClosePanel();
                    panels.UpgradePanel.SetActive(false);
                    panels.OptionsPanel.ClosePanel();
                    previousState = MenuState.Deck;
                    break;
                case MenuState.Combine:
                    panels.DeckPanel.SetActive(false);
                    panels.CombinePanel.OpenPanel();
                    panels.UpgradePanel.SetActive(false);
                    panels.OptionsPanel.ClosePanel();
                    previousState = MenuState.Combine;
                    break;
                case MenuState.Upgrade:
                    panels.DeckPanel.SetActive(false);
                    panels.CombinePanel.ClosePanel();
                    panels.UpgradePanel.SetActive(true);
                    panels.OptionsPanel.ClosePanel();
                    previousState = MenuState.Upgrade;
                    break;
                case MenuState.Options:
                    panels.DeckPanel.SetActive(false);
                    panels.CombinePanel.ClosePanel();
                    panels.UpgradePanel.SetActive(false);
                    panels.OptionsPanel.OpenPanel();
                    previousState = MenuState.Options;
                    break;
                case MenuState.Closed:
                    previousState = MenuState.Closed;
                    break;
            }
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
        UpdateMenu();
    }

    void OnCombineButtonClick()
    {
        state = MenuState.Combine;
        UpdateMenu();
    }

    void OnUpgradeButtonClick()
    {
        state = MenuState.Upgrade;
        UpdateMenu();
    }
    void OnOptionButtonClick()
    {
        state = MenuState.Options;
        UpdateMenu();
    }

    void ToggleMenu()
    {
        if(menuActive)
        {
            Time.timeScale = 1f;
            state = MenuState.Closed;
           // closePreviousPanel();
            UpdateMenu();
            MainPanel.SetActive(false);
            menuActive = false;
        }
        else
        {
            Time.timeScale = 0f; 
            MainPanel.SetActive(true);
            menuActive = true;
            state = MenuState.Deck;
            UpdateMenu();

        }
    }

    void closePreviousPanel()
    {
        switch (previousState)
        {
            case MenuState.Deck:
                panels.DeckPanel.GetComponent<InventoryManager>().CloseInventory();
                break;
            case MenuState.Combine:
                break;
            case MenuState.Upgrade:
                break;
            case MenuState.Options:
                break;
        }
    }
}
