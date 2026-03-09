using System.Collections;
using System.Security.Policy;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InventoryTests
{


    [Test]

    public void InventoryTestsInstantiateMenuCard()
    {

        
        GameObject uiControl = new GameObject();
        InventoryUIControl ui = uiControl.AddComponent<InventoryUIControl>();

        GameObject CardZone = new GameObject();

        GameObject EquipZone = new GameObject();

        GameObject WeaponZone = new GameObject();


        ui.CardZone = CardZone.transform;
        ui.EquipZone = EquipZone.transform;
        ui.WeaponZone = WeaponZone.transform;

        GameObject testprefab = new GameObject();

        ui.MenuCardPrefab = testprefab;
        ui.EmptySlotPrefab = testprefab;

        CardValue testCard = new CardValue("test card", null, CardType.SupportItems, "test card", CardRarity.Rare, CardAbility.Healing, 0);

        // attempt to instantiate a menu card with separate transforms
        ui.InstantiateMenuCard(testCard, CardZone.transform, InventoryConstants.EquipCard,0);
        ui.InstantiateMenuCard(testCard, EquipZone.transform, InventoryConstants.EquipCard,0);
        ui.InstantiateMenuCard(testCard, WeaponZone.transform, InventoryConstants.EquipCard, 0);


        Assert.AreEqual(CardZone.transform.childCount, 1);
        Assert.AreEqual(EquipZone.transform.childCount, 1);
        Assert.AreEqual(WeaponZone.transform.childCount, 1);
    }
    [Test]
    public void InventoryTestsTooManyCards()
    {


        GameObject uiControl = new GameObject();
        InventoryUIControl ui = uiControl.AddComponent<InventoryUIControl>();

        GameObject CardZone = new GameObject();

        GameObject EquipZone = new GameObject();

        GameObject WeaponZone = new GameObject();


        ui.CardZone = CardZone.transform;
        ui.EquipZone = EquipZone.transform;
        ui.WeaponZone = WeaponZone.transform;

        GameObject testprefab = new GameObject();

        ui.MenuCardPrefab = testprefab;
        ui.EmptySlotPrefab = testprefab;

        CardValue testCard = new CardValue("test card", null, CardType.SupportItems, "test card", CardRarity.Rare, CardAbility.Healing, 0);

        // attempt to flood a transform with more than 20 cards. It should stop accepting cards after 20
        for(int i = 0; i < 25; i++)
        {
            ui.InstantiateMenuCard(testCard, CardZone.transform, InventoryConstants.EquipCard, 0);
        }


        Assert.AreEqual(CardZone.transform.childCount, 20);

    }


}
