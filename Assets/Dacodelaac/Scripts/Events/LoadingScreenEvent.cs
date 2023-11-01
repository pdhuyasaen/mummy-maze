using UnityEngine;

namespace Dacodelaac.Events
{
    [CreateAssetMenu(menuName = "Event/Loading Screen Event")]
    public class LoadingScreenEvent : BaseEvent<LoadingScreenData>
    {
    }

    public class LoadingScreenData
    {
        public bool IsLaunching;
        public string Scene;
        public float MinLoadTime;
        public System.Func<bool> LaunchCondition;
    }
}