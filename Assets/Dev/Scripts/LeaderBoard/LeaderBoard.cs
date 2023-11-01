#if PlayFab
using System;
using System.Collections.Generic;
using Dacodelaac.Core;
using Dacodelaac.UI.Buttons;
using Dacodelaac.Variables;
using I2.Loc;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Scripts.LeaderBoard
{
    public class LeaderBoard : BaseMono
    {
        [SerializeField] private List<PlayerRanking> playerRankings;
        [SerializeField] private int countPerPage = 10;
        [SerializeField] private GameObject loading, ranking, pre, next;
        [SerializeField] private Image world, country;
        [SerializeField] private TMP_Text worldTxt, countryTxt;
        [SerializeField] private Color onColor, offColor;
        [SerializeField] private Sprite on, off;
        [SerializeField] private TMP_Text userName;
        [SerializeField] private TMP_Text rank, page;
        [SerializeField] private StringVariable user;
        [SerializeField] private IntegerVariable currentLevel;
        [SerializeField] private IntegerVariable score;
        [SerializeField] private StringVariable playFabId;
        [SerializeField] private PlayFabManager playFabManager;
        [SerializeField] private RippleButton worldBtn, countryBtn;

        [SerializeField] private int maxPage = 10;
        private bool isWorld = true;
        private bool isOnTop = false;
        private bool isLoadCountry = false;
        private bool isLoadWorld = false;
        private int currentPage = 0;
        private int totalPage = 0;

        private List<PlayerLeaderboardEntry> worldRanking = new List<PlayerLeaderboardEntry>();
        private List<PlayerLeaderboardEntry> countryRanking = new List<PlayerLeaderboardEntry>();

        public void Setup()
        {
            isLoadCountry = false;
            isLoadWorld = false;
            isWorld = true;
            currentPage = 0;
            totalPage = 0;
            ranking.SetActive(false);
            loading.SetActive(true);
            playFabManager.SendRecordLeaderBoard(currentLevel.Value);
        }

        public void OnSendScoreDone()
        {
            playFabManager.GetLeaderBoardRequest();
            playFabManager.GetLeaderBoardRequest(true);
        }

        public void OnLoadDone(bool worldLoaded)
        {
            if (worldLoaded)
            {
                isLoadWorld = true;
            }
            else
            {
                isLoadCountry = true;
            }

            if (isLoadWorld && isLoadCountry)
            {
                worldRanking = playFabManager.GetLeaderBoard();
                countryRanking = playFabManager.GetLeaderBoard(true);
                OnClickWorld();
                ranking.SetActive(true);
                loading.SetActive(false);
            }
        }

        private void SetupUI(bool isWorldRanking = true)
        {
            var rankingFetch = isWorldRanking ? worldRanking : countryRanking;
            var count = rankingFetch.Count;
            totalPage = count / 10 + (count % 10 != 0 ? 1 : 0);
            totalPage = Mathf.Min(totalPage, maxPage);

            pre.SetActive(currentPage != 0);
            next.SetActive(currentPage + 1 < totalPage);
            var local = page.GetComponent<LocalizationParamsManager>();
            local.SetParameterValue("PAGE", (currentPage + 1).ToString());
            for (var i = 0; i < playerRankings.Count; ++i)
            {
                var index = currentPage * countPerPage + i;
                if (index >= rankingFetch.Count)
                {
                    playerRankings[i].gameObject.SetActive(false);
                }
                else
                {
                    playerRankings[i].gameObject.SetActive(true);
                    playerRankings[i].Setup(rankingFetch[index]);
                }
            }
        }

        private void SetupPlayer()
        {
            var rankingFetch = isWorld ? worldRanking : countryRanking;
            var setUser = false;
            foreach (var rankInfo in rankingFetch)
            {
                if (rankInfo.PlayFabId == playFabId.Value)
                {
                    if (rankInfo.StatValue != score.Value)
                    {
                        rankInfo.StatValue = score.Value;
                        rankingFetch.Sort(new CompareScore());
                        for (var i = 0; i < rankingFetch.Count; ++i)
                        {
                            rankingFetch[i].Position = i;
                        }

                        if (isWorld)
                        {
                            worldRanking = rankingFetch;
                        }
                        else
                        {
                            countryRanking = rankingFetch;
                        }
                    }

                    setUser = true;
                    
                    var localize = rank.GetComponent<Localize>();
                    localize.SetTerm(isWorld ? "world_rank" : "country_rank");
                    
                    var local = rank.GetComponent<LocalizationParamsManager>();
                    local.SetParameterValue("RANK", (rankInfo.Position + 1).ToString());
                    var str = rankInfo.DisplayName.Split('|');
                    userName.text = str[0];
                    break;
                }
            }

            if (!setUser)
            {
                if (rankingFetch.Count > 0 && score.Value <= rankingFetch[^1].StatValue && rankingFetch.Count >= 100)
                {
                    var localize = rank.GetComponent<Localize>();
                    localize.SetTerm(isWorld ? "world_rank" : "country_rank");
                    
                    var local = rank.GetComponent<LocalizationParamsManager>();
                    local.SetParameterValue("RANK", "100+");
                    
                    var str = user.Value.Split('|');
                    userName.text = str[0];
                }
                else
                {
                    rankingFetch.Add(new PlayerLeaderboardEntry
                    {
                        DisplayName = user.Value,
                        PlayFabId = playFabId.Value,
                        Position = 101,
                        StatValue = score.Value
                    });

                    rankingFetch.Sort(new CompareScore());

                    for (var i = 0; i < rankingFetch.Count; ++i)
                    {
                        rankingFetch[i].Position = i;
                    }

                    foreach (var rankInfo in rankingFetch)
                    {
                        if (rankInfo.PlayFabId == playFabId.Value)
                        {
                            if (rankInfo.StatValue != score.Value)
                            {
                                rankInfo.StatValue = score.Value;
                                rankingFetch.Sort(new CompareScore());
                                for (var i = 0; i < rankingFetch.Count; ++i)
                                {
                                    rankingFetch[i].Position = i;
                                }

                                if (isWorld)
                                {
                                    worldRanking = rankingFetch;
                                }
                                else
                                {
                                    countryRanking = rankingFetch;
                                }
                            }
                            var localize = rank.GetComponent<Localize>();
                            localize.SetTerm(isWorld ? "world_rank" : "country_rank");
                    
                            var local = rank.GetComponent<LocalizationParamsManager>();
                            local.SetParameterValue("RANK", (rankInfo.Position + 1).ToString());
                            var str = rankInfo.DisplayName.Split('|');
                            userName.text = str[0];
                            break;
                        }
                    }

                    if (isWorld)
                    {
                        worldRanking = rankingFetch;
                    }
                    else
                    {
                        countryRanking = rankingFetch;
                    }
                }
            }
        }

        public void OnChangePage(int x)
        {
            if (currentPage + x >= totalPage) return;
            if (currentPage + x < 0) return;

            currentPage += x;
            SetupUI(isWorld);
        }

        public void OnClickWorld()
        {
            world.sprite = on;
            country.sprite = off;
            worldTxt.color = onColor;
            countryTxt.color = offColor;
            worldBtn.isInteractable = false;
            countryBtn.isInteractable = true;
            isWorld = true;
            currentPage = 0;
            SetupPlayer();
            SetupUI(isWorld);
        }

        public void OnClickCountry()
        {
            world.sprite = off;
            country.sprite = on;

            worldTxt.color = offColor;
            countryTxt.color = onColor;
            worldBtn.isInteractable = true;
            countryBtn.isInteractable = false;

            isWorld = false;
            currentPage = 0;
            SetupPlayer();
            SetupUI(isWorld);
        }

        class CompareScore : IComparer<PlayerLeaderboardEntry>
        {
            public int Compare(PlayerLeaderboardEntry x, PlayerLeaderboardEntry y)
            {
                if (x.StatValue == y.StatValue) return 0;
                return x.StatValue > y.StatValue ? -1 : 1;
            }
        }
    }
}

#endif