using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BattleCardControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image CardImage;
    public TextMeshProUGUI CardName;
    public TextMeshProUGUI CardDescription;

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
            CardName.text = cardValue.CardName;
            CardImage.sprite = cardValue.CardSprite;
            CardDescription.text = cardValue.CardDescribe;
        } else if (weaponValue != null) {
        
            CardName.text = weaponValue.WeaponName;
            CardImage.sprite = weaponValue.WeaponSprite;
            CardDescription.text = weaponValue.WeaponDescribe;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isCentered)
        {
            RectTransform rt = GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(originalAnchoredPos.x, originalAnchoredPos.y + hoverOffset);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isCentered)
        {
            GetComponent<RectTransform>().anchoredPosition = originalAnchoredPos;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
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
        Debug.Log($"Used card: {CardName.text}");

        BattlePlayerValue.Instance.UsedCard(cardValue);
        BattleManage.Instance.StartNextTurn();

    }
}
