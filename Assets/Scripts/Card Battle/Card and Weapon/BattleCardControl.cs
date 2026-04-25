// Authors: Vincent Luong and Shawn Meng
// Created by: Shawn Meng
// Modified by: Vincent Luong
// Some code generated with assistance from ChatGPT.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleCardControl : CardUIBase, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    private WeaponValue weaponValue;
    private CardValue cardValue;

    private Vector2 originalAnchoredPos;
    private Vector3 originalScale;
    private float hoverOffset = 200f;
    private Transform originalPosition; //allows the card to be centered at the center of the canvas screen, instead of the cardZone. 

    private bool isCentered = false;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip cardUseSound;

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

        audioSource.PlayOneShot(hoverSound);

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

        RectTransform rt = GetComponent<RectTransform>();

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            StartCoroutine(UseCard());
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
        originalPosition = rt.parent; //saves the original state 

        Canvas canvas = GetComponentInParent<Canvas>();
        rt.SetParent(canvas.transform, true);

        rt.SetAsLastSibling();

        RectTransform cardZoneRT = BattlePlayerUIManager.Instance.CardZone.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        //rt.anchoredPosition = new Vector2(0f, -cardZoneRT.anchoredPosition.y);
        //rt.anchoredPosition = Vector2.zero;
        rt.anchoredPosition = new Vector2(0f, 120f); //change this value to adjust how far up the card is when right clicking; lower value = lower on screen.
        rt.localScale = originalScale * 1f; //change the 1f if you want to make the card bigger when righ clicking. 
        isCentered = true;

    }

    public void CanceCardDetails()
    {
        RectTransform rt = GetComponent<RectTransform>();

        rt.SetParent(originalPosition, true);
        rt.anchoredPosition = originalAnchoredPos;
        rt.localScale = originalScale;
        isCentered = false;
    }

    public IEnumerator UseCard()
    {
        //Debug.Log($"Used card: {cardValue.CardName}");
        //cardValue.UseEffect(BattlePlayerValue.Instance);
        //BattlePlayerValue.Instance.RemoveCard(cardValue);
        //BattleManage.Instance.StartNextTurn();

        if (!BattleManage.Instance.IsPlayerTurn())
        {
            yield break;
        }

        if (BattleManage.Instance.IsPlayerActionLocked())
        {
            yield break;
        }

        if (BattleManage.Instance.BattleOver())
        {
            yield break;
        }

        if (cardValue != null)
            Debug.Log($"Used card: {cardValue.CardName}");

        else if(weaponValue != null)

            Debug.Log($"Used weapon: {weaponValue.WeaponName}");
        // 1. Pick first enemy for now (simple targeting)
        var target = BattleEnemyManager.Instance.currentEnemys.Count > 0
                ? BattleEnemyManager.Instance.currentEnemys[0]
                : null;

        if (target == null)
        {
            Debug.LogWarning("No enemy to target.");
            yield break;
        }

        BattleManage.Instance.SetPlayerActionLocked(true);

        // 2. Check if this card is an attack card (Knife)
        if (weaponValue != null)
        {
            Animator anim = BattlePlayerValue.Instance.getAnimator();
            anim.SetTrigger("Attack");
            yield return new WaitForSeconds(0.7f);
            UseWeaponEffect(BattlePlayerValue.Instance, BattleEnemyManager.Instance.GetEnemyBattleControls());
        }
        else
        {
            BattleAnimation ba = BattleManage.Instance.GetBattleAnimation();
            yield return StartCoroutine(ba.PlayCardAnimation(cardValue.CardName, BattlePlayerValue.Instance.getAnimator(), target.GetAnimator()));
            cardValue.UseEffect(BattlePlayerValue.Instance, BattleEnemyManager.Instance.GetEnemyValues());
        }
        audioSource.PlayOneShot(cardUseSound);

        // 3. Remove card from player's hand
        BattlePlayerValue.Instance.RemoveCard(cardValue);

        // 4. Next turn
        BattleManage.Instance.StartNextTurn();

        yield return new WaitForSeconds(0.5f);
    }

    

    void UseWeaponEffect(BattlePlayerValue playerValue, List<EnemyBattleControl> targets)
    {
        weaponValue.UseWeaponEffect(playerValue, targets);
    }


    public CardValue GetCardValue()
    {
        return cardValue;
    }

    public WeaponValue GetWeaponValue()
    {
        return weaponValue;
    }

    public void UpdateOriginalPosition() // called when cards are rearranged
    {
        RectTransform rt = GetComponent<RectTransform>();
        originalAnchoredPos = rt.anchoredPosition;
    }
}
