using System;
using System.Collections.Generic;
using Dacodelaac.Core;
using Dacodelaac.DebugUtils;
using Dacodelaac.Events;
using Dacodelaac.RemoteConfig;
using Dacodelaac.Utils;
using Dacodelaac.Variables;
using UnityEngine;
using Event = Dacodelaac.Events.Event;

namespace Dacodelaac.Ads
{
    public class Ads : BaseMono
    {
        [SerializeField] AbstractMediation deviceMediation;
#if UNITY_EDITOR
        [SerializeField] EditorMediation editorMediation;
#endif
        [SerializeField] Event adsInitializedEvent;

        [Header("Rewarded")]
        [SerializeField] bool rewarded = true;
        [SerializeField] float maxRewardedDelayTime = 30f;
        [SerializeField] Event rewardedShowedEvent;
        [SerializeField] Event rewardedClosedEvent;
        [SerializeField] Event rewardedChangedEvent;
        [SerializeField] Event rewardedRequestEvent;

        [Header("Interstitial")]
        [SerializeField] bool interstitial = true;
        [SerializeField] float maxInterstitialDelayTime = 30f;
        [SerializeField] Config showInterval;
        [SerializeField] Config showIntervalLevel;
        [SerializeField] Config minLevel;
        [SerializeField] Config levelStartShowInter;
        [SerializeField] Event interstitialShowedEvent;
        [SerializeField] Event interstitialClosedEvent;
        [SerializeField] Event interstitialRequestEvent;
        [SerializeField] IntegerVariable currentLevel;

        [Header("Banner")]
        [SerializeField] bool banner = true;
        [SerializeField] float maxBannerDelayTime = 30f;
        [SerializeField] BannerPosition bannerPos = BannerPosition.Bottom;
        [SerializeField] Event bannerShowedEvent;
        [SerializeField] Event bannerHidEvent;

        [Header("NoAds")] [SerializeField] private BooleanVariable noAds;

#if UNITY_EDITOR
        AbstractMediation Mediation => editorMediation;
#else
        AbstractMediation Mediation => deviceMediation;
#endif

        int playedLevelCount;
        int interstitialPlayedLevelCountReset;

        float bannerDelayTime;
        float rewardedDelayTime;
        float interstitialDelayTime;
        float interstitialLastTimeShow;

        AdsShowRewardedData adsShowRewardedData;
        
        readonly Queue<Action> queue = new Queue<Action>();

        void Start()
        {
            DontDestroyOnLoad(gameObject);

            Dacoder.Log("Initialize");
            playedLevelCount = interstitialPlayedLevelCountReset = 0;

            Mediation.Initialize(() =>
            {
                LoadAll();
                adsInitializedEvent.Raise();
            });
        }
        
        void Update()
        {
            lock (queue)
            {
                if (queue.Count > 0)
                {
                    queue.Dequeue().Invoke();    
                }
            }
        }

        public void Enqueue(Action action)
        {
            lock (queue)
            {
                queue.Enqueue(action);    
            }
        }

        void LoadAll()
        {
            if (banner)
            {
                AddBannerListeners();
                LoadBanner();
            }

            if (interstitial)
            {
                AddInterstitialListeners();
                LoadInterstitial();
            }

            if (rewarded)
            {
                AddRewardedListeners();
                LoadRewarded();
            }
        }

        public void OnLevelEnded()
        {
            playedLevelCount++;
            interstitialPlayedLevelCountReset++;
        }

        #region Banner

        void AddBannerListeners()
        {
            Mediation.OnBannerLoaded += () => Enqueue(OnBannerLoaded);
            Mediation.OnBannerFailedToLoad += e => Enqueue(OnBannerFailedToLoad);
            Mediation.OnBannerOpening += () => Enqueue(OnBannerOpening);
            Mediation.OnBannerClosed += () => Enqueue(OnBannerClosed);
            Mediation.OnBannerLeavingApplication += () => Enqueue(OnBannerLeavingApplication);
            Mediation.OnBannerAdPaid += (s, d, arg3, arg4, arg5) =>
                Enqueue(() => Mediation.OnAdPaid(s, d, arg3, arg4, arg5));
        }

