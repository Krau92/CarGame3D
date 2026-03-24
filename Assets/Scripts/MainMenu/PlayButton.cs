using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public void OnPlayButtonPressed()
    {
        MainMenuManager.Instance.NextMenuState();
    }
}
