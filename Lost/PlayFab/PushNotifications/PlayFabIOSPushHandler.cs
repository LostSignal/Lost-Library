//-----------------------------------------------------------------------
// <copyright file="PlayFabIOSPushHandler.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_IPHONE && USE_PLAYFAB_SDK

namespace Lost
{
    using System;
    using System.Collections;
    using PlayFab;
    using PlayFab.ClientModels;
    using UnityEngine;
    using UnityEngine.iOS;
    using NotificationServices = UnityEngine.iOS.NotificationServices;
    
    public class PlayFabIOSPushHandler : MonoBehaviour
    {
        private const float QueryTimerMax = 3.0f;
        private const float RetryWaitTime = 1.0f;
        private const int RetryCountMax = 10;

        private PlayFabPushHandler pushHandler;
        private float currentQueryTime = 0.0f;
        private string deviceToken = null;
        
        public void ScheduleLocalPushNotification(string title, string message, DateTime dateTime)
        {
            NotificationServices.ScheduleLocalNotification(new UnityEngine.iOS.LocalNotification()
            {
                alertAction = title,
                alertBody = message,
                fireDate = dateTime,
            });
        }

        private void Awake()
        {
            this.pushHandler.GetComponentInParent<PlayFabPushHandler>();
        }

        private IEnumerator Start()
        {        
            NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound, true);
        
            int retryCount = 0;
            while (this.deviceToken == null)
            {
                if (this.deviceToken == null && NotificationServices.deviceToken != null)
                {
                    this.deviceToken = BitConverter.ToString(NotificationServices.deviceToken).Replace("-", "").ToLower();
                }
        
                retryCount++;
        
                if (retryCount > RetryCountMax)
                {
                    yield break;
                }
        
                yield return new WaitForSeconds(RetryWaitTime);
            }

            // if we got here and still no deviceToken, then we timed out
            if (this.deviceToken == null)
            {
                Debug.LogError("PlayFabIOSPushHandler timed out waiting for the deviceToken.");

                // cleaning up this iOS push notification handler so it doesn't take up any cycles
                GameObject.Destroy(this);
                yield break;
            }

            PlayFabClientAPI.RegisterForIOSPushNotification(
                new RegisterForIOSPushNotificationRequest { DeviceToken = this.deviceToken }, 
                (result) => 
                {
                    Debug.Log("Push Notification Registration Successful!");
                },
                (error) =>
                {
                    Debug.Log("Error Registering for iOS Push Notifications!");
                    Debug.Log(error.Error);
                    Debug.Log(error.ErrorMessage);
                    Debug.Log(error.ErrorDetails);
                });
        }
        
        private void Update()
        {
            // no point checking if we don't have a device token yet
            if (this.deviceToken == null)
            {
                return;
            }

            this.currentQueryTime += Time.deltaTime;
            
            // early out if haven't met the query time
            if (this.currentQueryTime < QueryTimerMax)
            {
                return;
            }
        
            // reset timer
            this.currentQueryTime = 0.0f;
        
            // https://forum.unity3d.com/threads/using-the-new-notification-system.127016/
            if (NotificationServices.remoteNotificationCount != 0)
            {
                // iterating over all the remote notifications
                for (int i = 0; i < NotificationServices.remoteNotificationCount; i++)
                {
                    this.pushHandler.ReceivedPushNotification(NotificationServices.GetRemoteNotification(i).alertBody);
                }

                // clear all messages
                NotificationServices.ClearRemoteNotifications();
                NotificationServices.ClearLocalNotifications();
            }
        }
    }
}

#endif
