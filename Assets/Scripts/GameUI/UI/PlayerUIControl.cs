// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng
// No external source was used

using UnityEngine;
using UnityEngine.UI;
public class PlayerUIControl : MonoBehaviour
{

    public Button SaveButton;
    public Button LoadButton;

    private void Awake()
    {
        SaveButton.onClick.AddListener(OnSaveButtonClick);
        LoadButton.onClick.AddListener(OnLoadButtonClick);
    }


    void OnSaveButtonClick()
    {
        SaveLoadPanelControl.Instance.NormalSaveGame();
    }

    void OnLoadButtonClick()
    {
        SaveLoadPanelControl.Instance.ShowLoadPanel();
    }


    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
