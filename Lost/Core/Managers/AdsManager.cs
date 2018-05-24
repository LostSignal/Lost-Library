//-----------------------------------------------------------------------
// <copyright file="AdsManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    #if UNITY_ADS
    using UnityEngine.Advertisements;
    #endif

    public enum AdWatchedResult
    {
        Failed = 0,
        Skipped = 1,
        Finished = 2,
        NotReady = 3
    }

    public class AdsManager : Lost.SingletonGameObject<AdsManager>
    {
        public bool AreAdsSupported
        {
            get
            {
                #if UNITY_ADS
                return Advertisement.isSupported;
                #else
                return false;
                #endif
            }
        }

        public bool AreAdsInitialized
        {
            get
            {
                #if UNITY_ADS
                return Advertisement.isSupported && Advertisement.isInitialized;
                #else
                return false;
                #endif
            }
        }

        public bool IsAdReady
        {
            get
            {
                #if UNITY_ADS
                return Advertisement.isSupported && Advertisement.isInitialized && Advertisement.IsReady();
                #else
                return false;
                #endif
            }
        }

        public void ShowAd(Action<AdWatchedResult> watchResultCallback)
        {
            #if UNITY_ADS

            if (this.IsAdReady)
            {
                var options = new ShowOptions()
                {
                    resultCallback = new Action<ShowResult>(result =>
                    {
                        if (watchResultCallback != null)
                        {
                            switch (result)
                            {
                                case ShowResult.Failed:
                                    watchResultCallback(AdWatchedResult.Failed);
                                    break;
                                case ShowResult.Finished:
                                    watchResultCallback(AdWatchedResult.Finished);
                                    break;
                                case ShowResult.Skipped:
                                    watchResultCallback(AdWatchedResult.Skipped);
                                    break;
                                default:
                                    Debug.LogErrorFormat(this, "AdsManager.ShowAd encountered unknown ShowResult {0}", result);
                                    break;
                            }
                        }
                    })
                };

                Advertisement.Show(null, options);
            }
            else
            #endif
            {
                if (watchResultCallback != null)
                {
                    watchResultCallback(AdWatchedResult.NotReady);
                }
            }
        }

        public void PrintDebugOutput()
        {
            #if UNITY_ADS
            Debug.Log("UNITY_ADS Define Present");
            Debug.Log("Advertisement.isInitialized = " + Advertisement.isInitialized);
            Debug.Log("Advertisement.isSupported = " + Advertisement.isSupported);
            Debug.Log("Advertisement.testMode = " + Advertisement.testMode);
            #else
            Debug.Log("UNITY_ADS Define Is Not Present");
            #endif

            Debug.Log("AdsManager.Instance.AreAdsSupported = " + AdsManager.Instance.AreAdsSupported);
            Debug.Log("AdsManager.Instance.AreAdsInitialized = " + AdsManager.Instance.AreAdsInitialized);
            Debug.Log("AdsManager.Instance.IsAdReady = " + AdsManager.Instance.IsAdReady);
        }
    }
}
