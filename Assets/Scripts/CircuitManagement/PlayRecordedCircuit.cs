using System.Collections.Generic;
using ArcadeVP;
using UnityEngine;

public class PlayRecordedCircuit : MonoBehaviour
{
    public List<GameObject> objectsToDisable = new List<GameObject>();
    public List<MonoBehaviour> componentsToDisable = new List<MonoBehaviour>();
    public List<Collider> collidersToDisable = new List<Collider>();
    public List<Rigidbody> rigidbodiesToDisable = new List<Rigidbody>();
    public bool shouldOpenMenuAtTheEnd = true;
    [SerializeField] private Transform sphereTransform;
    [SerializeField] private ArcadeVehicleController carController;
    Vector3 sphereTransformOffset = Vector3.zero;
    LapRecordingContainerSO recordedData;
    bool isPlaying = false;
    float timeBetweenSamples = 0.1f;
    float currentTimeBetweenSamples = 0f;
    int currentIndex = 0;
    Vector3 lastPosition;
    Vector3 nextPosition;
    float steeringInput;
    Quaternion lastRotation;
    Quaternion nextRotation;



    void Update()
    {
        if (isPlaying)
        {
            currentTimeBetweenSamples += Time.deltaTime;

            bool finishedThisFrame = false;

            if (currentTimeBetweenSamples >= timeBetweenSamples)
            {
                lastPosition = nextPosition;
                lastRotation = nextRotation;

                recordedData.GetDataAt(currentIndex, out nextPosition, out nextRotation, out steeringInput, out finishedThisFrame);

                currentTimeBetweenSamples -= timeBetweenSamples;

                currentIndex++;
            }

            float percentage = Mathf.Min(currentTimeBetweenSamples / timeBetweenSamples, 1f);

            transform.position = Vector3.Lerp(lastPosition, nextPosition, percentage);
            transform.rotation = Quaternion.Slerp(lastRotation, nextRotation, percentage);

            if (sphereTransform != null)
            {
                sphereTransform.localPosition = Vector3.zero + sphereTransformOffset;
                sphereTransform.localRotation = Quaternion.identity;
            }

            if (carController != null)
            {
                carController.ForceWheelRotation(Vector3.Distance(transform.position, lastPosition), steeringInput);
            }

            if (finishedThisFrame)
            {
                transform.position = lastPosition;
                transform.rotation = lastRotation;
                isPlaying = false;



                ToggleComponents(true);

                foreach (var rb in rigidbodiesToDisable)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                if (shouldOpenMenuAtTheEnd)
                {
                    CameraManagement.Instance.StopReplay();
                    UIManager.Instance.ShowMenu(true, true);
                }
                else
                {
                    gameObject.SetActive(false);
                }

            }
        }
    }


    public void PlayReplay(LapRecordingContainerSO lapData, float samplesInterval)
    {
        if (sphereTransform != null && sphereTransformOffset == Vector3.zero)
        {
            sphereTransformOffset = sphereTransform.localPosition;
        }
        ToggleComponents(false);
        isPlaying = true;
        recordedData = lapData;
        timeBetweenSamples = samplesInterval;
        currentTimeBetweenSamples = 0f;
        currentIndex = 0;
        lastPosition = transform.position;
        lastRotation = transform.rotation;

        bool isEmpty;
        recordedData.GetDataAt(currentIndex, out nextPosition, out nextRotation, out steeringInput, out isEmpty);

        if (isEmpty)
        {
            isPlaying = false;
            ToggleComponents(true);

        }
        if (shouldOpenMenuAtTheEnd)
        {
            CameraManagement.Instance.StartReplay();
            UIManager.Instance.ShowMenu(false);
            UIManager.Instance.SetCanPause(false);
        }
    }

    private void ToggleComponents(bool state)
    {
        foreach (var obj in objectsToDisable)
        {
            obj.SetActive(state);
        }

        foreach (var rb in rigidbodiesToDisable)
        {
            rb.isKinematic = !state;
        }

        foreach (var collider in collidersToDisable)
        {
            collider.enabled = state;
        }
        foreach (var component in componentsToDisable)
        {
            if (component.isActiveAndEnabled && component.name == "ArcadeVehicleController")
            {
                ArcadeVehicleController avc = component as ArcadeVehicleController;
                avc.ReactivateComponent(false);
            }
            else
            {
                component.enabled = state;
            }
        }
    }






}
