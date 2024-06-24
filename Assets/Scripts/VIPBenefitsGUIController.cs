using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mkey;

public class VIPBenefitsGUIController : MonoBehaviour
{
    private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
    private VIPBenefits VIPInfo;

    [SerializeField] private Text PlayerstatusPointsText;
    [SerializeField] private Text StatusPointsNeededText;
    [SerializeField] private Text VIPStausNameText;
    [SerializeField] private Slider StatusPointsNeededSlider;
    [SerializeField] private Text StatusPointsGainedPercentage;
    private void Awake()
    {
        PlayerstatusPointsText.text = "YOUR STATUS POINTS: " + MPlayer.StatusPoints.ToString();
    }

    private void Start()
    {
        VIPInfo = FindObjectOfType<VIPBenefits>();
        int statusPointsNeeded = VIPInfo.GetStatusPointsOfStatus(MPlayer.Status);
        StatusPointsNeededText.text = "STATUS POINTS NEEDED TO LEVEL UP: " + statusPointsNeeded.ToString();

        VIPStausNameText.text = VIPInfo.GetVIPStatusName(MPlayer.Status).ToUpper();

        StatusPointsNeededSlider.maxValue = statusPointsNeeded;
        StatusPointsNeededSlider.value = MPlayer.StatusPoints;

        if(statusPointsNeeded <= 0)
            StatusPointsGainedPercentage.text = ((MPlayer.StatusPoints * 100) / 1).ToString() + "%";
        else
            StatusPointsGainedPercentage.text = ((MPlayer.StatusPoints * 100) / statusPointsNeeded).ToString() + "%";

    }
}
