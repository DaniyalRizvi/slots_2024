using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
#endif

/*
    18.05.2021
    add   internal void SetHaveSpin()
 */
namespace Mkey
{
	public class DailySpinController : MonoBehaviour
	{
        [SerializeField] private GameObject SpinWheelEffects;

        [SerializeField]
        private PopUpsController screenPrefab;
        [SerializeField]
        private Text timerText;
        [HideInInspector]
        public UnityEvent TimePassEvent;
        [SerializeField]
        private MkeyFW.FortuneWheelInstantiator fwInstantiator;

        #region temp vars
        private int hours = 1;
        private int minutes = 0; // for test
        private int speedUpHours = 1;
        private int speedUpMinutes = 0; // for test
        private GlobalTimer gTimer;
        private GlobalTimer speedUpTimer;
        private PopUpsController screen;
        private string timerName = "dailySpinTimer";
        private string speedUpTimerName = "speedUpSpinTimer";
        private bool debug = false;
        private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
        private GuiController MGui { get { return GuiController.Instance; } }
        private SoundMaster MSound { get { return SoundMaster.Instance; } }
        #endregion temp vars

        #region properties
        public float RestDays { get; private set; }
        public float RestHours { get; private set; }
        public float RestMinutes { get; private set; }
        public float RestSeconds { get; private set; }

        public bool IsWork { get; private set; }
        public static DailySpinController Instance { get; private set; }
        public static bool HaveDailySpin { get; private set; }
        #endregion properties

        [SerializeField] private GameObject SpinButton;
        [SerializeField] private GameObject SpeedUpButton;

        public PopUpsController SpeedUpSpinPU;

        public bool speedUpAvailed;
        #region regular
        private void Awake()
        {
            //PlayerPrefs.SetInt("FirstTimeSpin", 0);

            

            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            Debug.Log("Awake: " + name);

            if (PlayerPrefs.GetInt("speedUpAvailed", 0) == 1)
            {
                speedUpAvailed = true;
                SpeedUpButton.GetComponent<Image>().color = new Color(0.4f, 0.36f, 0.34f);
            }
        }

        private void Start()
        {
            
            if (timerText) timerText.text = "";
            IsWork = false;

            // set fortune wheel event handlers
            fwInstantiator.SpinResultEvent += (coins, isBigWin) =>
            {
                //MPlayer.AddCoins((int)(coins * MPlayer.StatusMultiplier));
                HaveDailySpin = false;
                StartNewTimer();
                if (fwInstantiator.MiniGame) fwInstantiator.MiniGame.SetBlocked(!HaveDailySpin, true);
            }; 

            fwInstantiator.CreateEvent +=(MkeyFW.WheelController wc)=>
            {
                if (screenPrefab) screen = MGui.ShowPopUp(screenPrefab);
                wc.SetBlocked(!HaveDailySpin, false);
            };

            fwInstantiator.CloseEvent += () => 
            {
                if (screen) screen.CloseWindow();
                if (timerText) timerText.text = "";
            };


            if (!HaveDailySpin)
            {
                // check existing timer and  last tick
                if (GlobalTimer.Exist(timerName))
                {
                    if(speedUpAvailed)
                    {
                        if (GlobalTimer.Exist(speedUpTimerName))
                        {
                            speedUpTimer = new GlobalTimer(speedUpTimerName);
                            speedUpTimer.TickRestDaysHourMinSecEvent += TickSpeedUpRestDaysHourMinSecHandler;
                            speedUpTimer.TimePassedEvent += TimePassedSpeedUpHandler;
                        }
                    }
                    StartExistingTimer();
                }
                else
                {
                    if (debug) Debug.Log("timer not exist: " + timerName);
                    StartNewTimer();
                }
            }
            if (PlayerPrefs.GetInt("FirstTimeSpin", 0) == 0)
            {
                SetHaveSpin();
                PlayerPrefs.SetInt("FirstTimeSpin", 1);
            }
        }

        private void Update()
        {
            if (IsWork)
            {
                gTimer.Update();
                if(speedUpAvailed)
                {
                    speedUpTimer.Update();
                }
            }
        }
        #endregion regular

        #region timerhandlers
        private void TickRestDaysHourMinSecHandler(int d, int h, int m, float s)
        {
            RestDays = d;
            RestHours = h;
            RestMinutes = m;
            RestSeconds = s;
            if(timerText /*&& fwInstantiator.MiniGame*/) timerText.text = String.Format("{0:00}:{1:00}:{2:00}", h, m, s);
        }
        private void TickSpeedUpRestDaysHourMinSecHandler(int d, int h, int m, float s)
        {
            RestDays = d;
            RestHours = h;
            RestMinutes = m;
            RestSeconds = s;
        }

