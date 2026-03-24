using UnityEngine;

[CreateAssetMenu(fileName = "LoadSceneInfoSO", menuName = "Scriptable Objects/LoadSceneInfoSO")]
public class LoadSceneInfoSO : ScriptableObject
{
    public GameObject playerCarPrefab;
    public string circuitSceneName;
    public int circuitID;

}
