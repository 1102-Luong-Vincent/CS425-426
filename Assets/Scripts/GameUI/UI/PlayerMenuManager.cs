// Author: Sean Masterson
// Created by: Sean Masterson
// Modified by: Sean Masterson and Shawn Meng
// no external source was used.


using TCG_CardMaker;
using UnityEngine;
using UnityEngine.UI;
using System;
using static ButtonEffect;
using System.Security.Cryptography.X509Certificates;

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
        Deck, Combine, Upgrade, Options, Closed
    }

    MenuState state = MenuState.Deck;
    MenuState previousState = MenuState.Deck;
    private bool menuActive = false;

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
            ClosePreviousPanel();
            Deck.panel.SetActive(state == MenuState.Deck);
           Combine.panel.SetActive(state == MenuState.Combine);
           Upgrade.panel.SetActive(state == MenuState.Upgrade);
           Option.panel.SetActive(state == MenuState.Options);
            previousState = state;



            //switch (state)
            //{
            //    case MenuState.Deck:
            //        panels.DeckPanel.SetActive(true);
            //        if (panels.DeckPanel != null)
            //            panels.DeckPanel.GetComponent<InventoryUIControl>().onInventoryOpen();
            //        panels.CombinePanel.ClosePanel();
            //        panels.UpgradePanel.SetActive(false);
            //        panels.OptionsPanel.ClosePanel();
            //        previousState = MenuState.Deck;
            //        break;
            //    case MenuState.Combine:
            //        panels.DeckPanel.SetActive(false);
            //        panels.CombinePanel.OpenPanel();
            //        panels.UpgradePanel.SetActive(false);
            //        panels.OptionsPanel.ClosePanel();
            //        previousState = MenuState.Combine;
            //        break;
            //    case MenuState.Upgrade:
            //        panels.DeckPanel.SetActive(false);
            //        panels.CombinePanel.ClosePanel();
            //        panels.UpgradePanel.SetActive(true);
            //        panels.OptionsPanel.ClosePanel();
            //        previousState = MenuState.Upgrade;
            //        break;
            //    case MenuState.Options:
            //        panels.DeckPanel.SetActive(false);
            //        panels.CombinePanel.ClosePanel();
            //        panels.UpgradePanel.SetActive(false);
            //        panels.OptionsPanel.OpenPanel();
            //        previousState = MenuState.Options;
            //        break;
            //    case MenuState.Closed:
            //        previousState = MenuState.Closed;
            //        break;
            //}
        }

    }






    void InitButtons()
    {
        OnGameMenuButtonClick(Deck.button, OnDeckButtonClick);
        OnGameMenuButtonClick(Combine.button, OnCombineButtonClick);
        OnGameMenuButtonClick(Upgrade.button, OnUpgradeButtonClick);
        OnGameMenuButtonClick(Option.button, OnOptionButtonClick);

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
        if (menuActive)
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

    void ClosePreviousPanel()
    {
        switch (previousState)
        {
            case MenuState.Deck: Deck.panel.HidePanel() ;break;
            case MenuState.Combine: Combine.panel.HidePanel(); break;
            case MenuState.Upgrade: Upgrade.panel.HidePanel(); break;
            case MenuState.Options: Option.panel.HidePanel(); break;

        }
    }
}
