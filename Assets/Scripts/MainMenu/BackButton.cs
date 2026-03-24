using UnityEngine;

public class BackButton : MonoBehaviour
{
    public void OnBackButtonPressed()
    {
        MainMenuManager.Instance.PreviousMenuState();
    }
}
