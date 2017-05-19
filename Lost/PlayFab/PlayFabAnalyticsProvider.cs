//-----------------------------------------------------------------------
// <copyright file="IAnalyticsHandler.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USE_PLAYFAB_SDK

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayFabAnalyticsProvider : AnalyticsProvider
    {
        public override void Identify(string userId, Dictionary<string, object> eventData, Gender gender, int age)
        {
            this.SendIdentityEventAsTrackEvent(userId, eventData, gender, age);
        }
        
        public override void Screen(string screenName, Dictionary<string, object> eventData)
        {
            this.SendScreenEventAsTrackEvent(screenName, eventData);
        }

        public override void Transaction(string productId, decimal amount, string currency, Dictionary<string, object> eventData, string receiptPurchaseData, string signature)
        {
            this.SendTransactionEventAsTrackEvent(productId, amount, currency, eventData, receiptPurchaseData, signature);
        }

        public override void TrackPosition(string eventName, Vector3 position)
        {
            // We're ignore these for PlayFab.  Would create too many calls.
        }

        public override void Track(string eventName, Dictionary<string, object> eventData)
        {
            // TODO [bgish] fill this out corretly.  Would be best to use PF instead of PlayFabClientAPI
            if (PF.IsLoggedIn)
            {
                PlayFab.PlayFabClientAPI.WritePlayerEvent(new PlayFab.ClientModels.WriteClientPlayerEventRequest
                {
                    EventName = eventName,
                    Body = eventData
                }, 
                response => { }, 
                error => { });
            }
        }
    }
}

#endif
