//-----------------------------------------------------------------------
// <copyright file="LoggingManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public class LoggingManager : SingletonGameObject<LoggingManager>
    {
        private const string logEventName = "log_event";
        private const string logTypeName = "log_type";
        private const string conditionName = "condition";
        private const string hashCodeName = "hash_code";
        private const string callstackName = "callstack";

        private List<ILoggingProvider> loggingProviders = new List<ILoggingProvider>();
        private HashSet<int> sentLogs = new HashSet<int>();

        private Dictionary<string, object> eventArgsCache = new Dictionary<string, object>()
        {
            { logTypeName, string.Empty },
            { conditionName, string.Empty },
            { hashCodeName, 0 },
        };

        private Dictionary<LogType, string> logTypeCache = new Dictionary<LogType, string>
        {
            { LogType.Assert, "Assert" },
            { LogType.Error, "Error" },
            { LogType.Exception, "Exception" },
            { LogType.Log, "Log" },
            { LogType.Warning, "Warning" },
        };

        public bool ForwardLoggingAsAnalyticEvents { get; set; }

        public bool DontForwardInfoLevelLogging { get; set; }

        public void AddLoggingProvider(ILoggingProvider loggingProvider)
        {
            this.loggingProviders.Add(loggingProvider);
        }

        protected override void Awake()
        {
            base.Awake();

            if (Application.isEditor == false)
            {
                Application.logMessageReceived += Application_logMessageReceived;
            }
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            int stackTraceHashCode = stackTrace.GetHashCode();

            // Forward all Logging as an Analytic Event (if we haven't seen it before this session)
            if (this.ForwardLoggingAsAnalyticEvents && this.sentLogs.Contains(stackTraceHashCode) == false)
            {
                // Making sure we don't send regular logs up if that flag is set
                if (this.DontForwardInfoLevelLogging == false || type != LogType.Log)
                {
                    this.sentLogs.Add(stackTraceHashCode);

                    this.eventArgsCache[logTypeName] = this.logTypeCache[type];
                    this.eventArgsCache[conditionName] = condition;
                    this.eventArgsCache[hashCodeName] = stackTraceHashCode;

                    // NOTE [bgish]: Currently can't do this, because it puts us over event size limits
                    this.eventArgsCache[callstackName] = string.Empty; // stackTrace

                    Lost.Analytics.AnalyticsEvent.Custom(logEventName, this.eventArgsCache);
                }
            }

            // Sending the log to all the providers
            for (int i = 0; i < this.loggingProviders.Count; i++)
            {
                this.loggingProviders[i].Log(condition, stackTrace, type);
            }
        }
    }
}
