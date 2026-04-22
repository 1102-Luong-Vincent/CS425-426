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
    public GameObject WeaponSelectorPanel;
    public Transform WeaponZone;
    public Button WeaponLibraryButton;
    public Button WeaponCancelButton;
    public Button DeckButton1;
    public Button DeckButton2;
    public Button DeckButton3;
    public static InventoryManager Instance;
    public InventoryUIControl control;

    public Sprite unarmedImage;
    public Sprite knifeImage;
    public Sprite pistolImage;
    public Sprite shotgunImage;

    public Image playerPortrait;

    GameObject selectedCard = null; //card player has left clicked on


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
        OnGameMenuButtonClick(WeaponLibraryButton, OnWeaponButtonClick);
        OnGameMenuButtonClick(WeaponCancelButton, OnWeaponCancelButtonClick);
        OnGameMenuButtonClick(DeckButton1, () => OnDeckButtonClick(0));
        OnGameMenuButtonClick(DeckButton2, () => OnDeckButtonClick(1));
        OnGameMenuButtonClick(DeckButton3, () => OnDeckButtonClick(2));
    }

    void CardClicked(GameObject card)
    {
        // check to see if we have already selected a card
        if (selectedCard == null)
        {
            selectedCard = card;
            selectedCard.GetComponent<MenuCardControl>().ToggleSelected();
        }
        else
        {
            // if we have already selected a card, make sure we aren't swapping the same card with itself
            if (selectedCard != card)
            {
                SwapCards(selectedCard, card);
                selectedCard.GetComponent<MenuCardControl>().ToggleSelected();
                selectedCard = null;
                control.RefreshInventory();
            }

        }

    }

    void WeaponClicked(GameObject weapon)
    {
        GameValue.Instance.GetPlayerValue().EquipmentWeapon = (weapon.GetComponent<MenuCardControl>().GetWeaponValue());
        switch (GameValue.Instance.GetPlayerValue().EquipmentWeapon.WeaponName)
        {
            case "Knife":
                playerPortrait.sprite = knifeImage;
                break;
            case "Pistol":
                playerPortrait.sprite = pistolImage;
                break;
            case "Shotgun":
                playerPortrait.sprite = shotgunImage;
                break;
            default:
                playerPortrait.sprite = unarmedImage;
                break;
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

        }
    }




    public void CloseInventory()
    {

        control.OnInventoryClose();
        if(WeaponSelectorPanel.activeInHierarchy == true)
        {
            OnWeaponCancelButtonClick();
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




    void OnWeaponCancelButtonClick()
    {
        for (int i = WeaponZone.childCount - 1; i >= 0; i--)
        {
            GameObject menucard = WeaponZone.GetChild(i).gameObject;

            Destroy(menucard);
        }
        WeaponSelectorPanel.SetActive(false);
    }

    void OnDeckButtonClick(int index)
    {
        playerValue.setActiveDeck(index);
        control.RefreshInventory();
    }

    void SwapCards(GameObject src, GameObject dst)
    {
        MenuCardControl srcControl = src.GetComponent<MenuCardControl>();
        MenuCardControl dstControl = dst.GetComponent<MenuCardControl>();

        CardValue temp = srcControl.GetCardValue();
        CardValue temp2 = dstControl.GetCardValue();
        int tmpIndex = srcControl.index;

        Debug.Log("Swapping index " + srcControl.index + " from " + srcControl.location + "with index " + dstControl.index + " from " + dstControl.location);
        if (srcControl.location == MenuCardControl.CardLocation.Deck)
        {
            // case 1: card 1 is in current deck, card 2 is in current deck
            if(dstControl.location == MenuCardControl.CardLocation.Deck)
            {
                playerValue.Decks[playerValue.GetActiveDeckIndex()][srcControl.index] = temp2;
                playerValue.Decks[playerValue.GetActiveDeckIndex()][dstControl.index] = temp;
            }
            //case 2: card 1 is in current deck, card 2 is in library
            else
            {   
                playerValue.Decks[playerValue.GetActiveDeckIndex()][srcControl.index] = temp2;
                if (temp != null)
                {
   
                    playerValue.HadCardsLibrary[dstControl.index] = temp;
                }
                else
                {
                    Debug.Log("don't put null cards in library plz!");
                }
            }
        }
        else
        {
            //case 3 : card 1 is in library, card 2 is in deck
            if (dstControl.location == MenuCardControl.CardLocation.Deck)
            {
                if (temp2 != null)
                {
                    playerValue.HadCardsLibrary[srcControl.index] = temp2;
                }
                else
                {
                    Debug.Log("don't put null cards in library plz!");
                }
                    playerValue.Decks[playerValue.GetActiveDeckIndex()][dstControl.index] = temp;
            }
            // case 4: card 1 is in library, card 2 is in library
            else
            {
                playerValue.HadCardsLibrary[srcControl.index] = temp2;
                playerValue.HadCardsLibrary[dstControl.index] = temp;
            }
        }
        srcControl.index = dstControl.index;
        dstControl.index = tmpIndex;
        //refresh current deck
        playerValue.setActiveDeck(playerValue.GetActiveDeckIndex());
    }
}
