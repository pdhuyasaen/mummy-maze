#if GA
using System;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

namespace Dacodelaac.AnalyticsSystem
{
    public class GAAnalytics : IAnalytics, IGameAnalyticsATTListener
    {
        Action onCompleted;
        
        public void Initialize(Action onCompleted)
        {
            this.onCompleted = onCompleted;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                GameAnalytics.RequestTrackingAuthorization(this);
            }
            else
            {
                GameAnalytics.Initialize();
                this.onCompleted?.Invoke();
                this.onCompleted = null;
            }
        }

        public void GameAnalyticsATTListenerNotDetermined()
        {
            GameAnalytics.Initialize();
            onCompleted?.Invoke();
            onCompleted = null;
        }

        public void GameAnalyticsATTListenerRestricted()
        {
            GameAnalytics.Initialize();
            onCompleted?.Invoke();
            onCompleted = null;
        }

        public void GameAnalyticsATTListenerDenied()
        {
            GameAnalytics.Initialize();
            onCompleted?.Invoke();
            onCompleted = null;
        }

        public void GameAnalyticsATTListenerAuthorized()
        {
            GameAnalytics.Initialize();
            onCompleted?.Invoke();
            onCompleted = null;
        }

        public void LogEvent(string eventName, Dictionary<string, object> param = null)
        {
            
        }

        public void OnLevelStart(Dictionary<string, object> param = null)
        {
            
        }

        public void OnLevelCompleted(Dictionary<string, object> param = null)
        {
            
        }
        
        public void OnLevelFailed(Dictionary<string, object> param = null)
        {
        }

        public void OnLevelRestart(Dictionary<string, object> param = null)
        {
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