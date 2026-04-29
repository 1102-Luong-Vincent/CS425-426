using UnityEngine;

public class RaindropController : MonoBehaviour
{
    [SerializeField] public ParticleSystem RaindropsParticleSystem;
    [SerializeField] AudioSource audio;
    [SerializeField] public AudioClip raindrops;

    private void Awake()
    {
        if(audio != null && raindrops != null)
        {
            audio.clip = raindrops;
            audio.loop = true;
            audio.Play();
        }
    }
    public void RaindropsPauseMusic()
    {
        if(audio != null && raindrops != null)
        {
            audio.Pause();
        }
    }

    public void RaindropsResumeMusic()
    {
        if(audio != null)
        {
            audio.UnPause();
        }
    }
}
