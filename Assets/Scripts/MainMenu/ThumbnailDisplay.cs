using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class ThumbnailDisplay : MonoBehaviour
{
    public BestCircuitTimeSummarySO bestCircuitTimeSummary;
    [SerializeField] private Image thumbnailImage;
    [SerializeField] private TMP_Text bestTimeText;

    public void UpdateThumbnail(Sprite newThumbnail)
    {
        thumbnailImage.sprite = newThumbnail;
    }

    public void UpdateBestTime(int circuitID)
    {
        float unformatedTime = bestCircuitTimeSummary.GetTotalTime(circuitID);
        string bestTime;

        bestTime =unformatedTime == 0f ? "--:--:---" : TimeFormatter.FormatTime(unformatedTime);

        bestTimeText.text = "Best total time: " + bestTime;
    }
}
