using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BestCircuitTimeSummary", menuName = "Scriptable Objects/BestCircuitTimeSummary")]
public class BestCircuitTimeSummarySO : ScriptableObject
{
    public List<BestLapData> bestLapDataList = new List<BestLapData>();

    public void ClearData()
    {
        bestLapDataList.Clear();
    }

    public void CopyFrom(List<BestLapData> newData)
    {
        ClearData();
        bestLapDataList = new List<BestLapData>(newData);
    }

    public void UpdateBestLapData(BestLapData newLapData)
    {
        int index = bestLapDataList.FindIndex(data => data.circuitIndex == newLapData.circuitIndex);
        if (index >= 0)
        {
            if (newLapData.totalTime < bestLapDataList[index].totalTime || bestLapDataList[index].totalTime == 0f)
            {
                bestLapDataList[index] = newLapData;
            }
        }
        else
        {
            bestLapDataList.Add(newLapData);
        }
    }

    public float GetTotalTime(int index)
    {
        BestLapData lapData = bestLapDataList.Find(data => data.circuitIndex == index);
        return lapData != null ? lapData.totalTime : 0f;
    }
}
