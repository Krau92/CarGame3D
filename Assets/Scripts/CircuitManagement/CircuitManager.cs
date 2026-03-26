using System;
using System.Collections.Generic;
using ArcadeVP;
using UnityEngine;

public struct LapData
    {
        public float lapTime;
        public int lapNumber;
    }

public class CircuitManager : MonoBehaviour
{
    //Singleton
    public static CircuitManager Instance;
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

    [SerializeField] private int totalLaps = 3;


    public Action OnCircuitStarted;
    public Action OnLapCompleted;
    public Action OnCircuitCompleted;
    public Action OnRestartCircuit;

    
    [SerializeField] private Transform initialCarPosition;
    public LoadSceneInfoSO sceneInfoSO;
    public LapRecordingContainerSO lapRecordingContainer;
    private Transform playerCarTransform;
    public ArcadeVehicleController playerCarController;
    private List<LapData> lapData;
    private float totalTime;
    private float lapTime;
    private int currentLap;
    private bool checkpointReached;
    private bool circuitStarted;

    void Update()
    {
        if (circuitStarted)
        {
            lapTime += Time.deltaTime;
            UIManager.Instance.UpdateLapTime(lapTime);
        }
    }

    public void GoalReached()
    {
        if (checkpointReached)
        {
            currentLap++;
            LapData currentLapData = new LapData()
            {
                lapTime = lapTime,
                lapNumber = currentLap
            };

            lapData.Add(currentLapData);
            checkpointReached = false;
            totalTime += lapTime;

            lapTime = 0f;
            UIManager.Instance.UpdateLapCounter(currentLap + 1);
            UIManager.Instance.UpdateCompletedLaps(lapData);
            OnLapCompleted?.Invoke();
            
            if (currentLap >= totalLaps)
            {
                playerCarController.SetCanMove(false);
                circuitStarted = false;
                UIManager.Instance.EndingPanel(totalTime);
                OnCircuitCompleted?.Invoke();
            }
        }
    }

    public void CheckpointReached()
    {
        checkpointReached = true;
    }

    public void StartCircuit()
    {
        currentLap = 0;
        totalTime = 0f;
        lapTime = 0f;
        lapData = new List<LapData>();
        OnCircuitStarted?.Invoke();
        
        UIManager.Instance.UpdateLapCounter(currentLap + 1);
        UIManager.Instance.UpdateLapTime(lapTime);
        UIManager.Instance.UpdateCompletedLaps(lapData);
        UIManager.Instance.ChangeLapTimePanelVisibility(true);

        playerCarController.SetCanMove(true);
        
        circuitStarted = true;
    }

    void Start()
    {
        if (playerCarTransform == null)
        {
            playerCarTransform = Instantiate(sceneInfoSO.playerCarPrefab).transform;
        }
        if (playerCarController == null)
        {
            playerCarController = playerCarTransform.GetComponent<ArcadeVehicleController>();
        }
        
        CameraManagement.Instance.RegisterCameras(playerCarTransform.gameObject);
        lapRecordingContainer.circuitIndex = sceneInfoSO.circuitID;

        if(AudioManager.Instance != null)
            AudioManager.Instance.StopMusic();
        

        RestartCircuit();
    }

     public void RestartCircuit()
    {
        Time.timeScale = 1f;
        circuitStarted = false;
        checkpointReached = false;
        playerCarController.SetCanMove(false);
        playerCarTransform.gameObject.SetActive(false);
        playerCarTransform.position = initialCarPosition.position;
        playerCarTransform.rotation = initialCarPosition.rotation;
        playerCarTransform.gameObject.SetActive(true);
        playerCarController.ForceStopVehicle();
        CameraManagement.Instance.StopReplay(); //To avoid potential issues
        UIManager.Instance.ChangeLapTimePanelVisibility(false);
        UIManager.Instance.ShowMenu(false, false);
        UIManager.Instance.StartCountDown();
        OnRestartCircuit?.Invoke();
    }



}
