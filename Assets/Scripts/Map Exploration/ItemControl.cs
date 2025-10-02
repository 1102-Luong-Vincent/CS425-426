using SmallScaleInc.TopDownPixelCharactersPack1;
using UnityEngine;
using UnityEngine.SceneManagement;  

public class ItemControl : MonoBehaviour
{
    public SceneType targetScene = SceneType.None;

    private void OnTriggerEnter(Collider other)
    {
       if (other.GetComponent<PlayerController>() != null)
        {
            if (targetScene == SceneType.None) return;
            GameValue.Instance.LoadSceneByEnum(targetScene);
        }
    }


}
