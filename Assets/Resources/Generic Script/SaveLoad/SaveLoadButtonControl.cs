using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SaveLoadButtonControl : MonoBehaviour
{
    public Button SaveLoadButton;
    public TextMeshProUGUI SaveLoadText;
    private SaveData saveData;

    private void Awake()
    {
        SaveLoadButton.onClick.AddListener(OnSaveLoadButtonClick);
    }

    public SaveData GetSaveData()
    {
        return saveData;
    }
   
    public void SetSaveData(SaveData saveData)
    {
        this.saveData = saveData;
        SaveLoadText.text = saveData.SaveTime.ToString();
    }

    void OnSaveLoadButtonClick()
    {
        SaveLoadPanelControl.Instance.SetSelSaveLoadButton(this);
        SelSavaLoadButton();

    }

    void SelSavaLoadButton()
    {
        SaveLoadButton.image.color = Color.red;
    }


    public void CancelSelSaveLoadButton()
    {
        SaveLoadButton.image.color = Color.white;

    }


}
