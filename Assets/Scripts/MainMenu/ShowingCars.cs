using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class ShowingCars : MonoBehaviour
{
    public Image speedBar, accBar, handlingBar;
    public TMP_Text carName, canDriftText;
    private float maxSpeed = 0f, maxAcc = 0f, maxHandling = 0f;
    Coroutine currentCoroutine;

    void Start()
    {
        maxSpeed = 0f;
        maxAcc = 0f;
        maxHandling = 0f;
        canDriftText.enabled = false;
    }

    public void SetMaxStats(List<CarInfo> cars)
    {
        foreach (CarInfo car in cars)
        {
            maxSpeed = Mathf.Max(maxSpeed, car.maxSpeed);
            maxAcc = Mathf.Max(maxAcc, car.acceleration);
            maxHandling = Mathf.Max(maxHandling, car.handling);
        }
    }

    public void UpdateStats(CarInfo car)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(UpdateStatsCoroutine(car));
        carName.text = car.carName;
        canDriftText.enabled = car.canDrift;
    }

    // Coroutine to animate update the stats bars
    private IEnumerator UpdateStatsCoroutine(CarInfo car)
    {
        float elapsedTime = 0f;
        float duration = 0.5f;

        float initialSpeedFill = speedBar.fillAmount;
        float targetSpeedFill = car.maxSpeed / maxSpeed;

        float initialAccFill = accBar.fillAmount;
        float targetAccFill = car.acceleration / maxAcc;

        float initialHandlingFill = handlingBar.fillAmount;
        float targetHandlingFill = car.handling / maxHandling;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            speedBar.fillAmount = Mathf.Lerp(initialSpeedFill, targetSpeedFill, t);
            accBar.fillAmount = Mathf.Lerp(initialAccFill, targetAccFill, t);
            handlingBar.fillAmount = Mathf.Lerp(initialHandlingFill, targetHandlingFill, t);

            yield return null;
        }


        speedBar.fillAmount = targetSpeedFill;
        accBar.fillAmount = targetAccFill;
        handlingBar.fillAmount = targetHandlingFill;
    }

}
