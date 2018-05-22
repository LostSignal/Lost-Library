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

        public abstract void Transaction(string productId, decimal amount, string currency);

        protected void SendIdentityEventAsTrackEvent(Gender gender, int age, Dictionary<string, object> eventData)
        {
            var eventDataCopy = eventData == null ? new Dictionary<string, object>() : new Dictionary<string, object>(eventData);

            eventDataCopy.AddOrOverwrite("Gender", gender);
            eventDataCopy.AddOrOverwrite("Age", age);

            this.Track("Identity", eventDataCopy);
        }

        protected void SendScreenEventAsTrackEvent(string screenName, Dictionary<string, object> eventData)
        {
            var eventDataCopy = eventData == null ? new Dictionary<string, object>() : new Dictionary<string, object>(eventData);

            eventDataCopy.AddOrOverwrite("ScreenName", screenName);

            this.Track("Screen", eventDataCopy);
        }
    }
}
