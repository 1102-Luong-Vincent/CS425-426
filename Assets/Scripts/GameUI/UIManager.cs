// Author: Vincent Luong
// Created by: Vincent Luong
// Modified by: Vincent Luong
// No external source was used.

using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] FadeTransition fadeTransition;


    private void Awake()
    {
        Debug.Log($"UIManager Awake: {name}, instanceID={GetInstanceID()}, scene={gameObject.scene.name}");
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
