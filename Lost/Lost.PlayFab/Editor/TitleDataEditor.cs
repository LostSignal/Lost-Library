//-----------------------------------------------------------------------
// <copyright file="TitleDataEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Linq;
    using Lost.AppConfig;
    using UnityEditor;
    using UnityEngine;

    #if ENABLE_PLAYFABADMIN_API
    using PlayFab.AdminModels;
    using PlayFab.PfEditor;
    #endif

    public abstract class TitleDataEditor<T> : Editor where T : IVersion, new()
    {
        private string[] titleDataVersions;
        private int activeDataVersionIndex;

        public void OnEnable()
        {
            var data = this.target as TitleData<T>;
            this.titleDataVersions = data.Data.Select(x => x.Version).ToArray();
            this.activeDataVersionIndex = data.Data.Count - 1;
        }

        public override void OnInspectorGUI()
        {
            this.DrawDefaultInspector();

            GUILayout.Label("");

            var data = target as TitleData<T>;

            using (new BeginHorizontalHelper())
            {
                GUILayout.Label("Selected Version", GUILayout.Width(120));
                this.activeDataVersionIndex = EditorGUILayout.Popup(this.activeDataVersionIndex, this.titleDataVersions, GUILayout.Width(80));
            }

            GUILayout.Label("");

            if (GUILayout.Button("Edit Version", GUILayout.Width(200)))
            {
                this.EditData(data.GetDataVersion(this.titleDataVersions[this.activeDataVersionIndex]));
            }

            #if ENABLE_PLAYFABADMIN_API
            if (GUILayout.Button("Upload Title Data", GUILayout.Width(200)))
            {
                var version = this.titleDataVersions[this.activeDataVersionIndex];
                var dataVersion = data.GetDataVersion(version);
                this.UploadTitleData(data.Key, version, dataVersion);
            }
            #endif

            GUILayout.Label("");
        }

        public abstract void EditData(T dataToEdit);

        #if ENABLE_PLAYFABADMIN_API
        protected abstract void UploadTitleData(string key, string version, T data);

        protected void SetTitleData(string titleDataKey, string json, System.Action onComplete = null)
        {
            this.SetTitleData(
                new SetTitleDataRequest { Key = titleDataKey, Value = json },
                (result) =>
                {
                    Debug.LogFormat("Successfully uploaded Title Data Key {0}", titleDataKey);

                    if (onComplete != null)
                    {
                        onComplete();
                    }
                },
                (error) =>
                {
                    Debug.LogErrorFormat("Unable to upload Title Data Key {0}: {1}", titleDataKey, error);
                });
        }

        private void SetTitleData(SetTitleDataRequest req, Action<SetTitleDataResult> resultCb, Action<PlayFab.PfEditor.EditorModels.PlayFabError> errorCb)
        {
            var playfabSettings = EditorAppConfig.ActiveAppConfig.GetSettings<PlayFabSettings>();
            var endpoint = string.Format("https://{0}.playfabapi.com", playfabSettings.TitleId);
            PlayFabEditorHttp.MakeApiCall("/Admin/SetTitleData", endpoint, req, resultCb, errorCb, playfabSettings.SecretKey);
        }
        #endif

        //// In order for SetTitleData to work, you must add this function to PlayFabEditorHttp.cs
        ////
        //// internal static void MakeApiCall<TRequestType, TResultType>(string api, string apiEndpoint, TRequestType request, Action<TResultType> resultCallback, Action<EditorModels.PlayFabError> errorCallback, string secretKey) where TResultType : class
        //// {
        ////     var url = apiEndpoint + api;
        ////     var req = JsonWrapper.SerializeObject(request, PlayFabEditorUtil.ApiSerializerStrategy);
        ////
        ////     //Set headers
        ////     var headers = new Dictionary<string, string>
        ////     {
        ////         {"Content-Type", "application/json"},
        ////         {"X-ReportErrorAsSuccess", "true"},
        ////         {"X-PlayFabSDK", PlayFabEditorHelper.EDEX_NAME + "_" + PlayFabEditorHelper.EDEX_VERSION},
        ////         {"X-SecretKey", secretKey },
        ////     };
        ////
        ////     //Encode Payload
        ////     var payload = System.Text.Encoding.UTF8.GetBytes(req.Trim());
        ////     var www = new WWW(url, payload, headers);
        ////     PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnHttpReq, api, PlayFabEditorHelper.MSG_SPIN_BLOCK);
        ////
        ////     EditorCoroutine.Start(Post(www, (response) => { OnWwwSuccess(api, resultCallback, errorCallback, response); }, (error) => { OnWwwError(errorCallback, error); }), www);
        //// }
    }
}
