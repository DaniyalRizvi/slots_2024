using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mkey
{
    public class AdsControl : MonoBehaviour
    {
        private BannerControl bannerControl;

        private bool customBannerRequest = false;
        private bool customInterstitialRequest = false;
        private bool customRewardedAdRequest = false;
        [Header("App ID Android")] [SerializeField] private string AndroidAppID;
        [Header("App ID IOS")] [SerializeField] private string IOSAppID;
        [Header("Test Mode")]
        [SerializeField]
        private bool isTesting = true;
        [Header("Child Protection")]
        [SerializeField]
        private bool isChildDirected = true;
       
        [Header("Banner")]
        [SerializeField]
        private bool requestBanner = true;
      //  [SerializeField]
     //   private string bannerAdUnitIdAndroid = "ca-app-pub-3940256099942544/6300978111"; // test
      //  [SerializeField]
      //  private string bannerAdUnitIdIos = "ca-app-pub-3940256099942544/2934735716"; // test

        [Header("Interstitial")]
        [SerializeField]
        private bool requestInterstitial = true;
      //  [SerializeField]
      //  private string interstitialAdUnitIdAndroid = "ca-app-pub-3940256099942544/1033173712";
      //  [SerializeField]
      //  private string interstitialAdUnitIdIos = "ca-app-pub-3940256099942544/4411468910";

        [Header("Rewarded ads")]
        [SerializeField]
        private bool requestRewardedAds = true;
    //    [SerializeField]
     //   private List<RewardAd> rewardAds;

        private float deltaTime = 0.0f;
        private Action<bool, string, double> rewardCallBack; 
        private Action rewardedOpenedCallBack; 
        private Action rewardedClosedCallBack; 
        private Action interstitialOpenedCallBack; 
        private Action interstitialClosedCallBack;
        private static string outputMessage = string.Empty;

        public static AdsControl Instance;

        #region regular
        private void Awake()
        {
            if (Instance) Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            IronSource.Agent.validateIntegration();
            
            if(isChildDirected)
                IronSource.Agent.setMetaData("is_child_directed","true");
            if(isTesting)
                IronSource.Agent.setMetaData("is_test_suite", "enable"); 
#if UNITY_ANDROID
            IronSource.Agent.init(AndroidAppID);
#elif UNITY_IPHONE
            IronSource.Agent.init(IOSAppID);
#endif
            if (requestRewardedAds) IronSource.Agent.loadRewardedVideo();

            if (requestInterstitial) IronSource.Agent.loadInterstitial();

            if (requestBanner)
            {
                IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
            }
            else requestBanner = false;
        }

        void OnEnable()
        {
            IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            IronSourceBannerEvents.onAdLoadedEvent += HandleOnAdLoaded;
            IronSourceBannerEvents.onAdLoadFailedEvent += HandleOnAdFailed;
            IronSourceInterstitialEvents.onAdReadyEvent += HandleInterstitialLoaded;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += HandleInterstitialFailedToLoad;
            IronSourceInterstitialEvents.onAdClosedEvent += HandleOnAdClosed;
            IronSourceRewardedVideoEvents.onAdClosedEvent += HandleRewardBasedVideoClosed;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += HandleRewardBasedVideoRewarded;
        }

        void OnDisable()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            IronSourceBannerEvents.onAdLoadedEvent -= HandleOnAdLoaded;
            IronSourceBannerEvents.onAdLoadFailedEvent -= HandleOnAdFailed;
            IronSourceInterstitialEvents.onAdReadyEvent -= HandleInterstitialLoaded;
            IronSourceInterstitialEvents.onAdLoadFailedEvent -= HandleInterstitialFailedToLoad;
            IronSourceInterstitialEvents.onAdClosedEvent -= HandleOnAdClosed;
            IronSourceRewardedVideoEvents.onAdClosedEvent -= HandleRewardBasedVideoClosed;
            IronSourceRewardedVideoEvents.onAdRewardedEvent -= HandleRewardBasedVideoRewarded;
        }
        private void SdkInitializationCompletedEvent(){
      
            if(isTesting)
                IronSource.Agent.launchTestSuite();
        }
        void OnActiveSceneChanged(Scene previousScene, Scene newScene)
        {
            if (requestBanner)
            {
                IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
            }
        }

        public bool IsBanner()
        {
            return requestBanner;
        }

        private void Update()
        {
            // Calculate simple moving average for time to render screen. 0.1 factor used as smoothing value.
            deltaTime += (Time.deltaTime - this.deltaTime) * 0.1f;
        }

        private void OnDestroy()
        {
            IronSource.Agent.destroyBanner();
        }
        #endregion regular

        #region banner
        public void HandleOnAdLoaded(IronSourceAdInfo adInfo)
        {
            print("HandleAdLoaded event received");
            bannerControl = FindObjectOfType<BannerControl>();
            bannerControl.ShowBannerActions();
        }
        public void HandleOnAdFailed(IronSourceError error)
        {
            Debug.LogError("Banner view failed to load an ad with error : " + error.getDescription());
            bannerControl = FindObjectOfType<BannerControl>();
            bannerControl.HideBannerActions();
        }

        public void RequestBanner()
        {
            IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
        }

        public void HideBanner()
        {
            IronSource.Agent.hideBanner();
        }

        public void ShowBanner()
        {
            IronSource.Agent.displayBanner();
        }
        #endregion banner

        #region interstitial
        private void RequestInterstitial()
        {
            IronSource.Agent.loadInterstitial();
        }

        public void ShowInterstitial(Action interstitialOpenedCallBack, Action interstitialClosedCallBack)
        {
            this.interstitialOpenedCallBack = interstitialOpenedCallBack;
            this.interstitialClosedCallBack = interstitialClosedCallBack;
            if (IronSource.Agent.isInterstitialReady())
            {
                IronSource.Agent.showInterstitial();
            }
            else
            {
                Debug.LogError("Interstitial ad is not ready yet.");
                RequestInterstitial();
            }
        }

        public void HandleInterstitialLoaded(IronSourceAdInfo adInfo)
        {
            print("HandleInterstitialLoaded event received");
        }

        public void HandleInterstitialFailedToLoad(IronSourceError error)
        {
            Debug.LogError("Interstitial ad failed to load with error: " + error.getDescription());
        }

        public void HandleOnAdClosed(IronSourceAdInfo adInfo)
        {
            interstitialClosedCallBack?.Invoke();
        }
        #endregion interstitial

        #region rewarded ad
        private void CreateAndLoadRewardedAd()
        {
            IronSource.Agent.loadRewardedVideo();
        }

        public void ShowRewardedAd(string adName, Action rewardedOpenedCallBack, Action rewardedClosedCallBack, Action<bool, string, double> rewardCallBack)
        {
            this.rewardedOpenedCallBack = rewardedOpenedCallBack;
            this.rewardedClosedCallBack = rewardedClosedCallBack;
            this.rewardCallBack = rewardCallBack;

            if (IronSource.Agent.isRewardedVideoAvailable())
            {
                IronSource.Agent.showRewardedVideo();
            }
            else
            {
                Debug.Log("Rewarded ad is not available.");
                CreateAndLoadRewardedAd();
            }
        }

        public void HandleRewardBasedVideoClosed(IronSourceAdInfo adInfo)
        {
            rewardedClosedCallBack?.Invoke();
        }

        public void HandleRewardBasedVideoRewarded(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            rewardCallBack?.Invoke(true, placement.getRewardName(), placement.getRewardAmount());
        }
        #endregion rewarded ad
    }

    [Serializable]
    public class RewardAd
    {
        [SerializeField]
        private string name = "rewardedad";
        [SerializeField]
        private string adUnitIdAndroid = "ca-app-pub-3940256099942544/5224354917";  // test
        [SerializeField]
        private string adUnitIdIOS = "ca-app-pub-3940256099942544/1712485313";      // test

        public string Name { get { return name; } }
        private Action<bool, string, double> rewardCallBack;
        private Action rewardedOpenedCallBack;
        private Action rewardedClosedCallBack;

        private string adUnitId = "";

        public void CreateAndLoadRewardedAd()
        {
#if UNITY_ANDROID
            adUnitId = adUnitIdAndroid;
#elif UNITY_IPHONE
            adUnitId = adUnitIdIOS;
#else
            adUnitId = "unexpected_platform";
#endif
            IronSource.Agent.loadRewardedVideo();
        }

        public void ShowRewardedAd(Action rewardedOpenedCallBack, Action rewardedClosedCallBack, Action<bool, string, double> rewardCallBack)
        {
            this.rewardedOpenedCallBack = rewardedOpenedCallBack;
            this.rewardedClosedCallBack = rewardedClosedCallBack;
            this.rewardCallBack = rewardCallBack;

            if (IronSource.Agent.isRewardedVideoAvailable())
            {
                IronSource.Agent.showRewardedVideo();
            }
            else
            {
                Debug.Log("Rewarded ad is not available.");
                CreateAndLoadRewardedAd();
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AdsControl))]
    public class AdsControlEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUIStyle sl = new GUIStyle(EditorStyles.label);
            sl.margin = new RectOffset(0,0,-15,-15);


            // GUILayout.Space(16);
            // GUILayout.Label("Android test ad units IDs ", EditorStyles.boldLabel);
            // EditorGUILayout.SelectableLabel("Banner:         ca-app-pub-3940256099942544/6300978111", sl);
            // EditorGUILayout.SelectableLabel("Interstitial:   ca-app-pub-3940256099942544/1033173712", sl);
            // EditorGUILayout.SelectableLabel("Rewarded Video: ca-app-pub-3940256099942544/5224354917", sl);
            // GUILayout.Space(8);
            // GUILayout.Label("IOS test ad units IDs ", EditorStyles.boldLabel);
            // EditorGUILayout.SelectableLabel("Banner:         ca-app-pub-3940256099942544/2934735716", sl);
            // EditorGUILayout.SelectableLabel("Interstitial:   ca-app-pub-3940256099942544/4411468910", sl);
            // EditorGUILayout.SelectableLabel("Rewarded Video: ca-app-pub-3940256099942544/1712485313", sl);


            GUILayout.Space(8);
            GUILayout.Label("IronSource links:", EditorStyles.boldLabel);
            if (LinkLabel(new GUIContent("Get Started")))
            {
                Application.OpenURL("https://developers.is.com/ironsource-mobile/unity/levelplay-starter-kit/");
            }
            if (LinkLabel(new GUIContent("Banner Ads")))
            {
                Application.OpenURL("https://developers.is.com/ironsource-mobile/unity/banner-integration-unity/");
            }
            if (LinkLabel(new GUIContent("Interstitial Ads")))
            {
                Application.OpenURL("https://developers.is.com/ironsource-mobile/unity/interstitial-integration-unity/");
            }
            if (LinkLabel(new GUIContent("Rewarded Ads")))
            {
                Application.OpenURL("https://developers.is.com/ironsource-mobile/unity/rewarded-video-integration-unity/");
            }
            if (LinkLabel(new GUIContent("Test Ads")))
            {
                Application.OpenURL("https://developers.is.com/ironsource-mobile/unity/unity-levelplay-test-suite/#step-1");
            }
        }

       private bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
        {
            var position = GUILayoutUtility.GetRect(label, EditorStyles.linkLabel, options);

            Handles.BeginGUI();
            Handles.color = EditorStyles.linkLabel.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
            Handles.color = Color.white;
            Handles.EndGUI();

            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            return GUI.Button(position, label, EditorStyles.linkLabel);
        }
    }
#endif
}

