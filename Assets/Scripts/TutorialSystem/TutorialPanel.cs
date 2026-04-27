using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TutorialPanel : MonoBehaviour
{
    public Image panel;
    public Image image;
    public TextMeshProUGUI text;
    void Start()
    {
        if(panel == null)
        {
            panel = GetComponent<Image>();
        }

        if(image == null)
        {
            image = transform.Find("Image").GetComponent<Image>();
        }
        if(text == null)
        {
            text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        }
        if(panel != null)
        {
            panel.raycastTarget = false;
        }
        if(image != null)
        {
            image.raycastTarget = false;
        }
        if(text != null)
        {
            text.raycastTarget = false;
        }
    }

    // Update is called once per frame
    public void SetColor(Color color)
    {
        if(panel!= null)
            panel.color = color;
        if(image != null)
            image.color = color;
        if (text != null)   
            text.color = color;
    }
}
