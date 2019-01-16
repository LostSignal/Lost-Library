//-----------------------------------------------------------------------
// <copyright file="PlayFabAndroidPushHandler.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_ANDROID && USING_PLAYFAB_SDK

namespace Lost
{
    using System.Collections;
    using PlayFab;
    using PlayFab.ClientModels;
    using UnityEngine;
    using System;

    public class PlayFabAndroidPushHandler : MonoBehaviour
    {
        #if !USING_PLAYFAB_ANDROID_SDK

        private void Awake()
        {
            Debug.LogError("Trying to use PlayFabAndroidPushHandler without the PlayFab Android PushNotification Plugin Installed.", this);
        }

        public void ScheduleLocalPushNotification(string title, string message, DateTime dateTime)
        {
            Debug.LogError("Trying to use PlayFabAndroidPushHandler without the PlayFab Android PushNotification Plugin Installed.", this);
        }

        #else

        private const float RetryWaitTime = 1.0f;
        private const int RetryCountMax = 10;

        private PlayFabPushHandler pushHandler;
        private string deviceToken;

        public void ScheduleLocalPushNotification(string title, string message, DateTime dateTime)
        {
            PlayFabAndroidPlugin.ScheduleNotification(message, dateTime);
        }

        private void Awake()
        {
            this.pushHandler = this.GetComponentInParent<PlayFabPushHandler>();
        }

        private IEnumerator Start()
        {
            PlayFabGoogleCloudMessaging._RegistrationCallback += (token, error) => { this.deviceToken = token; };

            PlayFabGoogleCloudMessaging._MessageCallback += (message) => { this.pushHandler.ReceivedPushNotification(message); };

            PlayFabGoogleCloudMessaging._RegistrationReadyCallback += (status) =>
            {
                if (status)
                {
                    PlayFabGoogleCloudMessaging.GetToken();     // request device token
                    PlayFabAndroidPlugin.UpdateRouting(false);  // suppress push notifications while app is running
                }
            };

            // TODO [bgish]:  This should probably be located in AppSettings, if not defined print error and early out GameObject.Destroy(this);
            PlayFabAndroidPlugin.Init("GoogleAppId");

            int retryCount = 0;
            while (this.deviceToken == null)
            {
                retryCount++;

                if (retryCount >= RetryCountMax)
                {
                    break;
                }

                yield return WaitForUtil.Seconds(RetryWaitTime);
            }

            // if we got here and still no deviceToken, then we timed out
            if (this.deviceToken == null)
            {
                Debug.LogError("PlayFabAndroidPushHandler timed out waiting for the RegistrationCallback to complete.");

                // cleaning up this Android push notification handler so it doesn't take up any cycles
                GameObject.Destroy(this);
                yield break;
            }

            PlayFabClientAPI.AndroidDevicePushNotificationRegistration(
                new AndroidDevicePushNotificationRegistrationRequest { DeviceToken = this.deviceToken },
                (result) =>
                {
                    Debug.Log("Push Notification Registration Successful!");
                },
                (error) =>
                {
                    Debug.Log("Error Registering for Android Push Notifications!");
                    Debug.Log(error.Error);
                    Debug.Log(error.ErrorMessage);
                    Debug.Log(error.ErrorDetails);
                });
        }

        private void OnApplicationFocus(bool focus)
        {
            // checking that we actually got a device token before updating routing
            if (string.IsNullOrEmpty(this.deviceToken) == false)
            {
                // if we lose focus, then we don't want to consume push notifications anymore
                PlayFabAndroidPlugin.UpdateRouting(!focus);
            }
        }

        #endif
    }
}

#endif
