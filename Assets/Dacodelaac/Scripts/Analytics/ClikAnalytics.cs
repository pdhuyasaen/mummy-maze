#if CRAZY_LABS_CLIK
using System.Collections.Generic;
using Tabtale.TTPlugins;

namespace Dacodelaac.AnalyticsSystem
{
    public class ClikAnalytics : IAnalytics
    {
        public void OnLevelUp(int lvl, Dictionary<string, object> param)
        {
            TTPGameProgression.FirebaseEvents.LevelUp(lvl, param);
        }

        public void OnLevelStart(int lvl, Dictionary<string, object> param)
        {
            TTPGameProgression.FirebaseEvents.MissionStarted(lvl, param);
        }

        public void OnLevelFailed(int lvl, Dictionary<string, object> param)
        {
            TTPGameProgression.FirebaseEvents.MissionFailed(param);
        }

        public void OnLevelCompleted(int lvl, Dictionary<string, object> param)
        {
            TTPGameProgression.FirebaseEvents.MissionComplete(param);
        }

        public void LogEvent(string eventName, Dictionary<string, object> param)
        {
            TTPAnalytics.LogEvent(0, eventName, param, false);
        }
    }
}
#endif