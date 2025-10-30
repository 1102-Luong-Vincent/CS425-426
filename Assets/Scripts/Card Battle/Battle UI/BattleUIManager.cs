using UnityEngine;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TurnText;


    void Start()
    {
        SetTurnText(BattleManage.Instance.Turn);
        Listener(true);
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
}
