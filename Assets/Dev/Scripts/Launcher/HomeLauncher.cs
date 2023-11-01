using Dacodelaac.Core;
using Dacodelaac.DataStorage;
using UnityEngine;
using Event = Dacodelaac.Events.Event;

namespace Dev.Scripts.Launcher
{
    public class HomeLauncher : BaseLauncher
    {
        [SerializeField] private Event loadingEventDoneEvent;
        [SerializeField] private Event hideBannerEvent;

        private void Start()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            loadingEventDoneEvent.Raise();
            hideBannerEvent.Raise();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                GameData.Save();
            }
        }

        private void OnApplicationQuit()
        {
            GameData.Save();
        }
    }
}