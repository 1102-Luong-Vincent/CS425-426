// Author: Sean Masterson
// Created by: Sean Masterson
// Modified by: Sean Masterson
// no external source was used.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MenuCardControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image CardImage;
    public TextMeshProUGUI CardName;
    public TextMeshProUGUI CardDescription;
    public GameObject CardSelectHighlight;


    private WeaponValue weaponValue;
    private CardValue cardValue;

    private Vector2 originalAnchoredPos;
    private Vector3 originalScale;
    private float hoverOffset = 200f;

    private bool isActive = true;
    private bool isCentered = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        }
        else if (weaponValue != null)
        {

            CardName.text = weaponValue.WeaponName;
            CardImage.sprite = weaponValue.WeaponSprite;
            CardDescription.text = weaponValue.WeaponDescribe;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isWeapon() && isActive)
            CardSelectHighlight.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CardSelectHighlight.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && !isWeapon() && isActive) // don't allow player to select weapon from inventory menu
        {
            SendMessageUpwards("CardClicked", this.gameObject);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            SendMessageUpwards("CardRightClicked", this.gameObject);
        }
    }


    public void ShowCardDetails()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.SetAsLastSibling();
        RectTransform cardZoneRT = BattlePlayerUIManager.Instance.CardZone.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0f, -cardZoneRT.anchoredPosition.y);
        //rt.localScale = originalScale * 2f;
       // isCentered = true;

    }

    public void CanceCardDetails()
    {
        RectTransform rt = GetComponent<RectTransform>();
       // rt.anchoredPosition = originalAnchoredPos;
       // rt.localScale = originalScale;
    }

    public CardValue GetCardValue()
    {
        return cardValue;
    }    

    public bool isWeapon()
    {
        return this.weaponValue != null;
    }

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }
}
