using UnityEngine;

public class FullScreenPanel : MonoBehaviour
{
    public void CountDown()
    {
        UIManager.Instance.CountDown();
    }

    public void ShowMenu()
    {
        UIManager.Instance.ShowMenu(true);
    }
}
