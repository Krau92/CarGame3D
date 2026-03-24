using UnityEngine;

public class ConfirmCarButton : MonoBehaviour
{
    public void OnConfirmCarButtonClicked()
    {
        MainMenuManager.Instance.StartGame();
    }
}
