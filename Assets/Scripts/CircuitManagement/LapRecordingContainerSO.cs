using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LapRecordingContainer", menuName = "Scriptable Objects/LapRecordingContainer")]

public class LapRecordingContainerSO : ScriptableObject
{
    public List<Vector3> carPositions = new List<Vector3>();
    public List<Quaternion> carRotations = new List<Quaternion>();
    public List<float> steeringInputs = new List<float>(); 
    public List<float> sampleTimes = new List<float>();
    public List<float> lapTimes = new List<float>();
    public int circuitIndex;
    public float totalTime = 0f;

    public void Reset()
    {
        carPositions.Clear();
        carRotations.Clear();
        steeringInputs.Clear();
        sampleTimes.Clear();
        lapTimes.Clear();
        totalTime = 0f;
    }

    public void AddNewData(Vector3 position, Quaternion rotation, float steering, float time)
    {
        carPositions.Add(position);
        carRotations.Add(rotation);
        steeringInputs.Add(steering);
        sampleTimes.Add(time);
    }

    public void GetDataAt(int index, out Vector3 position, out Quaternion rotation, out float steering, out bool ended)
    {
        if(index >= carPositions.Count)
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;
            steering = 0f;
            ended = true;
            return;
        }
        steering = steeringInputs[index];
        position = carPositions[index];
        rotation = carRotations[index];
        ended = false;
    }

    public void CopyFrom(LapRecordingContainerSO lapData)
    {
        carPositions = new List<Vector3>(lapData.carPositions);
        carRotations = new List<Quaternion>(lapData.carRotations);
        steeringInputs = new List<float>(lapData.steeringInputs);
        sampleTimes = new List<float>(lapData.sampleTimes);
        lapTimes = new List<float>(lapData.lapTimes);
        totalTime = lapData.totalTime;
        circuitIndex = lapData.circuitIndex;
    }

    public void CopyFrom(BestLapGhostData ghostData)
    {
        carPositions = new List<Vector3>(ghostData.carPositions);
        carRotations = new List<Quaternion>(ghostData.carRotations);
        sampleTimes = new List<float>(ghostData.sampleTimes);
        steeringInputs = new List<float>(ghostData.steeringInputs);
        lapTimes = new List<float>(ghostData.lapTimes);
        totalTime = ghostData.totalTime;
        circuitIndex = ghostData.circuitIndex;
    }

    public void SetEmptyData(int index)
    {
        Reset();
        circuitIndex = index;
    }

    public void SetLapTime(float time)
    {
        lapTimes.Add(time);
    }

    public void SetTotalTime(float time)
    {
        totalTime = time;
    }
}
