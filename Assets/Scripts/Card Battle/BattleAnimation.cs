using System.Collections;
using UnityEngine;

public class BattleAnimation : MonoBehaviour
{
    [SerializeField] private GameObject playerEffectPrefab;
    Vector3 offset = new Vector3(1.0f, 1.5f, 0);
    BattleAnimation Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public IEnumerator PlayCardAnimation(string cardName, Animator playerAnim, Animator enemyAnim)
    {
        playerAnim.SetTrigger("UseItem");
        yield return new WaitForSeconds(0.5f); // delay before effect starts
        if (playerEffectPrefab != null)
        {
            GameObject effect = Instantiate(playerEffectPrefab, playerAnim.transform.position+offset, Quaternion.identity);
            BattleItemSpriteEffect spriteEffect = effect.GetComponent<BattleItemSpriteEffect>();
            if (spriteEffect != null)
            {
                string path = $"Sprite/Card/SupportItems/{cardName}/{cardName}";
                spriteEffect.sprite = Resources.Load<Sprite>(path);
            }
        }
        switch (cardName)
        {
            case "Adrenal Medkit":
                yield return StartCoroutine(AdrenalMedkitAnimation(playerAnim, enemyAnim));
                break;
            case "Antidote Potion":
                yield return StartCoroutine(AntidotePotionAnimation(playerAnim, enemyAnim));
                break;
            case "Bandage":
                yield return StartCoroutine(BandageAnimation(playerAnim, enemyAnim));  
                break;
            case "Reflex Tonic":
                yield return StartCoroutine(ReflexTonicAnimation(playerAnim, enemyAnim));
                break;
            case "Berserker Wrap":
                yield return StartCoroutine(BerserkerWrapAnimation(playerAnim, enemyAnim));
                break;
            case "Boosted Buzz":
                yield return StartCoroutine(BoostedBuzzAnimation(playerAnim, enemyAnim));
                break;
            case "Combat Patch":
                yield return StartCoroutine(CombatPatchAnimation(playerAnim, enemyAnim));
                break;
            case "Stamina Capsule":
                yield return StartCoroutine(StaminaCapsuleAnimation(playerAnim, enemyAnim));
                break;
            case "Energy Potion":
                yield return StartCoroutine(EnergyPotionAnimation(playerAnim, enemyAnim));
                break;
            case "Field Surgery Kit":
                yield return StartCoroutine(FieldSurgeryKitAnimation(playerAnim, enemyAnim));
                break;
            case "Health Potion":
                yield return StartCoroutine(HealthPotionAnimation(playerAnim, enemyAnim));
                break;
            case "Liquid Courage Kit":
                yield return StartCoroutine(LiquidCourageKitAnimation(playerAnim, enemyAnim));
                break;
            case "Medkit":
                yield return StartCoroutine(MedkitAnimation(playerAnim, enemyAnim));
                break;
            case "Phoenix Shot":
                yield return StartCoroutine(PhoenixShotAnimation(playerAnim, enemyAnim));
                break;
            case "Emergency Capsule":
                yield return StartCoroutine(EmergencyCapsuleAnimation(playerAnim, enemyAnim));
                break;
            case "Fury Catalyst":
                yield return StartCoroutine(FuryCatalystAnimation(playerAnim, enemyAnim));
                break;
            case "Rapid Recovery Injector":
                yield return StartCoroutine(RapidRecoveryInjectorAnimation(playerAnim, enemyAnim));
                break;
            case "Revival Serum":
                yield return StartCoroutine(RevivalSerumAnimation(playerAnim, enemyAnim));
                break;
            case "Stimulant Wrap":
                yield return StartCoroutine(StimulantWrapAnimation(playerAnim, enemyAnim));
                break;
            case "Syringe":
                yield return StartCoroutine(SyringeAnimation(playerAnim, enemyAnim));
                break;
            default:
                Debug.LogWarning($"Unknown card name: {cardName}");
                break;
        }
    }
    public IEnumerator AdrenalMedkitAnimation(Animator playerAnim, Animator enemyAnim)
    {
        
        Debug.Log("Playing Adrenal Medkit animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator AntidotePotionAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Antidote Potion animation"); 
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator BandageAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Bandage animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator ReflexTonicAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Reflex Tonic animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator BerserkerWrapAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Berserker Wrap animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator BoostedBuzzAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Boosted Buzz animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator CombatPatchAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Combat Patch animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator StaminaCapsuleAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Stamina Capsule animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator EnergyPotionAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Energy Potion animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator FieldSurgeryKitAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Field Surgery Kit animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator HealthPotionAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Health Potion animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator LiquidCourageKitAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Liquid Courage Kit animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator MedkitAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Medkit animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator PhoenixShotAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Phoenix Shot animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator EmergencyCapsuleAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Emergency Capsule animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator FuryCatalystAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Fury Catalyst animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator RapidRecoveryInjectorAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Rapid Recovery Injector animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator RevivalSerumAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Revival Serum animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator StimulantWrapAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Stimulant Wrap animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }

    public IEnumerator SyringeAnimation(Animator playerAnim, Animator enemyAnim)
    {
        Debug.Log("Playing Syringe animation");
        yield return new WaitForSeconds(0.5f); // delay before animation starts
    }
}
