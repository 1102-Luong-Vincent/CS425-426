// Author: Sean Mastereson
// Created by: Sean Masterson
// Modified by: Sean Masterson and Vincent Luong
// No external sources were used

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ButtonEffect;
using static PlayerMenuManager;

public class InventoryUIControl :  PanelControl
{
    [SerializeField] GameObject playerHUD; //controls the health bar in the map

    public Transform WeaponZone;
    public Transform EquipZone;
    public Transform CardZone;
    public GameObject MenuCardPrefab;
    public GameObject EmptySlotPrefab;
    public TextMeshProUGUI HealthText;
    //public TextMeshProUGUI EnergyText;
    public TextMeshProUGUI CardsText;

    [Header("Buttons")]
    public Buttons buttons;

    [System.Serializable]
    public class Buttons
    {
        public Button SortAZButton;
        public Button SortZAButton;
        public Button SortRarityAscButton;
        public Button SortRarityDescButton;
    }




    public PlayerValue playerValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnInventoryOpen();
        InitButtons();
        playerHUD.SetActive(true); //sets the player hud to true when game starts
    }

    //display relevant playervalue info and populate inventory with menu cards based on playervalue when inventory is opened
    public void OnInventoryOpen()
    {
        if (playerValue == null) playerValue = GameValue.Instance.GetPlayerValue();
        HealthText.text = playerValue.GetHealth().ToString();
        //EnergyText.text = playerValue.GetEnergy().ToString();
        CardsText.text = playerValue.HadCardsLibrary.Count.ToString();

        // color deck buttons based on active deck


        //display available weapons
        InstantiateWeaponCards();

        // display equipped deck
        InstantiateDeck();

        // display remainder of player's card library
        InstantiateCardLibrary();
        
        playerHUD.SetActive(false); //hides the player hud upon inventory opening

    }

    //delete all menu cards from inventory to save memory and prepare for next time menu is opened
    public void OnInventoryClose()
    {
        playerHUD.SetActive(true); //displays the player hud once inventory closes

        // menu closed -- erase menu cards
        for (int i = WeaponZone.childCount - 1; i >= 0; i--)
        {
            GameObject menucard = WeaponZone.GetChild(i).gameObject;

            Destroy(menucard);
        }
        for (int i = EquipZone.childCount - 1; i >= 0; i--)
        {
            GameObject menucard = EquipZone.GetChild(i).gameObject;

            Destroy(menucard);
        }
        for (int i = CardZone.childCount - 1; i >= 0; i--)
        {
            GameObject menucard = CardZone.GetChild(i).gameObject;

            Destroy(menucard);
        }
    }

    public override void ShowPanel()
    {
        base.ShowPanel();
        RefreshInventory(); // refreshing instead of opening to prevent duplicate cards if player opens inventory multiple times without closing
    }

    public override void HidePanel()
    {
        base.HidePanel();
        OnInventoryClose();
    }

    public void RefreshInventory()
    {
        OnInventoryClose();
        OnInventoryOpen();
    }

    void InitButtons()
    {
        OnGameMenuButtonClick(buttons.SortAZButton, OnSortAZButtonClick);
        OnGameMenuButtonClick(buttons.SortZAButton, OnSortZAButtonClick);
        OnGameMenuButtonClick(buttons.SortRarityAscButton, OnSortRarityAscButtonClick);
        OnGameMenuButtonClick(buttons.SortRarityDescButton, OnSortRarityDescButtonClick);
    }

    void OnSortAZButtonClick()
    {
        buttons.SortAZButton.gameObject.SetActive(false);
        buttons.SortZAButton.gameObject.SetActive(true);
        SortCards("Alpha", "Ascending");
    }
    void OnSortZAButtonClick()
    {
        buttons.SortAZButton.gameObject.SetActive(true);
        buttons.SortZAButton.gameObject.SetActive(false);
        SortCards("Alpha", "Descending");
    }

    void OnSortRarityAscButtonClick()
    {
        buttons.SortRarityAscButton.gameObject.SetActive(false);
        buttons.SortRarityDescButton.gameObject.SetActive(true);
        SortCards("Rarity", "Ascending");
    }

    void OnSortRarityDescButtonClick()
    {
        buttons.SortRarityAscButton.gameObject.SetActive(true);
        buttons.SortRarityDescButton.gameObject.SetActive(false);
        SortCards("Rarity", "Descending");
    }

    void SortCards(string sortType, string order)
    {
        switch(sortType)
        {
            case "Alpha":
                List<Transform> children = new List<Transform>();
                foreach (Transform child in CardZone)
                {
                    if(child.name != "MenuCardSlot(Clone)")
                        children.Add(child);
                }

                switch (order)
                {
                    case InventoryConstants.Ascending:
                        children = children.OrderBy(child => child.name).ToList();
                        break;
                    case InventoryConstants.Descending:
                        children = children.OrderByDescending(child => child.name).ToList();
                        break;
                }


                for (int i = 0; i < children.Count; i++)
                {
                    children[i].SetSiblingIndex(i);
                }
                break;
            case "Rarity":
                List<Transform> rarityChildren = new List<Transform>();
                foreach (Transform child in CardZone)
                {
                    if (child.name != "MenuCardSlot(Clone)")
                        rarityChildren.Add(child);
                }
                switch (order)
                {
                    case InventoryConstants.Ascending:
                        rarityChildren = rarityChildren.OrderBy(child => child.GetComponent<MenuCardControl>().GetCardValue().rarity).ToList();
                        break;
                    case InventoryConstants.Descending:
                        rarityChildren = rarityChildren.OrderByDescending(child => child.GetComponent<MenuCardControl>().GetCardValue().rarity).ToList();
                        break;
                }
                for(int i = 0; i < rarityChildren.Count; i++)
                {
                    rarityChildren[i].SetSiblingIndex(i);
                }
                break;
            default:
                break;
        }
    }

    public void InstantiateWeaponCards()
    {


        foreach (WeaponValue val in playerValue.HadWeaponsLibrary)
        {
            GameObject card = Instantiate(MenuCardPrefab);
            card.name = (val.WeaponName + " card");
            card.tag = InventoryConstants.WeaponCard;
            MenuCardControl menucard = card.GetComponent<MenuCardControl>();
            card.transform.SetParent(WeaponZone);
            menucard.SetWeaponValue(val);
            if(val != playerValue.EquipmentWeapon)
            {
                //card.GetComponent<Button>().interactable = false;
            }
        }
    }
    public void InstantiateDeck()
    {

        for (int i = 0; i < playerValue.GetMaxCards(); i++)
        {
            InstantiateMenuCard(playerValue.GetActiveDeck()[i], EquipZone, InventoryConstants.EquipCard, i);

        }
    }

    //display the set of cards the player can add to their deck. this is the player's card library minus the cards currently in their deck.
    public void InstantiateCardLibrary()
    {
        List<CardValue> temp = new List<CardValue>(playerValue.HadCardsLibrary); // create temp list to avoid modifying original library when removing cards to display equipped deck
        foreach (CardValue val in playerValue.GetActiveDeck())
        {
            temp.Remove(val);
        }

        int index = 0;
        foreach (CardValue val in temp)
        {
            InstantiateMenuCard(val, CardZone, InventoryConstants.BattleCard, index);
            index++;
        }
    }

    // this method will instantiate a menu card prefab and parent it to the given transform.
    // if the given card value is null, it will instantiate an empty slot prefab instead.
    public void InstantiateMenuCard(CardValue val, Transform dest, string tag, int index)
    {
        GameObject card;
        if (val != null)
        {
            card = Instantiate(MenuCardPrefab);
            card.name = (val.CardName + "menu card");
            MenuCardControl menucard = card.GetComponent<MenuCardControl>();
            if (menucard != null)
            {
                menucard.SetCardValue(val);
            }
        }
        else
        {
            card = Instantiate(EmptySlotPrefab);
            card.name = "empty card slot";

        }


        card.transform.SetParent(dest);

        // tag card based on which zone it is in
        card.tag = tag;

        switch(tag)
        {
            case InventoryConstants.EquipCard:
                card.GetComponent<MenuCardControl>().location = MenuCardControl.CardLocation.Deck;
                card.GetComponent<MenuCardControl>().index = index;
                break;
            case InventoryConstants.BattleCard:
                card.GetComponent<MenuCardControl>().location = MenuCardControl.CardLocation.Library;
                card.GetComponent<MenuCardControl>().index = index;
                break;
        }
    }

}


public static class InventoryConstants
{
    public const string Ascending = "Ascending";
    public const string Descending = "Descending";
    public const string BattleCard = "BattleCard";
    public const string EquipCard = "EquipCard";
    public const string WeaponCard = "WeaponCard";

}