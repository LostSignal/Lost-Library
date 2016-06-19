//-----------------------------------------------------------------------
// <copyright file="ActionExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public static class ActionExtensions
    {
        public static void InvokeIfNotNull(this System.Action action)
        {
            if (action != null)
            {
                action.Invoke();
            }
        }
    }
}
