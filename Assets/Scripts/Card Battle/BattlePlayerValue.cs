using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePlayerValue : MonoBehaviour
{
    public static BattlePlayerValue Instance { get; private set; }
    private BattleCardControl selBattleCardControl;
    public BattleCardControl WeaponCardControl;
    public BattleCardControl CardControlPrefab;
    public Transform CardZone;
    public List<CardValue> BattleCards = new List<CardValue>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetBattlePlayerValue(PlayerValue playerValue)
    {
        BattleCards = playerValue.EquipmentCards;
        SetWeapon(playerValue.EquipmentWeapon);
        GenerateBattleCards();
    }


    void SetWeapon(WeaponValue weaponValue)
    {
        WeaponCardControl.SetWeaponValue(weaponValue);
    }


    public void GenerateBattleCards()
    {
        foreach (Transform child in CardZone)
        {
            Destroy(child.gameObject);
        }

        int count = BattleCards.Count;
        if (count == 0) return;

        RectTransform parentRt = CardZone.GetComponent<RectTransform>();
        float parentWidth = parentRt.rect.width;

        RectTransform cardRT = CardControlPrefab.GetComponent<RectTransform>();
        float cardWidth = cardRT.rect.width;

        float defaultSpacing = 30f; 
        float spacing = defaultSpacing;

        float totalWidth = count * cardWidth + (count - 1) * spacing;
        if (totalWidth > parentWidth)
        {
            spacing = (parentWidth - count * cardWidth) / (count - 1);
            totalWidth = parentWidth; 
        }

        float startX = -totalWidth / 2 + cardWidth / 2;

        for (int i = 0; i < count; i++)
        {
            CardValue cardValue = BattleCards[i];

            BattleCardControl cardObj = Instantiate(CardControlPrefab, CardZone);
            cardObj.SetCardValue(cardValue);

            RectTransform rt = cardObj.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            float xPos = startX + i * (cardWidth + spacing);
            rt.anchoredPosition = new Vector2(xPos, 0f);
        }
    }





    public void SetSelBattleCardControl(BattleCardControl battleCardControl)
    {
        if (this.selBattleCardControl != null)
        {
            this.selBattleCardControl.CanceCardDetails();
            this.selBattleCardControl = null;
        }
        this.selBattleCardControl = battleCardControl;
        if (battleCardControl != null)
        {
            battleCardControl.ShowCardDetails();

        }

    }


}
