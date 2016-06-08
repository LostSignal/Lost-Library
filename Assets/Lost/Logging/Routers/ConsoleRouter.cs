//-----------------------------------------------------------------------
// <copyright file="ConsoleRouter.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;
    
    public class ConsoleRouter : ILoggingRouter
    {
        //// TODO possible add color mockup later 
        //// Debug.Log("<color=red>Fatal error:</color> AssetBundle not found");
        
        public void Log(DateTime time, UnityEngine.Object context, LogLevel level, string channel, string message, Exception ex)
        {
            if (level == LogLevel.Info)
            {
                Debug.Log(message, context);
            }
            else if (level == LogLevel.Warning)
            {
                Debug.LogWarning(message, context);
            }
            else if (level == LogLevel.Error)
            {
                Debug.LogError(message, context);
            }
            else if (level == LogLevel.Exception)
            {
                Debug.LogException(new Exception(message, ex), context);
            }
        }
    }
}