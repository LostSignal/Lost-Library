//-----------------------------------------------------------------------
// <copyright file="Analytics.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Analytics
{
    using System;
    using System.Collections.Generic;

    public static class Analytics
    {
        public static Action<string, IDictionary<string, object>> CustomEventFired { get; set; }
        public static Action<string, decimal, string> TransactionEventFired { get; set; }

        public static AnalyticsResult CustomEvent(string customEventName, IDictionary<string, object> eventData)
        {
            if (CustomEventFired != null)
            {
                CustomEventFired(customEventName, eventData);
            }

            return AnalyticsEnums.Convert(UnityEngine.Analytics.Analytics.CustomEvent(customEventName, eventData));
        }
        
        public static AnalyticsResult Transaction(string productId, decimal amount, string currency)
        {
            if (TransactionEventFired != null)
            {
                TransactionEventFired(productId, amount, currency);
            }

            return AnalyticsEnums.Convert(UnityEngine.Analytics.Analytics.Transaction(productId, amount, currency));
        }
    }
}
