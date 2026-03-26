// Author: Vincent Luong
// Created by: Vincent Luong
// Modified by: Vincent Luong
// No external source was used.

using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
public class InteractableNotification : MonoBehaviour
{
    public static InteractableNotification Instance;

    [Header("Notification")]
    [SerializeField] public TextMeshProUGUI notificationText;
    [SerializeField] public float displayDuration = 3f; //displays for 3 seconds before fading out
    [SerializeField] public Image pickupIcon;
    [SerializeField] public float fadeDuration = 2f;

    [Header("Resource Drops")]
    [SerializeField] public TextMeshProUGUI resourceText;
    [SerializeField] public Image resourceIcon;

    private Coroutine Notification;
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        notificationText.gameObject.SetActive(false);
        pickupIcon.gameObject.SetActive(false);
        resourceText.gameObject.SetActive(false);
        resourceIcon.gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void ShowNotification(string itemName, Sprite icon = null)
    {
        if (Notification != null)
            StopCoroutine(Notification);

        //StopAllCoroutines();
        notificationText.text = itemName + " x1";

        notificationText.alpha = 1f;
        notificationText.gameObject.SetActive(true);

        if (icon != null)
        {
            pickupIcon.sprite = icon;

            Color c = pickupIcon.color;
            c.a = 1f;
            pickupIcon.color = c;

            pickupIcon.gameObject.SetActive(true);
        }
        else
        {
            pickupIcon.gameObject.SetActive(false);
        }
            //notificationText.gameObject.SetActive(true);
            Notification = StartCoroutine(HideAfterDuration());
    }

    public void ShowResourceNotification(string resourceName, Sprite icon = null, int amount = 1)
    {
        if (Notification != null)
            StopCoroutine(Notification);

        resourceText.text = $"{resourceName} x{amount}";
        resourceText.alpha = 1f;
        resourceText.gameObject.SetActive(true);

        if (icon != null)
        {
            resourceIcon.sprite = icon;
            Color c = resourceIcon.color;
            c.a = 1f;
            resourceIcon.color = c;
            resourceIcon.gameObject.SetActive(true);
        }
        else
        {
            resourceIcon.gameObject.SetActive(false);
        }

        Notification = StartCoroutine(HideResourceAfterDuration());
    }

    private IEnumerator HideAfterDuration()
    {
        //yield return new WaitForSeconds(displayDuration);
        //notificationText.gameObject.SetActive(false);
        //pickupIcon.gameObject.SetActive(false);

        yield return new WaitForSeconds(displayDuration);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            notificationText.alpha = alpha;

            if (pickupIcon != null)
            {
                Color c = pickupIcon.color;
                c.a = alpha;
                pickupIcon.color = c;
            }

            yield return null;
        }

        notificationText.gameObject.SetActive(false);
        pickupIcon.gameObject.SetActive(false);
    }

    private IEnumerator HideResourceAfterDuration()
    {
        yield return new WaitForSeconds(displayDuration);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            resourceText.alpha = alpha;

            if (resourceIcon != null)
            {
                Color c = resourceIcon.color;
                c.a = alpha;
                resourceIcon.color = c;
            }

            yield return null;
        }

        resourceText.gameObject.SetActive(false);
        resourceIcon.gameObject.SetActive(false);
    }
}