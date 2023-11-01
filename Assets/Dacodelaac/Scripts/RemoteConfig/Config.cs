using System;
using System.Globalization;
using Dacodelaac.Core;
using Dacodelaac.DebugUtils;
using UnityEngine;

namespace Dacodelaac.RemoteConfig
{
    [CreateAssetMenu(menuName = "RemoteConfig/Config")]
    public class Config : BaseSO, ISerializationCallbackReceiver
    {
        [SerializeField] string key;
        [SerializeField] string defaultValue;

        public string Key => key;
        public string DefaultValue => defaultValue;
        public ConfigValue Value => value;

        private string firebaseValue;
        ConfigValue value;

        public void FetchValue()
        {
#if REMOTE_CONFIG
            var value = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key);
            Dacoder.Log(key, "Default", this.value.DefaultValue, "Remote", value.StringValue, value.Source);
            try
            {
                firebaseValue = value.StringValue;
            }
            catch (Exception e)
            {
                firebaseValue = defaultValue;
            }
            
            try
            {
                this.value.DoubleValue = value.DoubleValue;
            }
            catch (Exception e)
            {
                this.value.ResetDouble();
            }

            try
            {
                this.value.LongValue = value.LongValue;
            }
            catch (Exception e)
            {
                this.value.ResetLong();
            }

            try
            {
                this.value.StringValue = value.StringValue;
            }
            catch (Exception e)
            {
                this.value.ResetString();
            }

            try
            {
                this.value.BooleanValue = value.BooleanValue;
            }
            catch (Exception e)
            {
                this.value.ResetBoolean();
            }
#endif
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            value = new ConfigValue(string.IsNullOrEmpty(firebaseValue) ? defaultValue : firebaseValue);
        }
    }

    public class ConfigValue
    {
        public string DefaultValue { get; set; }
        public double DoubleValue { get; set; }
        public long LongValue { get; set; }
        public string StringValue { get; set; }
        public bool BooleanValue { get; set; }

        public ConfigValue(string defaultValue)
        {
            DefaultValue = defaultValue;
            ResetString();
            ResetDouble();
            ResetLong();
            ResetBoolean();
        }

        public void ResetString()
        {
            StringValue = DefaultValue;
        }

        public void ResetDouble()
        {
            try
            {
                DoubleValue = Convert.ToDouble(DefaultValue, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                DoubleValue = default;
            }
        }

        public void ResetLong()
        {
            try
            {
                LongValue = Convert.ToInt64(DefaultValue, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                LongValue = default;
            }
        }

        public void ResetBoolean()
        {
            BooleanValue = DefaultValue == "true";
        }
    }
}