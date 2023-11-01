using System;
using System.Linq;
using Dacodelaac.Core;
using Firebase.Extensions;
using UnityEngine;
using Event = Dacodelaac.Events.Event;

namespace Dacodelaac.RemoteConfig
{
    public class RemoteConfig : BaseMono
    {
        [SerializeField] Event remoteConfigFetchedEvent;
        [SerializeField] Config[] configs;

        public void Fetch()
        {
#if REMOTE_CONFIG
            var defaults = configs.ToDictionary(c => c.Key, c => c.Value.DefaultValue as object);

            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
                .ContinueWithOnMainThread(t1 =>
                {
                    Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero)
                        .ContinueWithOnMainThread(
                            t2 =>
                            {
                                var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
                                switch (info.LastFetchStatus)
                                {
                                    case Firebase.RemoteConfig.LastFetchStatus.Success:
                                        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                                        break;
                                    case Firebase.RemoteConfig.LastFetchStatus.Failure:
                                        switch (info.LastFetchFailureReason)
                                        {
                                            case Firebase.RemoteConfig.FetchFailureReason.Error:
                                                //Debug.LogError("Fetch failed for unknown reason");
                                                break;
                                            case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                                                //Debug.LogError("Fetch throttled until " + info.ThrottledEndTime);
                                                break;
                                        }

                                        break;
                                    case Firebase.RemoteConfig.LastFetchStatus.Pending:
                                        //Debug.LogError("Latest Fetch call still pending.");
                                        break;
                                }

                                foreach (var config in configs)
                                {
                                    config.FetchValue();
                                }

                                remoteConfigFetchedEvent.Raise();
                            });
                });
#else
            remoteConfigFetchedEvent.Raise();
#endif
        }
    }
}