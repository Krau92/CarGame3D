using UnityEngine;
using UnityEngine.UI;

public class MainMenuAudioManaging : MonoBehaviour
{
    public ButtonsAudioManaging buttonsAudioManaging;
    [SerializeField] private AudioClip mainMenuIntroMusic;
    [SerializeField] private AudioClip mainMenuLoopMusic;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            Debug.Log("Setting audiomanager");
            AudioManager.Instance.PlayMusic(mainMenuIntroMusic, mainMenuLoopMusic);
        }
    }

    public void PlayButtonSelectedSFX()
    {
        if(buttonsAudioManaging != null)
        {
            buttonsAudioManaging.PlayButtonSelectedSFX();
        }
    }

    public void PlayButtonClickSFX()
    {
        if(buttonsAudioManaging != null)
        {
            buttonsAudioManaging.PlayButtonClickSFX();
        }   
    }

}
