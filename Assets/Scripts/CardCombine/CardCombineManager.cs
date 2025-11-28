using UnityEngine;

public class CardCombineManager : MonoBehaviour
{
    public static CardCombineManager Instance;

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


}
