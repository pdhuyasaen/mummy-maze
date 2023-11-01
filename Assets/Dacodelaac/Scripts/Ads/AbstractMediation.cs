using System;
using Dacodelaac.Core;
using UnityEngine;

namespace Dacodelaac.Ads
{
    public abstract class AbstractMediation : BaseSO
    {
        public abstract void Initialize(Action onCompleted);
        public abstract void OnApplicationPaused(bool isPaused);
        public abstract void OnAdPaid(string id, double value, string network, string placement, string adFormat);

        #region Banner

        public abstract event Action OnBannerLoaded;
        public abstract event Action<string> OnBannerFailedToLoad;
        public abstract event Action OnBannerOpening;
        public abstract event Action OnBannerClosed;
        public abstract event Action OnBannerClicked;
        public abstract event Action OnBannerLeavingApplication;
        public abstract event Action<string, double, string, string, string> OnBannerAdPaid;

        public abstract void LoadBanner(BannerPosition bannerPosition);
        public abstract void ShowBanner();
        public abstract void HideBanner();

        #endregion

        #region Interstitial

        public abstract event Action OnInterstitialLoaded;
        public abstract event Action<string> OnInterstitialFailedToLoad;
        public abstract event Action OnInterstitialShow;
        public abstract event Action<string> OnInterstitialFailedToShow;
        public abstract event Action OnInterstitialClicked;
        public abstract event Action OnInterstitialClosed;
        public abstract event Action OnInterstitialLeavingApplication;
        public abstract event Action<string, double, string, string, string> OnInterstitialAdPaid;

        public abstract bool IsInterstitialAvailable { get; }
        public abstract void LoadInterstitial();
        public abstract void ShowInterstitial();

        #endregion

        #region Rewarded

        public abstract event Action OnRewardedLoaded;
        public abstract event Action<string> OnRewardedFailedToLoad;
        public abstract event Action OnRewardedShow;
        public abstract event Action<string>  OnRewardedFailedToShow;
        public abstract event Action OnRewardedClicked;
        public abstract event Action OnRewardedUserEarnedReward;
        public abstract event Action OnRewardedClosed;
        public abstract event Action<string, double, string, string, string> OnRewardedAdPaid; 

        public abstract bool IsRewardedAvailable { get; }
        public abstract void LoadRewarded();
        public abstract void ShowRewarded();

        #endregion
    }
}