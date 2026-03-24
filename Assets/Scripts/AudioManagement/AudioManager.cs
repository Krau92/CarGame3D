using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioPlayer musicPlayer;
    public AudioPlayer loopPlayer;
    public AudioPlayer sfxPlayer;

    public static AudioManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip intro, AudioClip loop)
    {
        StopMusic();
        musicPlayer.PlayMusic(intro, false);
        if(loop != null)
        {
            float delay = intro != null ? intro.length : 0f;
            loopPlayer.PlayMusic(loop, true, delay);
        }
    }

    public void PlaySFX(AudioClip clip)
    {   
        sfxPlayer.PlaySFX(clip);
    }

    public void StopMusic()
    {
        musicPlayer.StopMusic();
        loopPlayer.StopMusic();
    }
    
}
