//-----------------------------------------------------------------------
// <copyright file="PlayFabPushHandler.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USE_PLAYFAB_SDK

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayFabPushHandler : Lost.SingletonGameObject<PlayFabPushHandler>
    {
        private Queue<string> pushNotifications = new Queue<string>();

        #if UNITY_IOS
        private PlayFabIOSPushHandler iOSPushHandler;
        #elif UNITY_ANDROID
        private PlayFabAndroidPushHandler androidPushHandler;
        #endif

        protected override void Awake()
        {
            base.Awake();

            // push notifications don't work in editor, so lets not startup that code
            if (Application.isEditor)
            {
                return;
            }

            #if UNITY_IPHONE
            this.iOSPushHandler = this.gameObject.AddComponent<PlayFabIOSPushHandler>();
            #elif UNITY_ANDROID
            this.androidPushHandler = this.gameObject.AddComponent<PlayFabAndroidPushHandler>();
            #endif
        }

        public bool HasPushNotifications
        {
            get { return this.pushNotifications.Count > 0; }
        }

        public string GetAndRemoveLatestPushNotification()
        {
            return this.pushNotifications.Dequeue();
        }

        public void ReceivedPushNotification(string pushMessage)
        {
            this.pushNotifications.Enqueue(pushMessage);
        }

        public void ScheduleLocalPushNotification(string title, string message, DateTime dateTime)
        {
            #if UNITY_IOS
            
            if (this.iOSPushHandler)
            {
                this.iOSPushHandler.ScheduleLocalPushNotification(title, message, dateTime);
            }

            #elif UNITY_ANDROID
            
            if (this.androidPushHandler)
            {
                this.androidPushHandler.ScheduleLocalPushNotification(title, message, dateTime);
            }
            
            #endif
        }
    }
}

#endif
