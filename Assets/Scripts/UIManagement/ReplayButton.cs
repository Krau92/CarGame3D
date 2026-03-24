using UnityEngine;

public class ReplayButton : MonoBehaviour
{
    [SerializeField] private LapRecording lapRecording;

    public void PlayReplay()
    {
        lapRecording.PlayReplay();
    }
}
