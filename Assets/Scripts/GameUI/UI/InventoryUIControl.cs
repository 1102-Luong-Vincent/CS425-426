using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static ButtonEffect;
using static PlayerMenuManager;

public class InventoryUIControl : MonoBehaviour
{
    public Transform WeaponZone;
    public Transform EquipZone;
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
        public Button SortAZButton;
        public Button SortZAButton;
    }

    PlayerValue playerValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        onInventoryOpen();
        InitButtons();
    }

    public void onInventoryOpen()
    {
        if (playerValue == null) playerValue = GameValue.Instance.GetPlayerValue();
        HealthText.text = playerValue.GetHealth().ToString();
        EnergyText.text = playerValue.GetEnergy().ToString();
        CardsText.text = playerValue.HadCardsLibrary.Count.ToString();

        //put weapon card in card zone
        GameObject weapon = Instantiate(MenuCardPrefab);
        WeaponValue wval = playerValue.EquipmentWeapon;
        weapon.name = (wval.WeaponName + " card");
        MenuCardControl wmenucard = weapon.GetComponent<MenuCardControl>();
        weapon.transform.SetParent(WeaponZone);
        wmenucard.SetWeaponValue(wval);

        int EquipCardCount = 0;
        // populate card zone with starting hand
        foreach (CardValue val in playerValue.EquipmentCards)
        {
            InstantiateCard(val, EquipZone);
            EquipCardCount++;
        }
        // populate rest of equip zone with empty slots
        for (int i = EquipCardCount; i < 5; i++)
        {
            GameObject emptySlot = Instantiate(EmptySlotPrefab);
            emptySlot.transform.SetParent(EquipZone);
            emptySlot.tag = "EquipCard";
        }

        int BattleCardCount = 0;
        // populate card zone with cards in PlayerValue
        foreach (CardValue val in playerValue.battleCardsList)
        {
            if(BattleCardCount >= playerValue.GetMaxCards()-EquipCardCount - 1) // total = 1 weapon card + # of equip cards
            {
                break;
            }
            InstantiateCard(val, CardZone);
            BattleCardCount++;
        }

        // populate rest of card zone with empty slots
        for(int i = BattleCardCount; i < playerValue.GetMaxCards()-6; i++)
        {
                GameObject emptySlot = Instantiate(EmptySlotPrefab);
                emptySlot.transform.SetParent(CardZone);
                emptySlot.tag = "BattleCard";
        }
    }

    public void onInventoryClose()
    {
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

    public void refreshInventory()
    {
        onInventoryClose();
        onInventoryOpen();
    }

    void InitButtons()
    {
        OnGameMenuButtonClick(buttons.SortAZButton, OnSortAZButtonClick);
        OnGameMenuButtonClick(buttons.SortZAButton, OnSortZAButtonClick);
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


    void InstantiateCard(CardValue val, Transform dest)
    {
        GameObject card = Instantiate(MenuCardPrefab);
        card.name = (val.CardName + " card");
        MenuCardControl menucard = card.GetComponent<MenuCardControl>();
        card.transform.SetParent(dest);
        menucard.SetCardValue(val);
        
        if(dest.gameObject == CardZone.gameObject)
        {
            card.tag =  "BattleCard";
        }
        else if(dest.gameObject == EquipZone.gameObject)
        {
            card.tag = "EquipCard";
        }
    }
}
