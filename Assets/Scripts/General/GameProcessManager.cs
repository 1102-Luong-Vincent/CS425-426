using UnityEngine;

public class GameProcessManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayMusic(GameValue.Instance.GetCurrentScence());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMusic(SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.GameStartScene: SoundManage.Instance.PlayBackgroundMusic(SoundManagerConstants.GameplayMusic); break;
            case SceneType.BattleScene: SoundManage.Instance.PlayBackgroundMusic(SoundManagerConstants.BattleMusic); break;

        }

    }
}
