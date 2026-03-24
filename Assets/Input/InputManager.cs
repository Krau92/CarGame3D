using System;
using UnityEditor;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerActions playerActions;
    private float moveInput;
    public static event Action<float> onMoveInput;
    public static event Action<float> onMoveUIInput;
    public static event Action <float> onAccelerationInput;
    public static event Action<bool> onBrakeAction;
    public static event Action onPauseAction;
    public static event Action onSubmitAction;

    public static InputManager Instance { get; private set; }  
    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Force 60 FPS
            Application.targetFrameRate = 60;
            Time.fixedDeltaTime = 0.02f; 
        }
        else
        {
            Destroy(gameObject);
        }

        playerActions = new PlayerActions();

    }


    // Enable input actions
    void OnEnable()
    {
        playerActions.Enable();
    }


    // Disable input actions
    void OnDisable()
    {
        if (playerActions != null)
            playerActions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = playerActions.Controls.Move.ReadValue<float>();
        onMoveInput?.Invoke(moveInput);

        float accelerationInput = playerActions.Controls.Acceleration.ReadValue<float>();
        onAccelerationInput?.Invoke(accelerationInput);

        if(playerActions.UI.Left.WasPressedThisFrame())
        {
            onMoveUIInput?.Invoke(-1f);
        }
        else if(playerActions.UI.Right.WasPressedThisFrame())
        {
            onMoveUIInput?.Invoke(1f);
        }
        
        if (playerActions.Controls.Brake.triggered)
        {
            onBrakeAction?.Invoke(true);
        }
        if (playerActions.Controls.Brake.WasReleasedThisFrame())
        {
            onBrakeAction?.Invoke(false);
        }
        if (playerActions.Controls.Pause.triggered)
        {
            onPauseAction?.Invoke();
        }
        if(playerActions.UI.Submit.WasPressedThisFrame())
        {
            onSubmitAction?.Invoke();
        }

    }

    public bool IsAnyKeyPressed()
    {
        return  playerActions.Controls.Acceleration.WasPressedThisFrame() || 
                playerActions.Controls.Brake.WasPressedThisFrame() ||
                playerActions.Controls.AnyKey.WasPressedThisFrame();
    }

}
