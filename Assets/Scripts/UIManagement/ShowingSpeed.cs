using UnityEngine.UI;
using ArcadeVP;
using TMPro;
using UnityEngine;

public class ShowingSpeed : MonoBehaviour
{
    ArcadeVehicleController carController;
    [SerializeField] private Image filledSpeedMeter;
    [SerializeField] TMP_Text speedText;
    [Range (1f, 6f)] [SerializeField] private float showVelMultiplier = 1f;
    float currentSpeed = 0f;
    float maxSpeed;
    float percentageOfMaxSpeed;

    public void SetCarController()
    {
        carController = CircuitManager.Instance.playerCarController;
        maxSpeed = carController.MaxSpeed;
    }
    void Update()
    {
        currentSpeed = carController.GetCurrentSpeed();
        percentageOfMaxSpeed = currentSpeed / maxSpeed;
        filledSpeedMeter.fillAmount = percentageOfMaxSpeed;
        int showingSpeed = Mathf.RoundToInt(currentSpeed * showVelMultiplier);
        speedText.text = showingSpeed.ToString();
    }
}
