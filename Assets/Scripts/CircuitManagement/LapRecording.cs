using UnityEngine;

public class LapRecording : MonoBehaviour
{
    private int circuitID;
    [SerializeField] private LapRecordingContainerSO bestLapData;
    [SerializeField] private LapRecordingContainerSO currentLapData;
    [SerializeField] private BestCircuitTimeSummarySO bestCircuitTimeSummary;

    [SerializeField] private float timeBetweenDataPoints = 0.1f;
    public Transform playerCarTransform;

    public GameObject ghostCarPrefab;
    float steerInput;


    GameObject ghostCarInstance;

    private float timer = 0f;
    private float startTime = 0f;
    bool isRecording = false;

    void OnEnable()
    {
        CircuitManager.Instance.OnCircuitStarted += StartRecording;
        CircuitManager.Instance.OnCircuitStarted += PlayGhost;
        CircuitManager.Instance.OnLapCompleted += RegisterLapTime;
        CircuitManager.Instance.OnCircuitCompleted += StopRecording;
        CircuitManager.Instance.OnRestartCircuit += Restart;
        InputManager.onMoveInput += SetSteerInput;
    }

    void OnDisable()
    {
        CircuitManager.Instance.OnCircuitStarted -= StartRecording;
        CircuitManager.Instance.OnCircuitStarted -= PlayGhost;
        CircuitManager.Instance.OnLapCompleted -= RegisterLapTime;
        CircuitManager.Instance.OnCircuitCompleted -= StopRecording;
        CircuitManager.Instance.OnRestartCircuit -= Restart;
        InputManager.onMoveInput -= SetSteerInput;
    }




    void FixedUpdate()
    {
        if (isRecording)
        {
            timer += Time.fixedDeltaTime;
            if (timer >= timeBetweenDataPoints)
            {
                RecordCurrentData(Time.time - startTime);
                timer -= timeBetweenDataPoints;
            }
        }
    }

    void StartRecording()
    {
        timer = 0f;
        SetCircuitID();
        currentLapData.Reset();
        currentLapData.circuitIndex = circuitID;
        isRecording = true;
        startTime = Time.time;
    }

    void StopRecording()
    {
        isRecording = false;
        currentLapData.SetTotalTime(Time.time - startTime);
        if (ghostCarInstance != null)
        {
            ghostCarInstance.SetActive(false);
        }
        if (currentLapData.totalTime != 0f)
        {
            UpdateBestLapData();
        }

    }

    public void ForceStopRecording()
    {
        isRecording = false;
        if (ghostCarInstance != null)
        {
            ghostCarInstance.SetActive(false);
        }
    }

    public void PlayReplay()
    {
        playerCarTransform.GetComponent<PlayRecordedCircuit>().PlayReplay(currentLapData, timeBetweenDataPoints);
    }

    public void PlayGhost()
    {
        if (bestLapData.totalTime == 0f)
        {
            return;
        }
        if (ghostCarInstance == null)
        {
            ghostCarInstance = Instantiate(ghostCarPrefab, bestLapData.carPositions[0], bestLapData.carRotations[0]);
        }
        ghostCarInstance.SetActive(true);
        ghostCarInstance.GetComponent<PlayRecordedCircuit>().PlayReplay(bestLapData, timeBetweenDataPoints);
        UIManager.Instance.SetReferenceLapTimes(bestLapData.lapTimes);
    }

    void RecordCurrentData(float time)
    {
        currentLapData.AddNewData(playerCarTransform.position, playerCarTransform.rotation, steerInput, time);
    }

    void RegisterLapTime()
    {
        Debug.Log("Lap completed in: " + (Time.time - startTime) + " seconds.");
        float lapTime = Time.time - startTime;
        currentLapData.SetLapTime(lapTime);
    }

    public void UpdateBestLapData()
    {
        if (currentLapData.totalTime < bestLapData.totalTime || bestLapData.totalTime == 0f)
        {
            bestLapData.CopyFrom(currentLapData);
            DataManager.SaveBestLap(currentLapData);
            BestLapData summaryBestLapData = new BestLapData
            {
                circuitIndex = currentLapData.circuitIndex,
                totalTime = currentLapData.totalTime
            };
            bestCircuitTimeSummary.UpdateBestLapData(summaryBestLapData);
            DataManager.SaveData(bestCircuitTimeSummary);
        }
    }

    void Restart()
    {
        if (playerCarTransform == null)
            playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;

        playerCarTransform.gameObject.SetActive(true);
        isRecording = false;
    }

    public void SetCircuitID()
    {
        circuitID = currentLapData.circuitIndex;
    }

    public void SetSteerInput(float input)
    {
        steerInput = input;
    }


}
