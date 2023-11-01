#if NOTIFICATION
using Unity.Notifications.Android;
using UnityEngine;

namespace Dacodelaac.Notifications
{
    [CreateAssetMenu]
    public class NotificationManager : ScriptableObject
    {
        [SerializeField] string channelId;
        [SerializeField] string channelName;
        [SerializeField] Importance importance;
        [SerializeField] string channelDescription;
        [SerializeField] int preScheduleTime;
        [SerializeField] NotificationStatement[] statements;

        public void Schedule()
        {
            AndroidNotificationCenter.CancelAllDisplayedNotifications();
            AndroidNotificationCenter.CancelAllNotifications();
                
            var channel = new AndroidNotificationChannel()
            {
                Id = channelId,
                Name = channelName,
                Importance = importance,
                Description = channelDescription,
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            for (var i = 0; i < preScheduleTime; i++)
            {
                var statement = statements[Random.Range(0, statements.Length)];
                var notification = new AndroidNotification()
                {
                    Title = statement.title,
                    Text = statement.text,
                    SmallIcon = statement.smallIcon,
                    LargeIcon = statement.largeIcon,
                    FireTime = System.DateTime.Now.AddSeconds(statement.interval * (i + 1))
                };

                AndroidNotificationCenter.SendNotification(notification, channelId);
            }
        }
    }

    [System.Serializable]
    public class NotificationStatement
    {
        public string text;
        public string title;
        public string smallIcon;
        public string largeIcon;
        public long interval;
    }
}
#endif