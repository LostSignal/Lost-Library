//-----------------------------------------------------------------------
// <copyright file="AnalyticsManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
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
        private List<AnalyticsProvider> analyticsProviders = new List<AnalyticsProvider>();
        private Dictionary<string, object> constantEventData = new Dictionary<string, object>();

        public Dictionary<string, object> ConstantEventData
        {
            get { return this.constantEventData; }
        }

        public void Register(AnalyticsProvider provider)
        {
            this.analyticsProviders.AddIfNotNullAndUnique(provider);
        }
        
        public void Identify(string userId, Dictionary<string, object> eventData = null, Gender gender = Gender.Unknown, int age = 0)
        {
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
            var eventDataCopy = this.CopyDictionary(eventData);

            // appending all the constant event data
            foreach (var keyValuePair in this.constantEventData)
            {
                this.AddIfDoesntExist(eventDataCopy, keyValuePair.Key, keyValuePair.Value);
            }

            return eventDataCopy;
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
    }
}
