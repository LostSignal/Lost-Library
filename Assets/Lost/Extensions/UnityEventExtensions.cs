//-----------------------------------------------------------------------
// <copyright file="UnityEventExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine.Events;
    
    public static class UnityEventExtensions
    {
        public static void InvokeIfNotNull(this UnityEvent unityEvent)
        {
            if (unityEvent != null)
            {
                unityEvent.Invoke();
            }
        }
    }
}
