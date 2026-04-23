// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng
// No external source was used

using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattlePlayerUIManager : MonoBehaviour
{
    public static BattlePlayerUIManager Instance { get; private set; }


    private BattleCardControl selBattleCardControl;
    public BattleCardControl WeaponCardControl;
    public BattleCardControl CardControlPrefab;
    public Transform CardZone;
    private Dictionary<CardValue, BattleCardControl> cardUIMap = new Dictionary<CardValue, BattleCardControl>();


    [Header("Health UI")]
    [SerializeField] public HealthUI healthUI;
    [System.Serializable]
    public struct HealthUI
    {
        public TextMeshProUGUI HealthText;
        public Slider HealthSlider;

    }

    private BattlePlayerValue playerValue;


    private void Awake()
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
        
    }


    public void SetPlayer(BattlePlayerValue value)
    {
        playerValue = value;

        Listener(true);

        SetWeapon();
        GenerateBattleCards();

        UpdateMaxHealthUI(playerValue.MaxHealth);
        UpdateHealthUI(playerValue.Health);
    }


    void Listener(bool isAdd)
    {
        if (playerValue != null)
        {
            playerValue.HealthListener(UpdateHealthUI, isAdd);
            playerValue.MaxHealthListener(UpdateMaxHealthUI, isAdd);
        }

    }


    private void OnDestroy()
    {
        Listener(false);
    }

    private void UpdateHealthUI(int health)
    {
        if (healthUI.HealthSlider != null)
            healthUI.HealthSlider.value = health;

        if (healthUI.HealthText != null)
            healthUI.HealthText.text = $"{health}/{playerValue.MaxHealth}";
    }

    private void UpdateMaxHealthUI(int maxHealth)
    {
        if (healthUI.HealthSlider != null)
            healthUI.HealthSlider.maxValue = maxHealth;
    }

    void SetWeapon()
    {

        WeaponCardControl.SetWeaponValue(playerValue.GetWeapon());
    }

    public void GenerateBattleCards()
    {
        ClearAllCardUI();
        List<CardValue> cards = playerValue.GetBattleCards();
        CreateCardObjects(cards);
        LayoutCards();
    }

    public void AddNewCard(CardValue newCard)
    {
        if (newCard == null) return;

        BattleCardControl cardObj = CreateCardObject(newCard);

        cardUIMap[newCard] = cardObj;
        LayoutCards();
    }

    public void RemoveCard(CardValue removeCard)
    {
        if (removeCard == null) return;

        if (cardUIMap.TryGetValue(removeCard, out BattleCardControl cardObj))
        {
            Destroy(cardObj.gameObject);    
            cardUIMap.Remove(removeCard); 
        }
        LayoutCards();
    }


    private BattleCardControl CreateCardObject(CardValue card)
    {
        BattleCardControl cardObj = Instantiate(CardControlPrefab, CardZone);
        cardObj.SetCardValue(card);

        RectTransform rt = cardObj.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        cardUIMap[card] = cardObj;

        return cardObj;
    }

    public void ClearAllCardUI()
    {
        foreach (Transform child in CardZone)
        {
            Destroy(child.gameObject);
        }
        cardUIMap.Clear();
    }


    private void CreateCardObjects(List<CardValue> cardList)
    {
        foreach (var card in cardList)
        {
            CreateCardObject(card);
        }
    }


    private void LayoutCards()
    {
        //int count = cardUIMap.Count;
        List<CardValue> cardOrder = playerValue.GetBattleCards();
        int count = cardOrder.Count;
        if (count == 0) return;

        RectTransform parentRt = CardZone.GetComponent<RectTransform>();
        float parentWidth = parentRt.rect.width;

        RectTransform cardRT = CardControlPrefab.GetComponent<RectTransform>();
        float cardWidth = cardRT.rect.width;

        float defaultSpacing = 30f;
        float spacing = defaultSpacing;

        float totalWidth = count * cardWidth + (count - 1) * spacing;
        if (totalWidth > parentWidth && count > 1)
        {
            spacing = (parentWidth - count * cardWidth) / (count - 1);
            totalWidth = parentWidth;
        }

        float startX = cardWidth / 2;

        //foreach (var kvp in cardUIMap)
        for (int i = 0; i < cardOrder.Count; i++)
        {
            CardValue card = cardOrder[i];

            if (!cardUIMap.TryGetValue(card, out BattleCardControl cardObj))
                continue;

            RectTransform rt = cardObj.GetComponent<RectTransform>();
            //RectTransform rt = kvp.Value.GetComponent<RectTransform>();
            float xPos = startX + i * (cardWidth + spacing);
            rt.anchoredPosition = new Vector2(xPos, 0f);
            cardObj.UpdateOriginalPosition();
            //i++;
        }
    }



    public void SetSelBattleCardControl(BattleCardControl battleCardControl)
    {
        if (this.selBattleCardControl != null)
        {
            this.selBattleCardControl.CanceCardDetails();
            this.selBattleCardControl = null;
        }
        this.selBattleCardControl = battleCardControl;
        if (battleCardControl != null)
        {
            battleCardControl.ShowCardDetails();

        }

    }
    public void HideAllCards()
    {
        if (CardZone != null)
        {
            CardZone.gameObject.SetActive(false);
        }

        if (WeaponCardControl != null)
        {
            WeaponCardControl.gameObject.SetActive(false);
        }

        foreach(var card in FindObjectsOfType<BattleCardControl>())
        {
            card.gameObject.SetActive(false);
        }
    }
}
