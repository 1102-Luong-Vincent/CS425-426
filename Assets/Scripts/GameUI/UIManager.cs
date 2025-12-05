using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] PauseManager pauseManager;
    [SerializeField] CardCombineManager cardCombineManager;
    [SerializeField] FadeTransition fadeTransition;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void FadeToScene(SceneType scene,Vector3 pos)
    {
        fadeTransition.FadeToScene(scene,pos);
    }



}
