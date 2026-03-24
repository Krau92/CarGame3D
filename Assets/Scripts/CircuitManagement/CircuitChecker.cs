using UnityEngine;

public class CircuitChecker : MonoBehaviour
{
    //Class that checks the player's progress through the circuit

    //Using an enum to recycle the script. Maybe add GAP checkpoints later?
    private enum CheckpointType { Checkpoint, Goal }
    [SerializeField] private CheckpointType checkpointType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ManageCheckedPoint();
        }
    }

    private void ManageCheckedPoint()
    {
        switch (checkpointType)
        {
            case CheckpointType.Checkpoint:
                CircuitManager.Instance.CheckpointReached();
                break;
            case CheckpointType.Goal:
                CircuitManager.Instance.GoalReached();
                break;
        }
    }
}
