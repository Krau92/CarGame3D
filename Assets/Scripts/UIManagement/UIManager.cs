using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public static class TimeFormatter
{
    public static string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    public static string FormatDiffTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);
        if (minutes == 0)
            return string.Format("{0:00}.{1:000}", seconds, milliseconds);
        else
            return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}
public class UIManager : MonoBehaviour
{
    private int counter = 0;
    public int currentLap = 0;
    [SerializeField] private GameObject showingSpeed;
    public LoadSceneInfoSO sceneInfoSO;
    public ButtonsAudioManaging buttonsAudioManaging;


    [Header("FullScreen Panel")]
    [SerializeField] private GameObject fullScreenPanel;
    [SerializeField] private TMP_Text fullScreenText;
    [SerializeField] private Animator fullScreenAnimator;
    [SerializeField] private float originalPanelAlpha;
    [SerializeField] private float waitTimeBetweenTexts = 0.5f;
    [SerializeField] private RectTransform initialTextPosition;
    [SerializeField] private RectTransform finalTextPosition;
    [SerializeField] private AudioClip countdownClip;
    [SerializeField] private AudioClip goClip;

    [Header("Menu Panel")]
    [SerializeField] private GameObject buttonsGroup;

    [Header("Lap Time Display")]
    [SerializeField] private GameObject lapTimePanel;
    [SerializeField] private TMP_Text lapTimeText;
    [SerializeField] private TMP_Text completedLapsText;
    [SerializeField] private float movingTextDuration = 1f;
    [Tooltip("Must be a curve between (0,0) and (1,1). Must end on (1,1) for smooth results.")]
    [SerializeField] private AnimationCurve movingTextCurve;

    private List<float> referenceLapTimes = new List<float>();
    bool canPause = false;
    public void SetCanPause(bool value)
    {
        canPause = value;
    }

    public static UIManager Instance;
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

    void OnEnable()
    {
        InputManager.onPauseAction += TogglePauseMenu;
    }

    void OnDisable()
    {
        InputManager.onPauseAction -= TogglePauseMenu;
    }


    public void StartCountDown()
    {
        canPause = false;
        referenceLapTimes.Clear();
        ShowMenu(false);
        //Reset flag to avoid misfunctions
        canPause = false;
        UpdatePosAndSize(fullScreenText.rectTransform, initialTextPosition);
        counter = 3;
        FadePanel(originalPanelAlpha, 0f);
        showingSpeed.SetActive(false);
        fullScreenPanel.SetActive(true);
        CountDown();
    }

    public void CountDown()
    {
        if (counter > 0)
        {
            fullScreenText.text = counter.ToString();
            fullScreenAnimator.SetTrigger("CountDown");
            if(AudioManager.Instance != null && countdownClip != null)
            {
                AudioManager.Instance.PlaySFX(countdownClip);
            }
            counter--;
        }
        else if (counter == 0)
        {
            FadePanel(0f, 0.5f);
            fullScreenText.text = "GO!";
            fullScreenAnimator.SetTrigger("CountDown");
            CircuitManager.Instance.StartCircuit();
            showingSpeed.SetActive(true);
            showingSpeed.GetComponent<ShowingSpeed>().SetCarController();
            if(AudioManager.Instance != null && goClip != null)
            {
                AudioManager.Instance.PlaySFX(goClip);
            }
            counter--;
        }
        else
        {
            HideFullScreenPanel();
        }
    }

    private void HideFullScreenPanel()
    {

        canPause = true;
        fullScreenText.text = "";
        fullScreenPanel.SetActive(false);
    }

    public void UpdateLapCounter(int lap)
    {
        currentLap = lap;
    }



    public void UpdateLapTime(float lapTime)
    {

        lapTimeText.text = "Lap " + currentLap + ": " + TimeFormatter.FormatTime(lapTime);
    }

