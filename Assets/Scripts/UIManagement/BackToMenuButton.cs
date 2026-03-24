using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuButton : MonoBehaviour
{

    [SerializeField] LapRecording lapRecording;
    public void OnBackToMenuButtonPressed()
    {
        lapRecording.UpdateBestLapData();
        SceneManager.LoadScene("MainMenu");
    }
}
