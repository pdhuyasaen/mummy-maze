#if IRON_SOURCE
using System;

namespace Dacodelaac.Ads
{
    public class IronSourceMediation : IMediation
    {
#if UNITY_ANDROID
        const string AppKey = "10a3eccd5";
        const string AdmobKey = "ca-app-pub-1605194961462621~3715539335";
#elif UNITY_IOS
        const string AppKey = "1271500dd";
        const string AdmobKey = "ca-app-pub-1605194961462621~2297622636";
#endif
        
        public void Initialize(Action onCompleted)
        {
            IronSource.Agent.init(AppKey);

            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += x =>
            {
                if (x) OnRewardedLoaded?.Invoke();
            };
            IronSourceEvents.onRewardedVideoAdOpenedEvent += () => OnRewardedOpening?.Invoke();
            IronSourceEvents.onRewardedVideoAdClickedEvent += x => OnRewardedClicked?.Invoke();
            IronSourceEvents.onRewardedVideoAdClosedEvent += () => OnRewardedClosed?.Invoke();
            IronSourceEvents.onRewardedVideoAdRewardedEvent += x => OnRewardedUserEarnedReward?.Invoke();
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += x => OnRewardedFailedToShow?.Invoke(x.ToString());
            IronSourceEvents.onRewardedVideoAdStartedEvent += () => { };
            IronSourceEvents.onRewardedVideoAdEndedEvent += () => { };

            IronSourceEvents.onInterstitialAdReadyEvent += () => OnInterstitialLoaded?.Invoke();
            IronSourceEvents.onInterstitialAdLoadFailedEvent += (x) => OnInterstitialFailedToLoad?.Invoke(x.ToString());
            IronSourceEvents.onInterstitialAdShowSucceededEvent += () => { };
            IronSourceEvents.onInterstitialAdShowFailedEvent += (x) => OnInterstitialFailedToShow?.Invoke(x.ToString());
            IronSourceEvents.onInterstitialAdClickedEvent += () => { };
            IronSourceEvents.onInterstitialAdOpenedEvent += () => OnInterstitialOpening?.Invoke();
            IronSourceEvents.onInterstitialAdClosedEvent += () => OnInterstitialClosed?.Invoke();

            IronSourceEvents.onBannerAdLoadedEvent += () => OnBannerLoaded?.Invoke();
            IronSourceEvents.onBannerAdLoadFailedEvent += x => OnBannerFailedToLoad?.Invoke(x.ToString());
            IronSourceEvents.onBannerAdScreenPresentedEvent += () => OnBannerOpening?.Invoke();
            IronSourceEvents.onBannerAdScreenDismissedEvent += () => OnBannerClosed?.Invoke();
            IronSourceEvents.onBannerAdClickedEvent += () => OnBannerClicked?.Invoke();
            IronSourceEvents.onBannerAdLeftApplicationEvent += () => OnBannerLeavingApplication?.Invoke();

            onCompleted?.Invoke();

#if DACODER_RELEASE
            IronSource.Agent.setAdaptersDebug(false);
#else
            IronSource.Agent.setAdaptersDebug(true);
            IronSource.Agent.validateIntegration();
#endif
        }

        public void OnApplicationPaused(bool isPaused)
        {
            IronSource.Agent.onApplicationPause(isPaused);
        }

        public event Action OnBannerLoaded;
        public event Action<string> OnBannerFailedToLoad;
        public event Action OnBannerOpening;
        public event Action OnBannerClosed;
        public event Action OnBannerLeavingApplication;
        public event Action OnBannerClicked;
        
        public void LoadBanner(Ads.BannerPosition bannerPosition)
        {
            var pos = IronSourceBannerPosition.BOTTOM;
            switch (bannerPosition)
            {
                case Ads.BannerPosition.Top:
                    pos = IronSourceBannerPosition.TOP;
                    break;
                default:
                    pos = IronSourceBannerPosition.BOTTOM;
                    break;
            }
            IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, pos);
        }

        public void ShowBanner()
        {
            IronSource.Agent.displayBanner();
        }

        public event Action OnInterstitialLoaded;
        public event Action<string> OnInterstitialFailedToLoad;
        public event Action OnInterstitialOpening;
        public event Action<string> OnInterstitialFailedToShow;
        public event Action OnInterstitialClicked;
        public event Action OnInterstitialClosed;
        public event Action OnInterstitialLeavingApplication;
        public bool IsInterstitialAvailable => IronSource.Agent.isInterstitialReady();
        public void LoadInterstitial()
        {
            IronSource.Agent.loadInterstitial();
        }

        public void ShowInterstitial()
        {
            IronSource.Agent.showInterstitial();
        }

        public event Action OnRewardedLoaded;
        public event Action<string> OnRewardedFailedToLoad;
        public event Action OnRewardedOpening;
        public event Action<string> OnRewardedFailedToShow;
        public event Action OnRewardedClicked;
        public event Action OnRewardedUserEarnedReward;
        public event Action OnRewardedClosed;
        public bool IsRewardedAvailable => IronSource.Agent.isRewardedVideoAvailable();
        public void LoadRewarded()
        {
        }

        public void ShowRewarded()
        {
            IronSource.Agent.showRewardedVideo();
        }
    }
}
#endif