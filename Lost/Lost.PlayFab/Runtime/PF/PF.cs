//-----------------------------------------------------------------------
// <copyright file="PF.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using System;
    using Lost.AppConfig;
    using PlayFab;
    using PlayFab.ClientModels;
    using PlayFab.Events;
    using UnityEngine;

    public delegate void OnServerNeedsReloginDelegate();

    public static partial class PF
    {
        public delegate void Action<T1, T2, T3, T4, T5>(T1 arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

        public static event OnServerNeedsReloginDelegate ServerNeedsRelogin;

        private static DateTime serverTime = DateTime.MinValue;
        private static float serverRealtimeSinceStartup = 0.0f;
        private static ISerializerPlugin serializerPlugin;

        public static PlayFabEvents PlayfabEvents { get; private set; }

        public static PushNotificationsHelper PushNotifications { get; private set; }
        public static CloudScriptHelper CloudScript { get; private set; }
        public static PurchasingHelper Purchasing { get; private set; }
        public static InventoryHelper Inventory { get; private set; }
        public static CharacterHelper Character { get; private set; }
        public static TitleDataHelper TitleData { get; private set; }
        public static VirtualCurrencyHelper VC { get; private set; }
        public static CatalogHelper Catalog { get; private set; }
        public static LoginHelper Login { get; private set; }
        public static UserHelper User { get; private set; }

        public static string CatalogVersion { get; private set; }
        public static int CloudScriptRevision { get; private set; }

        public static DateTime ServerUtcTime
        {
            get { return serverTime != DateTime.MinValue ? serverTime.AddSeconds(Time.realtimeSinceStartup - serverRealtimeSinceStartup) : serverTime; }
        }

        public static ISerializerPlugin SerializerPlugin
        {
            get
            {
                if (serializerPlugin == null)
                {
                    serializerPlugin = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
                }

                return serializerPlugin;
            }
        }

        static PF()
        {
            PlayFabSettings.TitleId = RuntimeAppConfig.Instance.GetTitleId();
            CatalogVersion = RuntimeAppConfig.Instance.GetCatalogVersion();
            CloudScriptRevision = RuntimeAppConfig.Instance.GetCloudScriptRevision();

            PlayfabEvents = PlayFabEvents.Init();
            PlayfabEvents.OnGetTimeResultEvent += PlayfabEvents_OnGetTimeResultEvent;
            PlayfabEvents.OnGlobalErrorEvent += PlayfabEvents_OnGlobalErrorEvent;

            // Creating all the helpers
            PushNotifications = new PushNotificationsHelper();
            CloudScript = new CloudScriptHelper();
            Purchasing = new PurchasingHelper();
            Inventory = new InventoryHelper();
            Character = new CharacterHelper();
            TitleData = new TitleDataHelper();
            VC = new VirtualCurrencyHelper();
            Catalog = new CatalogHelper();
            Login = new LoginHelper();
            User = new UserHelper();
        }

        public static long ConvertPlayFabIdToLong(string playfabId)
        {
            return System.Convert.ToInt64(playfabId, 16);
        }

        public static int GetVirtualCurrenyPrice(this CatalogItem catalogItem, string currency)
        {
            if (catalogItem.VirtualCurrencyPrices.TryGetValue(currency, out uint price))
            {
                return (int)price;
            }

            return 0;
        }

        public static int GetVirtualCurrenyPrice(this StoreItem storeItem, string currency)
        {
            if (storeItem.VirtualCurrencyPrices.TryGetValue(currency, out uint price))
            {
                return (int)price;
            }

            return 0;
        }

        public static string GetVirtualCurrencyId(this StoreItem storeItem)
        {
            foreach (var currency in storeItem.VirtualCurrencyPrices)
            {
                if (currency.Key != "RM")
                {
                    return currency.Key;
                }
            }

            return null;
        }

        public static int GetCost(this StoreItem storeItem, string virtualCurrencyId)
        {
            if (storeItem.VirtualCurrencyPrices.TryGetValue(virtualCurrencyId, out uint cost))
            {
                return (int)cost;
            }

            return 0;
        }

        private static void PlayfabEvents_OnGetTimeResultEvent(GetTimeResult result)
        {
            serverRealtimeSinceStartup = Time.realtimeSinceStartup;
            serverTime = result.Time;
        }

        private static void PlayfabEvents_OnGlobalErrorEvent(PlayFab.SharedModels.PlayFabRequestCommon request, PlayFabError error)
        {
            if (error.Error == PlayFabErrorCode.InvalidSessionTicket)
            {
                Debug.Log("ServerNeedsRelogin");
                ServerNeedsRelogin?.Invoke();
            }
        }
    }
}

#endif
