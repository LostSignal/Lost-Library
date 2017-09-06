//-----------------------------------------------------------------------
// <copyright file="AnalyticsManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public enum Gender
    {
        Male = 0,
        Female = 1,
        Unknown = 2
    }

    public class AnalyticsManager : SingletonGameObject<AnalyticsManager>
    {
        private static readonly float NewSessionWaitTimeInSeconds = 30.0f;

        private List<AnalyticsProvider> analyticsProviders = new List<AnalyticsProvider>();
        private Dictionary<string, object> constantEventData = new Dictionary<string, object>();
        private HashSet<int> sentLogs = new HashSet<int>();
        private Guid session = Guid.NewGuid();
        private int sessionCount = 1;
        private float lostFocusTime = -1.0f;

        public Dictionary<string, object> ConstantEventData
        {
            get
            {
                if (this.constantEventData.Count == 0)
                {
                    this.constantEventData = new Dictionary<string, object>
                    {
                        { "App.Name",  Application.productName },
                        { "App.Version",  Application.version },
                        { "App.IsEditor",  Application.isEditor },
                        { "App.OS",  SystemInfo.operatingSystem },
                        { "App.ActiveConfig",  AppSettings.ActiveConfig.Name },

                        { "Device.Id",  SystemInfo.deviceUniqueIdentifier },
                        { "Device.Model",  SystemInfo.deviceModel },
                        { "Device.Name",  SystemInfo.deviceName },
                        { "Device.Platform",  Platform.CurrentDevicePlatform.ToString() },
                        { "Device.RuntimePlatform",  Application.platform.ToString() },

                        { "Screen.Width",  UnityEngine.Screen.width },
                        { "Screen.Height",  UnityEngine.Screen.height },
                        { "Screen.DPI",  UnityEngine.Screen.dpi },
                    };

                    this.WriteSessionData();
                }

                return this.constantEventData;
            }
        }

        public void Register(AnalyticsProvider provider)
        {
            this.analyticsProviders.AddIfNotNullAndUnique(provider);
        }

        public void Identify(string userId, Dictionary<string, object> eventData = null, Gender gender = Gender.Unknown, int age = 0)
        {
            // TODO [bgish]: Need to set the userId on PerformanceReporting (if they put the API back)
            Dictionary<string, object> allEventData = this.GetAllEventData(eventData);

            foreach (var provider in this.analyticsProviders)
            {
                provider.Identify(userId, allEventData, gender, age);
            }
        }

        public void Screen(string screenName, Dictionary<string, object> eventData = null)
        {
            Dictionary<string, object> allEventData = this.GetAllEventData(eventData);

            foreach (var provider in this.analyticsProviders)
            {
                provider.Screen(screenName, allEventData);
            }
        }

        public void Transaction(string productId, decimal amount, string currency, Dictionary<string, object> eventData, string receiptPurchaseData, string signature)
        {
            Dictionary<string, object> allEventData = this.GetAllEventData(eventData);

            foreach (var provider in this.analyticsProviders)
            {
                provider.Transaction(productId, amount, currency, allEventData, receiptPurchaseData, signature);
            }
        }

        public void TrackPosition(string eventName, Vector3 position)
        {
            foreach (var implementor in this.analyticsProviders)
            {
                implementor.TrackPosition(eventName, position);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (Application.isEditor == false)
            {
                Application.logMessageReceived += Application_logMessageReceived;
            }
        }
        
        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                float delta = Time.realtimeSinceStartup - this.lostFocusTime;

                if (this.lostFocusTime > 0 && delta > NewSessionWaitTimeInSeconds)
                {
                    this.session = Guid.NewGuid();
                    this.sessionCount++;
                    this.WriteSessionData();
                }
            }
            else
            {
                this.lostFocusTime = Time.realtimeSinceStartup;
            }
        }

        private void Track(string eventName, Dictionary<string, object> eventData)
        {
            Dictionary<string, object> allEventData = this.GetAllEventData(eventData);

            foreach (var provider in this.analyticsProviders)
            {
                provider.Track(eventName, allEventData);
            }
        }

        private Dictionary<string, object> GetAllEventData(Dictionary<string, object> eventData)
        {
            var contantDataCopy = this.CopyDictionary(this.ConstantEventData);

            // appending all the event data
            foreach (var keyValuePair in eventData)
            {
                this.AddIfDoesntExist(contantDataCopy, keyValuePair.Key, keyValuePair.Value);
            }

            return contantDataCopy;
        }

        private void AddIfDoesntExist(Dictionary<string, object> eventData, string key, object value)
        {
            if (eventData.ContainsKey(key) == false)
            {
                eventData.Add(key, value);
            }
            else
            {
                Debug.LogErrorFormat(this, "Tried adding already existing Key \"{0}\" -> Value \"{1}\" to analytics eventData.", key, value);
            }
        }

        private Dictionary<string, object> CopyDictionary(Dictionary<string, object> eventData)
        {
            return eventData == null ? new Dictionary<string, object>() : new Dictionary<string, object>(eventData);
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            int hashCode = stackTrace.GetHashCode();

            if (this.sentLogs.Contains(hashCode) == false)
            {
                this.sentLogs.Add(hashCode);

                this.Track("LogEvent", new Dictionary<string, object>
                {
                    { "Condition", condition },
                    { "StackTrace", stackTrace },
                    { "LogType", type.ToString() },
                });
            }
        }

        private void WriteSessionData()
        {
            this.ConstantEventData.AddOrOverwrite("App.SessionGuid", session.ToString());
            this.ConstantEventData.AddOrOverwrite("App.SessionCount", sessionCount.ToString());
        }
    }
}
