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
        SaveLoadPanelControl.Instance.ShowPanel();
    }


    void Start()
    {
        
    }
    void Update()
    {
        
    }
}
