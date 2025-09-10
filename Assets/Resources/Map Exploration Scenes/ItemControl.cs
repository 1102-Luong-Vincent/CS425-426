using UnityEngine;
using UnityEngine.SceneManagement;  

public class ItemControl : MonoBehaviour
{
    public SceneType targetScene = SceneType.None;

    private void OnTriggerEnter(Collider other)
    {
       if (other.GetComponent<PlayerMoveControl>() != null)
        {
            Debug.Log("Hit happend");
            if (targetScene == SceneType.None) return;
            PlayerValue.Instance.LoadSceneByEnum(targetScene);
        }
    }




}
