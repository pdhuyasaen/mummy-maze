#if MAX
using System;
using System.Collections.Generic;
using Dacodelaac.Events;
using UnityEngine;

namespace Dacodelaac.Ads
{
    [CreateAssetMenu(menuName = "Ads/MAX Mediation")]
    public class MaxMediation : AbstractMediation
    {
        [SerializeField] string sdkKey;
        [Header("Android")]
        [SerializeField] string androidRewardedUnitKey;
        [SerializeField] string androidInterstitialUnitKey;
        [SerializeField] string androidBannerUnitKey;
        [Header("iOS")]
        [SerializeField] string iOSRewardedUnitKey;
        [SerializeField] string iOSInterstitialUnitKey;
        [SerializeField] string iOSBannerUnitKey;
        [Header("Events")]
        [SerializeField] DictionaryEvent analyticsAdPaidEvent;
        
#if UNITY_ANDROID
        string RewardedUnitKey => androidRewardedUnitKey;
        string InterstitialUnitKey => androidInterstitialUnitKey;
        string BannerUnitKey => androidBannerUnitKey;
#else
        string RewardedUnitKey => iOSRewardedUnitKey;
        string InterstitialUnitKey => iOSInterstitialUnitKey;
        string BannerUnitKey => iOSBannerUnitKey;
#endif

        public override void Initialize(Action onCompleted)
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += config =>
            {
#if !DACODER_RELEASE
                MaxSdk.ShowMediationDebugger();
#endif
                onCompleted?.Invoke();
            };

            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += (unit, info) => OnRewardedLoaded?.Invoke();
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += (unit, error) => OnRewardedFailedToLoad?.Invoke(error.Message);
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += (unit, info) => OnRewardedShow?.Invoke();
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += (unit, info) => OnRewardedClicked?.Invoke();
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += (unit, info) => OnRewardedClosed?.Invoke();
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += (unit, error, info) => OnRewardedFailedToShow?.Invoke(error.Message);
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += (unit, reward, info) => OnRewardedUserEarnedReward?.Invoke();
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += (unit, info) => OnRewardedAdPaid?.Invoke(info.AdUnitIdentifier, info.Revenue, info.NetworkName, info.Placement, info.AdFormat);
            
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += (unit, info) => OnInterstitialLoaded?.Invoke();
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += (unit, error) => OnInterstitialFailedToLoad?.Invoke(error.Message);
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += (unit, info) => OnInterstitialShow?.Invoke();
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += (unit, info) => OnInterstitialClicked?.Invoke();
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += (unit, info) => OnInterstitialClosed?.Invoke();
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += (unit, error, info) => OnInterstitialFailedToShow?.Invoke(error.Message);
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += (unit, info) => OnInterstitialAdPaid?.Invoke(info.AdUnitIdentifier, info.Revenue, info.NetworkName, info.Placement, info.AdFormat);

            MaxSdkCallbacks.Banner.OnAdLoadedEvent += (unit, info) => OnBannerLoaded?.Invoke();
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += (unit, error) => OnBannerFailedToLoad?.Invoke(error.Message);
            MaxSdkCallbacks.Banner.OnAdExpandedEvent += (unit, info) => OnBannerOpening?.Invoke();
            MaxSdkCallbacks.Banner.OnAdCollapsedEvent += (unit, info) => OnBannerClosed?.Invoke();
            MaxSdkCallbacks.Banner.OnAdClickedEvent += (unit, info) => OnBannerClicked?.Invoke();
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent +=  (unit, info) => OnBannerAdPaid?.Invoke(info.AdUnitIdentifier, info.Revenue, info.NetworkName, info.Placement, info.AdFormat);

            MaxSdk.SetSdkKey(sdkKey);
            MaxSdk.SetUserId(SystemInfo.deviceUniqueIdentifier);
            MaxSdk.SetVerboseLogging(true);
            MaxSdk.InitializeSdk();
        }

        public override void OnApplicationPaused(bool isPaused) { }
        
        public override void OnAdPaid(string unitName, double revenue, string network, string placement, string adFormat)
        {
            analyticsAdPaidEvent.Raise(new Dictionary<string, object>()
            {
                {"mediation", "applovin_max_sdk"}, {"revenue", revenue}, {"currency", "USD"}, {"network", network},
                {"placement", placement}, {"unit", unitName}, {"adFormat", adFormat}
            });
        }

        public override event Action OnBannerLoaded;
        public override event Action<string> OnBannerFailedToLoad;
        public override event Action OnBannerOpening;
        public override event Action OnBannerClosed;
        public override event Action OnBannerLeavingApplication;
        public override event Action<string, double, string, string, string> OnBannerAdPaid;
        public override event Action OnBannerClicked;

        public override void LoadBanner(BannerPosition bannerPosition)
        {
            MaxSdkBase.BannerPosition pos;
            switch (bannerPosition)
            {
                case BannerPosition.Top:
                    pos = MaxSdkBase.BannerPosition.TopCenter;
                    break;
                case BannerPosition.Bottom:
                    pos = MaxSdkBase.BannerPosition.BottomCenter;
                    break;
                case BannerPosition.TopLeft:
                    pos = MaxSdkBase.BannerPosition.TopLeft;
                    break;
                case BannerPosition.TopRight:
                    pos = MaxSdkBase.BannerPosition.TopRight;
                    break;
                case BannerPosition.BottomLeft:
                    pos = MaxSdkBase.BannerPosition.BottomLeft;
                    break;
                case BannerPosition.BottomRight:
                    pos = MaxSdkBase.BannerPosition.BottomRight;
                    break;
                case BannerPosition.Center:
                    pos = MaxSdkBase.BannerPosition.Centered;
                    break;
                default:
                    pos = MaxSdkBase.BannerPosition.BottomCenter;
                    break;
            }

            MaxSdk.CreateBanner(BannerUnitKey, pos);
        }

        public override void ShowBanner()
        {
            MaxSdk.ShowBanner(BannerUnitKey);
        }

        public override void HideBanner()
        {
            MaxSdk.HideBanner(BannerUnitKey);
        }

        public override event Action OnInterstitialLoaded;
        public override event Action<string> OnInterstitialFailedToLoad;
        public override event Action OnInterstitialShow;
        public override event Action<string> OnInterstitialFailedToShow;
        public override event Action OnInterstitialClicked;
        public override event Action OnInterstitialClosed;
        public override event Action OnInterstitialLeavingApplication;
        public override event Action<string, double, string, string, string> OnInterstitialAdPaid;
        public override bool IsInterstitialAvailable => MaxSdk.IsInterstitialReady(InterstitialUnitKey);
        public override void LoadInterstitial() { MaxSdk.LoadInterstitial(InterstitialUnitKey); }

        public override void ShowInterstitial() { MaxSdk.ShowInterstitial(InterstitialUnitKey); }

        public override event Action OnRewardedLoaded;
        public override event Action<string> OnRewardedFailedToLoad;
        public override event Action OnRewardedShow;
        public override event Action<string> OnRewardedFailedToShow;
        public override event Action OnRewardedClicked;
        public override event Action OnRewardedUserEarnedReward;
        public override event Action OnRewardedClosed;
        public override event Action<string, double, string, string, string> OnRewardedAdPaid;
        public override bool IsRewardedAvailable => MaxSdk.IsRewardedAdReady(RewardedUnitKey);
        public override void LoadRewarded() { MaxSdk.LoadRewardedAd(RewardedUnitKey); }

        public override void ShowRewarded() { MaxSdk.ShowRewardedAd(RewardedUnitKey); }
    }
}
#endif