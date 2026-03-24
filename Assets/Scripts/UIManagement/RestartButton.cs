using UnityEngine;

public class RestartButton : MonoBehaviour
{
    public LapRecording lapRecording;
    public void RestartCircuit()
    {
        lapRecording.ForceStopRecording();
        CircuitManager.Instance.RestartCircuit();
    }
}
