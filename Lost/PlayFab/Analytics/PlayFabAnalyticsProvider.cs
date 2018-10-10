//-----------------------------------------------------------------------
// <copyright file="IAnalyticsHandler.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USE_PLAYFAB_SDK

namespace Lost
{
    using System.Collections.Generic;

    public class PlayFabAnalyticsProvider : IAnalyticsProvider
    {
        void IAnalyticsProvider.FlushRequested()
        {
            // TODO [bgish]: Implement
        }

        void IAnalyticsProvider.CustomEvent(long sessionId, string eventName, IDictionary<string, object> eventData)
        {
            // TODO [bgish]: Make sure to add the playfabId
            // TODO [bgish]: Make sure to add the sessionId

            // TODO [bgish]: fill this out corretly.  Would be best to use PF instead of PlayFabClientAPI
            // TODO [bgish]: should also save off in case it fails and we can try again later (even save to disk if neccessary
            //if (PF.IsLoggedIn)
            {
                PlayFab.PlayFabClientAPI.WritePlayerEvent(new PlayFab.ClientModels.WriteClientPlayerEventRequest
                {
                    EventName = eventName,
                    Body = (Dictionary<string, object>)eventData
                },
                response => { },
                error => { });
            }
        }

        void IAnalyticsProvider.Transaction(long sessionId, string productId, decimal amount, string currency, string receiptPurchaseData, string signature)
        {
            // TODO [bgish]: Implement
        }
    }
}

#endif
