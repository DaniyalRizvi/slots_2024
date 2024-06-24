using Mkey;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class RandomBoostersController : MonoBehaviour
{
    [SerializeField]
    private bool startFromZeroDayReward = false;
    [SerializeField]
    private bool repeatingReward = true;
    public Action<double, double> WorkingTimePassedEvent;
    public Action<int, int, int, float> WorkingTickRestDaysHourMinSecEvent;
    public Action<int, int, int, float> BoosterRestMinEvent;

    [HideInInspector]
    public UnityEvent TimePassEvent;

    #region properties
    public float RestDays { get; private set; }
    public float RestHours { get; private set; }
    public float RestMinutes { get; private set; }
    public float RestSeconds { get; private set; }
    public bool IsWork { get; private set; }
    private int NextBooster { get; set; }
    public int RewardDay { get; private set; }
    public bool RepeatingReward { get { return repeatingReward; } }

    public float BoosterRestMinutes { get; private set; }
    public float BoosterRestSeconds { get; private set; }

    public static RandomBoostersController Instance;

    #endregion properties

    #region regular

    #region temp vars
    private GuiController MGui => GuiController.Instance;
    private int hours = 0;
    private int minutes = 8; // for test
    private int boosterMinutes = 10; 
    private GlobalTimer gTimer;
    private GlobalTimer boosterGTimer;
    private string timerName = "randomboosters";
    private string boosterExpireTimerName = "boosterexpiretime";
    private string nextRandomBoostersKey = "nextrandomboosters";
    private bool debug = true;
    private SoundMaster MSound => SoundMaster.Instance;
    #endregion temp vars

    private void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Debug.Log("Awake: " + name);

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
        {
            gTimer.Update();
            boosterGTimer.Update();
        }
    }

    private void OnDestroy()
    {

    }
    #endregion regular

    #region reward day
    private void IncNextRewardHour()
    {
        SetNextRandomBoosters(NextBooster + 1);
    }

    private void SetNextRandomBoosters(int order)
    {
        NextBooster = order;
        PlayerPrefs.SetInt(nextRandomBoostersKey, NextBooster);
    }

    private void LoadNextRewardDay()
    {
        NextBooster = PlayerPrefs.GetInt(nextRandomBoostersKey, 0);
    }

    private void ResetNextRewardDay()
    {
        if (debug) Debug.Log("reset reward day");
        SetNextRandomBoosters(0);
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

    private void TickBoosterRestMinHandler(int d, int h, int m, float s)
    {
        BoosterRestMinutes = m;
        BoosterRestSeconds = s;
        //WorkingTickRestDaysHourMinSecEvent?.Invoke(d, h, m, s);
    }

    private void BoosterTimeExpiredHandler(double initTime, double realyTime)
    {
        if(PlayerPrefs.GetInt("2xXP!", 1) == 2)
        {
            PlayerPrefs.SetInt("2xXP!", 1);
            FindObjectOfType<LevelGUIController>().ChangeToNormalSlider();
        }
        
        if(PlayerPrefs.GetInt("2xCoin!", 1) == 2)
        {
            PlayerPrefs.SetInt("2xCoin!", 1);
            AllWinsMultiplied wins = FindObjectOfType<AllWinsMultiplied>();
            if(wins != null)
            {
                wins.HideIndicator();
            }
        }
        Debug.Log("EXPIRING BOOSTERS");
    }

    private async void TimePassedHandler(double initTime, double realyTime)
    {
        await WaitForGUIController();
        if (debug) Debug.Log("time passed");
        IsWork = false;
        RewardDay = NextBooster;
        IncNextRewardHour();
        //StartCoroutine(ShowRewardPopup());
        TimePassEvent?.Invoke();
        StartNewTimer();
    }

    async Task WaitForGUIController()
    {
        while (FindObjectOfType<GuiController>().transform.childCount > 0)
        {
            await Task.Yield();
        }
    }
    #endregion timerhandlers
    //private IEnumerator ShowRewardPopup()
    //{
    //    yield return new WaitForSeconds(1f);
    //    MGui.ShowPopUp(dailyRewardPUPrefab);
    //}

    #region timers
    private void StartNewTimer()
    {
        if (debug) Debug.Log("start new");
        TimeSpan ts = new TimeSpan(hours, minutes, 0);
        gTimer = new GlobalTimer(timerName, ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
        gTimer.TickRestDaysHourMinSecEvent += TickRestDaysHourMinSecHandler;
        gTimer.TimePassedEvent += TimePassedHandler;
        //gTimer.TickRestDaysHourMinSecEvent += TickBoosterRestMinHandler;
        //StartCoroutine(BoosterTimer());
        IsWork = true;

        TimeSpan BoosterTs = new TimeSpan(0, boosterMinutes, 0);
        boosterGTimer = new GlobalTimer(boosterExpireTimerName, BoosterTs.Days, BoosterTs.Hours, BoosterTs.Minutes, BoosterTs.Seconds);
        boosterGTimer.TickRestDaysHourMinSecEvent += TickBoosterRestMinHandler;
        boosterGTimer.TimePassedEvent += BoosterTimeExpiredHandler;
    }

    private void StartExistingTimer()
    {
        gTimer = new GlobalTimer(timerName);
        gTimer.TickRestDaysHourMinSecEvent += TickRestDaysHourMinSecHandler;
        gTimer.TimePassedEvent += TimePassedHandler;
        //gTimer.TickRestDaysHourMinSecEvent += TickBoosterRestMinHandler;
        //StartCoroutine(BoosterTimer());
        IsWork = true;

        boosterGTimer = new GlobalTimer(boosterExpireTimerName);
        boosterGTimer.TickRestDaysHourMinSecEvent += TickBoosterRestMinHandler;
        boosterGTimer.TimePassedEvent += BoosterTimeExpiredHandler;
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
        RewardDay = NextBooster;
        IncNextRewardHour();
    }
    #endregion reward
}

#if UNITY_EDITOR
[CustomEditor(typeof(RandomBoostersController))]
public class RandomBoostersControllerEditor : Editor
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
                    RandomBoostersController t = (RandomBoostersController)target;
                    t.ResetData();
                }
                EditorGUILayout.EndHorizontal();
            }

        }
        else
        {
            if (GUILayout.Button("Next rew day"))
            {
                RandomBoostersController t = (RandomBoostersController)target;
                t.NextHourTest();
            }
        }
    }
}
#endif
