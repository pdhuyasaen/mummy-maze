#if PlayFab
using Dacodelaac.UI.Popups;
using Dacodelaac.Variables;
using UnityEngine;

namespace Dev.Scripts.LeaderBoard
{
    public class LeaderBoardPopup : BasePopup
    {
        [SerializeField] private StringVariable userName;
        [SerializeField] private StringVariable countryCode;

        [SerializeField] private LoginRankPopup loginRank;
        [SerializeField] private LeaderBoard leaderBoard;
        [SerializeField] private GameObject loading;

        protected override void BeforeShow(object data = null)
        {
            base.BeforeShow(data);
            leaderBoard.gameObject.SetActive(false);
            loginRank.gameObject.SetActive(false);
            loading.SetActive(true);
            
            if (string.IsNullOrEmpty(userName.Value))
            {
                var region = System.Globalization.RegionInfo.CurrentRegion;
                countryCode.Value = region.Name;
            }
        }

        protected override void AfterShown()
        {
            base.AfterShown();
            if (string.IsNullOrEmpty(userName.Value))
            {
                SetupLoginRank();
            }
            else
            {
                SetupLeaderBoard();
            }
        }

        private void SetupLoginRank()
        {
            loginRank.gameObject.SetActive(true);
            leaderBoard.gameObject.SetActive(false);
            loading.SetActive(false);
            loginRank.Setup();
        }

        public void SetupLeaderBoard()
        {
            leaderBoard.gameObject.SetActive(true);
            loginRank.gameObject.SetActive(false);
            leaderBoard.Setup();
        }
        
    }
}

#endif
