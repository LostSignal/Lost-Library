//-----------------------------------------------------------------------
// <copyright file="TitleDataHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using System.Collections.Generic;

    public class TitleDataHelper
    {
        private Dictionary<string, string> titleData = new Dictionary<string, string>();

        public TitleDataHelper()
        {
            PF.PlayfabEvents.OnLoginResultEvent += PlayfabEvents_OnLoginResultEvent;
        }

        // TODO [bgish]: Need to make this an UnityTask and have it get the info from PlayFab if it isn't in the dictionary
        public string Get(string titleDataKey)
        {
            return this.titleData[titleDataKey];
        }

        private void PlayfabEvents_OnLoginResultEvent(PlayFab.ClientModels.LoginResult result)
        {
            var titleDataDictionary = result?.InfoResultPayload?.TitleData;

            if (titleDataDictionary != null)
            {
                foreach (var titleDataKeyValuePair in titleDataDictionary)
                {
                    if (this.titleData.ContainsKey(titleDataKeyValuePair.Key))
                    {
                        this.titleData[titleDataKeyValuePair.Key] = titleDataKeyValuePair.Value;
                    }
                    else
                    {
                        this.titleData.Add(titleDataKeyValuePair.Key, titleDataKeyValuePair.Value);
                    }
                }
            }
        }
    }
}

#endif
