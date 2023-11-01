using UnityEngine;

namespace Dacodelaac.Events
{
    [CreateAssetMenu(menuName = "Event/Ads Request Show Rewarded Event")]
    public class AdsShowRewardedEvent : BaseEvent<AdsShowRewardedData>
    {
    }

    public class AdsShowRewardedData
    {
        public System.Action OnAvailable;
        public System.Action OnNotAvailable;
        public System.Action OnCompleted;
        public System.Action OnClosed;
    }
}