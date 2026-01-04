// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng
// No external source was used

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSlotUI : CardUIBase
{
    [SerializeField] Image HightLightImage;
    [SerializeField] Button slotButton;
    private CardValue currentCard;

    private void Awake()
    {
        HightLightImage.gameObject.SetActive(false);
        slotButton.onClick.AddListener(OnSlotButtonClick);
    }

    void OnSlotButtonClick()
    {
        CardCombineManager.Instance.SelectCard(this);

    }

    public void HighLightCard(bool highLight)
    {
        HightLightImage.gameObject.SetActive(highLight);
    }

    public void SetCard(CardValue card)
    {
        currentCard = card;

        if (card == null)
        {
            Clear();
            return;
        }
        base.SetCardUI(card);
        cardImage.color = Color.white;

    }

    public override void Clear()
    {
        base.Clear();
        HightLightImage.gameObject.SetActive(false);
    }


    public CardValue GetCardValue() { 
        return currentCard;
    }

}
