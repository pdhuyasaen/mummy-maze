#if ADJUST
using System;
using System.Collections.Generic;
using com.adjust.sdk;
using Dacodelaac.DebugUtils;
using Dacodelaac.Utils;
using UnityEngine;

namespace Dacodelaac.Analytics
{
    [CreateAssetMenu(menuName = "Analytics/Adjust")]
    public class AdjustAnalytics : AbstractAnalyticsProvider
    {
        [SerializeField] string adjustAppToken = "7zffljai7lhc";

        public override void Initialize(Action onCompleted)
        {
            var adjustConfig = new AdjustConfig(
                adjustAppToken,
                AdjustEnvironment.Production, // AdjustEnvironment.Sandbox to test in dashboard
                true
            );
            adjustConfig.setLogLevel(AdjustLogLevel.Info); // AdjustLogLevel.Suppress to disable logs
            adjustConfig.setSendInBackground(true);
            new GameObject("Adjust").AddComponent<Adjust>(); // do not remove or rename
            // Adjust.addSessionCallbackParameter("foo", "bar"); // if requested to set session-level parameters
            //adjustConfig.setAttributionChangedDelegate((adjustAttribution) => {
            //  Debug.LogFormat("Adjust Attribution Callback: ", adjustAttribution.trackerName);
            //});
            Adjust.start(adjustConfig);
            
            onCompleted?.Invoke();
        }

        public override void LogEvent(string eventName, Dictionary<string, object> param = null)
        {
            Dacoder.Log("Adjust.LogEvent", eventName, param.ToString<string, object>());
            Adjust.trackEvent(new AdjustEvent(eventName));
        }

        public override void OnLevelStart(Dictionary<string, object> param = null)
        {
        }

        public override void OnLevelCompleted(Dictionary<string, object> param = null)
        {
        }

        public override void OnLevelFailed(Dictionary<string, object> param = null)
        {
        }

        public override void OnLevelRestart(Dictionary<string, object> param = null)
        {
        }

        public override void OnAdPaid(Dictionary<string, object> param = null)
        {
            Dacoder.Log("Adjust.OnAdPaid", param.ToString<string, object>());
            if (param != null)
            {
                var adRevenue = new AdjustAdRevenue(param["mediation"].ToString());
                adRevenue.setRevenue((double)param["revenue"], param["currency"].ToString());
                adRevenue.setAdRevenueNetwork(param["network"].ToString());
                adRevenue.setAdRevenuePlacement(param["placement"].ToString());
                adRevenue.setAdRevenueUnit(param["unit"].ToString());
                Adjust.trackAdRevenue(adRevenue);
            }
        }
    }
}
#endif