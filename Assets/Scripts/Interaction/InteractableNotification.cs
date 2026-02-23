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
    [SerializeField] public float displayDuration = 4f; //displays for 4 seconds before fading out
    [SerializeField] public Image pickupIcon;
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
    }

    // Update is called once per frame
    public void ShowNotification(string itemName, Sprite icon = null)
    {
        StopAllCoroutines();
        notificationText.text = itemName + " x1";

        if(icon != null)
        {
            pickupIcon.sprite = icon;
            pickupIcon.gameObject.SetActive(true);
        }
        notificationText.gameObject.SetActive(true);
        StartCoroutine(HideAfterDuration());
    }

    private IEnumerator HideAfterDuration()
    {
        yield return new WaitForSeconds(displayDuration);
        notificationText.gameObject.SetActive(false);
        pickupIcon.gameObject.SetActive(false);
    }
}
