using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuCardControl : MonoBehaviour
{
    public Image CardImage;
    public TextMeshProUGUI CardName;
    public TextMeshProUGUI CardDescription;


    private WeaponValue weaponValue;
    private CardValue cardValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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

}
