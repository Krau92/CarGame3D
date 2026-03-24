using System.Collections.Generic;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManagement : MonoBehaviour
{
    [SerializeField] private float timeBetweenCameraSwitches = 5f;
    List<CinemachineCamera> cameras = new List<CinemachineCamera>();
    private CinemachineCamera currentCamera, defaultCamera;
    Coroutine activeCoroutine;


    public static CameraManagement Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCamera(CinemachineCamera newCamera)
    {
        if (currentCamera != null)
        {
            currentCamera.Priority = 0;
        }
        newCamera.Priority = 10;
        currentCamera = newCamera;
    }

    public void SetDefaultCamera()
    {
        if (defaultCamera != null)
        {
            SetCamera(defaultCamera);
        }
        else
        {
            Debug.LogError("Default camera not set!");
        }
    }

    public void RegisterCameras(GameObject car)
    {
        cameras.Clear();
        CinemachineCamera[] foundCameras = car.GetComponentsInChildren<CinemachineCamera>();
        cameras.AddRange(foundCameras);
        foreach (var cam in cameras)
        {
            if (cam.tag == "DefaultCamera")
            {
                defaultCamera = cam;
                SetCamera(defaultCamera);
                break;
            }
        }

        if (cameras.Count == 0)
        {
            Debug.LogError("No cameras found in the player car prefab!");
        }
    }

    public void StartReplay()
    {
        SetDefaultCamera();
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        activeCoroutine = StartCoroutine(ReplayCameraSequence());
    }

    IEnumerator ReplayCameraSequence()
    {
        while (true)
        {
            int randomIndex = Random.Range(0, cameras.Count);
            SetCamera(cameras[randomIndex]);
            yield return new WaitForSeconds(timeBetweenCameraSwitches);
        }

    }

    public void StopReplay()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        SetDefaultCamera();
    }
}
