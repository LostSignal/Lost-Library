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
            this.SendIdentityEventAsTrackEvent(gender, age, eventData);
        }

        public override void Screen(string screenName, Dictionary<string, object> eventData)
        {
            this.SendScreenEventAsTrackEvent(screenName, eventData);
        }

        public override void Transaction(string productId, decimal amount, string currency)
        {
            // Do nothing, it already handles this with IAP Validation
        }

        public override void TrackPosition(string eventName, Vector3 position)
        {
            // We're ignore these for PlayFab.  Would create too many calls.
        }

        public override void Track(string eventName, Dictionary<string, object> eventData)
        {
            // TODO [bgish]: fill this out corretly.  Would be best to use PF instead of PlayFabClientAPI
            // TODO [bgish]: should also save off in case it fails and we can try again later (even save to disk if neccessary
            //if (PF.IsLoggedIn)
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
