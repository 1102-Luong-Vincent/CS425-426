using UnityEngine;

public class BattleManage : MonoBehaviour
{
    public static BattleManage Instance { get; private set; }
    public PlayerValue playerValue;

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
        BattlePlayerValue.Instance.SetBattlePlayerValue(playerValue);

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
