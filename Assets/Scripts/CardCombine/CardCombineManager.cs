// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng
// Some code generated with assistance from ChatGPT.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CardCombineManager : MonoBehaviour
{
    public static CardCombineManager Instance;

    [SerializeField] GameObject CardCombinePanel;

    [Header("UI Slots")]
    [SerializeField] CardUI FirstCard;
    CardSlotUI FirstSlot;
    [SerializeField] CardUI SecondCard;
    CardSlotUI SecondSlot;
    [SerializeField] CardUI ResultCard;

    [Header("Buttons")]
    [SerializeField] Button CombineButton;
    [SerializeField] Button ClearButton;


    [SerializeField] CardSlotUI slotPrefab;

    List<CardSlotUI> spawnedSlots = new List<CardSlotUI>();
    [SerializeField] ScrollRect slotList;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ClearAllSlots();

        if (CombineButton != null)
            CombineButton.onClick.AddListener(CombineCards);

        if (ClearButton != null)
            ClearButton.onClick.AddListener(ClearAllSlots);

        ClosePanel();
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) SwitchPanel();
    }


    void SwitchPanel()
    {
        if (!CardCombinePanel.activeSelf)
        {
            OpenPanel(); 
        } else
        {
            ClosePanel();
        }
    }



    public void OpenPanel()
    {
        CardCombinePanel.SetActive(true);

        List<CardValue> cardList = GameValue.Instance.GetPlayerValue().HadCardsLibrary;

        foreach (var card in cardList)
        {
            var slot = Instantiate(slotPrefab, slotList.content);
            slot.SetCard(card);  
            spawnedSlots.Add(slot);
        }
    }

    public void ClosePanel()
    {
        foreach (var slot in spawnedSlots)
        {
            if (slot != null)Destroy(slot.gameObject);
        }
        spawnedSlots.Clear();
        ClearAllSlots();
        CardCombinePanel.SetActive(false);
    }


    // Called by card-click logic from bottom inventory
    public void SelectCard(CardSlotUI cardSlot)
    {
        if (cardSlot == null) return;

        if (cardSlot == FirstSlot)
        {
            ClearFirstCard();
            UpdateResult();
            cardSlot.HighLightCard(false);
            return;
        }

        if (cardSlot == SecondSlot)
        {
            ClearSecondCard();
            UpdateResult();
            cardSlot.HighLightCard(false);
            return;
        }

        // First slot empty
        if (FirstSlot == null)
        {
            FirstSlot = cardSlot;
            FirstCard.SetCardUI(cardSlot.GetCardValue());
            cardSlot.HighLightCard(true);
            UpdateResult();
            return;
        }

        // Second slot empty
        if (SecondSlot == null)
        {
            SecondSlot = cardSlot;
            SecondCard.SetCardUI(cardSlot.GetCardValue());
            cardSlot.HighLightCard(true);
            UpdateResult();
            return;
        }

        Debug.Log("[CardCombine] Both slots full. Clear first.");
    }





    //public void SetFirstCardSlot(CardValue card)
    //{
    //    FirstCardSelect = card;
    //    FirstCardSlot.SetCard(card);
    //    TryAutoCombine();
    //}

    //public void SetSecondCardSlot(CardValue card)
    //{
    //    SecondCardSelect = card;
    //    SecondCardSlot.SetCard(card);
    //    TryAutoCombine();
    //}

    //private void TryAutoCombine()
    //{
    //    if (FirstCardSelect != null && SecondCardSelect != null)
    //    {
    //        CombineCards();
    //    }
    //}

    public void CombineCards()
    {
        if (FirstSlot == null || SecondSlot == null)
        {
            Debug.LogWarning("[Combine] Missing input cards.");
            return;
        }

        CardValue first = FirstSlot.GetCardValue();
        CardValue second = SecondSlot.GetCardValue();

        Debug.Log($"[Combine] Trying: {first.CardName} + {second.CardName}");

        CardValue result = CardCombinations.Instance.Combine(first, second);

        var player = GameValue.Instance.GetPlayerValue();

        if (result != null)
        {
            Debug.Log("[Combine] SUCCESS → " + result.CardName);

            player.HadCardsLibrary.Remove(first);
            player.HadCardsLibrary.Remove(second);
            player.HadCardsLibrary.Add(result);
            ResultCard.SetCardUI(result);
            ClearAllSlots();
            RefreshSlotList();

        }
        else
        {
            Debug.Log("[Combine] FAILED");

            ResultCard.ShowFailure();
            player.HadCardsLibrary.Remove(first);
            player.HadCardsLibrary.Remove(second);
            ClearAllSlots();
            RefreshSlotList();
        }
    }


    void RefreshSlotList()
    {
        foreach (var slot in spawnedSlots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }
        spawnedSlots.Clear();
        List<CardValue> cardList = GameValue.Instance.GetPlayerValue().HadCardsLibrary;

        foreach (var card in cardList)
        {
            var slot = Instantiate(slotPrefab, slotList.content);
            slot.SetCard(card);
            spawnedSlots.Add(slot);
        }
    }



    void UpdateResult()
    {
        if (FirstSlot == null || SecondSlot == null)
        {
            ResultCard.Clear();
            return;
        }

        CardValue result = CardCombinations.Instance.GetResultCard(
            FirstSlot.GetCardValue(),
            SecondSlot.GetCardValue()
        );

        ResultCard.SetCardUI(result);
    }


    public void ClearAllSlots()
    {
        ClearFirstCard();
        ClearSecondCard();
        ResultCard.Clear();
    }


    void ClearFirstCard()
    {
        FirstCard.Clear(); FirstSlot = null;
       
    }

    void ClearSecondCard()
    {
        SecondCard.Clear(); SecondSlot = null;
    }

}
