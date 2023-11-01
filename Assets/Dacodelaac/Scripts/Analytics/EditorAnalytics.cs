using System;
using System.Collections.Generic;
using Dacodelaac.DebugUtils;
using Dacodelaac.Utils;
using UnityEngine;

namespace Dacodelaac.Analytics
{
    [CreateAssetMenu(menuName = "Analytics/EditorAnalytics")]
    public class EditorAnalytics : AbstractAnalyticsProvider
    {
        public override void Initialize(Action onCompleted)
        {
            onCompleted?.Invoke();
        }

        public override void LogEvent(string eventName, Dictionary<string, object> param = null)
        {
            Dacoder.Log(eventName, param.ToString<string, object>());
        }

        public override void OnLevelStart(Dictionary<string, object> param = null)
        {
            Dacoder.Log(param.ToString<string, object>());
        }

        public override void OnLevelCompleted(Dictionary<string, object> param = null)
        {
            Dacoder.Log(param.ToString<string, object>());
        }

        public override void OnLevelFailed(Dictionary<string, object> param = null)
        {
            Dacoder.Log(param.ToString<string, object>());
        }

        public override void OnLevelRestart(Dictionary<string, object> param = null)
        {
            Dacoder.Log(param.ToString<string, object>());
        }

        public override void OnAdPaid(Dictionary<string, object> param = null)
        {
            Dacoder.Log(param.ToString<string, object>());
        }

        public override void OnWatchRewarded()
        {
            Dacoder.Log("OnWatchRewarded");
        }

        public override void OnRequestReward()
        {
            Dacoder.Log("OnRequestRewarded");
        }

        public override void OnRequestInterstitial()
        {
            Dacoder.Log("OnRequestInterstitial");
        }

        public override void OnWatchInterstitial()
        {
            Dacoder.Log("OnWatchInterstitial");
        }

        public override void OnBannerShowed()
        {
            Dacoder.Log("OnBannerShowed");
        }
    }
}