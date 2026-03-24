using UnityEngine;

public class TestMainMenu : MonoBehaviour
{
    public void OnClickNextButton()
    {
        MainMenuManager.Instance.NextMenuState();
    }

    public void OnClickPreviousButton()
    {
        MainMenuManager.Instance.PreviousMenuState();
    }
}
