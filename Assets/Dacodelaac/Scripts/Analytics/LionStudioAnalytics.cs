// #define LION
#if LION
using System;
using System.Collections.Generic;
using Dacodelaac.DebugUtils;
using Dacodelaac.Utils;
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Debugging;

namespace Dacodelaac.AnalyticsSystem
{
    public class LionStudioAnalytics : IAnalytics
    {
        public void Initialize(Action onCompleted)
        {
            LionAnalytics.GameStart();
            LionDebugger.Hide();
            onCompleted?.Invoke();
        }

        public void LogEvent(string eventName, Dictionary<string, object> param = null)
        {
            
        }

        public void OnLevelStart(Dictionary<string, object> param = null)
        {
            Dacoder.Log("LionStudioAnalytics.OnLevelStart", param.ToString<string, object>());
            LionAnalytics.LevelStart((int)param["level"], (int)param["attemptNum"]);
        }

        public void OnLevelCompleted(Dictionary<string, object> param = null)
        {
            Dacoder.Log("LionStudioAnalytics.OnLevelCompleted", param.ToString<string, object>());
            LionAnalytics.LevelComplete((int)param["level"], (int)param["attemptNum"]);
        }
        
        public void OnLevelFailed(Dictionary<string, object> param = null)
        {
            Dacoder.Log("LionStudioAnalytics.OnLevelFailed", param.ToString<string, object>());
            LionAnalytics.LevelFail((int)param["level"], (int)param["attemptNum"]);
        }

        public void OnLevelRestart(Dictionary<string, object> param = null)
        {
            Dacoder.Log("LionStudioAnalytics.OnLevelRestart", param.ToString<string, object>());
            LionAnalytics.LevelRestart((int)param["level"], (int)param["attemptNum"]);
        }

        public void OnItemPurchased(Dictionary<string, object> param = null)
        {
            
        }

        public void OnRevivedClicked(Dictionary<string, object> param = null)
        {
            
        }

        public void OnPowerUpClick(Dictionary<string, object> param = null)
        {
            
        }

        public void OnPowerUpUsage(Dictionary<string, object> param = null)
        {
            
        }

        public void OnAdImpRwX(int count, Dictionary<string, object> param = null)
        {
            
        }

        public void OnRwCheck(Dictionary<string, object> param = null)
        {
            
        }

        public void OnRwAvailable(Dictionary<string, object> param = null)
        {
            
        }

        public void OnRwNotAvailable(Dictionary<string, object> param = null)
        {
            
        }

        public void OnRwShowLoadFailed(Dictionary<string, object> param = null)
        {
            
        }

        public void OnRwOpened(Dictionary<string, object> param = null)
        {
            
        }

        public void OnRwClosed(Dictionary<string, object> param = null)
        {
            
        }

        public void OnAdImpIsX(int count, Dictionary<string, object> param = null)
        {
            
        }

        public void OnFsOpportunity(Dictionary<string, object> param = null)
        {
            
        }

        public void OnFsOpportunityCancelled(Dictionary<string, object> param = null)
        {
            
        }

        public void OnFsLoad(Dictionary<string, object> param = null)
        {
            
        }

        public void OnFsLoadFailed(Dictionary<string, object> param = null)
        {
            
        }

        public void OnFsReady(Dictionary<string, object> param = null)
        {
            
        }

        public void OnFsNotReady(Dictionary<string, object> param = null)
        {
            
        }

        public void OnFsShowFailed(Dictionary<string, object> param = null)
        {
            
        }

        public void OnFsShow(Dictionary<string, object> param = null)
        {
            
        }

        public void OnFsClosed(Dictionary<string, object> param = null)
        {
            
        }

        public void OnFsClicked(Dictionary<string, object> param = null)
        {
            
        }
    }
}
#endif