        private void TimePassedHandler(double initTime, double realyTime)
        {
            IsWork = false;
            if (timerText) timerText.text = "";
            HaveDailySpin = true;
            Debug.Log("time passed daily spin");
            if (fwInstantiator.MiniGame)
            {
                Debug.Log("time passed daily spin - > start mini game");
                fwInstantiator.MiniGame.SetBlocked(!HaveDailySpin, false);
            }

            if (debug) Debug.Log("daily spin timer time passed, have daily spin");

            SpinButton.SetActive(true);
            SpeedUpButton.SetActive(false);
            SpinWheelEffects.SetActive(true);

            TimePassEvent?.Invoke();
        }
        private void TimePassedSpeedUpHandler(double initTime, double realyTime)
        {
            speedUpAvailed = false;
            PlayerPrefs.SetInt("speedUpAvailed", 0);
            SpeedUpButton.GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
        #endregion timerhandlers

        #region timers
        private void StartNewTimer()
        {
            if (debug) Debug.Log("start new daily spin timer");
            if(speedUpAvailed)
            {
                hours = 0;
                minutes = 10;
            }
            else
            {
                hours = 1;
                minutes = 0;
            }
            TimeSpan ts = new TimeSpan(hours, minutes, 0);
            gTimer = new GlobalTimer(timerName, ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
            gTimer.TickRestDaysHourMinSecEvent += TickRestDaysHourMinSecHandler;
            gTimer.TimePassedEvent += TimePassedHandler;
            IsWork = true;

            SpinButton.SetActive(false);
            SpeedUpButton.SetActive(true);
            SpinWheelEffects.SetActive(false);
        }

        public void SpeedUpButtonPressed()
        {
            if (MPlayer.Gems >= 3)
            {
                SpeedUpButton.GetComponent<Image>().color = new Color(0.4f, 0.36f, 0.34f);
                speedUpAvailed = true;
                PlayerPrefs.SetInt("speedUpAvailed", 1);
                TimeSpan ts = new TimeSpan(speedUpHours, speedUpMinutes, 0);
                speedUpTimer = new GlobalTimer(speedUpTimerName, ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
                speedUpTimer.TickRestDaysHourMinSecEvent += TickSpeedUpRestDaysHourMinSecHandler;
                speedUpTimer.TimePassedEvent += TimePassedSpeedUpHandler;

                MPlayer.AddGems(-3);
                StartNewTimer();
            }
        }

        private void StartExistingTimer()
        {
            if (debug) Debug.Log("start existing daily spin timer");
            gTimer = new GlobalTimer(timerName);
            gTimer.TickRestDaysHourMinSecEvent += TickRestDaysHourMinSecHandler;
            gTimer.TimePassedEvent += TimePassedHandler;
            IsWork = true;

            SpinButton.SetActive(false);
            SpeedUpButton.SetActive(true);
            SpinWheelEffects.SetActive(false);
        }
        #endregion timers

        public void OpenSpinGame()
        {
            fwInstantiator.Create(false);
            if (fwInstantiator.MiniGame)
            {
                fwInstantiator.MiniGame.BackGroundButton.clickEvent.AddListener(()=> { fwInstantiator.ForceClose(); });
            }
        }

        public void CloseSpinGame()
        {
            fwInstantiator.Close();
        }

        public void ResetData()
        {
            GlobalTimer.RemoveTimerPrefs(timerName);
        }

        internal void SetHaveSpin()
        {
            if (HaveDailySpin) return;
            HaveDailySpin = true;
            if (gTimer != null)
            {
                gTimer.RemoveTimerPrefs();
                IsWork = false;
            }
            if (timerText) timerText.text = "";
            SpinButton.SetActive(true);
            SpeedUpButton.SetActive(false);
            SpinWheelEffects.SetActive(true);

            if (fwInstantiator.MiniGame)
            {
                Debug.Log("time passed daily spin - > start mini game");
                fwInstantiator.MiniGame.SetBlocked(false, false);
            }
        }
        public void ShowSpeedUpSpinPU()
        {
            if (!speedUpAvailed)
            {
                GetComponent<ShowGuiPopUp>().ShowPopUp(SpeedUpSpinPU);
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DailySpinController))]
    public class DailySpinControllerEditor : Editor
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
                    if (GUILayout.Button("Reset Data"))
                    {
                        DailySpinController t = (DailySpinController)target;
                        t.ResetData();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal("box");
                if (GUILayout.Button("Set daily spin"))
                {
                    DailySpinController t = (DailySpinController)target;
                    t.SetHaveSpin();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
#endif
}
