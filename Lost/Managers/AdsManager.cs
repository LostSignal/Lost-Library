//-----------------------------------------------------------------------
// <copyright file="AdsManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    #if UNITY_ADS
    using UnityEngine.Advertisements;
    #endif
    
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
        
        public bool CanShowAd
        {
            get
            {
                #if UNITY_ADS
                return this.AreAdsInitialized && Advertisement.IsReady();
                #else
                return false;
                #endif
            }
        }
        
        public void Show()
        {
            #if UNITY_ADS
            if (this.CanShowAd)
            {            
                var options = new ShowOptions();

                options.resultCallback = new System.Action<ShowResult>(result => 
                {
                    Debug.Log(result.ToString(), this);  // ShowResult.Finished, ShowResult.Skipped or ShowResult.Failed
                });
                
                Advertisement.Show(null, options);
            }
            #endif
        }
    }
}
