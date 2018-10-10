//-----------------------------------------------------------------------
// <copyright file="AnalyticsEvent.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Analytics
{
    using System.Collections.Generic;

    public static class AnalyticsEvent
    {
        // FTUE
        private const string EventNameFirstInteraction = "first_interaction";
        private const string EventNameTutorialStart = "tutorial_start";
        private const string EventNameTutorialStep = "tutorial_step";
        private const string EventNameTutorialSkip = "tutorial_skip";
        private const string EventNameTutorialComplete = "tutorial_complete";

        // Game Progress
        private const string EventNameLevelStart = "level_start";
        private const string EventNameLevelFail = "level_fail";
        private const string EventNameLevelQuit = "level_quit";
        private const string EventNameLevelSkip = "level_skip";
        private const string EventNameLevelComplete = "level_complete";
        private const string EventNameGameStart = "game_start";
        private const string EventNameGameOver = "game_over";
        private const string EventNameLevelUp = "level_up";

        // Monetization
        private const string EventNameStoreOpened = "store_opened";
        private const string EventNameStoreItemClick = "store_item_click";
        private const string EventNameIapTransaction = "iap_transaction";
        private const string EventNameItemAcquired = "item_acquired";
        private const string EventNameItemSpent = "item_spent";
        private const string EventNameAdOffer = "ad_offer";
        private const string EventNameAdStart = "ad_start";
        private const string EventNameAdSkip = "ad_skip";
        private const string EventNameAdComplete = "ad_complete";
        private const string EventNamePostAdAction = "post_ad_action";

        // Engagement/Social
        private const string EventNamePushNotificationEnable = "push_notification_enable";
        private const string EventNamePushNotificationClick = "push_notification_click";
        private const string EventNameChatMessageSent = "chat_msg_sent";
        private const string EventNameAchievementStep = "achievement_step";
        private const string EventNameAchievementUnlocked = "achievement_unlocked";
        private const string EventNameUserSignup = "user_signup";
        private const string EventNameSocialShare = "social_share";
        private const string EventNameSocialShareAccept = "social_share_accept";

        // Application Navigation
        private const string EventNameScreenVisit = "screen_visit";
        private const string EventNameCutsceneStart = "cutscene_start";
        private const string EventNameCutsceneSkip = "cutscene_skip";


        // Column Names
        private const string EventColumnActionId = "action_id";
        private const string EventColumnCutsceneName = "scene_name";
        private const string EventColumnScreenName = "screen_name";
        private const string EventColumnPushNotificationMessageId = "message_id";

        // Column Names - Levels
        private const string EventColumnNewLevelName = "new_level_name";
        private const string EventColumnNewLevelIndex = "new_level_index";
        private const string EventColumnLevelName = "level_name";
        private const string EventColumnLevelIndex = "level_index";

        // Column Names - Ads
        private const string EventColumnAdRewarded = "rewarded";
        private const string EventColumnAdNetwork = "network";
        private const string EventColumnAdPlacementId = "placement_id";

        // Column Names - Tutorial
        private const string EventColumnTutorialId = "tutorial_id";
        private const string EventColumnTutorialStepIndex = "step_index";

        // Column Names - Achievements
        private const string EventColumnAchievementId = "achievement_id";
        private const string EventColumnAchievementStepIndex = "step_index";

        // Column Names - Social Sharing
        private const string EventColumnSocialShareType = "share_type";
        private const string EventColumnSocialNetwork = "social_network";
        private const string EventColumnSocialSenderId = "sender_id";
        private const string EventColumnSocialRecipientId = "recipient_id";
        private const string EventColumnSocialAuthorizationNetwork = "authorization_network";

        // Column Names - Monetization
        private const string EventColumnMonetizationCurrencyType = "currency_type";
        private const string EventColumnMonetizationTransactionContext = "transaction_context";
        private const string EventColumnMonetizationItemId = "item_id";
        private const string EventColumnMonetizationItemType = "item_type";
        private const string EventColumnMonetizationLevel = "level";
        private const string EventColumnMonetizationTransactionId = "transaction_id";
        private const string EventColumnMonetizationPrice = "price";
        private const string EventColumnMonetizationAmount = "amount";
        private const string EventColumnMonetizationBalance = "balance";
        private const string EventColumnMonetizationStoreType = "type";
        private const string EventColumnMonetizationItemName = "item_name";
        
        private static Dictionary<string, object> eventDataCache = new Dictionary<string, object>();

        #region Standard Analytics Wrappers

        public static AnalyticsResult AchievementStep(int stepIndex, string achievementId, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameAchievementStep, eventData
                .SafeAdd(EventColumnAchievementStepIndex, stepIndex)
                .SafeAdd(EventColumnAchievementId, achievementId));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.AchievementStep(stepIndex, achievementId, eventData));
            #endif
        }

        public static AnalyticsResult AchievementUnlocked(string achievementId, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameAchievementUnlocked, eventData
                .SafeAdd(EventColumnAchievementId, achievementId));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.AchievementUnlocked(achievementId, eventData));
            #endif
        }

        public static AnalyticsResult AdComplete(bool rewarded, AdvertisingNetwork network, string placementId = null, IDictionary<string, object> eventData = null)
        {
            return AdComplete(rewarded, AnalyticsEnums.Get(network), placementId, eventData);
        }

        public static AnalyticsResult AdComplete(bool rewarded, string network = null, string placementId = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameAdComplete, eventData
                .SafeAdd(EventColumnAdRewarded, rewarded)
                .SafeAdd(EventColumnAdNetwork, network)
                .SafeAdd(EventColumnAdPlacementId, placementId));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.AdComplete(rewarded, network, placementId, eventData));
            #endif
        }
        
        public static AnalyticsResult AdOffer(bool rewarded, AdvertisingNetwork network, string placementId = null, IDictionary<string, object> eventData = null)
        {
            return AdOffer(rewarded, AnalyticsEnums.Get(network), placementId, eventData);
        }

        public static AnalyticsResult AdOffer(bool rewarded, string network = null, string placementId = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameAdOffer, eventData
                .SafeAdd(EventColumnAdRewarded, rewarded)
                .SafeAdd(EventColumnAdNetwork, network)
                .SafeAdd(EventColumnAdPlacementId, placementId));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.AdOffer(rewarded, network, placementId, eventData));
            #endif
        }

        public static AnalyticsResult AdSkip(bool rewarded, AdvertisingNetwork network, string placementId = null, IDictionary<string, object> eventData = null)
        {
            return AdSkip(rewarded, AnalyticsEnums.Get(network), placementId, eventData);
        }

        public static AnalyticsResult AdSkip(bool rewarded, string network = null, string placementId = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameAdSkip, eventData
                .SafeAdd(EventColumnAdRewarded, rewarded)
                .SafeAdd(EventColumnAdNetwork, network)
                .SafeAdd(EventColumnAdPlacementId, placementId));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.AdSkip(rewarded, network, placementId, eventData));
            #endif
        }

        public static AnalyticsResult AdStart(bool rewarded, AdvertisingNetwork network, string placementId = null, IDictionary<string, object> eventData = null)
        {
            return AdStart(rewarded, AnalyticsEnums.Get(network), placementId, eventData);
        }

        public static AnalyticsResult AdStart(bool rewarded, string network = null, string placementId = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameAdStart, eventData
                .SafeAdd(EventColumnAdRewarded, rewarded)
                .SafeAdd(EventColumnAdNetwork, network)
                .SafeAdd(EventColumnAdPlacementId, placementId));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.AdStart(rewarded, network, placementId, eventData));
            #endif
        }

        public static AnalyticsResult ChatMessageSent(IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameChatMessageSent, eventData);

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.ChatMessageSent(eventData));
            #endif
        }

        public static AnalyticsResult Custom(string eventName, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(eventName, eventData);

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.Custom(eventName, eventData));
            #endif
        }
        
        public static AnalyticsResult CutsceneSkip(string name, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameCutsceneSkip, eventData
                .SafeAdd(EventColumnCutsceneName, name));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.CutsceneSkip(name, eventData));
            #endif
        }

        public static AnalyticsResult CutsceneStart(string name, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameCutsceneStart, eventData
                .SafeAdd(EventColumnCutsceneName, name));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.CutsceneStart(name, eventData));
            #endif
        }

        public static AnalyticsResult FirstInteraction(string actionId = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameFirstInteraction, eventData
                .SafeAdd(EventColumnActionId, actionId));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.FirstInteraction(actionId, eventData));
            #endif
        }

        public static AnalyticsResult GameOver(string name = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameGameOver, eventData
                .SafeAdd(EventColumnLevelName, name));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.GameOver(name, eventData));
            #endif
        }

        public static AnalyticsResult GameOver(int index, string name = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameGameOver, eventData
                .SafeAdd(EventColumnLevelIndex, index)
                .SafeAdd(EventColumnLevelName, name));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.GameOver(index, name, eventData));
            #endif
        }

        public static AnalyticsResult GameStart(IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameGameStart, eventData);

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.GameStart(eventData));
            #endif
        }

        public static AnalyticsResult IAPTransaction(string transactionContext, float price, string itemId, string itemType = null, string level = null, string transactionId = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameIapTransaction, eventData
                .SafeAdd(EventColumnMonetizationTransactionContext, transactionContext)
                .SafeAdd(EventColumnMonetizationPrice, price)
                .SafeAdd(EventColumnMonetizationItemId, itemId)
                .SafeAdd(EventColumnMonetizationItemType, itemType)
                .SafeAdd(EventColumnMonetizationLevel, level)
                .SafeAdd(EventColumnMonetizationTransactionId, transactionId));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.IAPTransaction(transactionContext, price, itemId, itemType, level, transactionId, eventData));
            #endif
        }
        
        public static AnalyticsResult ItemAcquired(AcquisitionType currencyType, string transactionContext, float amount, string itemId, string itemType = null, string level = null, string transactionId = null, IDictionary<string, object> eventData = null)
        {
            return ItemAcquired(currencyType, transactionContext, amount, itemId, float.MinValue, itemType, level, transactionId, eventData);
        }

        public static AnalyticsResult ItemAcquired(AcquisitionType currencyType, string transactionContext, float amount, string itemId, float balance, string itemType = null, string level = null, string transactionId = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameItemAcquired, eventData
                .SafeAdd(EventColumnMonetizationCurrencyType, AnalyticsEnums.Get(currencyType))
                .SafeAdd(EventColumnMonetizationTransactionContext, transactionContext)
                .SafeAdd(EventColumnMonetizationAmount, amount)
                .SafeAdd(EventColumnMonetizationItemId, itemId)
                .SafeAddIf(EventColumnMonetizationBalance, balance, balance != float.MinValue)
                .SafeAdd(EventColumnMonetizationItemType, itemType)
                .SafeAdd(EventColumnMonetizationLevel, level)
                .SafeAdd(EventColumnMonetizationTransactionId, transactionId));

            #if UNITY_ANALYTICS
            if (balance == float.MinValue)
            {
                return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.ItemAcquired(AnalyticsEnums.Convert(currencyType), transactionContext, amount, itemId, itemType, level, transactionId, eventData));
            }
            else
            {
                return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.ItemAcquired(AnalyticsEnums.Convert(currencyType), transactionContext, amount, itemId, balance, itemType, level, transactionId, eventData));
            }            
            #endif
        }

        public static AnalyticsResult ItemSpent(AcquisitionType currencyType, string transactionContext, float amount, string itemId, string itemType = null, string level = null, string transactionId = null, IDictionary<string, object> eventData = null)
        {
            return ItemSpent(currencyType, transactionContext, amount, itemId, float.MinValue, itemType, level, transactionId, eventData);
        }

        public static AnalyticsResult ItemSpent(AcquisitionType currencyType, string transactionContext, float amount, string itemId, float balance, string itemType = null, string level = null, string transactionId = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameItemSpent, eventData
                .SafeAdd(EventColumnMonetizationCurrencyType, AnalyticsEnums.Get(currencyType))
                .SafeAdd(EventColumnMonetizationTransactionContext, transactionContext)
                .SafeAdd(EventColumnMonetizationAmount, amount)
                .SafeAdd(EventColumnMonetizationItemId, itemId)
                .SafeAddIf(EventColumnMonetizationBalance, balance, balance != float.MinValue)
                .SafeAdd(EventColumnMonetizationItemType, itemType)
                .SafeAdd(EventColumnMonetizationLevel, level)
                .SafeAdd(EventColumnMonetizationTransactionId, transactionId));

            #if UNITY_ANALYTICS
            if (balance == float.MinValue)
            {
                return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.ItemSpent(AnalyticsEnums.Convert(currencyType), transactionContext, amount, itemId, itemType, level, transactionId, eventData));
            }
            else
            {
                return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.ItemSpent(AnalyticsEnums.Convert(currencyType), transactionContext, amount, itemId, balance, itemType, level, transactionId, eventData));
            }            
            #endif
        }
        
        public static AnalyticsResult LevelComplete(string name, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelComplete, eventData
                .SafeAdd(EventColumnLevelName, name));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelComplete(name, eventData));
            #endif
        }

        public static AnalyticsResult LevelComplete(int index, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelComplete, eventData
                .SafeAdd(EventColumnLevelIndex, index));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelComplete(index, eventData));
            #endif
        }

        public static AnalyticsResult LevelComplete(string name, int index, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelComplete, eventData
                .SafeAdd(EventColumnLevelName, name)
                .SafeAdd(EventColumnLevelIndex, index));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelComplete(name, index, eventData));
            #endif
        }

        public static AnalyticsResult LevelFail(string name, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelFail, eventData
                .SafeAdd(EventColumnLevelName, name));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelFail(name, eventData));
            #endif
        }

        public static AnalyticsResult LevelFail(int index, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelFail, eventData
                .SafeAdd(EventColumnLevelIndex, index));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelFail(index, eventData));
            #endif
        }

        public static AnalyticsResult LevelFail(string name, int index, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelFail, eventData
                .SafeAdd(EventColumnLevelName, name)
                .SafeAdd(EventColumnLevelIndex, index));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelFail(name, index, eventData));
            #endif
        }

        public static AnalyticsResult LevelQuit(string name, int index, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelQuit, eventData
                .SafeAdd(EventColumnLevelName, name)
                .SafeAdd(EventColumnLevelIndex, index));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelQuit(name, index, eventData));
            #endif
        }

        public static AnalyticsResult LevelQuit(string name, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelQuit, eventData
                .SafeAdd(EventColumnLevelName, name));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelQuit(name, eventData));
            #endif
        }

        public static AnalyticsResult LevelQuit(int index, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelQuit, eventData
                .SafeAdd(EventColumnLevelIndex, index));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelQuit(index, eventData));
            #endif
        }

        public static AnalyticsResult LevelSkip(string name, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelSkip, eventData
                .SafeAdd(EventColumnLevelName, name));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelSkip(name, eventData));
            #endif
        }

        public static AnalyticsResult LevelSkip(int index, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelSkip, eventData
                .SafeAdd(EventColumnLevelIndex, index));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelSkip(index, eventData));
            #endif
        }

        public static AnalyticsResult LevelSkip(string name, int index, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelSkip, eventData
                .SafeAdd(EventColumnLevelName, name)
                .SafeAdd(EventColumnLevelIndex, index));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelSkip(name, index, eventData));
            #endif
        }

        public static AnalyticsResult LevelStart(string name, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelStart, eventData
                .SafeAdd(EventColumnLevelName, name));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelStart(name, eventData));
            #endif
        }

        public static AnalyticsResult LevelStart(int index, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelStart, eventData
                .SafeAdd(EventColumnLevelIndex, index));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelStart(index, eventData));
            #endif
        }

        public static AnalyticsResult LevelStart(string name, int index, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelStart, eventData
                .SafeAdd(EventColumnLevelName, name)
                .SafeAdd(EventColumnLevelIndex, index));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelStart(name, index, eventData));
            #endif
        }

        public static AnalyticsResult LevelUp(string name, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelUp, eventData
                .SafeAdd(EventColumnNewLevelName, name));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelUp(name, eventData));
            #endif
        }

        public static AnalyticsResult LevelUp(string name, int index, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelUp, eventData
                .SafeAdd(EventColumnNewLevelName, name)
                .SafeAdd(EventColumnNewLevelIndex, index));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelUp(name, index, eventData));
            #endif
        }

        public static AnalyticsResult LevelUp(int index, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameLevelUp, eventData
                .SafeAdd(EventColumnNewLevelIndex, index));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.LevelUp(index, eventData));
            #endif
        }

        public static AnalyticsResult PostAdAction(bool rewarded, AdvertisingNetwork network, string placementId = null, IDictionary<string, object> eventData = null)
        {
            return PostAdAction(rewarded, AnalyticsEnums.Get(network), placementId, eventData);
        }

        public static AnalyticsResult PostAdAction(bool rewarded, string network = null, string placementId = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNamePostAdAction, eventData
                .SafeAdd(EventColumnAdRewarded, rewarded)
                .SafeAdd(EventColumnAdNetwork, network)
                .SafeAdd(EventColumnAdPlacementId, placementId));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.PostAdAction(rewarded, network, placementId, eventData));
            #endif
        }

        public static AnalyticsResult PushNotificationClick(string message_id, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNamePushNotificationClick, eventData
                .SafeAdd(EventColumnPushNotificationMessageId, message_id));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.PushNotificationClick(message_id, eventData));
            #endif
        }

        public static AnalyticsResult PushNotificationEnable(IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNamePushNotificationEnable, eventData);

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.PushNotificationEnable(eventData));
            #endif
        }
        
        public static AnalyticsResult ScreenVisit(ScreenName screenName, IDictionary<string, object> eventData = null)
        {
            return ScreenVisit(AnalyticsEnums.Get(screenName), eventData);
        }

        public static AnalyticsResult ScreenVisit(string screenName, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameScreenVisit, eventData
                .SafeAdd(EventColumnScreenName, screenName));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.ScreenVisit(screenName, eventData));
            #endif
        }
        
        public static AnalyticsResult SocialShare(ShareType shareType, string socialNetwork, string senderId = null, string recipientId = null, IDictionary<string, object> eventData = null)
        {
            return SocialShare(AnalyticsEnums.Get(shareType), socialNetwork, senderId, recipientId, eventData);
        }

        public static AnalyticsResult SocialShare(ShareType shareType, SocialNetwork socialNetwork, string senderId = null, string recipientId = null, IDictionary<string, object> eventData = null)
        {
            return SocialShare(AnalyticsEnums.Get(shareType), AnalyticsEnums.Get(socialNetwork), senderId, recipientId, eventData);
        }

        public static AnalyticsResult SocialShare(string shareType, SocialNetwork socialNetwork, string senderId = null, string recipientId = null, IDictionary<string, object> eventData = null)
        {
            return SocialShare(shareType, AnalyticsEnums.Get(socialNetwork), senderId, recipientId, eventData);
        }

        public static AnalyticsResult SocialShare(string shareType, string socialNetwork, string senderId = null, string recipientId = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameSocialShare, eventData
                .SafeAdd(EventColumnSocialShareType, shareType)
                .SafeAdd(EventColumnSocialNetwork, socialNetwork)
                .SafeAdd(EventColumnSocialSenderId, senderId)
                .SafeAdd(EventColumnSocialRecipientId, recipientId));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.SocialShare(shareType, socialNetwork, senderId, recipientId, eventData));
            #endif
        }
        
        public static AnalyticsResult SocialShareAccept(ShareType shareType, SocialNetwork socialNetwork, string senderId = null, string recipientId = null, IDictionary<string, object> eventData = null)
        {
            return SocialShareAccept(AnalyticsEnums.Get(shareType), AnalyticsEnums.Get(socialNetwork), senderId, recipientId, eventData);
        }

        public static AnalyticsResult SocialShareAccept(string shareType, SocialNetwork socialNetwork, string senderId = null, string recipientId = null, IDictionary<string, object> eventData = null)
        {
            return SocialShareAccept(shareType, AnalyticsEnums.Get(socialNetwork), senderId, recipientId, eventData);
        }

        public static AnalyticsResult SocialShareAccept(ShareType shareType, string socialNetwork, string senderId = null, string recipientId = null, IDictionary<string, object> eventData = null)
        {
            return SocialShareAccept(AnalyticsEnums.Get(shareType), socialNetwork, senderId, recipientId, eventData);
        }
        
        public static AnalyticsResult SocialShareAccept(string shareType, string socialNetwork, string senderId = null, string recipientId = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameSocialShareAccept, eventData
                .SafeAdd(EventColumnSocialShareType, shareType)
                .SafeAdd(EventColumnSocialNetwork, socialNetwork)
                .SafeAdd(EventColumnSocialSenderId, senderId)
                .SafeAdd(EventColumnSocialRecipientId, recipientId));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.SocialShareAccept(shareType, socialNetwork, senderId, recipientId, eventData));
            #endif
        }

        public static AnalyticsResult StoreItemClick(StoreType storeType, string itemId, string itemName = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameStoreItemClick, eventData
                .SafeAdd(EventColumnMonetizationStoreType, AnalyticsEnums.Get(storeType))
                .SafeAdd(EventColumnMonetizationItemId, itemId)
                .SafeAdd(EventColumnMonetizationItemName, itemName));
            
            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.StoreItemClick(AnalyticsEnums.Convert(storeType), itemId, itemName, eventData));
            #endif
        }

        public static AnalyticsResult StoreOpened(StoreType storeType, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameStoreOpened, eventData
                .SafeAdd(EventColumnMonetizationStoreType, AnalyticsEnums.Get(storeType)));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.StoreOpened(AnalyticsEnums.Convert(storeType), eventData));
            #endif
        }

        public static AnalyticsResult TutorialComplete(string tutorialId = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameTutorialComplete, eventData
                .SafeAdd(EventColumnTutorialId, tutorialId));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.TutorialComplete(tutorialId, eventData));
            #endif
        }

        public static AnalyticsResult TutorialSkip(string tutorialId = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameTutorialSkip, eventData
                .SafeAdd(EventColumnTutorialId, tutorialId));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.TutorialSkip(tutorialId, eventData));
            #endif
        }

        public static AnalyticsResult TutorialStart(string tutorialId = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameTutorialStart, eventData
                .SafeAdd(EventColumnTutorialId, tutorialId));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.TutorialStart(tutorialId, eventData));
            #endif
        }

        public static AnalyticsResult TutorialStep(int stepIndex, string tutorialId = null, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameTutorialStep, eventData
                .SafeAdd(EventColumnTutorialStepIndex, stepIndex)
                .SafeAdd(EventColumnTutorialId, tutorialId));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.TutorialStep(stepIndex, tutorialId, eventData));
            #endif
        }

        public static AnalyticsResult UserSignup(AuthorizationNetwork authorizationNetwork, IDictionary<string, object> eventData = null)
        {
            return UserSignup(AnalyticsEnums.Get(authorizationNetwork), eventData);
        }

        public static AnalyticsResult UserSignup(string authorizationNetwork, IDictionary<string, object> eventData = null)
        {
            Analytics.CustomEvent(EventNameUserSignup, eventData
                .SafeAdd(EventColumnSocialAuthorizationNetwork, authorizationNetwork));

            #if UNITY_ANALYTICS
            return AnalyticsEnums.Convert(UnityEngine.Analytics.AnalyticsEvent.UserSignup(authorizationNetwork, eventData));
            #endif
        }

        #endregion
        
        private static IDictionary<string, object> SafeAdd(this IDictionary<string, object> eventData, string key, object value)
        {
            if (eventData == null)
            {
                eventDataCache.Clear();
                eventData = eventDataCache;
            }

            if (eventData.ContainsKey(key) == false)
            {
                eventData.Add(key, value);
            }
            else
            {
                UnityEngine.Debug.LogErrorFormat("Tried adding already existing Key \"{0}\" -> Value \"{1}\" to analytics eventData.", key, value);
            }

            return eventData;
        }

        private static IDictionary<string, object> SafeAddIf(this IDictionary<string, object> eventData, string key, object value, bool condition)
        {
            if (condition)
            {
                return SafeAdd(eventData, key, value);
            }

            return eventData;
        }
    }
}
