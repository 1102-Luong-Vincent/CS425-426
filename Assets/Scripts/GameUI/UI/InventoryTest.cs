using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using static ButtonEffect;
public class InventoryTest : MonoBehaviour
{
    Button fillButton;
    public InventoryUIControl control;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fillButton = GetComponent<Button>();
        OnGameMenuButtonClick(fillButton, OnFillButtonClick);
    }

    // Update is called once per frame
    void OnFillButtonClick()
    {
        string[] allCards = {
        "Bandage", "Syringe", "Medkit", "Revival Serum", "Pills", "Rage Pill",
        "Drugs", "Beer", "Health Potion", "Energy Potion", "Antidote Potion",
        "Field Surgery Kit", "Adrenal Medkit", "Combat Patch", "Berserker Wrap",
        "Stimulant Wrap", "Liquid Courage Kit", "Rapid Recovery Injector",
        "Phoenix Shot", "Boosted Buzz"
        };

        foreach (string cardName in allCards)
        {
            CardValue foundCard = GameValue.Instance.GetInitCardValue(cardName);
            if (foundCard != null)
            {
                GameValue.Instance.GetPlayerValue().HadCardsLibrary.Add(foundCard);
            }
            else
            {
                Debug.LogWarning($"Card {cardName} not found in GameValue library!");
            }
        }
        control.refreshInventory();
    }
}
