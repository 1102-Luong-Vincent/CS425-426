using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class CardUIBase : MonoBehaviour
{
    [Header("Card UI Components")]
    [SerializeField] protected Image cardImage;
    [SerializeField] protected TextMeshProUGUI cardNameText;
    [SerializeField] protected TextMeshProUGUI cardDescriptionText;

    public virtual void SetCardUI(CardValue cardValue)
    {
        if (cardNameText != null)cardNameText.text = cardValue.CardName;
        if (cardDescriptionText != null)cardDescriptionText.text = cardValue.CardDescribe;
        if (cardImage != null)cardImage.sprite = cardValue.CardSprite;
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
