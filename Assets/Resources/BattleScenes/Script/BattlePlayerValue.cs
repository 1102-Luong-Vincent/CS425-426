using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePlayerValue : MonoBehaviour
{
    public static BattlePlayerValue Instance { get; private set; }
    public BattleCardControl CardControl;
    public Transform CardZone;
    public List<CardValue> BattleCards = new List<CardValue>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetBattlePlayerValue(PlayerValue playerValue)
    {
        BattleCards = playerValue.EquipmentCards;
        GenerateBattleCards();
    }


    public void GenerateBattleCards()
    {
        foreach (Transform child in CardZone)
        {
            Destroy(child.gameObject);
        }

        int count = BattleCards.Count;
        if (count == 0) return;

        RectTransform parentRt = CardZone.GetComponent<RectTransform>();
        float parentWidth = parentRt.rect.width;

        RectTransform cardRT = CardControl.GetComponent<RectTransform>();
        float cardWidth = cardRT.rect.width;

        float spacing = 0f;

        if (count > 1)
        {
            spacing = (parentWidth - cardWidth) / (count - 1); 
        }

        for (int i = 0; i < count; i++)
        {
            CardValue cardValue = BattleCards[i];

            BattleCardControl cardObj = Instantiate(CardControl, CardZone);
            cardObj.SetCardValue(cardValue);

            RectTransform rt = cardObj.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            float xPos = -parentWidth / 2 + cardWidth / 2 + i * spacing; 
            rt.anchoredPosition = new Vector2(xPos, 0f);
        }
    }


}
