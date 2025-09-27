using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Weapon currentWeapon;
    public Weapon knife;
    public Weapon pistol;
    public Weapon shotgun;
    public Weapon none;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Weapon knife = new Weapon("Knife", WeaponType.Knife, 4, 1, 3, 3f, false);
        knife.uniqueAbilityType = UniqueAbilityType.KnifeDoubleHit;

        Weapon pistol = new Weapon("Pistol", WeaponType.Pistol, 10, 1, 3, 1.5f, false);
        pistol.uniqueAbilityType = UniqueAbilityType.PistolPiercing;

        Weapon shotgun = new Weapon("Shotgun", WeaponType.Shotgun, 20, 1, 3, 1f, false);
        shotgun.uniqueAbilityType = UniqueAbilityType.ShotgunDismemberment;

        currentWeapon = none;
    }

    public void UseCard()
    {
        currentWeapon.CountAttack();

        if (currentWeapon.uniqueAbility)
        {
            Debug.Log($"{currentWeapon.weaponName} unique ability is ready!");
        }
    }

    //public void UseUniqueAbility(List<Enemy> targets)
    //{
    //    currentWeapon.UniqueAbility(targets);
    //    //apply unique ability effect here
    //}
    
    public void SwitchWeapons(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
    }
}
