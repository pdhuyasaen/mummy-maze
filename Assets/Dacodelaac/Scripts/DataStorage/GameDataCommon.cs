using Dacodelaac.Utils;

namespace Dacodelaac.DataStorage
{
    public static partial class GameData
    {
        const string LAST_LOGIN_TIME_KEY = "LAST_LOGIN_TIME";
        const string LAST_ACTIVE_TIME_KEY = "LAST_ACTIVE_TIME";
        
        static DataStorage _storage;
        static DataStorage Storage => _storage ?? (_storage = new DataStorage("data.dat"));

        public static double LastLoginTime
        {
            get => TimeUtils.TicksToSeconds(Storage.Get<long>(LAST_LOGIN_TIME_KEY));
            set => Storage.Set(LAST_LOGIN_TIME_KEY, TimeUtils.SecondsToTicks(value));
        }
        
        public static double LastActiveTime
        {
            get => TimeUtils.TicksToSeconds(Storage.Get<long>(LAST_ACTIVE_TIME_KEY));
            set => Storage.Set(LAST_ACTIVE_TIME_KEY, TimeUtils.SecondsToTicks(value));
        }

        public static T Get<T>(string key, T defaultValue = default)
        {
            return Storage.Get<T>(key, defaultValue);
        }

        public static void Set<T>(string key, T data)
        {
            Storage[key] = data;
        }
        
        public static void Load(IDataPersistent dataPersistent, bool root = false)
        {
            Storage.Load(dataPersistent, root);
        }

        public static void Store(IDataPersistent dataPersistent, bool root = false)
        {
            Storage.Store(dataPersistent, root);
        }

        public static void Save()
        {
            Storage.Save();
        }

        public static void Clear()
        {
            _storage = null;
        }
    }
}