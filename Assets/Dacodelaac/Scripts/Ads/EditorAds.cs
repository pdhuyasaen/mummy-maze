using Dacodelaac.Core;
using UnityEngine;

namespace Dacodelaac.Ads
{
    public class EditorAds : BaseMono
    {
        [SerializeField] EditorMediation mediation;

        void Awake()
        {
#if !UNITY_EDITOR
            Destroy(gameObject);
#endif
        }

        public void OnCloseReward()
        {
            mediation.CloseRewarded();
        }

        public void OnCompleteReward()
        {
            mediation.CompleteRewarded();
        }

        public void OnCloseInterstitial()
        {
            mediation.CloseInterstitial();
        }
    }
}