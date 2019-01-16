//-----------------------------------------------------------------------
// <copyright file="AdsManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;

    public class AdsManager : Lost.SingletonGameObject<AdsManager>
    {
        private IAdProvider provider;

        public string CurrentProviderName
        {
            get { return this.provider != null ? this.provider.ProviderName : null; }
        }

        public bool AreAdsSupported
        {
            get { return this.provider != null && this.provider.AreAdsSupported; }
        }

        public bool AreAdsInitialized
        {
            get { return this.provider != null && this.provider.AreAdsInitialized; }
        }

        public void SetAdProvider(IAdProvider adProvider)
        {
            this.provider = adProvider;
        }

        public bool IsAdReady(string placementId)
        {
            return this.provider != null ? this.provider.IsAdReady(placementId) : false;
        }

        public void ShowAd(string placementId, bool isRewarded, Action<AdWatchedResult> watchResultCallback = null, IDictionary<string, object> eventData = null)
        {
            if (this.provider == null)
            {
                if (watchResultCallback != null)
                {
                    watchResultCallback(AdWatchedResult.ProviderNotset);
                }
            }
            else if (this.AreAdsSupported == false)
            {
                if (watchResultCallback != null)
                {
                    watchResultCallback(AdWatchedResult.AdsNotSupported);
                }
            }
            else if (this.AreAdsInitialized == false)
            {
                if (watchResultCallback != null)
                {
                    watchResultCallback(AdWatchedResult.AdsNotInitialized);
                }
            }
            else if (this.IsAdReady(placementId) == false)
            {
                if (watchResultCallback != null)
                {
                    watchResultCallback(AdWatchedResult.AdNotReady);
                }
            }
            else
            {
                Lost.Analytics.AnalyticsEvent.AdStart(isRewarded, this.provider.ProviderName, placementId, eventData);

                this.provider.ShowAd(placementId, isRewarded, (result) =>
                {
                    switch (result)
                    {
                        case AdWatchedResult.AdFinished:
                            Lost.Analytics.AnalyticsEvent.AdComplete(isRewarded, this.provider.ProviderName, placementId, eventData);
                            break;

                        case AdWatchedResult.AdSkipped:
                            Lost.Analytics.AnalyticsEvent.AdSkip(isRewarded, this.provider.ProviderName, placementId, eventData);
                            break;

                        case AdWatchedResult.AdFailed:
                            UnityEngine.Debug.LogErrorFormat("Ad from privider {0} failed to play.", this.provider.ProviderName);
                            break;

                        default:
                            UnityEngine.Debug.LogErrorFormat("Found Unknown AdWatchResult type {0}", result);
                            break;
                    }

                    if (watchResultCallback != null)
                    {
                        watchResultCallback(result);
                    }
                });
            }
        }
    }
}
