// Author: Shawn Meng
// Created by: Shawn Meng
// Modified by: Shawn Meng
// no external source was used.


using UnityEngine;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TurnText;
    [SerializeField] GameObject GameOverPanel;


    void Start()
    {
        SetTurnText(BattleManage.Instance.Turn);
        Listener(true);
        GameOverPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        Listener(false);
    }


    public void SetTurnText(int turn)
    {
        TurnText.text = $"Turn {turn}";
    }



    void Listener(bool isAdd)
    {
        BattleManage.Instance.TurnListener(SetTurnText, isAdd);
    }


   
    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayGameOver()
    {
        GameOverPanel.SetActive(true);
    }
}
