
using UnityEngine;

[CreateAssetMenu(fileName = "New Circuit Info", menuName = "Scriptable Objects/Circuit Info")]
public class CircuitInfo : ScriptableObject
{
    public int circuitID;
    public string circuitName;
    public string circuitScene;
    public Sprite circuitThumbnail;

}
