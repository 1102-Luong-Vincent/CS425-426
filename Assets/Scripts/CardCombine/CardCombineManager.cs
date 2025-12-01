using UnityEngine;
using UnityEngine.UI;

public class CardCombineManager : MonoBehaviour
{
    public static CardCombineManager Instance;

    [Header("UI Slots")]
    public CardSlotUI FirstCardSlot;
    public CardSlotUI SecondCardSlot;
    public CardSlotUI ResultSlot;

    [Header("Buttons")]
    public Button CombineButton;
    public Button ClearButton;

    private CardValue FirstCardSelect;
    private CardValue SecondCardSelect;
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
    }

    // Called by card-click logic from bottom inventory
    public void SelectCard(CardValue card)
    {
        if (card == null) return;

        // If first slot empty → fill it
        if (FirstCardSelect == null)
        {
            FirstCardSelect = card;
            FirstCardSlot.SetCard(card);
            return;
        }

        // If second slot empty → fill it
        if (SecondCardSelect == null)
        {
            SecondCardSelect = card;
            SecondCardSlot.SetCard(card);
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
        if (FirstCardSelect == null || SecondCardSelect == null)
        {
            Debug.LogWarning("[Combine] Missing input cards.");
            return;
        }

        Debug.Log($"[Combine] Trying: {FirstCardSelect.CardName} + {SecondCardSelect.CardName}");

        // Use your CardCombinations system
        CardValue result = CardCombinations.Instance.Combine(FirstCardSelect, SecondCardSelect);

        if (result != null)
        {
            Debug.Log("[Combine] SUCCESS → " + result.CardName);
            ResultSlot.SetCard(result);

            // OPTIONAL: Add resulting card to inventory here later
        }
        else
        {
            Debug.Log("[Combine] FAILED");
            ResultSlot.ShowFailure();
        }
    }

    public void ClearAllSlots()
    {
        FirstCardSelect = null;
        SecondCardSelect = null;

        FirstCardSlot.Clear();
        SecondCardSlot.Clear();
        ResultSlot.Clear();
    }

}
