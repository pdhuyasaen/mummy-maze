#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Dacodelaac.DataStorage;
using Dacodelaac.DebugUtils;
using Dacodelaac.Utils;

namespace Dacodelaac.EditorUtils
{
    [InitializeOnLoad]
    public class EditorMenu
    {
        [MenuItem("Game/Enable Run in Background", false, 22)]
        public static void EnableRunInBG()
        {
            Application.runInBackground = true;
        }

        [MenuItem("Game/Enable Run in Background", true, 22)]
        public static bool EnableRunInBGConditional()
        {
            return !Application.runInBackground;
        }

        [MenuItem("Game/Disable Run in Background", false, 22)]
        public static void DisableRunInBG()
        {
            Application.runInBackground = false;
        }

        [MenuItem("Game/Disable Run in Background", true, 22)]
        public static bool DisableRunInBGConditional()
        {
            return Application.runInBackground;
        }

        [MenuItem("Game/Clear Data", false, 44)]
        public static void ClearData()
        {
            PlayerPrefs.DeleteAll();
            var persistentDataPath = DataStorage.DataStorage.GetPersistentDataPath();
            if (Directory.Exists(persistentDataPath))
            {
                Dacoder.LogFormat("Deleted data directory {0}", persistentDataPath);
                Directory.Delete(persistentDataPath, true);
            }

            var remoteConfigPath =
                Path.Combine(Directory.GetParent(Application.dataPath).FullName, "remote_config_data");
            if (File.Exists(remoteConfigPath))
            {
                Dacoder.LogFormat("Deleted remote config {0}", remoteConfigPath);
                File.Delete(remoteConfigPath);
            }

            GameData.Clear();
        }

        [MenuItem("Game/Pause", false, 55)]
        public static void PauseGame()
        {
            Time.timeScale = 0;
        }

        [MenuItem("Game/Pause", true, 55)]
        public static bool PauseGameCondition()
        {
            return Time.timeScale > 0;
        }

        [MenuItem("Game/Resume", false, 55)]
        public static void ResumeGame()
        {
            Time.timeScale = 1;
        }

        [MenuItem("Game/Resume", true, 55)]
        public static bool ResumeGameCondition()
        {
            return Time.timeScale < 1;
        }

        [MenuItem("Tools/Add CustomViewSize")]
        public static void AddCustomViewSize()
        {
            GameViewUtils.AddCustomSize();
        }

        #region FLAGS

        [MenuItem("Flags/REMOTE_CONFIG")]
        public static void RemoteConfigFlag()
        {
            SwitchFlag("REMOTE_CONFIG");
        }

        [MenuItem("Flags/REMOTE_CONFIG", true)]
        public static bool IsRemoteConfigFlagEnabled()
        {
            Menu.SetChecked("Flags/REMOTE_CONFIG", IsFlagEnabled("REMOTE_CONFIG"));
            return true;
        }

        [MenuItem("Flags/DACODER_RELEASE")]
        public static void ReleaseFlag()
        {
            SwitchFlag("DACODER_RELEASE");
        }

        [MenuItem("Flags/DACODER_RELEASE", true)]
        public static bool IsReleaseEnabled()
        {
            Menu.SetChecked("Flags/DACODER_RELEASE", IsFlagEnabled("DACODER_RELEASE"));
            return true;
        }

        [MenuItem("Flags/Ads/ADMOB")]
        public static void AdmobFlag()
        {
            SwitchFlag("ADMOB");
        }

        [MenuItem("Flags/Ads/ADMOB", true)]
        public static bool IsAdmobEnabled()
        {
            Menu.SetChecked("Flags/Ads/ADMOB", IsFlagEnabled("ADMOB"));
            return true;
        }

        [MenuItem("Flags/Ads/MAX")]
        public static void MaxFlag()
        {
            SwitchFlag("MAX");
        }

        [MenuItem("Flags/Ads/MAX", true)]
        public static bool IsMaxEnabled()
        {
            Menu.SetChecked("Flags/Ads/MAX", IsFlagEnabled("MAX"));
            return true;
        }

        [MenuItem("Flags/Ads/IRON_SOURCE")]
        public static void IronSourceFlag()
        {
            SwitchFlag("IRON_SOURCE");
        }

        [MenuItem("Flags/Ads/IRON_SOURCE", true)]
        public static bool IsIronSourceEnabled()
        {
            Menu.SetChecked("Flags/Ads/IRON_SOURCE", IsFlagEnabled("IRON_SOURCE"));
            return true;
        }

        [MenuItem("Flags/Analytics/FIREBASE")]
        public static void FirebaseFlag()
        {
            SwitchFlag("FIREBASE");
        }

        [MenuItem("Flags/Analytics/FIREBASE", true)]
        public static bool IsFirebaseEnabled()
        {
            Menu.SetChecked("Flags/Analytics/FIREBASE", IsFlagEnabled("FIREBASE"));
            return true;
        }

        [MenuItem("Flags/Analytics/ADJUST")]
        public static void AdjustFlag()
        {
            SwitchFlag("ADJUST");
        }

        [MenuItem("Flags/Analytics/ADJUST", true)]
        public static bool IsAdjustEnabled()
        {
            Menu.SetChecked("Flags/Analytics/ADJUST", IsFlagEnabled("ADJUST"));
            return true;
        }

        [MenuItem("Flags/Analytics/FACEBOOK")]
        public static void FacebookFlag()
        {
            SwitchFlag("FACEBOOK");
        }

        [MenuItem("Flags/Analytics/FACEBOOK", true)]
        public static bool IsFacebookEnabled()
        {
            Menu.SetChecked("Flags/Analytics/FACEBOOK", IsFlagEnabled("FACEBOOK"));
            return true;
        }

        [MenuItem("Flags/Analytics/GA")]
        public static void GAFlag()
        {
            SwitchFlag("GA");
        }

        [MenuItem("Flags/Analytics/GA", true)]
        public static bool IsGAEnabled()
        {
            Menu.SetChecked("Flags/Analytics/GA", IsFlagEnabled("GA"));
            return true;
        }

        [MenuItem("Flags/NOTIFICATION")]
        public static void NotificationFlag()
        {
            SwitchFlag("NOTIFICATION");
        }

        [MenuItem("Flags/NOTIFICATION", true)]
        public static bool IsNotificationEnabled()
        {
            Menu.SetChecked("Flags/NOTIFICATION", IsFlagEnabled("NOTIFICATION"));
            return true;
        }

        [MenuItem("Flags/LEADERBOARD/PlayFab")]
        public static void LeaderboardPlayFabFlag()
        {
            SwitchFlag("PlayFab");
        }

        [MenuItem("Flags/LEADERBOARD/PlayFab", true)]
        public static bool IsPlayFabEnabled()
        {
            Menu.SetChecked("Flags/LEADERBOARD/PlayFab", IsFlagEnabled("PlayFab"));
            return true;
        }

        static void SwitchFlag(string flag)
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                out var defines);
            var enabled = defines.Contains(flag);
            defines = enabled ? defines.Where(value => value != flag).ToArray() : defines.Append(flag).ToArray();
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
        }

        static bool IsFlagEnabled(string flag)
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                out var defines);
            return defines.Contains(flag);
        }

        #endregion
    }
}
#endif