using UnityEngine;
using TMPro;

public class BattlePlayerTestUIManager : MonoBehaviour
{
    public static BattlePlayerTestUIManager Instance { get; private set; }
    public TextMeshProUGUI PlayerStateText;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }



    public void CheckPlayerState(BattlePlayerValue player)
    {
        PlayerStateText.text =
            $"isBleeding {player.state.isBleeding} \n" +
            $"isPoisoned {player.state.isPoisoned} \n" +
            $"AttackBuff:{player.state.AttackBuff} \n" +
            $"DefenseBuff:{player.state.DefenseBuff} \n" +
            $"CriticalDamageBuff:{player.state.CriticalDamageBuff} \n" +
            $"CriticalChanceBuff:{player.state.CriticalChanceBuff} \n";
    }


}
