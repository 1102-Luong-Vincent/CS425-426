using UnityEngine;
using UnityEngine.UI;
public class TestScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button BackButton;
    void Start()
    {
        if (BackButton != null) BackButton.onClick.AddListener(OnBackButtonClick);
    }

    void OnBackButtonClick()
    {
        GameValue.Instance.LoadSceneByEnum(SceneType.TempMap);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
