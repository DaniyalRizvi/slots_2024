using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Mkey;

public class FreeSpinsGUIController : MonoBehaviour
{
    [SerializeField]
    private PopUpsController freeSpinsPUPrefab;
    [SerializeField]
    private float delay;
    [SerializeField]
    private bool showOnlyAtStart = true;
    [SerializeField]
    private UnityEvent<string> TimeUpdateEvent;

    #region temp vars
    private GuiController MGui => GuiController.Instance;
    private FreeSpinsController DRC => FreeSpinsController.Instance;
    private int rewDay = -1;
    #endregion temp vars

    #region regular
    private IEnumerator Start()
    {
        while (!DRC) yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        DRC.TimePassEvent.AddListener(TimePassedEventHandler);
        DRC.WorkingTickRestDaysHourMinSecEvent += WorkingDealTickRestDaysHourMinSecHandler;
        DRC.WorkingTimePassedEvent += WorkingDealTimePassedHandler;
        rewDay = DRC.RewardDay;
        if (rewDay >= 0)
        {
            StartCoroutine(ShowRewardPopup());
        }
    }

    private void OnDestroy()
    {
        if (DRC) DRC.TimePassEvent.RemoveListener(TimePassedEventHandler);
    }
    #endregion regular

    private IEnumerator ShowRewardPopup()
    {
        yield return new WaitForSeconds(delay);
        MGui.ShowPopUp(freeSpinsPUPrefab);
    }

    private void TimePassedEventHandler()
    {
        rewDay = DRC.RewardDay;
        if (rewDay >= 0)
        {
            if (!showOnlyAtStart) StartCoroutine(ShowRewardPopup());
        }
    }

    private void WorkingDealTickRestDaysHourMinSecHandler(int d, int h, int m, float s)
    {
        TimeUpdateEvent?.Invoke(String.Format("{0:00}:{1:00}:{2:00}", h, m, s));
    }

    private void WorkingDealTimePassedHandler(double initTime, double realyTime)
    {
        TimeUpdateEvent?.Invoke(String.Format("{0:00}:{1:00}:{2:00}", 0, 0, 0));
    }
}