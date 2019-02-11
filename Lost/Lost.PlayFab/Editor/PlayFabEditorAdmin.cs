//-----------------------------------------------------------------------
// <copyright file="PlayFabEditorAdmin.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using PlayFab;
    using PlayFab.AdminModels;
    using System.Threading.Tasks;
    using UnityEngine;

    public static class PlayFabEditorAdmin
    {
        public static PlayFabResult<GetCatalogItemsResult> GetCatalogItems(string catalotVersion)
        {
            PlayFabSettingsHelper.Initialize();

            PlayFabResult<GetCatalogItemsResult> result = null;

            var task = Task.Run(async () =>
            {
                result = await PlayFab.PlayFabAdminAPI.GetCatalogItemsAsync(new GetCatalogItemsRequest
                {
                    CatalogVersion = catalotVersion,
                });
            });

            task.Wait();

            return result;
        }

        public static PlayFabResult<SetTitleDataResult> SetTitleDataTask(string titleDataKey, string json)
        {
            PlayFabSettingsHelper.Initialize();

            PlayFabResult<SetTitleDataResult> result = null;

            var task = Task.Run(async () =>
            {
                result = await PlayFab.PlayFabAdminAPI.SetTitleDataAsync(new SetTitleDataRequest
                {
                    Key = titleDataKey,
                    Value = json,
                });
            });

            task.Wait();

            return result;
        }

        public static void SetTitleDataAndPrintErrorOrSuccess(string titleDataKey, string json)
        {
            var setTitleData = SetTitleDataTask(titleDataKey, json);

            if (setTitleData?.Error != null)
            {
                Debug.LogErrorFormat("Unable to upload Title Data Key {0}", titleDataKey);
                Debug.LogError("Error: " + setTitleData?.Error?.Error);
                Debug.LogError("ErrorMessage: " + setTitleData?.Error?.ErrorMessage);
                Debug.LogError("ErrorDetails: " + setTitleData?.Error?.ErrorDetails);
            }
            else if (setTitleData?.Result != null)
            {
                Debug.LogFormat("Successfully uploaded Title Data Key {0}", titleDataKey);
            }
        }
    }
}

#endif
