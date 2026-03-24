using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void OnQuitButtonPressed()
    {
        //Checking  on editor
        Debug.Log("Quit button pressed");
        Application.Quit();
    }
}
