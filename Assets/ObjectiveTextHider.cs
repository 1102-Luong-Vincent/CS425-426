using TMPro;
using UnityEngine;

public class ObjectiveTextHider : MonoBehaviour
{
    TextMeshProUGUI text;
    bool isHidden = false;
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (text == null)
        {
            Debug.LogError("ObjectiveTextHider: No TextMeshProUGUI component found on the GameObject.");
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (isHidden)
            {
                text.enabled = true;
                isHidden = false;
            }
            else
            {
                text.enabled = false;
                isHidden = true;
            }
        }
    }
}
