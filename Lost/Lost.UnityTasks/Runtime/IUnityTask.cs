//-----------------------------------------------------------------------
// <copyright file="IUnityTask.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;

    public interface IUnityTask
    {
        Exception Exception { get; }
        bool HasError { get; }
        bool IsCanceled { get; }
        bool DidTimeout { get; }
        bool IsDone { get; }
        void Cancel();
    }
}
