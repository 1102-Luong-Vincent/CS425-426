using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSlotUI : MonoBehaviour
{
    public Image cardImage;
    public TextMeshProUGUI cardNameText;

    private CardValue currentCard;

    public void SetCard(CardValue card)
    {
        currentCard = card;

        if (card == null)
        {
            Clear();
            return;
        }

        cardNameText.text = card.CardName;

        // If you have card art:
        cardImage.sprite = card.CardSprite;
        cardImage.color = Color.white;
    }

    public void ShowFailure()
    {
        currentCard = null;
        cardNameText.text = "Failed!";
        cardImage.color = Color.red;
        cardImage.sprite = null;
    }

    public void Clear()
    {
        currentCard = null;
        cardNameText.text = "";
        cardImage.sprite = null;
        cardImage.color = new Color(1, 1, 1, 0.2f); // faded
    }
}