        void DelayLoadBanner()
        {
            bannerDelayTime = Mathf.Min(bannerDelayTime + 5f, maxBannerDelayTime);
            Dacoder.Log("DelayLoadBanner", bannerDelayTime);
            this.Delay(bannerDelayTime, true, LoadBanner);
        }

        void LoadBanner()
        {
            Dacoder.Log("LoadBanner");
            Mediation.LoadBanner(bannerPos);
        }

        public void ShowBanner()
        {
            if (noAds.Value) return;
            Dacoder.Log("ShowBanner");
            Mediation.ShowBanner();
            bannerShowedEvent.Raise();
        }

        public void HideBanner()
        {
            Dacoder.Log("HideBanner");
            Mediation.HideBanner();
            bannerHidEvent.Raise();
        }

        void OnBannerLoaded()
        {
            bannerDelayTime = 0f;
            Dacoder.Log("OnBannerLoaded");
            ShowBanner();
        }

        void OnBannerFailedToLoad()
        {
            Dacoder.Log("OnBannerFailedToLoad");
            DelayLoadBanner();
        }

        void OnBannerOpening()
        {
            Dacoder.Log("OnBannerOpening");
        }

        void OnBannerClosed()
        {
            Dacoder.Log("OnBannerClosed");
        }

        void OnBannerLeavingApplication()
        {
            Dacoder.Log("OnBannerLeavingApplication");
        }

        #endregion

        #region Rewarded

        void AddRewardedListeners()
        {
            Mediation.OnRewardedLoaded += () => Enqueue(OnRewardedLoaded);
            Mediation.OnRewardedFailedToLoad += e => Enqueue(() => OnRewardedFailedToLoad(e));
            Mediation.OnRewardedShow += () => Enqueue(OnRewardedShow);
            Mediation.OnRewardedFailedToShow += e => Enqueue(() => OnRewardedFailedToShow(e));
            Mediation.OnRewardedClicked += () => Enqueue(OnRewardedClicked);
            Mediation.OnRewardedUserEarnedReward +=
                () => Enqueue(OnRewardedUserEarnedReward);
            Mediation.OnRewardedClosed += () => Enqueue(OnRewardedClosed);
            Mediation.OnRewardedAdPaid += (s, d, arg3, arg4, arg5) =>
                Enqueue(() => Mediation.OnAdPaid(s, d, arg3, arg4, arg5));
        }

        public bool IsRewardedAvailable()
        {
            Dacoder.Log("IsRewardedAvailable");
            return Mediation.IsRewardedAvailable;
        }

        void DelayLoadRewarded()
        {
            rewardedDelayTime = Mathf.Min(rewardedDelayTime + 5f, maxRewardedDelayTime);
            Dacoder.Log("DelayLoadRewarded", rewardedDelayTime);
            this.Delay(rewardedDelayTime, true, LoadRewarded);
        }

        void LoadRewarded()
        {
            Dacoder.Log("LoadRewarded");
            Mediation.LoadRewarded();
        }

        public void ShowRewarded(AdsShowRewardedData data)
        {
            adsShowRewardedData = data;
            rewardedRequestEvent.Raise();
            if (IsRewardedAvailable())
            {
                adsShowRewardedData.OnAvailable?.Invoke();
                Dacoder.Log("ShowRewarded");
                rewardedShowedEvent.Raise();
                Mediation.ShowRewarded();
            }
            else
            {
                adsShowRewardedData.OnNotAvailable?.Invoke();
            }
        }

        void OnRewardedLoaded()
        {
            rewardedDelayTime = 0;
            Dacoder.Log("OnRewardedLoaded");
            rewardedChangedEvent.Raise();
        }

        void OnRewardedFailedToLoad(string error)
        {
            Dacoder.Log("OnRewardedFailedToLoad", error);
            DelayLoadRewarded();
        }

        void OnRewardedShow()
        {
            Dacoder.Log("OnRewardedShow");
        }

        void OnRewardedFailedToShow(string error)
        {
            Dacoder.Log("OnRewardedFailedToShow", error);
            DelayLoadRewarded();
        }

        void OnRewardedClicked()
        {
            Dacoder.Log("OnRewardedClicked");
        }

        void OnRewardedUserEarnedReward()
        {
            Dacoder.Log("OnRewardedUserEarnedReward");

            adsShowRewardedData.OnCompleted?.Invoke();
        }

