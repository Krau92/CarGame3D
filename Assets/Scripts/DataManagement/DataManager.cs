using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

[Serializable]
public class BestLapDataListWrapper
{
    public List<BestLapData> items;
}

public static class DataManager
{
    public static void SaveData(BestCircuitTimeSummarySO summary)
    {
        BestLapDataListWrapper wrapper = new BestLapDataListWrapper();
        wrapper.items = new List<BestLapData>(summary.bestLapDataList);
        string jsonData = JsonUtility.ToJson(wrapper, true);
        string filePath = Application.persistentDataPath + "/best_circuit_time_summary.json";

        File.WriteAllText(filePath, jsonData);

        Debug.Log($"Game saved to {filePath}");
    }

    public static void SaveBestLap(LapRecordingContainerSO lapData)
    {
        BestLapGhostData ghostData = new BestLapGhostData();
        ghostData.CopyFrom(lapData);

        string jsonData = JsonUtility.ToJson(ghostData, true);
        string filePath = Application.persistentDataPath + $"/ghost_best_lap_circuit_{lapData.circuitIndex}.json";
        File.WriteAllText(filePath, jsonData);
        Debug.Log($"Best lap saved to {filePath}");
    }

    public static void LoadData(BestCircuitTimeSummarySO summary)
    {
        string filePath = Application.persistentDataPath + "/best_circuit_time_summary.json";

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            BestLapDataListWrapper wrapper = JsonUtility.FromJson<BestLapDataListWrapper>(jsonData);
            summary.CopyFrom(wrapper.items);
        }
        else
        {
            Debug.LogWarning($"Save file not found at {filePath}");
        }
    }

    public static void LoadBestLap(int circuitIndex, LapRecordingContainerSO lapData)
    {
        string filePath = Application.persistentDataPath + $"/ghost_best_lap_circuit_{circuitIndex}.json";

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            BestLapGhostData ghostData = JsonUtility.FromJson<BestLapGhostData>(jsonData);

            lapData.CopyFrom(ghostData);
        }
        else
        {
            lapData.Reset();
            Debug.LogWarning($"Best lap file not found at {filePath}");
        }

    }

    public static void DeleteData()
    {
        string filePath = Application.persistentDataPath + "/best_circuit_time_summary.json";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Save file deleted at {filePath}");
        }
        else
        {
            Debug.LogWarning($"Save file not found at {filePath}");
        }

        string[] ghostFiles = Directory.GetFiles(Application.persistentDataPath, "ghost_best_lap_circuit_*.json");
        if (ghostFiles.Length == 0)
        {
            Debug.LogWarning("No ghost lap files found to delete.");
            return;
        }
        foreach (string ghostFile in ghostFiles)
        {
            File.Delete(ghostFile);
            Debug.Log($"Deleted ghost lap file: {ghostFile}");
        }
    }

}
