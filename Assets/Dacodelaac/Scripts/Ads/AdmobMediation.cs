#if ADMOB
using System;
using Dacodelaac.AnalyticsSystem;
using GoogleMobileAds.Api;
 
namespace Dacodelaac.Ads
{
    public class AdmobMediation : IMediation
    {
#if UNITY_ANDROID
        const string BannerUnitId = "ca-app-pub-8566745611252640/8640424615";
        const string InterstitialUnitId = "ca-app-pub-8566745611252640/9462551693";
        const string RewardedUnitId = "ca-app-pub-8566745611252640/2075016261";
#else
        const string BannerUnitId = "ca-app-pub-3472223975139027/4528815480";
        const string InterstitialUnitId = "ca-app-pub-3472223975139027/3181086550";
        const string RewardedUnitId = "ca-app-pub-3472223975139027/3215733814";
#endif

        public void Initialize(Action onCompleted)
        {
            MobileAds.Initialize(status => { onCompleted?.Invoke(); });
        }

        public void OnApplicationPaused(bool isPaused)
        {
        }
        
        public void OnAdPaid(string id, double value, string currencyCode, string placement, string adFormat)
        {
            Analytics.OnAdPaid("admob_sdk", value / 1000000f, currencyCode, null, placement, id, adFormat);
        }

        #region Banner

        public event Action OnBannerLoaded;
        public event Action<string> OnBannerFailedToLoad;
        public event Action OnBannerOpening;
        public event Action OnBannerClicked;
        public event Action OnBannerClosed;
        public event Action OnBannerLeavingApplication;
        public event Action<string, double, string, string, string> OnBannerAdPaid;

        BannerView bannerView;

        public void LoadBanner(Ads.BannerPosition bannerPosition)
        {
            var adPosition = AdPosition.Bottom;
            switch (bannerPosition)
            {
                case Ads.BannerPosition.Top:
                    adPosition = AdPosition.Top;
                    break;
                case Ads.BannerPosition.Bottom:
                    adPosition = AdPosition.Bottom;
                    break;
                case Ads.BannerPosition.TopLeft:
                    adPosition = AdPosition.BottomLeft;
                    break;
                case Ads.BannerPosition.TopRight:
                    adPosition = AdPosition.TopRight;
                    break;
                case Ads.BannerPosition.BottomLeft:
                    adPosition = AdPosition.BottomLeft;
                    break;
                case Ads.BannerPosition.BottomRight:
                    adPosition = AdPosition.BottomRight;
                    break;
                case Ads.BannerPosition.Center:
                    adPosition = AdPosition.Center;
                    break;
                default:
                    adPosition = AdPosition.Bottom;
                    break;
            }

            bannerView = new BannerView(BannerUnitId, AdSize.Banner, adPosition);
            bannerView.OnAdLoaded += (sender, args) => { OnBannerLoaded?.Invoke(); };
            bannerView.OnAdFailedToLoad += (sender, args) => { OnBannerFailedToLoad?.Invoke(args.LoadAdError.ToString()); };
            bannerView.OnAdOpening += (sender, args) => { OnBannerOpening?.Invoke(); };
            bannerView.OnAdClosed += (sender, args) => { OnBannerClosed?.Invoke(); };
            bannerView.OnPaidEvent += (sender, args) => { OnBannerAdPaid?.Invoke(BannerUnitId, args.AdValue.Value, args.AdValue.CurrencyCode, "", ""); };
            bannerView.LoadAd(new AdRequest.Builder().Build());
        }

        public void ShowBanner()
        {
        }

        #endregion

        #region Interstitial

        public event Action OnInterstitialLoaded;
        public event Action<string> OnInterstitialFailedToLoad;
        public event Action OnInterstitialShow;
        public event Action OnInterstitialClicked;
        public event Action<string> OnInterstitialFailedToShow;
        public event Action OnInterstitialClosed;
        public event Action OnInterstitialLeavingApplication;
        public event Action<string, double, string, string, string> OnInterstitialAdPaid;

        InterstitialAd interstitial;

        public bool IsInterstitialAvailable => interstitial?.IsLoaded() ?? false;

        public void LoadInterstitial()
        {
            interstitial?.Destroy();
            interstitial = new InterstitialAd(InterstitialUnitId);
            interstitial.OnAdLoaded += (sender, args) => { OnInterstitialLoaded?.Invoke(); };
            interstitial.OnAdFailedToLoad += (sender, args) => { OnInterstitialFailedToLoad?.Invoke(args.LoadAdError.ToString()); };
            interstitial.OnAdOpening += (sender, args) => { OnInterstitialShow?.Invoke(); };
            interstitial.OnAdFailedToShow += (sender, args) => { OnInterstitialFailedToShow?.Invoke(args.AdError.GetMessage()); };
            interstitial.OnAdClosed += (sender, args) => { OnInterstitialClosed?.Invoke(); };
            interstitial.OnPaidEvent += (sender, args) => { OnInterstitialAdPaid?.Invoke(InterstitialUnitId, args.AdValue.Value, args.AdValue.CurrencyCode, "", ""); };
            var request = new AdRequest.Builder().Build();
            interstitial.LoadAd(request);
        }

        public void ShowInterstitial()
        {
            if (IsInterstitialAvailable)
            {
                interstitial.Show();
            }
        }

        #endregion

        #region Rewarded

        public event Action OnRewardedLoaded;
        public event Action<string> OnRewardedFailedToLoad;
        public event Action OnRewardedShow;
        public event Action OnRewardedClicked;
        public event Action<string> OnRewardedFailedToShow;
        public event Action OnRewardedUserEarnedReward;
        public event Action OnRewardedClosed;
        public event Action<string, double, string, string, string> OnRewardedAdPaid;

        RewardedAd rewardedAd;

        public bool IsRewardedAvailable => rewardedAd?.IsLoaded() ?? false;

        public void LoadRewarded()
        {
            rewardedAd = new RewardedAd(RewardedUnitId);
            rewardedAd.OnAdLoaded += (sender, args) => { OnRewardedLoaded?.Invoke(); };
            rewardedAd.OnAdFailedToLoad += (sender, args) => { OnRewardedFailedToLoad?.Invoke(args.LoadAdError.ToString()); };
            rewardedAd.OnAdOpening += (sender, args) => { OnRewardedShow?.Invoke(); };
            rewardedAd.OnAdFailedToShow += (sender, args) => { OnRewardedFailedToShow?.Invoke(args.AdError.GetMessage()); };
            rewardedAd.OnUserEarnedReward += (sender, args) => { OnRewardedUserEarnedReward?.Invoke(); };
            rewardedAd.OnAdClosed += (sender, args) => { OnRewardedClosed?.Invoke(); };
            rewardedAd.OnPaidEvent += (sender, args) => { OnRewardedAdPaid?.Invoke(RewardedUnitId, args.AdValue.Value, args.AdValue.CurrencyCode, "", ""); };
            rewardedAd.LoadAd(new AdRequest.Builder().Build());
        }

        public void ShowRewarded()
        {
            if (IsRewardedAvailable)
            {
                rewardedAd.Show();
            }
        }

        #endregion
    }
}
#endif