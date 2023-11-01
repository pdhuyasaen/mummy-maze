#if FIREBASE
using System;
using System.Collections.Generic;
using System.Linq;
using Dacodelaac.DebugUtils;
using Dacodelaac.Utils;
using Firebase.Analytics;
using UnityEngine;

namespace Dacodelaac.Analytics
{
    [CreateAssetMenu(menuName = "Analytics/Firebase")]
    public class FirebaseAnalytics : AbstractAnalyticsProvider
    {
        bool ready;
        
        public override void Initialize(Action onCompleted)
        {
            ready = true;
            if (Application.platform == RuntimePlatform.Android)
            {
                // Android => We must resolve the dependencies first
                ready = false;
                Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    var dependencyStatus = task.Result;
                    if (dependencyStatus == Firebase.DependencyStatus.Available)
                    {
                        var app = Firebase.FirebaseApp.DefaultInstance;
                        ready = true;
                        // fetch data in here
                        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
                        onCompleted?.Invoke();
                    }
                    else
                    {
                        Debug.LogErrorFormat("Firebase.FirebaseApp.CheckAndFixDependenciesAsync: Could not resolve all Firebase dependencies: {0}", dependencyStatus);
                    }
                });
            }
            else
            {
                var app = Firebase.FirebaseApp.DefaultInstance;
                onCompleted?.Invoke();
                Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
            }
            
            
        }
        
        public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
            Debug.Log("Received Registration Token: " + token.Token);
        }

        public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
            Debug.Log("Received a new message from: " + e.Message.From);
        }
        
        public override void LogEvent(string eventName, Dictionary<string, object> param = null)
        {
            Dacoder.Log("Firebase.LogEvent", eventName, param.ToString<string, object>());
            
            if (param == null)
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName);
            }
            else
            {
                var parameters = new List<Parameter>();
                parameters.AddRange(param.Select(p => new Parameter(p.Key, p.Value.ToString())));
                Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, parameters.ToArray());
            }
        }

        public override void OnLevelStart(Dictionary<string, object> param = null)
        {
            LogEvent("LEVEL_START", new Dictionary<string, object>()
            {
                {"LEVEL_INDEX_NAME", $"Level_index-{param["index"]}_name-{param["name"]}"},
            });
        }

        public override void OnLevelCompleted(Dictionary<string, object> param = null)
        {
            LogEvent("LEVEL_COMPLETED", new Dictionary<string, object>()
            {
                {"LEVEL_INDEX_NAME", $"Level_index-{param["index"]}_name-{param["name"]}"},
            });
        }

        public override void OnLevelFailed(Dictionary<string, object> param = null)
        {
            LogEvent("LEVEL_FAILED", new Dictionary<string, object>()
            {
                {"LEVEL_INDEX_NAME", $"Level_index-{param["index"]}_name-{param["name"]}"},
            });
        }

        public override void OnLevelRestart(Dictionary<string, object> param = null)
        {
            LogEvent("LEVEL_RESTART", new Dictionary<string, object>()
            {
                {"LEVEL_INDEX_NAME", $"Level_index-{param["index"]}_name-{param["name"]}"}
            });
        }
        
        public override void OnAdPaid(Dictionary<string, object> param = null)
        {
            Dacoder.Log("Firebase.OnAdPaid", param.ToString<string, object>());
            if (param != null)
            {
                var impressionParameters = new[]
                {
                    new Parameter("ad_platform", param["mediation"].ToString()),
                    new Parameter("ad_source", param["network"].ToString()),
                    new Parameter("ad_unit_name", param["unit"].ToString()),
                    new Parameter("ad_format", param["adFormat"].ToString()),
                    new Parameter("value", (double)param["revenue"]),
                    new Parameter("currency", param["currency"].ToString())
                };
                Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
            }
        }

        public override void OnWatchRewarded()
        {
            LogEvent("WATCH_REWARD");
        }

        public override void OnRequestReward()
        {
            LogEvent("REQUEST_REWARD");
        }

        public override void OnRequestInterstitial()
        {
            LogEvent("REQUEST_INTERSTITIAL");
        }

        public override void OnWatchInterstitial()
        {
            LogEvent("WATCH_INTERSTITIAL");
        }

        public override void OnBannerShowed()
        {
            LogEvent("BANNER_SHOW");
        }
    }
}
#endif