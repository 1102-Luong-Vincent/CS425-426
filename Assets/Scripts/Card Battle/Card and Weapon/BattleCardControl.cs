using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleCardControl : CardUIBase, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    private WeaponValue weaponValue;
    private CardValue cardValue;

    private Vector2 originalAnchoredPos;
    private Vector3 originalScale;
    private float hoverOffset = 200f;

    private bool isCentered = false;

    void Start()
    {
        RectTransform rt = GetComponent<RectTransform>();
        originalAnchoredPos = rt.anchoredPosition;
        originalScale = rt.localScale;
    }

    bool IsInCombineScene()
    {
        return SceneManager.GetActiveScene().name == "CombineScene";
    }
    public void SetCardValue(CardValue cardValue)
    {
        this.cardValue = cardValue;
        UpUIData();
    }

    public void SetWeaponValue(WeaponValue weaponValue)
    {
        this.weaponValue = weaponValue;
        UpUIData();
    }


    void UpUIData()
    {
        if (cardValue != null)
        {
            SetCardUI(cardValue);

        } else if (weaponValue != null) {
        
           base.cardNameText.text = weaponValue.WeaponName;
            base.cardImage.sprite = weaponValue.WeaponSprite;
            cardDescriptionText.text = weaponValue.WeaponDescribe;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (IsInCombineScene())
        //    return;

        if (!isCentered)
        {
            RectTransform rt = GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(originalAnchoredPos.x, originalAnchoredPos.y + hoverOffset);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (IsInCombineScene())
        //    return;

        if (!isCentered)
        {
            GetComponent<RectTransform>().anchoredPosition = originalAnchoredPos;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if (IsInCombineScene())
        //{
        //    if (eventData.button == PointerEventData.InputButton.Left && cardValue != null)
        //    {
        //        Debug.Log("[CombineScene] Selecting card: " + cardValue.CardName);
        //        CardCombineManager.Instance.SelectCard(cardValue);
        //    }
        //    return;
        //}

        RectTransform rt = GetComponent<RectTransform>();

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UseCard();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (!isCentered)
            {
                BattlePlayerUIManager.Instance.SetSelBattleCardControl(this);
             }
            else
            {
                BattlePlayerUIManager.Instance.SetSelBattleCardControl(null);
            }
        }
    }


    public void ShowCardDetails()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.SetAsLastSibling();
        RectTransform cardZoneRT = BattlePlayerUIManager.Instance.CardZone.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0f, -cardZoneRT.anchoredPosition.y);
        rt.localScale = originalScale * 2f;
        isCentered = true;

    }

    public void CanceCardDetails()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchoredPosition = originalAnchoredPos;
        rt.localScale = originalScale;
        isCentered = false;
    }

    void UseCard()
    {
        //Debug.Log($"Used card: {cardValue.CardName}");
        //cardValue.UseEffect(BattlePlayerValue.Instance);
        //BattlePlayerValue.Instance.RemoveCard(cardValue);
        //BattleManage.Instance.StartNextTurn();

        Debug.Log($"Used card: {cardValue.CardName}");

        // 1. Pick first enemy for now (simple targeting)
        var target = BattleEnemyManager.Instance.currentEnemys.Count > 0
            ? BattleEnemyManager.Instance.currentEnemys[0]
            : null;

        if (target == null)
        {
            Debug.LogWarning("No enemy to target.");
            return;
        }

        // 2. Check if this card is an attack card (Knife)
        if (cardValue.CardName == "Knife")
        {
            // Apply player card effect
            BattleManage.Instance.ApplyPlayerCardEffect(cardValue, target);
        }
        else
        {
            // All non-attack cards follow the original logic
            cardValue.UseEffect(BattlePlayerValue.Instance);
        }

        // 3. Remove card from player's hand
        BattlePlayerValue.Instance.RemoveCard(cardValue);

        // 4. Next turn
        BattleManage.Instance.StartNextTurn();

    }
}
