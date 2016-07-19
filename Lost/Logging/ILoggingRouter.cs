//-----------------------------------------------------------------------
// <copyright file="ILoggingRouter.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    
    public interface ILoggingRouter
    {
        void Log(DateTime time, UnityEngine.Object context, LogLevel level, string channel, string message, Exception exception);
    }
}