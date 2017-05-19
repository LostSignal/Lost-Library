//-----------------------------------------------------------------------
// <copyright file="IAnalyticsHandler.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;
        
    public abstract class AnalyticsProvider
    {
        public abstract void Identify(string userId, Dictionary<string, object> eventData, Gender gender, int age);

        public abstract void Screen(string screenName, Dictionary<string, object> eventData = null);

        public abstract void Track(string eventName, Dictionary<string, object> eventData);
        
        public abstract void TrackPosition(string eventName, Vector3 position);

        public abstract void Transaction(string productId, decimal amount, string currency, Dictionary<string, object> eventData, string receiptPurchaseData, string signature);
        
        protected void SendIdentityEventAsTrackEvent(string userId, Dictionary<string, object> eventData, Gender gender, int age)
        {
            var eventDataCopy = new Dictionary<string, object>(eventData);

            this.AddIfDoesntExist(eventDataCopy, "UserId", userId);
            this.AddIfDoesntExist(eventDataCopy, "Gender", gender);
            this.AddIfDoesntExist(eventDataCopy, "Age", age);

            this.Track("Identity", eventDataCopy);
        }

        protected void SendScreenEventAsTrackEvent(string screenName, Dictionary<string, object> eventData)
        {
            var eventDataCopy = new Dictionary<string, object>(eventData);

            this.AddIfDoesntExist(eventDataCopy, "ScreenName", screenName);

            this.Track("Screen", eventDataCopy);
        }

        protected void SendTransactionEventAsTrackEvent(string productId, decimal amount, string currency, Dictionary<string, object> eventData, string receiptPurchaseData, string signature)
        {
            var eventDataCopy = new Dictionary<string, object>(eventData);

            this.AddIfDoesntExist(eventDataCopy, "ProductId", productId);
            this.AddIfDoesntExist(eventDataCopy, "Amount", amount);
            this.AddIfDoesntExist(eventDataCopy, "Currency", currency);
            this.AddIfDoesntExist(eventDataCopy, "ReceiptPurchaseData", receiptPurchaseData);
            this.AddIfDoesntExist(eventDataCopy, "Signature", signature);

            this.Track("Transaction", eventDataCopy);
        }
        
        private void AddIfDoesntExist(Dictionary<string, object> eventData, string key, object value)
        {
            if (eventData.ContainsKey(key) == false)
            {
                eventData.Add(key, value);
            }
            else
            {
                Debug.LogErrorFormat("Tried adding already existing Key \"{0}\" -> Value \"{1}\" to analytics eventData.", key, value);
            }
        }
    }
}
