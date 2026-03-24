using UnityEngine;

public class ButtonsAudioManaging : MonoBehaviour
{
    [SerializeField] private AudioClip buttonSelectedSFX;
    [SerializeField] private AudioClip buttonClickSFX;

    public void PlayButtonSelectedSFX()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(buttonSelectedSFX);
        }
    }

    public void PlayButtonClickSFX()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(buttonClickSFX);
        }
    }
}
