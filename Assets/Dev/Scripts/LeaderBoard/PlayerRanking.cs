#if  PlayFab
using Coffee.UIEffects;
using Dacodelaac.Core;
using Dacodelaac.Variables;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CountryCode = Pancake.GameService.CountryCode;

namespace Dev.Scripts.LeaderBoard
{
    public class PlayerRanking : BaseMono
    {
        [SerializeField] private Image flag;
        [SerializeField] private Image frame;
        [SerializeField] private TMP_Text userName, score, rank;
        [SerializeField] private GameObject top;
        [SerializeField] private GameObject currentPlayerBorder;
        [SerializeField] private Sprite firstFrame, secondFrame, thirdFrame, currentFrame, normalFrame;
        [SerializeField] private Color top1TextRank, top2TextRank, top3TextRank, currentTextRank, normalTextRank;
        [SerializeField] private Color top1TextName, top2TextName, top3TextName, currentTextName, normalTextName;

        [SerializeField] private StringVariable playFabId;
        [SerializeField] private CountryCode countryCode;
        
        public void Setup(PlayerLeaderboardEntry player)
        {
            var str = player.DisplayName.Split('|');
            var country = countryCode.Get(str[^1].Trim());
            flag.sprite = country.icon;
            
            //set text
            userName.text = str[0];
            score.text = player.StatValue.ToString();
            rank.text = (player.Position + 1).ToString();
            
            //set color
            var visual = (playFabId.Value == player.PlayFabId) ? 0 : (player.Position < 3 ? 1 : 2);
            
            top.SetActive(false);
            currentPlayerBorder.SetActive(false);
            switch (visual)
            {
                case 0:
                    userName.color = currentTextName;
                    score.color = currentTextName;
                    rank.color = currentTextRank;
                    frame.sprite = currentFrame;
                    currentPlayerBorder.SetActive(true);
                    break;
                case 1:
                    if (player.Position == 0)
                    {
                        userName.color = top1TextName;
                        score.color = top1TextName;
                        rank.color = top1TextRank;
                        frame.sprite = firstFrame;
                    }
                    else if (player.Position == 1)
                    {
                        userName.color = top2TextName;
                        score.color = top2TextName;
                        rank.color = top2TextRank;
                        frame.sprite = secondFrame;
                    }
                    else
                    {
                        userName.color = top3TextName;
                        score.color = top3TextName;
                        rank.color = top3TextRank;
                        frame.sprite = thirdFrame;
                    }
                    top.SetActive(true);
                    break;
                case 2:
                    userName.color = normalTextName;
                    score.color = normalTextName;
                    rank.color = normalTextRank;
                    frame.sprite = normalFrame;
                    break;
            }
            
            TryGetComponent<UIShiny>(out var uiShiny);
            if (uiShiny != null) uiShiny.enabled = player.Position < 3;
        }
    }
}

#endif
