//-----------------------------------------------------------------------
// <copyright file="AnalyticsManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using Lost.AppConfig;
    using UnityEngine;

    public class AnalyticsManager : SingletonGameObject<AnalyticsManager>
    {
        private static readonly string AnonymousIdKey = "AnnonymousId";
        private static readonly float NewSessionWaitTimeInSeconds = 30.0f;

        List<IAnalyticsProvider> analyticsProviders = new List<IAnalyticsProvider>();

        private float lostFocusTime = -1.0f;
        private bool pauseFlushing;
        private string anonymousId;

        public string AnonymousId
        {
            get
            {
                if (string.IsNullOrEmpty(this.anonymousId))
                {
                    string playerPrefsAnonymousId = LostPlayerPrefs.GetString(AnonymousIdKey, null);

                    if (string.IsNullOrEmpty(playerPrefsAnonymousId))
                    {
                        this.anonymousId = Guid.NewGuid().ToString();
                        LostPlayerPrefs.SetString(AnonymousIdKey, this.anonymousId);
                        LostPlayerPrefs.Save();
                    }
                    else
                    {
                        this.anonymousId = playerPrefsAnonymousId;
                    }
                }

                return this.anonymousId;
            }
        }

        public long SessionId
        {
            get { return UnityEngine.Analytics.AnalyticsSessionInfo.sessionId; }
        }

        public void RegisterAnalyticsProvider(IAnalyticsProvider provider)
        {
            this.analyticsProviders.AddIfNotNullAndUnique(provider);
        }

        public bool PauseFlushing
        {
            get
            {
                return this.pauseFlushing;
            }

            set
            {
                if (this.pauseFlushing != value)
                {
                    this.pauseFlushing = value;

                    if (this.pauseFlushing == false)
                    {
                        this.ForceFlush();
                    }
                }
            }
        }

        public void Flush()
        {
            if (this.pauseFlushing)
            {
                return;
            }

            this.ForceFlush();
        }

        public void ResetAnonymousId()
        {
            LostPlayerPrefs.DeleteKey(AnonymousIdKey);
            LostPlayerPrefs.Save();
        }

        protected override void Awake()
        {
            base.Awake();

            UnityEngine.Analytics.Analytics.initializeOnStartup = true;
            UnityEngine.Analytics.Analytics.enabled = true;

            // Toggle this on if you wish to see debug logs for every analytic event
            UnityEngine.Analytics.AnalyticsEvent.debugMode = false;

            if (UnityEngine.Analytics.AnalyticsSessionInfo.userId != AnonymousId)
            {
                UnityEngine.Analytics.Analytics.SetUserId(AnonymousId);
            }

            Analytics.AnalyticsEvent.CustomEventFired += this.EventFired;
        }

        private void EventFired(string eventName, IDictionary<string, object> eventData)
        {
            for (int i = 0; i < this.analyticsProviders.Count; i++)
            {
                this.analyticsProviders[i].CustomEvent(this.SessionId, eventName, eventData);
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                float delta = Time.realtimeSinceStartup - this.lostFocusTime;

                if (this.lostFocusTime > 0 && delta > NewSessionWaitTimeInSeconds)
                {
                    // this.session = Guid.NewGuid();
                    // this.sessionCount++;
                    // this.SendNewSessionEvent();
                }
            }
            else
            {
                this.lostFocusTime = Time.realtimeSinceStartup;
                this.ForceFlush();
            }
        }

        private void SendNewSessionEvent()
        {
            Analytics.AnalyticsEvent.Custom("new_session", new Dictionary<string, object>
            {
                { "active_config", RuntimeAppConfig.Instance.AppConfigName },
                { "app_name",  Application.productName },
                { "app_version",  Application.version },
                { "app_is_editor",  Application.isEditor },
                { "app_runtime_platform",  Application.platform.ToString() },
                { "app_language",  Application.systemLanguage.ToString() },
            });
        }

        private void SendCloudBuildEvent()
        {
            var manifest = CloudBuildManifest.Find();

            if (manifest == null)
            {
                return;
            }

            Analytics.AnalyticsEvent.Custom("unity_cloud_build", new Dictionary<string, object>
            {
                { "branch",  manifest.ScmBranch },
                { "commit_id",  manifest.ScmCommitId },
                { "build_number",  manifest.BuildNumber },
                { "target_name",  manifest.CloudBuildTargetName },
                { "unity_version",  manifest.UnityVersion },
                { "bundle_id",  manifest.BundleId },
                { "project_id",  manifest.ProjectId },
            });
        }

        private void ForceFlush()
        {
            for (int i = 0; i < this.analyticsProviders.Count; i++)
            {
                this.analyticsProviders[i].FlushRequested();
            }
        }
    }
}
