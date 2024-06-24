using Mkey;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FreeSpinsController : MonoBehaviour
{
    [SerializeField]
    private PopUpsController dailyRewardPUPrefab;

    [SerializeField]
    private bool startFromZeroDayReward = false;
    [SerializeField]
    private bool repeatingReward = true;
    public Action<double, double> WorkingTimePassedEvent;
    public Action<int, int, int, float> WorkingTickRestDaysHourMinSecEvent;
    [HideInInspector]
    public UnityEvent TimePassEvent;

    #region properties
    public float RestDays { get; private set; }
    public float RestHours { get; private set; }
    public float RestMinutes { get; private set; }
    public float RestSeconds { get; private set; }
    public bool IsWork { get; private set; }
    private int NextHourlyFreeSpins { get; set; }
    public int RewardDay { get; private set; }
    public bool RepeatingReward { get { return repeatingReward; } }

    public static FreeSpinsController Instance;
    public bool IsDealTime { get; private set; }
    #endregion properties

    #region regular

    #region temp vars
    private GuiController MGui => GuiController.Instance;
    private int hours = 1;
    private int minutes = 0; // for test
    private GlobalTimer gTimer;
    private string timerName = "hourlyfreespins";
    private string nextHourlyFreeSpinsKey = "nexthourlyfreespins";
    private bool debug = false;
    private SoundMaster MSound => SoundMaster.Instance;
    #endregion temp vars

    private void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //Debug.Log("Awake: " + name);

        IsWork = false;
        LoadNextRewardDay();
        RewardDay = -1;

        // check existing timer and  last tick
        if (GlobalTimer.Exist(timerName))
        {
            if (debug) Debug.Log("timer exist: " + timerName);
            DateTime lT = GlobalTimer.GetLastTick(timerName);
            TimeSpan tS = DateTime.Now - lT;
            TimeSpan dRTS = new TimeSpan(hours, minutes, 0);

            if (tS > dRTS) // interrupted game
            {
                if (debug) Debug.Log("daily reward interrupted, timespan: " + tS);
                ResetNextRewardDay();
                StartNewTimer();
            }
            else
            {
                if (debug) Debug.Log("daily reward not interrupted, timespan: " + tS);
                StartExistingTimer();
            }
        }
        else
        {
            if (debug) Debug.Log("timer not exist: " + timerName);
            StartNewTimer();
            if (startFromZeroDayReward)
            {
                ResetNextRewardDay();
                RewardDay = 0;
                 IncNextRewardHour();
            }
        }
    }

    private void Update()
    {
        if (IsWork)
            gTimer.Update();
    }

    private void OnDestroy()
    {

    }
    #endregion regular

    #region reward day
    private void  IncNextRewardHour()
    {
        SetNextHourlyFreeSpins(NextHourlyFreeSpins + 1);
    }

    private void SetNextHourlyFreeSpins(int order)
    {
        NextHourlyFreeSpins = order;
        PlayerPrefs.SetInt(nextHourlyFreeSpinsKey, NextHourlyFreeSpins);
    }

    private void LoadNextRewardDay()
    {
        NextHourlyFreeSpins = PlayerPrefs.GetInt(nextHourlyFreeSpinsKey, 0);
    }

    private void ResetNextRewardDay()
    {
        if (debug) Debug.Log("reset reward day");
        SetNextHourlyFreeSpins(0);
    }
    #endregion reward day

    #region timerhandlers
    private void TickRestDaysHourMinSecHandler(int d, int h, int m, float s)
    {
        RestDays = d;
        RestHours = h;
        RestMinutes = m;
        RestSeconds = s;
        WorkingTickRestDaysHourMinSecEvent?.Invoke(d, h, m, s);
    }

    private void TimePassedHandler(double initTime, double realyTime)
    {
        if (debug) Debug.Log("time passed");
        IsWork = false;
        RewardDay = NextHourlyFreeSpins;
         IncNextRewardHour();
        //StartCoroutine(ShowRewardPopup());
        TimePassEvent?.Invoke();
        StartNewTimer();
    }
    #endregion timerhandlers
    private IEnumerator ShowRewardPopup()
    {
        yield return new WaitForSeconds(1f);
        MGui.ShowPopUp(dailyRewardPUPrefab);
    }

    #region timers
    private void StartNewTimer()
    {
        if (debug) Debug.Log("start new");
        TimeSpan ts = new TimeSpan(hours, minutes, 0);
        gTimer = new GlobalTimer(timerName, ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
        gTimer.TickRestDaysHourMinSecEvent += TickRestDaysHourMinSecHandler;
        gTimer.TimePassedEvent += TimePassedHandler;
        IsWork = true;
    }

    private void StartExistingTimer()
    {
        gTimer = new GlobalTimer(timerName);
        gTimer.TickRestDaysHourMinSecEvent += TickRestDaysHourMinSecHandler;
        gTimer.TimePassedEvent += TimePassedHandler;
        IsWork = true;
    }
    #endregion timers

    #region reward
    public void ApplyReward()
    {
        RewardDay = -1;
    }

    public void ResetData()
    {
        ResetNextRewardDay();
        GlobalTimer.RemoveTimerPrefs(timerName);
    }

    internal void NextHourTest()
    {
        RewardDay = NextHourlyFreeSpins;
         IncNextRewardHour();
    }
    #endregion reward
}

#if UNITY_EDITOR
[CustomEditor(typeof(FreeSpinsController))]
public class FreeSpinsControllerEditor : Editor
{
    private bool test = true;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (!EditorApplication.isPlaying)
        {
            if (test = EditorGUILayout.Foldout(test, "Test"))
            {
                EditorGUILayout.BeginHorizontal("box");
                if (GUILayout.Button("Reset reward"))
                {
                    FreeSpinsController t = (FreeSpinsController)target;
                    t.ResetData();
                }
                EditorGUILayout.EndHorizontal();
            }

        }
        else
        {
            if (GUILayout.Button("Next rew day"))
            {
                FreeSpinsController t = (FreeSpinsController)target;
                t.NextHourTest();
            }
        }
    }
}
#endif



