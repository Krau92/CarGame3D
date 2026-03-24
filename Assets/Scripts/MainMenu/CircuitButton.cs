using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CircuitButton : MonoBehaviour, IPointerEnterHandler
{
    public CircuitInfo circuitInfo;
    public ThumbnailDisplay thumbnail;
    [SerializeField] private TMP_Text circuitNameText;

    void Start()
    {
        circuitNameText.text = circuitInfo.circuitName;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        thumbnail.UpdateThumbnail(circuitInfo.circuitThumbnail);
        thumbnail.UpdateBestTime(circuitInfo.circuitID);
    }

    public void ChooseCircuit()
    {
        MainMenuManager.Instance.SetCircuit(circuitInfo);
    }


}
