// Author: Sean Masterson
// Created by: Sean Masterson
// Modified by: Sean Masterson and Shawn Meng, Yuhan Tang
// no external source was used.

using TCG_CardMaker;
using UnityEngine;
using UnityEngine.UI;
using System;
using static ButtonEffect;

public class PlayerMenuManager : MonoBehaviour
{
    public static PlayerMenuManager Instance;

    public GameObject MainPanel;

    public ButtonAndPanel Deck;
    public ButtonAndPanel Combine;
    public ButtonAndPanel Upgrade;
    public ButtonAndPanel Option;

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

    #region Unity Lifecycle

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
        if (Input.GetKeyDown(KeyCode.Tab) )
        {
            if (currentState == MenuState.Closed)
            {
                OpenSpecificMenu(MenuState.Deck);
            } else
            {
                CloseAllMenus();
            }
        }
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
    }

    void OnCombineButtonClick()
    {
        OpenSpecificMenu(MenuState.Combine);
    }

    void OnUpgradeButtonClick()
    {
        OpenSpecificMenu(MenuState.Upgrade);
    }

    void OnOptionButtonClick()
    {
        OpenSpecificMenu(MenuState.Options);
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
        Debug.Log(state);
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