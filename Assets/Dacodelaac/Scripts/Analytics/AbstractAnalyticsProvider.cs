using System.Collections.Generic;
using Dacodelaac.Core;

namespace Dacodelaac.Analytics
{
    public abstract class AbstractAnalyticsProvider : BaseSO
    {
        public abstract void Initialize(System.Action onCompleted);
        public abstract void LogEvent(string eventName, Dictionary<string, object> param = null);

        #region Level
        public abstract void OnLevelStart(Dictionary<string, object> param = null);
        public abstract void OnLevelCompleted(Dictionary<string, object> param = null);
        public abstract void OnLevelFailed(Dictionary<string, object> param = null);
        public abstract void OnLevelRestart(Dictionary<string, object> param = null);

        #endregion

        #region GameSpecifics

        #endregion

        #region Ads

        public abstract void OnAdPaid(Dictionary<string, object> param = null);
        
        public abstract void OnWatchRewarded();

        public abstract void OnRequestReward();
        public abstract void OnRequestInterstitial();
        public abstract void OnWatchInterstitial();
        public abstract void OnBannerShowed();
        
        #endregion
    }
}