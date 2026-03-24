using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum ButtonType
{
    Default,
    RaceEnded,
    PauseMenu,
}

public class ButtonStructure : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    private Button buttonComponent;

    [SerializeField] private bool isDefault = false;
    public bool IsDefault => isDefault;
    public bool isActive = true;
    [SerializeField] private ButtonType buttonType = ButtonType.Default;
    public ButtonType GetButtonType() => buttonType;

    void Start()
    {
        buttonComponent = GetComponent<Button>();
    }

    public void ButtonClicked()
    {
        buttonComponent.onClick.Invoke();
    }

    public void Activate(bool active)
    {
        isActive = active;
        gameObject.SetActive(active);
    }

    public void OnSelect(BaseEventData eventData)
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonComponent.Select();
    }
}
