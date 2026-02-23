using UnityEngine;

public class BattlePlayerController : MonoBehaviour
{
    [SerializeField] ParticleSystem bloodEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void SpawnBlood()
    {

        if (bloodEffect == null) return;

        ParticleSystem effect = Instantiate(bloodEffect, transform.position, Quaternion.identity);

        effect.Play();

        Destroy(effect.gameObject, effect.main.duration + 1.0f);
    }
}
