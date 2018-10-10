//-----------------------------------------------------------------------
// <copyright file="UnityAdsProvider.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{    
    public class UnityAdsProvider : IAdProvider
    {
        string IAdProvider.ProviderName
        {
            get { return "UnityAds"; }
        }
    
        #if !UNITY_ADS
    
        bool IAdProvider.AreAdsSupported
        {
            get { return false; }
        }
    
        bool IAdProvider.AreAdsInitialized
        {
            get { return false; }
        }
    
        bool IAdProvider.IsAdReady(string placementId)
        {
            return false;
        }

        void IAdProvider.ShowAd(string placementId, bool isRewarded, System.Action<AdWatchedResult> watchResultCallback)
        {
            if (watchResultCallback != null)
            {
                watchResultCallback(AdWatchedResult.AdsNotInitialized);
            }
        }
    
        #else
    
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
    
        #endif
    }
}
