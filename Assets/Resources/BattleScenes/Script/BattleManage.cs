using UnityEngine;

public class BattleManage : MonoBehaviour
{
    public static BattleManage Instance { get; private set; }
   
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }



    void Start()
    {
        SetValue();
    }


    void SetValue()
    {
        Test();
    }


    void Test()
    {
        BattlePlayerValue.Instance.SetBattlePlayerValue(GameValue.Instance.GetPlayerValue());

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
