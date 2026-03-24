using ArcadeVP;
using Unity.Cinemachine;
using UnityEngine;

public class CarInfo : MonoBehaviour
{
    public CinemachineCamera carCamera;
    public GameObject carPrefab;
    public string carName;
    ArcadeVehicleController carController;
    [HideInInspector] public float maxSpeed;
    [HideInInspector] public float acceleration;
    [HideInInspector] public float handling;
    [HideInInspector] public bool canDrift;

    void Start()
    {
        carController = carPrefab.GetComponent<ArcadeVehicleController>();
        maxSpeed = carController.MaxSpeed;
        acceleration = carController.accelaration;
        handling = carController.turn;
        canDrift = carController.kartLike;
    }
}
