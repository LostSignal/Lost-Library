//-----------------------------------------------------------------------
// <copyright file="IAnalyticsHandler.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;

    public interface IAnalyticsProvider
    {
        void FlushRequested();

        void CustomEvent(long sessionId, string eventName, IDictionary<string, object> eventData);

        void Transaction(long sessionId, string productId, Decimal amount, string currency, string receiptPurchaseData, string signature);
    }
}
