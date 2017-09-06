//-----------------------------------------------------------------------
// <copyright file="IAnalyticsHandler.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_ANALYTICS

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Analytics;

    public class UnityAnalyticsProvider : AnalyticsProvider
    {
        public override void Identify(string userId, Dictionary<string, object> eventData, Gender gender, int age)
        {
            Analytics.SetUserId(userId);
            Analytics.SetUserGender((UnityEngine.Analytics.Gender)gender);

            int birthYear = this.GetBirthYear(age);

            if (birthYear != 0)
            {
                Analytics.SetUserBirthYear(birthYear);
            }

            this.SendIdentityEventAsTrackEvent(gender, age, eventData);
        }
        
        public override void Screen(string screenName, Dictionary<string, object> eventData)
        {
            this.SendScreenEventAsTrackEvent(screenName, eventData);
        }

        public override void Transaction(string productId, decimal amount, string currency, Dictionary<string, object> eventData, string receiptPurchaseData, string signature)
        {
            Analytics.Transaction(productId, amount, currency, receiptPurchaseData, signature);

            // also send as Track event because Unity doesn't let you pass in eventData
            SendTransactionEventAsTrackEvent(productId, amount, currency, eventData, receiptPurchaseData, signature);
        }

        public override void TrackPosition(string eventName, Vector3 position)
        {
            Analytics.CustomEvent(eventName, position);
        }

        public override void Track(string eventName, Dictionary<string, object> eventData)
        {
            Analytics.CustomEvent(eventName, eventData);
        }

        private int GetBirthYear(int age)
        {
            if (age > 0)
            {
                int currentYear = DateTime.Now.Year;
                int birthYear = currentYear - age;

                if (birthYear < 1899 || birthYear > 2100)
                {
                    Debug.LogErrorFormat("Birth year is invalid, possible clock hacking:  Age = {0}, Current Year = {1}, BirthYear = {2}", age, currentYear, birthYear);
                }
                else
                {
                    return birthYear;
                }
            }

            return 0;
        }
    }
}

#endif
