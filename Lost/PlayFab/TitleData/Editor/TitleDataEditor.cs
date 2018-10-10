//-----------------------------------------------------------------------
// <copyright file="TitleDataEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Linq;
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
            this.activeDataVersionIndex = 0;
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
            this.GetEndpoint();
            PlayFabEditorHttp.MakeApiCall("/Admin/SetTitleData", this.GetEndpoint(), req, resultCb, errorCb);
        }

        private string GetEndpoint()
        {
            PlayFabEditorDataService.SharedSettings.TitleId = AppSettings.ActiveConfig.PlayfabTitleId;
            PlayFabEditorDataService.SharedSettings.DeveloperSecretKey = AppSettings.ActiveConfig.PlayfabSecretId;
            return "https://" + PlayFabEditorDataService.ActiveTitle.Id + PlayFabEditorHelper.TITLE_ENDPOINT;
        }
        #endif
    }
}
