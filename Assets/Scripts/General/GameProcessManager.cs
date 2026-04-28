// Author: Shawn Meng 
// Created by: Shawn Meng
// Modified by: Shawn Meng and Vincent Luong
// No external source was used


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
        if (SoundManage.Instance == null)
        {
            Debug.LogWarning($"[GameProcessManager] SoundManage.Instance is null while trying to play music for scene {sceneType}.");
            return;
        }

        switch (sceneType)
        {
            case SceneType.GameStartScene: SoundManage.Instance.PlayBackgroundMusic(SoundManagerConstants.GameplayMusic); break;
            case SceneType.BattleScene: SoundManage.Instance.PlayBackgroundMusic(SoundManage.Instance.GetBattleSceneMusic()); break;
                
        }

    }
}
