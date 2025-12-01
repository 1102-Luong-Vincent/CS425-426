using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ButtonEffect;
using static GameMenuControl;

public class InventoryUIControl : MonoBehaviour
{
    public Transform CardZone;
    public GameObject MenuCardPrefab;
    public GameObject EmptySlotPrefab;
    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI EnergyText;
    public TextMeshProUGUI CardsText;

    [Header("Buttons")]
    public Buttons buttons;

    [System.Serializable]
    public class Buttons
    {
        public Button WeaponButton;
        public Button CardsButton;
        public Button SortAZButton;
        public Button SortZAButton;
    }

    PlayerValue playerValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerValue = GameValue.Instance.GetPlayerValue();
        //playerValue.EquipmentCards = playerValue.battleCardsList;// for testing delete this later
        onInventoryOpen();
        InitButtons();
    }

    public void onInventoryOpen()
    {
        HealthText.text = playerValue.GetHealth().ToString();
        EnergyText.text = playerValue.GetEnergy().ToString();
        CardsText.text = playerValue.HadCardsLibrary.Count.ToString();
        GameObject weapon = Instantiate(MenuCardPrefab);
        WeaponValue wval = playerValue.EquipmentWeapon;
        weapon.name = (wval.WeaponName + " card");
        MenuCardControl wmenucard = weapon.GetComponent<MenuCardControl>();
        weapon.transform.SetParent(CardZone);
        wmenucard.SetWeaponValue(wval);
        // populate card zone with cards in PlayerValue
        for(int i = 0; i < 20; i++)
        {
            if (i <= playerValue.EquipmentCards.Count - 1)
            {
                GameObject card = Instantiate(MenuCardPrefab);
                CardValue val = playerValue.EquipmentCards[i];
                card.name = (val.CardName + " card");
                MenuCardControl menucard = card.GetComponent<MenuCardControl>();
                card.transform.SetParent(CardZone);
                menucard.SetCardValue(val);
            }
            else
            {
                GameObject emptySlot = Instantiate(EmptySlotPrefab);
                emptySlot.transform.SetParent(CardZone);
            }
        }
    }

    public void onInventoryClose()
    {
        // menu closed -- erase menu cards
        for (int i = CardZone.childCount - 1; i >= 0; i--)
        {
            GameObject menucard = CardZone.GetChild(i).gameObject;

            Destroy(menucard);
        }
    }

    void InitButtons()
    {
        OnGameMenuButtonClick(buttons.WeaponButton, OnWeaponButtonClick);
        OnGameMenuButtonClick(buttons.CardsButton, OnCardsButtonClick);
        OnGameMenuButtonClick(buttons.SortAZButton, OnSortAZButtonClick);
        OnGameMenuButtonClick(buttons.SortZAButton, OnSortZAButtonClick);
    }

    void OnWeaponButtonClick()
    {

    }

    void OnCardsButtonClick()
    {

    }

    void OnSortAZButtonClick()
    {
        SortCards("Alpha", "Ascending");
    }
    void OnSortZAButtonClick()
    {
        SortCards("Alpha", "Descending");
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
                    case "Ascending":
                        children = children.OrderBy(child => child.name).ToList();
                        break;
                    case "Descending":
                        children = children.OrderByDescending(child => child.name).ToList();
                        break;
                }


                for (int i = 0; i < children.Count; i++)
                {
                    children[i].SetSiblingIndex(i);
                }
                break;
            default:
                break;
        }
    }
    void UpdateGameValue()
    {
   
    }
}
