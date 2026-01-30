// Author: Sean Masterson
// Created by: Sean Masterson
// Modified by: Sean Masterson
// no external source was used.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ButtonEffect;
public class InventoryManager : MonoBehaviour
{

    PlayerValue playerValue;
    public GameObject MenuCardPrefab;
    public GameObject CardSelectorPanel;
    public GameObject CardViewerPanel;
    public GameObject WeaponSelectorPanel;
    public Transform SelectorCardZone;
    public Transform AllCardZone;
    public Transform WeaponZone;
    public Button CancelButton;
    public Button CardLibraryButton;
    public Button CardCancelButton;
    public Button WeaponLibraryButton;
    public Button WeaponCancelButton;
    public static InventoryManager Instance;
    public InventoryUIControl control;
    GameObject targetCard; // card we want to replace
    bool replacingCard = false; // when we select a card, are we replacing it?
    void Awake()
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
        playerValue = GameValue.Instance.GetPlayerValue();
        OnGameMenuButtonClick(CancelButton, OnCancelButtonClick);
        OnGameMenuButtonClick(CardLibraryButton, OnCardsButtonClick);
        OnGameMenuButtonClick(WeaponLibraryButton, OnWeaponButtonClick);
        OnGameMenuButtonClick(CardCancelButton, OnCardCancelButtonClick);
        OnGameMenuButtonClick(WeaponCancelButton, OnWeaponCancelButtonClick);
    }

    void CardClicked(GameObject card)
    {
        if(!replacingCard)
        {
            targetCard = card;
            OpenCardSelector();
        }
        else
        {
            CardValue newCard = card.GetComponent<MenuCardControl>().GetCardValue();
            CardValue oldCard = targetCard.GetComponent<MenuCardControl>().GetCardValue();

            if(targetCard.gameObject.tag == "EquipCard")
            {
                playerValue.EquipmentCards.Remove(oldCard);
                playerValue.EquipmentCards.Add(newCard);
            }
            else if (targetCard.gameObject.tag == "BattleCard")
            {
                playerValue.battleCardsList.Remove(oldCard);
                playerValue.battleCardsList.Add(newCard);
            }

                control.RefreshInventory();
            CloseCardSelector();
        }
    }

    void CardRightClicked(GameObject card)
    {
        if (!replacingCard)
        {
            if (card.tag == "BattleCard")
            {
                playerValue.battleCardsList.Remove(card.GetComponent<MenuCardControl>().GetCardValue());
            }
            else if (card.tag == "EquipCard")
            {
                playerValue.EquipmentCards.Remove(card.GetComponent<MenuCardControl>().GetCardValue());
            }
            control.RefreshInventory();
        }
        else
        {
            CardValue newCard = card.GetComponent<MenuCardControl>().GetCardValue();
            CardValue oldCard = targetCard.GetComponent<MenuCardControl>().GetCardValue();

            if (targetCard.gameObject.tag == "EquipCard")
            {
                playerValue.EquipmentCards.Remove(oldCard);
                playerValue.EquipmentCards.Add(newCard);
            }
            else if (targetCard.gameObject.tag == "BattleCard")
            {
                playerValue.battleCardsList.Remove(oldCard);
                playerValue.battleCardsList.Add(newCard);
            }

            control.RefreshInventory();
            CloseCardSelector();
        }
    }

    void OpenCardSelector()
    {
        replacingCard = true;
        CardSelectorPanel.SetActive(true);

        //make list of cards not already equipped
        List<CardValue> AvailableCards = new List<CardValue>(playerValue.HadCardsLibrary);
        foreach(CardValue val in playerValue.battleCardsList)
        {
            AvailableCards.Remove(val);
        }
        foreach(CardValue val in playerValue.EquipmentCards)
        {
            AvailableCards.Remove(val);
        }

        // populate card zone with cards in PlayerValue
        foreach (CardValue val in AvailableCards)
        {
            GameObject card = Instantiate(MenuCardPrefab);
            card.name = (val.CardName + " card");
            MenuCardControl menucard = card.GetComponent<MenuCardControl>();
            card.transform.SetParent(SelectorCardZone);
            menucard.SetCardValue(val);
        }
    }
    void CloseCardSelector()
    {
        targetCard = null;
        replacingCard = false;
        // menu closed -- erase menu cards
        for (int i = SelectorCardZone.childCount - 1; i >= 0; i--)
        {
            GameObject menucard = SelectorCardZone.GetChild(i).gameObject;

            Destroy(menucard);
        }
        CardSelectorPanel.SetActive(false);
    }

    void OnCancelButtonClick()
    {
        CloseCardSelector();
    }

    public void CloseInventory()
    {
        CloseCardSelector();
        control.OnInventoryClose();
        if(WeaponSelectorPanel.activeInHierarchy == true)
        {
            OnWeaponCancelButtonClick();
        }
        if (CardViewerPanel.activeInHierarchy == true)
        {
            OnCardCancelButtonClick();
        }
    }


    void OnWeaponButtonClick()
    {
        WeaponSelectorPanel.SetActive(true);
        foreach (WeaponValue val in playerValue.HadWeaponsLibrary)
        {
            GameObject card = Instantiate(MenuCardPrefab);
            card.name = (val.WeaponName + " card");
            MenuCardControl menucard = card.GetComponent<MenuCardControl>();
            card.transform.SetParent(WeaponZone);
            menucard.SetWeaponValue(val);
        }
    }

    void OnCardsButtonClick()
    {
        CardViewerPanel.SetActive(true);
        foreach (CardValue val in playerValue.HadCardsLibrary)
        {
            GameObject card = Instantiate(MenuCardPrefab);
            card.name = (val.CardName + " card");
            MenuCardControl menucard = card.GetComponent<MenuCardControl>();
            card.transform.SetParent(AllCardZone);
            menucard.Deactivate();
            menucard.SetCardValue(val);
        }
    }

    void OnCardCancelButtonClick()
    {
        for (int i = AllCardZone.childCount - 1; i >= 0; i--)
        {
            GameObject menucard = AllCardZone.GetChild(i).gameObject;

            Destroy(menucard);
        }
        CardViewerPanel.SetActive(false);
    }
    void OnWeaponCancelButtonClick()
    {
        for (int i = WeaponZone.childCount - 1; i >= 0; i--)
        {
            GameObject menucard = WeaponZone.GetChild(i).gameObject;

            Destroy(menucard);
        }
        WeaponSelectorPanel.SetActive(false);
    }
}
