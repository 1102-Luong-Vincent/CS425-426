// Author: Sean Masterson
// Created by: Sean Masterson
// Modified by: Sean Masterson and Shawn Meng, Yuhan Tang
// no external source was used.

//using TCG_CardMaker;
using UnityEngine;
using UnityEngine.UI;
using System;
using static ButtonEffect;
using UnityEngine.Audio;

public class PlayerMenuManager : MonoBehaviour
{
    public static PlayerMenuManager Instance;

    public GameObject MainPanel;

    [SerializeField] private GameObject darknessOverlay;
    private bool darknessOverlayWasActiveBeforeMenu;
    private bool hasStoredDarknessOverlayState;

    public ButtonAndPanel Deck;
    public ButtonAndPanel Combine;
    public ButtonAndPanel Upgrade;
    public ButtonAndPanel Option;

    [SerializeField] public AudioSource audio;
    [SerializeField] public AudioClip buttonClickSound;

    [Serializable]

    public class ButtonAndPanel
    {
        public Button button;
        public PanelControl panel;
    }

    public enum MenuState
    {
        Deck,
        Combine,
        Upgrade,
        Options,
        Closed
    }

    private MenuState currentState = MenuState.Closed;

    [SerializeField] GameObject playerHUD;
    [SerializeField] GameObject playerMiniMap;

    #region Unity Lifecycle

    public bool menuToggleEnabled = true;
    private bool menuOpen = false;
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        InitButtons();
        CloseAllMenus();
    }

    private void Update()
    {
        // Toggle menu with Tab or Escape key
        if (Input.GetKeyDown(KeyCode.Tab) && menuToggleEnabled && GameValue.Instance.GetCurrentScence() != SceneType.BattleScene)
        {
            if (currentState == MenuState.Closed && GameValue.Instance.GetCurrentScence() != SceneType.BattleScene)
            {
                OpenSpecificMenu(MenuState.Deck);
                playerHUD.SetActive(false);
                playerMiniMap.SetActive(false);
                menuOpen = true;
            } else
            {
                menuOpen = false;
                CloseAllMenus();
                playerHUD.SetActive(true);
                playerMiniMap.SetActive(true);

            }
        }
    }

    public bool IsMenuOpen()
    {
        return menuOpen;
    }   
    #endregion

    #region Initialization

    /// <summary>
    /// Initialize all button click listeners
    /// </summary>
    void InitButtons()
    {
        OnGameMenuButtonClick(Deck.button, OnDeckButtonClick);
        OnGameMenuButtonClick(Combine.button, OnCombineButtonClick);
        OnGameMenuButtonClick(Upgrade.button, OnUpgradeButtonClick);
        OnGameMenuButtonClick(Option.button, OnOptionButtonClick);
    }

    #endregion

    #region Button Callbacks

    void OnDeckButtonClick()
    {
        OpenSpecificMenu(MenuState.Deck);
        audio.PlayOneShot(buttonClickSound);
        playerHUD.SetActive(false);
        playerMiniMap.SetActive(false);
    }

    void OnCombineButtonClick()
    {
        OpenSpecificMenu(MenuState.Combine);
        audio.PlayOneShot(buttonClickSound);
        playerHUD.SetActive(false);
        playerMiniMap.SetActive(false);
    }

    void OnUpgradeButtonClick()
    {
        OpenSpecificMenu(MenuState.Upgrade);
        audio.PlayOneShot(buttonClickSound);
        playerHUD.SetActive(false);
        playerMiniMap.SetActive(false);
    }

    void OnOptionButtonClick()
    {
        OpenSpecificMenu(MenuState.Options);
        audio.PlayOneShot(buttonClickSound);
        playerHUD.SetActive(false);
        playerMiniMap.SetActive(false);
    }

    #endregion

    #region Public API

    /// <summary>
    /// Open a specific menu panel
    /// </summary>
    /// <param name="targetState">The menu state to open</param>
    public void OpenSpecificMenu(MenuState targetState)
    {
        if (targetState == MenuState.Closed)
        {
            CloseAllMenus();
            return;
        }

        // Activate main panel if not already active
        if (IsClose())
        {
            Time.timeScale = 0f;
            MainPanel.SetActive(true);

            // When open the inventory close the darkness overlay
            //if (darknessOverlay != null && !hasStoredDarknessOverlayState)
            //{
            //    darknessOverlayWasActiveBeforeMenu = darknessOverlay.activeSelf;
            //    hasStoredDarknessOverlayState = true;
            //    darknessOverlay.SetActive(false);
            //}
        }

        // Update state and refresh UI
        currentState = targetState;
        OpenPanelByState(currentState);
    }

    /// <summary>
    /// Close all menu panels and return to game
    /// </summary>
    public void CloseAllMenus()
    {
        Time.timeScale = 1f;

        // Hide all panels
        Deck.panel.HidePanel();
        Combine.panel.HidePanel();
        Upgrade.panel.HidePanel();
        Option.panel.HidePanel();

        MainPanel.SetActive(false);

        // Open back up the darkness overlay when close the inventory
        //if (darknessOverlay != null && hasStoredDarknessOverlayState)
        //{
        //    darknessOverlay.SetActive(darknessOverlayWasActiveBeforeMenu);
        //    hasStoredDarknessOverlayState = false;
        //}

        currentState = MenuState.Closed;
    }


    /// <summary>
    /// Get current menu state
    /// </summary>
    public MenuState GetCurrentState()
    {
        return currentState;
    }

    #endregion

    #region Private Methods


    /// <summary>
    /// Close panel based on menu state
    /// </summary>
    private void ClosePanelByState(MenuState state)
    {
        switch (state)
        {
            case MenuState.Deck:
                Deck.panel.HidePanel();
                break;
            case MenuState.Combine:
                Combine.panel.HidePanel();
                break;
            case MenuState.Upgrade:
                Upgrade.panel.HidePanel();
                break;
            case MenuState.Options:
                Option.panel.HidePanel();
                break;
        }
    }

    /// <summary>
    /// Open panel based on menu state
    /// </summary>
    private void OpenPanelByState(MenuState state)
    {
        Deck.panel.SetActive(state == MenuState.Deck);
        Combine.panel.SetActive(state == MenuState.Combine);
        Upgrade.panel.SetActive(state == MenuState.Upgrade);
        Option.panel.SetActive(state == MenuState.Options);

    }

    bool IsClose()
    {
        return currentState == MenuState.Closed;
    }

    #endregion
}