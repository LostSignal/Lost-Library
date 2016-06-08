//-----------------------------------------------------------------------
// <copyright file="Logger.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;

    public static class Logger
    {
        private static readonly List<ILoggingRouter> Routers = new List<ILoggingRouter>();

        static Logger()
        {
            Routers.Add(new ConsoleRouter());
        }
        
        public static void AddLoggingRouter(ILoggingRouter newRouter)
        {
            Routers.AddIfNotNullAndUnique(newRouter);
        }

        public static void LogInfo(string message, params object[] args)
        {
            Log(null, LogLevel.Info, string.Format(message, args), null);
        }

        public static void LogInfo(UnityEngine.Object context, string message, params object[] args)
        {
            Log(context, LogLevel.Info, string.Format(message, args), null);
        }

        public static void LogWarning(string message, params object[] args)
        {
            Log(null, LogLevel.Warning, string.Format(message, args), null);
        }
        
        public static void LogWarning(UnityEngine.Object context, string message, params object[] args)
        {
            Log(context, LogLevel.Warning, string.Format(message, args), null);
        }

        public static void LogError(string message, params object[] args)
        {
            Log(null, LogLevel.Error, string.Format(message, args), null);
        }
        
        public static void LogError(UnityEngine.Object context, string message, params object[] args)
        {
            Log(context, LogLevel.Error, string.Format(message, args), null);
        }

        public static void LogException(Exception ex, string message, params object[] args)
        {
            Log(null, LogLevel.Exception, string.Format(message, args), ex);
        }

        public static void LogException(UnityEngine.Object context, Exception ex, string message, params object[] args)
        {
            Log(context, LogLevel.Exception, string.Format(message, args), ex);
        }

        private static void Log(UnityEngine.Object context, LogLevel level, string message, Exception ex)
        {
            for (int i = 0; i < Routers.Count; i++)
            {
                Routers[i].Log(DateTime.UtcNow, context, level, string.Empty, message, ex);
            }
        }
    }
}