using UnityEngine;

public class FastTestButton : MonoBehaviour
{
    public void FinishLap()
    {
        CircuitManager.Instance.CheckpointReached();
        CircuitManager.Instance.GoalReached();
    }
}