        void OnRewardedClosed()
        {
            Dacoder.Log("OnRewardedClosed");
            
            adsShowRewardedData.OnClosed?.Invoke();
            rewardedClosedEvent.Raise();
            rewardedChangedEvent.Raise();

            ResetInterstitialInterval();
            LoadRewarded();
        }

        #endregion

        #region Interstitial

        void AddInterstitialListeners()
        {
            Mediation.OnInterstitialLoaded += () => Enqueue(OnInterstitialLoaded);
            Mediation.OnInterstitialFailedToLoad += e => Enqueue(() => OnInterstitialFailedToLoad(e));
            Mediation.OnInterstitialShow += () => Enqueue(OnInterstitialShow);
            Mediation.OnInterstitialFailedToShow += e => Enqueue(() => OnInterstitialFailedToShow(e));
            Mediation.OnInterstitialClicked += () => Enqueue(OnInterstitialClicked);
            Mediation.OnInterstitialClosed += () => Enqueue(OnInterstitialClosed);
            Mediation.OnInterstitialLeavingApplication += () => Enqueue(OnInterstitialLeavingApplication);
            Mediation.OnInterstitialAdPaid += (s, d, arg3, arg4, arg5) =>
                Enqueue(() => Mediation.OnAdPaid(s, d, arg3, arg4, arg5));
        }

        public bool IsInterstitialAvailable()
        {
            if (currentLevel.Value < levelStartShowInter.Value.LongValue)
            {
                return false;
            }
            
            if (playedLevelCount < minLevel.Value.LongValue)
            {
                return false;
            }

            if (Time.time - interstitialLastTimeShow < showInterval.Value.DoubleValue)
            {
                return false;
            }

            if (interstitialPlayedLevelCountReset < showIntervalLevel.Value.LongValue)
            {
                return false;
            }

            if (!Mediation.IsInterstitialAvailable)
            {
                return false;
            }

            return true;
        }

        void DelayLoadInterstitial()
        {
            interstitialDelayTime = Mathf.Min(interstitialDelayTime + 5f, maxInterstitialDelayTime);
            Dacoder.Log("DelayLoadInterstitial", interstitialDelayTime);
            this.Delay(interstitialDelayTime, true, () => LoadInterstitial());
        }

        void LoadInterstitial()
        {
            Dacoder.Log("LoadInterstitial");
            Mediation.LoadInterstitial();
        }

        public void ShowInterstitial()
        {
            interstitialRequestEvent.Raise();
            if (IsInterstitialAvailable() && !noAds.Value)
            {
                Dacoder.Log("ShowInterstitial");
                interstitialShowedEvent.Raise();
                Mediation.ShowInterstitial();   
            }
        }

        void OnInterstitialLoaded()
        {
            interstitialDelayTime = 0;
            Dacoder.Log("OnInterstitialLoaded");
        }

        void OnInterstitialFailedToLoad(string error)
        {
            Dacoder.Log("OnInterstitialFailedToLoad", error);
            DelayLoadInterstitial();
        }

        void OnInterstitialShow()
        {
            Dacoder.Log("OnInterstitialShow");
        }

        void OnInterstitialFailedToShow(string error)
        {
            Dacoder.Log("OnInterstitialFailedToShow", error);
            DelayLoadInterstitial();
        }

        void OnInterstitialClicked()
        {
            Dacoder.Log("OnInterstitialClicked");
        }

        void OnInterstitialClosed()
        {
            Dacoder.Log("OnInterstitialClosed");
            interstitialClosedEvent.Raise();
            ResetInterstitialInterval(true);
            LoadInterstitial();
        }

        void OnInterstitialLeavingApplication()
        {
            Dacoder.Log("OnInterstitialLeavingApplication");
        }

        void ResetInterstitialInterval(bool resetPlayedLevelCount = false)
        {
            interstitialLastTimeShow = Time.time;
            if (resetPlayedLevelCount)
            {
                interstitialPlayedLevelCountReset = 0;
            }
        }

        #endregion
    }

    public enum BannerPosition
    {
        Top = 0,
        Bottom = 1,
        TopLeft = 2,
        TopRight = 3,
        BottomLeft = 4,
        BottomRight = 5,
        Center = 6
    }
}