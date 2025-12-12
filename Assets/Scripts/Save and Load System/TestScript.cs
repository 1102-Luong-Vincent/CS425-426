// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng
// No external source was used

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
        GameValue.Instance.LoadSceneByEnum(SceneType.Level_1);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
