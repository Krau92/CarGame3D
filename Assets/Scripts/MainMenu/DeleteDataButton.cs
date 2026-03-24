using UnityEngine;

public class DeleteDataButton : MonoBehaviour
{
    public BestCircuitTimeSummarySO bestCircuitTimeSummary;
    public void OnDeleteDataButtonClicked()
    {
        DataManager.DeleteData();
        bestCircuitTimeSummary.ClearData();
    }
}
