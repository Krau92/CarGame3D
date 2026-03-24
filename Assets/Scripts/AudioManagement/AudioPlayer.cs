using UnityEngine;
using System.Collections;   

public class AudioPlayer : MonoBehaviour
{
    AudioSource audioSource;
    
    float fadeOutIntensity = 1f; // Adjust this value to control the speed of the fade-out
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMusic(AudioClip clip, bool loop, float delay = 0f)
    {
        StopAllCoroutines();
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.PlayScheduled(AudioSettings.dspTime + 0.1 + delay);
    }

    public void PlaySFX(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void StopMusic()
    {
        if(audioSource.isPlaying)
        {
            StartCoroutine(StopMusicCoroutine());
        }
    }

    IEnumerator StopMusicCoroutine()
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0.1f)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, fadeOutIntensity * Time.deltaTime);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; 
    }

}
