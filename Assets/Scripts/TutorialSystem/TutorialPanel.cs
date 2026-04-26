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
    }

    // Update is called once per frame
    public void SetColor(Color color)
    {
        panel.color = color;
        image.color = color;
        text.color = color;
    }
}
