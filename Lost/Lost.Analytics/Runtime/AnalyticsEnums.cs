//-----------------------------------------------------------------------
// <copyright file="AnalyticsEnums.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Lost.Analytics
{
    public static class AnalyticsEnums
    {
        // Enum Name Caching Dictionaries
        private static Dictionary<ScreenName, string> screenNameCache = new Dictionary<ScreenName, string>();
        private static Dictionary<AdvertisingNetwork, string> advertisingNetworkCache = new Dictionary<AdvertisingNetwork, string>();
        private static Dictionary<AcquisitionType, string> acquisitionTypeCache = new Dictionary<AcquisitionType, string>();
        private static Dictionary<ShareType, string> shareTypeCache = new Dictionary<ShareType, string>();
        private static Dictionary<SocialNetwork, string> socialNetworkCache = new Dictionary<SocialNetwork, string>();
        private static Dictionary<StoreType, string> storeTypeCache = new Dictionary<StoreType, string>();
        private static Dictionary<AuthorizationNetwork, string> authorizationNetworkCache = new Dictionary<AuthorizationNetwork, string>();

        static AnalyticsEnums()
        {
            foreach (var value in Enum.GetValues(typeof(ScreenName)).Cast<ScreenName>())
            {
                Get(value);
            }

            foreach (var value in Enum.GetValues(typeof(AdvertisingNetwork)).Cast<AdvertisingNetwork>())
            {
                Get(value);
            }

            foreach (var value in Enum.GetValues(typeof(AcquisitionType)).Cast<AcquisitionType>())
            {
                Get(value);
            }

            foreach (var value in Enum.GetValues(typeof(ShareType)).Cast<ShareType>())
            {
                Get(value);
            }

            foreach (var value in Enum.GetValues(typeof(SocialNetwork)).Cast<SocialNetwork>())
            {
                Get(value);
            }

            foreach (var value in Enum.GetValues(typeof(StoreType)).Cast<StoreType>())
            {
                Get(value);
            }

            foreach (var value in Enum.GetValues(typeof(AuthorizationNetwork)).Cast<AuthorizationNetwork>())
            {
                Get(value);
            }
        }

        public static string Get(ScreenName screenName)
        {
            string enumString;

            if (screenNameCache.TryGetValue(screenName, out enumString) == false)
            {
                enumString = screenName.ToString();
                screenNameCache.Add(screenName, enumString);
            }

            return enumString;
        }

        public static string Get(AdvertisingNetwork advertisingNetwork)
        {
            string enumString;

            if (advertisingNetworkCache.TryGetValue(advertisingNetwork, out enumString) == false)
            {
                enumString = advertisingNetwork.ToString();
                advertisingNetworkCache.Add(advertisingNetwork, enumString);
            }

            return enumString;
        }

        public static string Get(AcquisitionType acquisitionType)
        {
            string enumString;

            if (acquisitionTypeCache.TryGetValue(acquisitionType, out enumString) == false)
            {
                enumString = acquisitionType.ToString();
                acquisitionTypeCache.Add(acquisitionType, enumString);
            }

            return enumString;
        }

        public static string Get(ShareType shareType)
        {
            string enumString;

            if (shareTypeCache.TryGetValue(shareType, out enumString) == false)
            {
                enumString = shareType.ToString();
                shareTypeCache.Add(shareType, enumString);
            }

            return enumString;
        }

        public static string Get(SocialNetwork socialNetwork)
        {
            string enumString;

            if (socialNetworkCache.TryGetValue(socialNetwork, out enumString) == false)
            {
                enumString = socialNetwork.ToString();
                socialNetworkCache.Add(socialNetwork, enumString);
            }

            return enumString;
        }

        public static string Get(StoreType storeType)
        {
            string enumString;

            if (storeTypeCache.TryGetValue(storeType, out enumString) == false)
            {
                enumString = storeType.ToString();
                storeTypeCache.Add(storeType, enumString);
            }

            return enumString;
        }

        public static string Get(AuthorizationNetwork authorizationNetwork)
        {
            string enumString;

            if (authorizationNetworkCache.TryGetValue(authorizationNetwork, out enumString) == false)
            {
                enumString = authorizationNetwork.ToString();
                authorizationNetworkCache.Add(authorizationNetwork, enumString);
            }

            return enumString;
        }

        #if UNITY_ANALYTICS

        public static UnityEngine.Analytics.ScreenName Convert(ScreenName screenName)
        {
            return (UnityEngine.Analytics.ScreenName)((int)screenName);
        }

        public static AnalyticsResult Convert(UnityEngine.Analytics.AnalyticsResult analyticsResult)
        {
            return (AnalyticsResult)((int)analyticsResult);
        }

        public static UnityEngine.Analytics.AdvertisingNetwork Convert(AdvertisingNetwork advertisingNetwork)
        {
            return (UnityEngine.Analytics.AdvertisingNetwork)((int)advertisingNetwork);
        }

        public static UnityEngine.Analytics.AcquisitionType Convert(AcquisitionType acquisitionType)
        {
            return (UnityEngine.Analytics.AcquisitionType)((int)acquisitionType);
        }

        public static UnityEngine.Analytics.ShareType Convert(ShareType shareType)
        {
            return (UnityEngine.Analytics.ShareType)((int)shareType);
        }

        public static UnityEngine.Analytics.SocialNetwork Convert(SocialNetwork socialNetwork)
        {
            return (UnityEngine.Analytics.SocialNetwork)((int)socialNetwork);
        }

        public static UnityEngine.Analytics.StoreType Convert(StoreType storeType)
        {
            return (UnityEngine.Analytics.StoreType)((int)storeType);
        }

        public static UnityEngine.Analytics.AuthorizationNetwork Convert(AuthorizationNetwork authorizationNetwork)
        {
            return (UnityEngine.Analytics.AuthorizationNetwork)((int)authorizationNetwork);
        }

        #endif
    }

    public class AnalyticsEventAttribute : Attribute
    {
    }

    public class EnumCase : AnalyticsEventAttribute
    {
        public Styles Style;

        public EnumCase(Styles style)
        {
            this.Style = style;
        }

        public enum Styles
        {
            None = 0,
            Snake = 1,
            Lower = 2
        }
    }

    public enum AnalyticsResult
    {
        //
        // Summary:
        //     Analytics API result: Success.
        Ok = 0,
        //
        // Summary:
        //     Analytics API result: Analytics not initialized.
        NotInitialized = 1,
        //
        // Summary:
        //     Analytics API result: Analytics is disabled.
        AnalyticsDisabled = 2,
        //
        // Summary:
        //     Analytics API result: Too many parameters.
        TooManyItems = 3,
        //
        // Summary:
        //     Analytics API result: Argument size limit.
        SizeLimitReached = 4,
        //
        // Summary:
        //     Analytics API result: Too many requests.
        TooManyRequests = 5,
        //
        // Summary:
        //     Analytics API result: Invalid argument value.
        InvalidData = 6,
        //
        // Summary:
        //     Analytics API result: This platform doesn't support Analytics.
        UnsupportedPlatform = 7
    }

    [EnumCase(EnumCase.Styles.Snake)]
    public enum ScreenName
    {
        None = 0,
        MainMenu = 1,
        Settings = 2,
        Map = 3,
        Lose = 4,
        Win = 5,
        Credits = 6,
        Title = 7,
        IAPPromo = 8,
        CrossPromo = 9,
        FeaturePromo = 10,
        Hint = 11,
        Pause = 12,
        Inventory = 13,
        Leaderboard = 14,
        Achievements = 15,
        Lobby = 16
    }

    [EnumCase(EnumCase.Styles.Lower)]
    public enum AdvertisingNetwork
    {
        None = 0,
        Aarki = 1,
        AdAction = 2,
        AdapTv = 3,
        Adcash = 4,
        AdColony = 5,
        AdMob = 6,
        AerServ = 7,
        Airpush = 8,
        Altrooz = 9,
        Ampush = 10,
        AppleSearch = 11,
        AppLift = 12,
        AppLovin = 13,
        Appnext = 14,
        AppNexus = 15,
        Appoday = 16,
        Appodeal = 17,
        AppsUnion = 18,
        Avazu = 19,
        BlueStacks = 20,
        Chartboost = 21,
        ClickDealer = 22,
        CPAlead = 23,
        CrossChannel = 24,
        CrossInstall = 25,
        Epom = 26,
        Facebook = 27,
        Fetch = 28,
        Fiksu = 29,
        Flurry = 30,
        Fuse = 31,
        Fyber = 32,
        Glispa = 33,
        Google = 34,
        GrowMobile = 35,
        HeyZap = 36,
        HyperMX = 37,
        Iddiction = 38,
        IndexExchange = 39,
        InMobi = 40,
        Instagram = 41,
        Instal = 42,
        Ipsos = 43,
        IronSource = 44,
        Jirbo = 45,
        Kimia = 46,
        Leadbolt = 47,
        Liftoff = 48,
        Manage = 49,
        Matomy = 50,
        MediaBrix = 51,
        MillenialMedia = 52,
        Minimob = 53,
        MobAir = 54,
        MobileCore = 55,
        Mobobeat = 56,
        Mobusi = 57,
        Mobvista = 58,
        MoPub = 59,
        Motive = 60,
        Msales = 61,
        NativeX = 62,
        OpenX = 63,
        Pandora = 64,
        PropellerAds = 65,
        Revmob = 66,
        RubiconProject = 67,
        SiriusAd = 68,
        Smaato = 69,
        SponsorPay = 70,
        SpotXchange = 71,
        StartApp = 72,
        Tapjoy = 73,
        Taptica = 74,
        Tremor = 75,
        TrialPay = 76,
        Twitter = 77,
        UnityAds = 78,
        Vungle = 79,
        Yeahmobi = 80,
        YuMe = 81
    }

    [EnumCase(EnumCase.Styles.Snake)]
    public enum AcquisitionType
    {
        Soft = 0,
        Premium = 1
    }

    [EnumCase(EnumCase.Styles.Snake)]
    public enum ShareType
    {
        None = 0,
        TextOnly = 1,
        Image = 2,
        Video = 3,
        Invite = 4,
        Achievement = 5
    }

    [EnumCase(EnumCase.Styles.Snake)]
    public enum StoreType
    {
        Soft = 0,
        Premium = 1
    }

    [EnumCase(EnumCase.Styles.Lower)]
    public enum SocialNetwork
    {
        None = 0,
        Facebook = 1,
        Twitter = 2,
        Instagram = 3,
        GooglePlus = 4,
        Pinterest = 5,
        WeChat = 6,
        SinaWeibo = 7,
        TencentWeibo = 8,
        QQ = 9,
        Zhihu = 10,
        VK = 11,
        OK_ru = 12
    }

    [EnumCase(EnumCase.Styles.Lower)]
    public enum AuthorizationNetwork
    {
        None = 0,
        Internal = 1,
        Facebook = 2,
        Twitter = 3,
        Google = 4,
        GameCenter = 5
    }
}
