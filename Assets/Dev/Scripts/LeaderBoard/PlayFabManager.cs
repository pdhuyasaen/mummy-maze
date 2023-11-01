#if PlayFab
using System;
using System.Collections.Generic;
using Dacodelaac.Core;
using Dacodelaac.DataStorage;
using Dacodelaac.Events;
using Dacodelaac.Utils;
using Dacodelaac.Variables;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Event = Dacodelaac.Events.Event;

namespace Dev.Scripts.LeaderBoard
{
    [CreateAssetMenu(menuName = "PlayFab/PlayFab Manager")]
    public class PlayFabManager : BaseSO
    {
        [SerializeField] private StringVariable customId;
        [SerializeField] private StringVariable userName;
        [SerializeField] private StringVariable countryCode;
        [SerializeField] private StringVariable playFabId;
        [SerializeField] private string tableName = "RANK_LEVEL";
        [SerializeField] private DoubleVariable lastTimeSync;
        [SerializeField] private DoubleVariable lastTimeSyncCountry;
        [SerializeField] private IntegerVariable scorePlayer;
        [SerializeField] private double limitTime = 300;
        [SerializeField] private Event showLeaderBoardPopup;
        [SerializeField] private Event submitNameDoneEvent;
        [SerializeField] private StringEvent submitNameErrorEvent;
        [SerializeField] private Event loadWorldBoardDoneEvent;
        [SerializeField] private Event loadCountryBoardDoneEvent;
        [SerializeField] private Event sendRecordDoneEvent;
        [SerializeField] private Event loginDoneEvent;
        [SerializeField] private BooleanVariable isLogged;
        
        private List<PlayerLeaderboardEntry> worldPlayer = new List<PlayerLeaderboardEntry>();
        private List<PlayerLeaderboardEntry> countryPlayer = new List<PlayerLeaderboardEntry>();
        private int scoreCache;
        public void Login()
        {
            if (isLogged.Value)
            {
                loginDoneEvent.Raise();
                showLeaderBoardPopup.Raise();
                return;
            }
            if (string.IsNullOrEmpty(customId.Value))
            {
                customId.Value = SystemInfo.deviceUniqueIdentifier + "_" + DateTime.Now;
                GameData.Save();
            }
            
            var request = new LoginWithCustomIDRequest
            {
                CustomId = customId.Value,
                CreateAccount = true
            };
            
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
        }

        private void OnLoginSuccess(LoginResult result)
        {
            if (string.IsNullOrEmpty(playFabId.Value))
            {
                playFabId.Value = result.PlayFabId;
                GameData.Save();
            }
            isLogged.Value = true;
            loginDoneEvent.Raise();
            showLeaderBoardPopup.Raise();
        }

        private void OnError(PlayFabError error)
        {
            isLogged.Value = false;
            Debug.Log(error);
            loginDoneEvent.Raise();
        }

        public void SendRecordLeaderBoard(int score)
        {
            if (score == scorePlayer.Value)
            {
                sendRecordDoneEvent.Raise();
                return;
            }
            else
            {
                scoreCache = score;
            }

            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = tableName,
                        Value = score
                    },
                    new StatisticUpdate
                    {
                        StatisticName = tableName + "_" + countryCode.Value,
                        Value = score
                    }
                }
            };
            PlayFabClientAPI.UpdatePlayerStatistics(request, OnUpdateSuccess, OnUpdateError);
        }

        private void OnUpdateSuccess(UpdatePlayerStatisticsResult result)
        {
            scorePlayer.Value = scoreCache;
            GameData.Save();
            sendRecordDoneEvent.Raise();
        }

        private void OnUpdateError(PlayFabError error)
        {
            Debug.Log(error);
        }

        public void GetLeaderBoardRequest(bool isCountry = false)
        {
            if (TimeUtils.CurrentSeconds - lastTimeSync.Value < limitTime && !isCountry && lastTimeSync.Value != 0)
            {
                loadWorldBoardDoneEvent.Raise();
                return;
            } 
            
            if (TimeUtils.CurrentSeconds - lastTimeSyncCountry.Value < limitTime && isCountry && lastTimeSyncCountry.Value != 0)
            {
                loadCountryBoardDoneEvent.Raise();
                return;
            }
            
            var request = new GetLeaderboardRequest
            {
                StatisticName = tableName + (isCountry ? "_" + countryCode.Value : ""),
                StartPosition = 0,
                MaxResultsCount = 100
            };
            
            PlayFabClientAPI.GetLeaderboard(request, isCountry ? OnGetLeaderBoardCountrySuccess : OnGetLeaderBoardWorldSuccess, OnGetLeaderBoardError);
        }

        private void OnGetLeaderBoardWorldSuccess(GetLeaderboardResult result)
        {
            worldPlayer = result.Leaderboard;
            lastTimeSync.Value = TimeUtils.CurrentSeconds;
            loadWorldBoardDoneEvent.Raise();
        }
        
        private void OnGetLeaderBoardCountrySuccess(GetLeaderboardResult result)
        {
            countryPlayer = result.Leaderboard;
            lastTimeSyncCountry.Value = TimeUtils.CurrentSeconds;
            loadCountryBoardDoneEvent.Raise();
        }

        public List<PlayerLeaderboardEntry> GetLeaderBoard(bool isCountry = false)
        {
            return isCountry ? countryPlayer : worldPlayer;
        }

        private void OnGetLeaderBoardError(PlayFabError error)
        {
            Debug.Log(error);
        }

        public void OnSubmitName(string displayName)
        {
            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = displayName
            };
            
            PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnSubmitNameSuccess, OnSubmitNameError);
        }

        private void OnSubmitNameSuccess(UpdateUserTitleDisplayNameResult result)
        {
            userName.Value = result.DisplayName;
            GameData.Save();
            submitNameDoneEvent.Raise();
        }

        private void OnSubmitNameError(PlayFabError error)
        {
            submitNameErrorEvent.Raise(error.ErrorMessage);
        }
    }
}
#endif
