using System.Collections.Generic;
using UnityEngine;

public class ButtonsGroup : MonoBehaviour
{
    private List<ButtonStructure> buttons = new List<ButtonStructure>();

    void Awake()
    {
        buttons.AddRange(GetComponentsInChildren<ButtonStructure>(true));
    }

    public void ShowButtons(bool show, bool circuitEnded = false)
    {
        foreach (var button in buttons)
        {
            switch (button.GetButtonType())
            {
                case ButtonType.Default:
                    button.Activate(show);
                    break;
                case ButtonType.RaceEnded:
                    button.Activate(show && circuitEnded);
                    break;
                case ButtonType.PauseMenu:
                    button.Activate(show && !circuitEnded);
                    break;
            }
        }
    }
}
