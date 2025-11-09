using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TCG_CardMaker
{
    public class CardView : MonoBehaviour
    {

        [Header("Card Details")]
        [SerializeField] private TextMeshProUGUI txtTitle;
        [SerializeField] private TextMeshProUGUI txtDescription;
        [SerializeField] private TextMeshProUGUI txtCost;
        [SerializeField] private TextMeshProUGUI txtCardType;
        [SerializeField] private Image imgBorder;
        [SerializeField] private Image imgArt;

        [SerializeField] public CardSO cardData;

        public void SetData(CardSO card)
        {
            this.cardData = card;
            UpdateCardUI();
        }

        public void UpdateCardUI()
        {
            if (cardData == null)
                return;

            txtTitle.text = cardData.Title;
            txtCardType.text = cardData.Type.ToString();
            txtDescription.text = SpecialWords.Instance.GetSpecialWordsFormat(cardData.Description);
            txtCost.text = cardData.Cost.ToString();
            imgBorder.sprite = cardData.Border;
            imgArt.sprite = cardData.Art;
        }

       

        private void OnValidate()
        {
            UpdateCardUI();
        }
    }
}
