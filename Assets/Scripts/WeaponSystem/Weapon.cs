using Unity.VisualScripting;
using UnityEngine;

public enum WeaponType
{
    Knife,
    Pistol,
    Shotgun
}

public enum UniqueAbilityType
{
    None,
    KnifeDoubleHit,
    PistolPiercing,
    ShotgunDismemberment
}
public class Weapon
{
    public string weaponName;
    public WeaponType type;
    public UniqueAbilityType uniqueAbilityType = UniqueAbilityType.None;

    public int weaponDamage;
    public int level;
    public int maxLevel = 3;
    public int attackSinceLastAbility = 0;
    public int attackRequiredForAbility = 3; //initial starting point
    public int incrementAttackForAbility = 3; //increases attack use before ability can be used again
    

    public float attackSpeed;
    
    public bool uniqueAbility = false;

    public Weapon(string name, WeaponType type, int damage, int level, int maxLevel, float speed, bool uniqueAbility)
    {
        this.weaponName = name;
        this.type = type;   
        this.weaponDamage = damage;
        this.level = 1;
        this.maxLevel = maxLevel;
        this.attackSpeed = speed;
        this.uniqueAbility = false;
    }

    public void Upgrade()
    {
        if(level >= maxLevel)
        {
            Debug.Log($"{weaponName} is already max level!" );
            return;
        }
        level++;

        weaponDamage = Mathf.RoundToInt(weaponDamage * (1 + 0.1f * level)); //damage increases by 10% per level
        Debug.Log($"{weaponName} has leveled up to {level}, damage increased: {weaponDamage}");
    }

    public void CountAttack()
    {
        attackSinceLastAbility++;

        if (attackSinceLastAbility >= attackRequiredForAbility) 
        {
            uniqueAbility = true;
            Debug.Log($"{weaponName} is ready for use");
        }
    }

    //public void UniqueAbility(List<Enemy> targets) //ability effects. not yet active.
    //{
    //    if (!uniqueAbility)
    //    {
    //        Debug.Log("Ability is not ready yet");
    //        return;
    //    }

    //    switch (uniqueAbilityType)
    //    {
    //        case UniqueAbilityType.KnifeDoubleHit:
    //            if(targets.Count > 0)
    //            {
    //                targets[0].TakeDamage(weaponDamage);
    //                targets[0].TakeDamage(weaponDamage);
    //            }
    //            break;

    //        case UniqueAbilityType.PistolPiercing:
    //            foreach(Enemy enemy in targets)
    //            {
    //                enemy.TakeDamage(weaponDamage);
    //                Debug.Log($"{weaponName} pierced through multiple enemies");
    //            }
    //            break;

    //        case UniqueAbilityType.ShotgunDismemberment:
    //            if(targets.Count > 0)
    //            {
    //                targets[0].TakeDamage(weaponDamage);
    //                targets[0].ApplyStatusEffect(StatusEffect.Weakened, 1);

    //            }
    //            Debug.Log($"{weaponName} maimed enemies!");
    //            break;

    //        case UniqueAbilityType.None:
    //        default:
    //            Debug.Log("No unique ability assigned.");
    //            break;
    //    }
    //    Debug.Log($"{weaponName} has used its unique ability");

    //    attackSinceLastAbility = 0;
    //    attackRequiredForAbility += incrementAttackForAbility;
    //    uniqueAbility = false;
    //}
}
