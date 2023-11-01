using Dacodelaac.Core;
using Dacodelaac.DataStorage;
using UnityEngine;
using Event = Dacodelaac.Events.Event;

namespace Dev.Scripts.Launcher
{
    public class GameLauncher : BaseLauncher
    {
        [SerializeField] private Event loadingEventDoneEvent;
        [SerializeField] private Event showBannerEvent;
        private void Start()
        {
            Initialize();
            loadingEventDoneEvent.Raise();
        }
        
        public void OnPauseGame(bool pause)
        {
            Time.timeScale = pause ? 0f : 1f;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                GameData.Save();
            }
        }

        public void OnApplicationQuit()
        {
            GameData.Save();
        }
    }
}