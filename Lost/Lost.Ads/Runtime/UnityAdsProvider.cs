//-----------------------------------------------------------------------
// <copyright file="UnityAdsProvider.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_ADS

namespace Lost
{
    public class UnityAdsProvider : IAdProvider
    {
        public UnityAdsProvider(string appleAppStoreId, string googlePlayId)
        {
            #if UNITY_ADS && UNITY_IPHONE

            if (string.IsNullOrWhiteSpace(appleAppStoreId) == false)
            {
                UnityEngine.Advertisements.Advertisement.Initialize(appleAppStoreId);
            }
            
            #elif UNITY_ADS && UNITY_ANDROID
            
            if (string.IsNullOrWhiteSpace(googlePlayId) == false)
            {
                UnityEngine.Advertisements.Advertisement.Initialize(googlePlayId);
            }

            #endif
        }

        string IAdProvider.ProviderName
        {
            get { return "UnityAds"; }
        }

        #if UNITY_ADS

        bool IAdProvider.AreAdsSupported
        {
            get { return UnityEngine.Advertisements.Advertisement.isSupported; }
        }

        bool IAdProvider.AreAdsInitialized
        {
            get { return UnityEngine.Advertisements.Advertisement.isSupported && UnityEngine.Advertisements.Advertisement.isInitialized; }
        }

        bool IAdProvider.IsAdReady(string placementId)
        {
            return this.IsPlacementIdAdReady(placementId);
        }

        void IAdProvider.ShowAd(string placementId, bool isRewarded, System.Action<AdWatchedResult> watchResultCallback)
        {
            var options = new UnityEngine.Advertisements.ShowOptions()
            {
                resultCallback = new System.Action<UnityEngine.Advertisements.ShowResult>(result =>
                {
                    if (watchResultCallback != null)
                    {
                        switch (result)
                        {
                            case UnityEngine.Advertisements.ShowResult.Failed:
                                watchResultCallback(AdWatchedResult.AdFailed);
                                break;
                            case UnityEngine.Advertisements.ShowResult.Skipped:
                                watchResultCallback(AdWatchedResult.AdSkipped);
                                break;
                            case UnityEngine.Advertisements.ShowResult.Finished:
                                watchResultCallback(AdWatchedResult.AdFinished);
                                break;
                            default:
                                UnityEngine.Debug.LogErrorFormat("UnityAdProvider.ShowAd() encountered unknown ShowResult {0}", result);
                                break;
                        }
                    }
                })
            };

            UnityEngine.Advertisements.Advertisement.Show(placementId, options);
        }

        private bool IsPlacementIdAdReady(string placementId)
        {
            return UnityEngine.Advertisements.Advertisement.isSupported &&
                   UnityEngine.Advertisements.Advertisement.isInitialized &&
                   UnityEngine.Advertisements.Advertisement.IsReady(placementId);
        }

        #else

        bool IAdProvider.AreAdsSupported => false;

        bool IAdProvider.AreAdsInitialized => false;

        bool IAdProvider.IsAdReady(string placementId)
        {
            return false;
        }

        void IAdProvider.ShowAd(string placementId, bool isRewarded, System.Action<AdWatchedResult> watchResultCallback)
        {
            watchResultCallback?.Invoke(AdWatchedResult.AdsNotSupported);
        }

        #endif
    }
}

#endif
