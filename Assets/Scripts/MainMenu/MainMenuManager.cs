using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private enum MenuState
    {
        MainMenu,
        CircuitSelection,
        CarSelection
    }
    public LapRecordingContainerSO ghostData;
    public BestCircuitTimeSummarySO bestCircuitTimeSummary;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject circuitSelectionPanel;
    [SerializeField] private GameObject carSelectionPanel;
    [SerializeField] private List<CarInfo> cars;
    public LoadSceneInfoSO loadSceneInfo;
    ShowingCars showingCars;
    int currentCarIndex = 0;
    private CircuitInfo selectedCircuit;
    public MainMenuAudioManaging mainMenuAudioManaging;

    private MenuState currentMenuState;
    public static MainMenuManager Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnEnable()
    {
        InputManager.onMoveUIInput += HandleMoveInput;
        InputManager.onPauseAction += PreviousMenuState;
        InputManager.onSubmitAction += StartGame;
    }
    void OnDisable()
    {
        InputManager.onMoveUIInput -= HandleMoveInput;
        InputManager.onPauseAction -= PreviousMenuState;
        InputManager.onSubmitAction -= StartGame;

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }

        DataManager.LoadData(bestCircuitTimeSummary);
        currentCarIndex = 0;
        currentMenuState = MenuState.MainMenu;
        showingCars = GetComponent<ShowingCars>();
        ExecuteMenuState();

        Debug.Log("There is a SceneInfoSO in mainmenumanager: " + (loadSceneInfo != null));
    }

    void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        circuitSelectionPanel.SetActive(false);
        carSelectionPanel.SetActive(false);
    }

    void ShowCircuitSelection()
    {
        mainMenuPanel.SetActive(false);
        circuitSelectionPanel.SetActive(true);
        carSelectionPanel.SetActive(false);
    }

    void ShowCarSelection()
    {
        showingCars.SetMaxStats(cars);
        mainMenuPanel.SetActive(false);
        circuitSelectionPanel.SetActive(false);
        carSelectionPanel.SetActive(true);
    }

    //Maybe for showing something in the future.
    void HideAllMenus()
    {
        mainMenuPanel.SetActive(false);
        circuitSelectionPanel.SetActive(false);
        carSelectionPanel.SetActive(false);
    }

    public void SetCircuit(CircuitInfo circuitInfo)
    {
        selectedCircuit = circuitInfo;

        loadSceneInfo.circuitSceneName = selectedCircuit.circuitScene;
        loadSceneInfo.circuitID = selectedCircuit.circuitID;
        NextMenuState();
    }

    public void NextMenuState()
    {
        int maxIndex = (int)MenuState.CarSelection;
        currentMenuState = (MenuState)Mathf.Min((int)currentMenuState + 1, maxIndex);
        ExecuteMenuState();
    }

    public void PreviousMenuState()
    {
        cars[currentCarIndex].carCamera.Priority = 10;
        currentMenuState = (MenuState)Mathf.Max((int)currentMenuState - 1, 0);
        ExecuteMenuState();
    }

    void ExecuteMenuState()
    {
        switch (currentMenuState)
        {
            case MenuState.MainMenu:
                ShowMainMenu();
                break;
            case MenuState.CircuitSelection:
                ShowCircuitSelection();
                break;
            case MenuState.CarSelection:
                ShowCarSelection();
                ShowCar(currentCarIndex);
                break;
        }
    }

    void ShowCar(int index)
    {
        int lastCarIndex = currentCarIndex;
        currentCarIndex = Mathf.Clamp(index, 0, cars.Count - 1);
        showingCars.UpdateStats(cars[currentCarIndex]);

        if (currentCarIndex > 0)
            cars[currentCarIndex - 1].carCamera.Priority = 10;

        cars[currentCarIndex].carCamera.Priority = 12;

        Debug.Log("Setting selected car to loadsceneinfo: " + cars[currentCarIndex].carPrefab.name);
        loadSceneInfo.playerCarPrefab = cars[currentCarIndex].carPrefab;

        if (currentCarIndex != lastCarIndex)
            mainMenuAudioManaging.PlayButtonSelectedSFX();
    }

    void HandleMoveInput(float moveInput)
    {
        if (currentMenuState == MenuState.CarSelection)
        {
            if (moveInput > 0.5f)
            {
                ShowCar(currentCarIndex - 1);
            }
            else if (moveInput < -0.5f)
            {
                ShowCar(currentCarIndex + 1);
            }


        }
    }

    public void StartGame()
    {
        
        Debug.Log("StartGame called with menu state: " + currentMenuState);
        if (currentMenuState != MenuState.CarSelection)
            return;

        mainMenuAudioManaging.PlayButtonClickSFX();
        DataManager.LoadBestLap(selectedCircuit.circuitID, ghostData);
        SceneManager.LoadScene(selectedCircuit.circuitScene);
    }
}
