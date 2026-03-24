using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class BestLapData
{
    public int circuitIndex;

    public float totalTime = 0f;

    public void CopyFrom(LapRecordingContainerSO lapData)
    {
        totalTime = lapData.totalTime;
        circuitIndex = lapData.circuitIndex;
    }
}

[Serializable]
public class BestLapGhostData
{
    public int circuitIndex;
    public float totalTime = 0f;
    public List<Vector3> carPositions = new List<Vector3>();
    public List<Quaternion> carRotations = new List<Quaternion>();
    public List<float> steeringInputs = new List<float>();
    public List<float> sampleTimes = new List<float>();
    public List<float> lapTimes = new List<float>();    

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
}
