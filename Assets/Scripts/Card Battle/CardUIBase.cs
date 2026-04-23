// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng
// No external source was used

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class CardUIBase : MonoBehaviour
{
    [Header("Card UI Components")]
    [SerializeField] protected Image cardImage;
    [SerializeField] protected Image cardBorder;
    [SerializeField] protected TextMeshProUGUI cardNameText;
    [SerializeField] protected TextMeshProUGUI cardDescriptionText;

    [SerializeField] protected Sprite commonBorder;
    [SerializeField] protected Sprite rareBorder;
    [SerializeField] protected Sprite veryRareBorder;
    [SerializeField] protected Sprite epicBorder;
    public virtual void SetCardUI(CardValue cardValue)
    {
        if (cardNameText != null)cardNameText.text = cardValue.CardName;
        if (cardDescriptionText != null)cardDescriptionText.text = cardValue.CardDescribe;
        if (cardImage != null)cardImage.sprite = cardValue.CardSprite;
        switch(cardValue.rarity)
        {
            case CardRarity.Common:
                if (cardBorder != null) cardBorder.sprite = commonBorder;
                break;
            case CardRarity.Rare:
                if (cardBorder != null) cardBorder.sprite = rareBorder;
                break;
            case CardRarity.VeryRare:
                if (cardBorder != null) cardBorder.sprite = veryRareBorder;
                break;
            case CardRarity.Epic:
                if (cardBorder != null) cardBorder.sprite = epicBorder;
                break;
            default:
                break;
        }
    }


    public virtual void ShowFailure()
    {
        cardNameText.text = "Failed!";
        cardImage.color = Color.red;
        cardImage.sprite = null;
    }

    public virtual void Clear()
    {
        cardNameText.text = "";
        cardImage.sprite = null;
        cardDescriptionText.text = "";
        cardImage.color = new Color(1, 1, 1, 0.2f); // faded
    }
}
