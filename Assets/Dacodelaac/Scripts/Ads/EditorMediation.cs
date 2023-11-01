using System;
using Dacodelaac.DebugUtils;
using UnityEngine;
using Event = Dacodelaac.Events.Event;
using Random = UnityEngine.Random;

namespace Dacodelaac.Ads
{
    [CreateAssetMenu(menuName = "Ads/Editor Mediation")]
    public class EditorMediation : AbstractMediation
    {
        [SerializeField, Range(0, 1)] float interstitialLoadSuccessProbability = 1;
        [SerializeField, Range(0, 1)] float rewardedLoadSuccessProbability = 1;
        
        public override void Initialize(Action onCompleted)
        {
            onCompleted?.Invoke();
        }

        public override void OnApplicationPaused(bool isPaused)
        {
        }

        public override void OnAdPaid(string id, double value, string network, string placement, string adFormat)
        {
            Dacoder.Log(id, value, network, placement, adFormat);
        }
        
        public override event Action OnBannerLoaded;
        public override event Action<string> OnBannerFailedToLoad;
        public override event Action OnBannerOpening;
        public override event Action OnBannerClosed;
        public override event Action OnBannerClicked;
        public override event Action OnBannerLeavingApplication;
        public override event Action<string, double, string, string, string> OnBannerAdPaid;

        public override void LoadBanner(BannerPosition bannerPosition)
        {
            OnBannerLoaded?.Invoke();
        }

        public override void ShowBanner()
        {
            OnBannerOpening?.Invoke();
        }

        public override void HideBanner()
        {
            
        }

        public override event Action OnInterstitialLoaded;
        public override event Action<string> OnInterstitialFailedToLoad;
        public override event Action OnInterstitialShow;
        public override event Action<string> OnInterstitialFailedToShow;
        public override event Action OnInterstitialClicked;
        public override event Action OnInterstitialClosed;
        public override event Action OnInterstitialLeavingApplication;
        public override event Action<string, double, string, string, string> OnInterstitialAdPaid;
        public override bool IsInterstitialAvailable => isInterstitialAvailable;

        bool isInterstitialAvailable;
        public override void LoadInterstitial()
        {
            isInterstitialAvailable = Random.value <= interstitialLoadSuccessProbability;
            if (isInterstitialAvailable)
            {
                OnInterstitialLoaded?.Invoke();
            }
            else
            {
                OnInterstitialFailedToLoad?.Invoke("editor interstitial load failed");
            }
        }

        public override void ShowInterstitial()
        {
            isInterstitialAvailable = false;
            OnInterstitialShow?.Invoke();
        }
        
        public void CloseInterstitial()
        {
            OnInterstitialClosed?.Invoke();
            OnInterstitialAdPaid?.Invoke("interstitial", 1234, "editor", "placement", "adFormat");
        }

        public override event Action OnRewardedLoaded;
        public override event Action<string> OnRewardedFailedToLoad;
        public override event Action OnRewardedShow;
        public override event Action<string> OnRewardedFailedToShow;
        public override event Action OnRewardedClicked;
        public override event Action OnRewardedUserEarnedReward;
        public override event Action OnRewardedClosed;
        public override event Action<string, double, string, string, string> OnRewardedAdPaid;
        public override bool IsRewardedAvailable => isRewardedAvailable;
        
        bool isRewardedAvailable;
        
        public override void LoadRewarded()
        {
            isRewardedAvailable = Random.value <= rewardedLoadSuccessProbability;
            if (isRewardedAvailable)
            {
                OnRewardedLoaded?.Invoke();
            }
            else
            {
                OnRewardedFailedToLoad?.Invoke("rewarded interstitial load failed");
            }
        }

        public override void ShowRewarded()
        {
            isRewardedAvailable = false;
            OnRewardedShow?.Invoke();
        }

        public void CompleteRewarded()
        {
            OnRewardedUserEarnedReward?.Invoke();
            OnRewardedAdPaid?.Invoke("rewarded", 1234, "editor", "placement", "adFormat");
        }

        public void CloseRewarded()
        {
            OnRewardedClosed?.Invoke();
        }
    }
}