    public void UpdateCompletedLaps(List<LapData> lapData)
    {
        string completedLaps = "";

        if (lapData != null && lapData.Count > 0)
        {
            float lapTimeAdd = 0f;
            foreach (LapData lap in lapData)
            {
                completedLaps += "Lap " + lap.lapNumber + ": " + TimeFormatter.FormatTime(lap.lapTime);
                lapTimeAdd += lap.lapTime;
                if (referenceLapTimes.Count != 0)
                {
                    float lapDifference = lapTimeAdd - referenceLapTimes[lap.lapNumber - 1];
                    string differenceText = lapDifference >= 0 ? " (+" : " (-";
                    differenceText += TimeFormatter.FormatDiffTime(Mathf.Abs(lapDifference));
                    differenceText += ")";
                    completedLaps += differenceText;
                }

                completedLaps += "\n";
            }
        }

        completedLapsText.text = completedLaps;
    }

    public void SetReferenceLapTimes(List<float> lapTimes)
    {
        referenceLapTimes.Clear();
        referenceLapTimes.AddRange(lapTimes);
    }

    public void ChangeLapTimePanelVisibility(bool show)
    {
        lapTimePanel.SetActive(show);
    }

    private void FadePanel(float targetAlpha, float duration)
    {
        StartCoroutine(FadePanelCoroutine(targetAlpha, duration));
    }

    private IEnumerator FadePanelCoroutine(float targetAlpha, float duration)
    {

        Image panelImage = fullScreenPanel.GetComponent<Image>();
        Color colorPanel = panelImage.color;
        float startAlpha = colorPanel.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            colorPanel.a = newAlpha;
            panelImage.color = colorPanel;
            yield return null;
        }

        colorPanel.a = targetAlpha;
        panelImage.color = colorPanel;
    }

    public void EndingPanel(float totalTime)
    {
        showingSpeed.SetActive(false);
        canPause = false;
        UpdatePosAndSize(fullScreenText.rectTransform, initialTextPosition);
        fullScreenPanel.SetActive(true);
        FadePanel(originalPanelAlpha, 0.5f);
        StartCoroutine(ShowEndingPanel(totalTime));
    }

    private IEnumerator ShowEndingPanel(float totalTime)
    {
        fullScreenText.text = "GOAL REACHED!";
        WaitForSeconds wait = new WaitForSeconds(waitTimeBetweenTexts);
        yield return wait;
        fullScreenText.text = "Total Time: " + TimeFormatter.FormatTime(totalTime);

        WaitUntil waitForInput = new WaitUntil(() => InputManager.Instance.IsAnyKeyPressed());
        yield return waitForInput;


        float timer = 0f;

        while (timer < movingTextDuration)
        {
            timer += Time.deltaTime;
            float percentage = Mathf.Min(timer / movingTextDuration, 1f);
            float value = movingTextCurve.Evaluate(percentage);
            fullScreenText.rectTransform.anchorMax = Vector2.Lerp(initialTextPosition.anchorMax, finalTextPosition.anchorMax, value);
            fullScreenText.rectTransform.anchorMin = Vector2.Lerp(initialTextPosition.anchorMin, finalTextPosition.anchorMin, value);

            yield return null;
        }
        UpdatePosAndSize(fullScreenText.rectTransform, finalTextPosition);
        ShowMenu(true, true);

    }

    public void ShowMenu(bool show, bool circuitEnded = false)
    {
        Time.timeScale = show ? 0f : 1f;
        
        fullScreenPanel.SetActive(show);
        
        FadePanel(originalPanelAlpha, 0.5f);

        buttonsGroup.SetActive(show);
        buttonsGroup.GetComponent<ButtonsGroup>().ShowButtons(show, circuitEnded);
        canPause = !(show && circuitEnded);
    }

    private void UpdatePosAndSize(RectTransform rectTransform, RectTransform target)
    {
        rectTransform.anchorMax = target.anchorMax;
        rectTransform.anchorMin = target.anchorMin;
        rectTransform.anchoredPosition = target.anchoredPosition;
        rectTransform.sizeDelta = target.sizeDelta;
    }

    public void TogglePauseMenu()
    {
        if (!canPause)
            return;

        bool isActive = fullScreenPanel.activeSelf;
        ShowMenu(!isActive, false);
    }

}
