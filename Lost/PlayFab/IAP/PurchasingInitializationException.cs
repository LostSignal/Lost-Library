//-----------------------------------------------------------------------
// <copyright file="PurchasingInitializationException.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_PURCHASING

namespace Lost
{
    using UnityEngine.Purchasing;

    public class PurchasingInitializationException : System.Exception
    {
        public InitializationFailureReason FailureReason { get; private set; }

        public PurchasingInitializationException(InitializationFailureReason failureReason)
        {
            this.FailureReason = failureReason;
        }
    }
}

#endif
