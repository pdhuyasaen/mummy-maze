using Dacodelaac.Events;
using Dacodelaac.Utils;
using Dacodelaac.Variables;
using Dev.Scripts.InAppPurchasing;
using UnityEngine;

namespace Dacodelaac.Core
{
    public class Launcher : BaseLauncher
    {
        [SerializeField] LoadingScreenEvent loadingScreenEvent;
        [SerializeField] private DoubleVariable lastTimeLogin;

        [SerializeField] private bool adsInitialized;
        [SerializeField] private bool analyticsInitialized;
        [SerializeField] private bool remoteConfigFetched;
        [SerializeField] private Store store;

        private void Start()
        {
            Initialize();
        }
        public override void Initialize()
        {
            base.Initialize();
            //store.Initialize();
            Application.targetFrameRate = 60;   
#if !UNITY_EDITOR
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            Input.multiTouchEnabled = false;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
            adsInitialized = true;
            analyticsInitialized = true;
            remoteConfigFetched = true;
            lastTimeLogin.Value = TimeUtils.CurrentTicksUtc;

            loadingScreenEvent.Raise(new LoadingScreenData
            {
                IsLaunching = true,
                Scene = "HomeScene",
                MinLoadTime = 4,
                LaunchCondition = () => adsInitialized && analyticsInitialized && remoteConfigFetched,
            });
        }

        public void OnAdsInitialize()
        {
            adsInitialized = true;
        }

        public void OnAnalyticsInitialized()
        {
            analyticsInitialized = true;
        }

        public void RemoteConfigFetched()
        {
            remoteConfigFetched = true;
        }
    }
}