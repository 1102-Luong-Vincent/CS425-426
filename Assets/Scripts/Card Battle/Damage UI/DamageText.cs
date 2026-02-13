using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public static DamageText Instance;

    [SerializeField] private TextMeshProUGUI damageText; // assign prefab in inspector
    public float displayDuration = 2f; // duration to display damage text
    public float timer;

    private void Awake()
    {
        Instance = this;
        damageText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (damageText.gameObject.activeSelf)
        {
            // simple float-up effect
            damageText.transform.position += Vector3.up * 20f * Time.deltaTime;

            // count down
            timer -= Time.deltaTime;
            if (timer <= 0f)
                damageText.gameObject.SetActive(false);
        }
    }
    public void ShowDamage(int amount, Transform target)
    {

        // set the damage amount
        damageText.text = amount.ToString();
        damageText.gameObject.SetActive(true);

        // reset timer
        timer = displayDuration;
    }
}
