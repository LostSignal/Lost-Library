//-----------------------------------------------------------------------
// <copyright file="Settings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    
    public abstract class Settings
    {
        #region settings definition

        private interface ISetting
        {
            void Commit();

            void Revert();

            bool IsDirty { get; }
        }

        private abstract class Setting<T> : ISetting<T>, ISetting where T : System.IComparable<T>
        {
            private Dictionary<int, T> settings;
            private T defaultValue;
            private T currentValue;
            private bool isDirty;
            private int key;

            public Setting(Dictionary<int, T> settings, int key, T defaultValue)
            {
                this.settings = settings;
                this.key = key;
                this.defaultValue = defaultValue;
                this.currentValue = this.GetValue(this.key, this.defaultValue);
                this.isDirty = false;
            }

            public T Value
            {
                get
                {
                    return this.currentValue;
                }

                set
                {
                    if (this.currentValue.CompareTo(value) != 0)
                    {
                        this.currentValue = value;
                        this.isDirty = true;
                    }
                }
            }

            public bool IsDirty
            {
                get { return isDirty; }
            }

            public void Revert()
            {

                this.currentValue = this.GetValue(this.key, this.defaultValue);
                this.isDirty = false;
            }

            public void Commit()
            {
                this.SetValue(this.key, this.currentValue);
                this.isDirty = false;
            }

            protected T GetValue(int key, T defaultValue)
            {
                T foundValue;
                if (settings.TryGetValue(key, out foundValue))
                {
                    return foundValue;
                }
                else
                {
                    return defaultValue;
                }
            }

            protected void SetValue(int key, T value)
            {
                settings[key] = value;
            }
        }
        
        #endregion

        #region settings implementation

        private class FloatSetting : Setting<float>, IFloatSetting
        {
            public FloatSetting(Dictionary<int, float> settings, int key, float defaultValue) : base(settings, key, defaultValue) { }
        }

        private class BoolSetting : Setting<bool>, IBoolSetting
        {
            public BoolSetting(Dictionary<int, bool> settings, int key, bool defaultValue) : base(settings, key, defaultValue) { }
        }

        private class IntSetting : Setting<int>, IIntSetting
        {
            public IntSetting(Dictionary<int, int> settings, int key, int defaultValue) : base(settings, key, defaultValue) { }
        }

        private class StringSetting : Setting<string>, IStringSetting
        {
            public StringSetting(Dictionary<int, string> settings, int key, string defaultValue) : base(settings, key, defaultValue) { }
        }

        private class DateTimeSetting : Setting<DateTime>, IDateTimeSetting
        {
            public DateTimeSetting(Dictionary<int, DateTime> settings, int key, DateTime defaultValue) : base(settings, key, defaultValue) { }
        }

        #endregion

        /// <summary>
        /// Internal class for storing all our settings data.
        /// </summary>
        [Serializable]
        private class SettingsData
        {
            #pragma warning disable 0649
            [SerializeField] public Dictionary<int, bool> BooleanValues = new Dictionary<int, bool>();
            [SerializeField] public Dictionary<int, string> StringValues = new Dictionary<int, string>();
            [SerializeField] public Dictionary<int, int> IntValues = new Dictionary<int, int>();
            [SerializeField] public Dictionary<int, float> FloatValues = new Dictionary<int, float>();
            [SerializeField] public Dictionary<int, DateTime> DateTimeValues = new Dictionary<int, DateTime>();
            #pragma warning restore 0649
        }
        
        private readonly List<ISetting> settings = new List<ISetting>();
        
        private readonly object readWriteLock = new object();
        
        [SerializeField]
        private SettingsData data;
        
        protected Settings(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                this.data = new SettingsData();
            }
            else
            {
                this.data = JsonUtility.FromJson<SettingsData>(json);
            }
        }
        
        protected IBoolSetting GetBoolSetting(int key, bool defaultValue)
        {
            var setting = new BoolSetting(this.data.BooleanValues, key, defaultValue);
            this.settings.Add(setting);
            return setting;
        }
        
        protected IStringSetting GetStringSetting(int key, string defaultValue)
        {
            var setting = new StringSetting(this.data.StringValues, key, defaultValue);
            this.settings.Add(setting);
            return setting;
        }
        
        protected IIntSetting GetIntSetting(int key, int defaultValue)
        {
            var setting = new IntSetting(this.data.IntValues, key, defaultValue);
            this.settings.Add(setting);
            return setting;
        }
        
        protected IFloatSetting GetFloatSetting(int key, float defaultValue)
        {
            var setting = new FloatSetting(this.data.FloatValues, key, defaultValue);
            this.settings.Add(setting);
            return setting;
        }
        
        protected IDateTimeSetting GetDateTimeSetting(int key, DateTime defaultValue)
        {
            var setting = new DateTimeSetting(this.data.DateTimeValues, key, defaultValue);
            this.settings.Add(setting);
            return setting;
        }
        
        public bool IsDirty
        {
            get
            {
                lock (readWriteLock)
                {
                    foreach (var setting in this.settings)
                    {
                        if (setting.IsDirty)
                        {
                            return true;
                        }
                    }
                        
                    return false;
                }
            }
        }

        public void Commit()
        {
            lock (readWriteLock)
            {
                foreach (var setting in this.settings) setting.Commit();
            }
        }
        
        public void Revert()
        {
            lock (readWriteLock)
            {
                foreach (var setting in this.settings) setting.Revert();
            }
        }
        
        public string SerializeToJson()
        {
            lock (readWriteLock)
            {
                // commits everything before serializing
                foreach (var setting in this.settings)
                {
                    setting.Commit();
                }

                return JsonUtility.ToJson(this.data);
            }
        }
    }
}
  