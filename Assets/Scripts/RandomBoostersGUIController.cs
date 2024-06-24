using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Mkey;

public class RandomBoostersGUIController : MonoBehaviour
{
    [SerializeField]
    private PopUpsController RandomBooster2xXP;
    [SerializeField]
    private PopUpsController RandomBooster2xCoins;
    [SerializeField]
    private float delay;
    [SerializeField]
    private bool showOnlyAtStart = true;
    [SerializeField]
    private UnityEvent<string> TimeUpdateEvent;

    #region temp vars
    private GuiController MGui => GuiController.Instance;
    private RandomBoostersController RBC => RandomBoostersController.Instance;
    private int rewDay = -1;
    #endregion temp vars

    #region regular
    private IEnumerator Start()
    {
        while (!RBC) yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        RBC.TimePassEvent.AddListener(TimePassedEventHandler);
        RBC.WorkingTickRestDaysHourMinSecEvent += WorkingDealTickRestDaysHourMinSecHandler;
        RBC.WorkingTimePassedEvent += WorkingDealTimePassedHandler;
        rewDay = RBC.RewardDay;
        if (rewDay >= 0)
        {
            //StartCoroutine(ShowRewardPopup());
        }
    }

    private void OnDestroy()
    {
        if (RBC) RBC.TimePassEvent.RemoveListener(TimePassedEventHandler);
    }
    #endregion regular

    private IEnumerator ShowRewardPopup()
    {
        yield return new WaitForSeconds(delay);
        float randomBoosterProb = UnityEngine.Random.Range(0f, 1f);

        if (randomBoosterProb < 0.5f)
        {
            if(PlayerPrefs.GetInt("2xXP!", 1) == 1)
                MGui.ShowPopUp(RandomBooster2xXP);
            else if(PlayerPrefs.GetInt("2xCoin!", 1) == 1)
                MGui.ShowPopUp(RandomBooster2xCoins);
        }
        else if (randomBoosterProb > 0.5f)
        {
            if (PlayerPrefs.GetInt("2xCoin!", 1) == 1)
                MGui.ShowPopUp(RandomBooster2xCoins);
            else if (PlayerPrefs.GetInt("2xXP!", 1) == 1)
                MGui.ShowPopUp(RandomBooster2xXP);
        }
    }

    private void TimePassedEventHandler()
    {
        rewDay = RBC.RewardDay;
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
