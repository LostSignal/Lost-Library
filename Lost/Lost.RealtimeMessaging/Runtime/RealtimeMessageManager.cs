//-----------------------------------------------------------------------
// <copyright file="RealtimeMessageManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost{
    using System;
    using System.Collections.Generic;
    using IO.Ably;
    using Lost.AppConfig;
    using Newtonsoft.Json.Linq;
    using UnityEngine;

    public class RealtimeMessageManager : SingletonGameObject<RealtimeMessageManager>    {
        private Dictionary<string, Type> messageTypes = new Dictionary<string, Type>();
        private HashSet<string> subscribedChannels = new HashSet<string>();
        
        private AblyRealtime ably;
        
        public void RegisterType<T>() where T : RealtimeMessage, new()
        {
            this.messageTypes.Add(typeof(T).Name, typeof(T));
        }
        
        public void Subscribe(string channel)
        {
            if (this.subscribedChannels.Contains(channel) == false)
            {
                this.ably.Channels.Get(channel).Subscribe(this.MessageReceived);
                this.subscribedChannels.Add(channel);
            }
        }
        
        public void Unsubscribe(string channel)
        {
            if (this.subscribedChannels.Contains(channel))
            {
                this.ably.Channels.Get(channel).Unsubscribe(this.MessageReceived);
                this.subscribedChannels.Remove(channel);
            }
        }
                
        protected override void Awake()
        {
            base.Awake();

            var ablyKey = RuntimeAppConfig.Instance.GetString(RealtimeMessagingSettingsRuntime.AblyKeyKey);
            this.ably = new AblyRealtime(ablyKey);
            this.SubscribeToPlayFabId();
        
            // If we haven't logged in yet, then the subscribe will fail, so registering for login event so we can subscribe again
            PF.PlayfabEvents.OnLoginResultEvent += this.PlayfabEvents_OnLoginResultEvent;
        }
        
        private void PlayfabEvents_OnLoginResultEvent(PlayFab.ClientModels.LoginResult result)
        {
            this.SubscribeToPlayFabId();
        }
        
        private void SubscribeToPlayFabId()
        {
            if (string.IsNullOrEmpty(PF.User.PlayFabId) == false)
            {
                this.Subscribe(PF.User.PlayFabId);
            }
        }
        
        private void MessageReceived(Message message)
        {
            string json = message.Data as string;
            JObject jObject = null;

            try
            {
                jObject = JObject.Parse(json);
            }
            catch
            {
                Debug.LogError($"Received RealtimeMessage with invalid Json: {json}");
                return;
            }

            string realtimeMessageType = jObject["Type"]?.ToString();

            if (realtimeMessageType == null)
            {
                Debug.LogError($"Received Json that was not a RealtimeMessage: {json}");
            }
            else if (this.messageTypes.TryGetValue(realtimeMessageType, out Type type))
            {
                var realtimeMessage = JsonUtil.Deserialize(json, type);

                // TODO [bgish]: Forward realtimeMessage onto the message subscription system

                Debug.Log($"Received RealtimeMessage of type {realtimeMessageType} and json: {json}");
            }
            else
            {
                Debug.LogError($"Received RealtimeMessage of unknown type {realtimeMessageType}");
            }
        }    }}