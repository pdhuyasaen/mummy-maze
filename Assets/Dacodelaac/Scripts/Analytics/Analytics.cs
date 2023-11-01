using System.Collections.Generic;
using Dacodelaac.Core;
using Dacodelaac.DebugUtils;
using UnityEngine;
using Event = Dacodelaac.Events.Event;

namespace Dacodelaac.Analytics
{
    public class Analytics : BaseMono
    {
        [SerializeField] Event analyticsInitializedEvent;
        [SerializeField] AbstractAnalyticsProvider[] providers;

        int initializedCount;

        void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            if (providers.Length == 0)
            {
                analyticsInitializedEvent.Raise();
                return;
            }

            initializedCount = 0;

            foreach (var provider in providers)
            {
                provider.Initialize(OnProviderInitialized);
            }
        }

        void OnProviderInitialized()
        {
            initializedCount++;
            if (initializedCount >= providers.Length)
            {
                Dacoder.Log("Analytics initialized!");
                analyticsInitializedEvent.Raise();
            }
        }

        public void LogEventName(string eventName)
        {
            foreach (var provider in providers)
            {
                provider.LogEvent(eventName);
            }
        }

        public void LogEvent(string eventName, Dictionary<string, object> param = null)
        {
            foreach (var provider in providers)
            {
                provider.LogEvent(eventName, param);
            }
        }

        #region Level

        public void OnLevelStart(Dictionary<string, object> param = null)
        {
            foreach (var provider in providers)
            {
                provider.OnLevelStart(param);
            }
        }

        public void OnLevelCompleted(Dictionary<string, object> param = null)
        {
            foreach (var provider in providers)
            {
                provider.OnLevelCompleted(param);
            }
        }

        public void OnLevelFailed(Dictionary<string, object> param = null)
        {
            foreach (var provider in providers)
            {
                provider.OnLevelFailed(param);
            }
        }

        public void OnLevelRestart(Dictionary<string, object> param = null)
        {
            foreach (var provider in providers)
            {
                provider.OnLevelRestart(param);
            }
        }

        #endregion

        #region Ads

        public void OnAdPaid(Dictionary<string, object> param = null)
        {
            foreach (var provider in providers)
            {
                provider.OnAdPaid(param);
            }
        }
        
        public void OnWatchRewardedAds()
        {
            foreach (var provider in providers)
            {
                provider.OnWatchRewarded();
            }
        }
        
        public void OnRequestRewardedAds()
        {
            foreach (var provider in providers)
            {
                provider.OnRequestReward();
            }
        }
        
        public void OnRequestInterstitialAds()
        {
            foreach (var provider in providers)
            {
                provider.OnRequestInterstitial();
            }
        }
        
        public void OnWatchInterstitialAds()
        {
            foreach (var provider in providers)
            {
                provider.OnWatchInterstitial();
            }
        }
        
        public void OnBannerShow()
        {
            foreach (var provider in providers)
            {
                provider.OnBannerShowed();
            }
        }

        #endregion
    }
}